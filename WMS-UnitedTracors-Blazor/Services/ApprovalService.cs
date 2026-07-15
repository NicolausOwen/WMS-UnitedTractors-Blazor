using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using WMS_UnitedTracors_Blazor.Helpers;

namespace WMS_UnitedTracors_Blazor.Services;

public class ApprovalService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    private readonly IEmailService _emailService;

    public ApprovalService(IDbContextFactory<ApplicationDbContext> factory, IEmailService emailService)
    {
        _factory = factory;
        _emailService = emailService;
    }

    public async Task<(Dictionary<string, List<Transaction>> GroupedApprovals, List<Transaction> PendingReturns, List<ProfileRequest> PendingProfileRequests, Dictionary<string, List<Transaction>> GroupedHandovers, Dictionary<string, List<Transaction>> GroupedDocumentations)> GetApprovalsAsync(int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var query = _context.Transactions
            .Include(t => t.Product)
                .ThenInclude(p => p.Category)
            .Include(t => t.ProductVariant)
            .Include(t => t.Requester)
            .Include(t => t.Division)
            .OrderByDescending(t => t.created_at)
            .AsQueryable();

        var perms = await ResolvePermsAsync(_context, currentUserId);

        List<int>? allowedCategoryIds = null;
        bool isGlobalAdmin = true;

        bool isSuperAdmin = Permissions.All.All(p => perms.Contains(p));
        bool hasApprovalPerms = perms.Contains(Permissions.ApprovalStage1) || 
                                perms.Contains(Permissions.ApprovalStage2) || 
                                perms.Contains(Permissions.ApprovalHandover) || 
                                perms.Contains(Permissions.ApprovalHandoverFinal) || 
                                perms.Contains(Permissions.UsersManage);

        if (hasApprovalPerms || isSuperAdmin)
        {
            var userAdminRoles = await _context.UserAdminRoles.Where(uar => uar.UserId == currentUserId).ToListAsync();
            isGlobalAdmin = isSuperAdmin || userAdminRoles.Any(uar => uar.CategoryId == null);
            if (!isGlobalAdmin)
            {
                allowedCategoryIds = userAdminRoles.Where(uar => uar.CategoryId != null).Select(uar => uar.CategoryId!.Value).ToList();
            }
        }

        // Predicate for what approvals to show:
        // Users only see requests that require their specific approval role
        query = query.Where(t => 
            // If they can approve stage 1
            (perms.Contains(Permissions.ApprovalStage1) && 
                (t.status == WorkflowStatuses.PendingStaffInventory || (t.request_type == "BORROW" && t.status == WorkflowStatuses.Pending)) && 
                (isGlobalAdmin || allowedCategoryIds == null || (t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value)))
            ) ||
            // If they can approve stage 2
            (perms.Contains(Permissions.ApprovalStage2) && 
                (t.status == WorkflowStatuses.PendingAdmin || (t.request_type == "BORROW" && t.status == WorkflowStatuses.Pending)) && 
                (isGlobalAdmin || allowedCategoryIds == null || (t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value)))
            ) ||
            // If they can approve manager
            (perms.Contains(Permissions.ApprovalManager) && 
                (t.status == WorkflowStatuses.PendingManager && t.request_type == "GIVEAWAY")
            ));

        var transactions = await query.ToListAsync();

        bool isSuper = string.Equals(userRole, "superadmin", StringComparison.OrdinalIgnoreCase) || 
                       string.Equals(userRole, "Super Admin", StringComparison.OrdinalIgnoreCase);

        if (!isSuper && perms.Contains(Permissions.ApprovalStage2))
        {
            transactions = transactions.Where(t => {
                if (t.status == WorkflowStatuses.PendingAdmin || (t.request_type == "BORROW" && t.status == WorkflowStatuses.Pending))
                {
                    var categoryName = t.Product?.Category?.name;
                    bool isElektronik = string.Equals(categoryName, "Elektronik", StringComparison.OrdinalIgnoreCase);
                    
                    if (isElektronik)
                    {
                        return string.Equals(userRole, "PIC Studio", StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        return string.Equals(userRole, "Team Leader Infrastructure", StringComparison.OrdinalIgnoreCase);
                    }
                }
                return true;
            }).ToList();
        }

        var groupedApprovals = transactions
            .GroupBy(t => $"{t.created_at:yyyy-MM-dd HH:mm}_{t.requester_id}_{t.applicant_name}")
            .ToDictionary(g => g.Key, g => g.ToList());

        var pendingReturns = new List<Transaction>();
        var pendingProfileRequests = new List<ProfileRequest>();
        var groupedHandovers = new Dictionary<string, List<Transaction>>();

        if (perms.Contains(Permissions.ApprovalHandover))
        {
            var returnsQuery = _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Include(t => t.Division)
                .Include(t => t.Approver)
                .Where(t => t.status == WorkflowStatuses.Approved && t.pending_return_quantity > 0 && t.is_return_draft == 0);

            if (!isGlobalAdmin && allowedCategoryIds != null)
            {
                returnsQuery = returnsQuery.Where(t => t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value));
            }

            pendingReturns = await returnsQuery
                .OrderByDescending(t => t.updated_at)
                .ToListAsync();
        }

        if (perms.Contains(Permissions.UsersManage))
        {
            pendingProfileRequests = await _context.ProfileRequests
                .Include(pr => pr.User)
                .Include(pr => pr.Division)
                .Where(pr => pr.status == "PENDING")
                .OrderByDescending(pr => pr.created_at)
                .ToListAsync();
        }

        if (perms.Contains(Permissions.ApprovalHandover) || perms.Contains(Permissions.ApprovalHandoverFinal))
        {
            var handoversQ = _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Include(t => t.Division)
                .Where(t =>
                    t.type == "OUT" &&
                    (t.request_type == "BORROW" || t.request_type == "GIVEAWAY") &&
                    (
                        (perms.Contains(Permissions.ApprovalHandover) && 
                            (t.status == WorkflowStatuses.WaitingHandover || 
                            (t.status == WorkflowStatuses.WaitingHandoverConfirm && t.handover_uploaded_by == "USER"))) ||
                        (perms.Contains(Permissions.ApprovalHandoverFinal) && t.status == WorkflowStatuses.WaitingAdminHandover)
                    ));

            if (!isGlobalAdmin && allowedCategoryIds != null)
            {
                handoversQ = handoversQ.Where(t => t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value));
            }

            var handoversQuery = await handoversQ
                .OrderByDescending(t => t.updated_at)
                .ToListAsync();

            groupedHandovers = handoversQuery
                .GroupBy(t => t.group_id)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        var groupedDocumentations = new Dictionary<string, List<Transaction>>();
        if (perms.Contains(Permissions.ApprovalHandover) || perms.Contains(Permissions.ApprovalHandoverFinal))
        {
            var docQ = _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Include(t => t.Division)
                .Where(t => t.type == "OUT" && t.request_type == "GIVEAWAY" && t.status == WorkflowStatuses.WaitingAdminDocumentation);

            if (!isGlobalAdmin && allowedCategoryIds != null)
            {
                docQ = docQ.Where(t => t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value));
            }

            var docList = await docQ
                .OrderByDescending(t => t.updated_at)
                .ToListAsync();

            groupedDocumentations = docList
                .GroupBy(t => t.group_id)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        return (groupedApprovals, pendingReturns, pendingProfileRequests, groupedHandovers, groupedDocumentations);
    }

    public async Task<string?> ApproveAsync(int id, string? notes, int currentUserId, string? userRole, bool sendNotification = true)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions
            .Include(t => t.Product)
                .ThenInclude(p => p.Category)
            .Include(t => t.Requester)
            .FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!CanApprove(transaction, perms, userRole)) return "Transaction is no longer pending or is not assigned to your approval stage.";

        if (transaction.request_type == "GIVEAWAY")
        {
            return await ApproveGiveawayAsync(_context, transaction, notes, currentUserId, perms, sendNotification);
        }

        // BORROW stage 1: Staff Inventoris menyetujui -> lanjut ke tahap Admin (belum potong stok).
        if (transaction.status == WorkflowStatuses.PendingStaffInventory && perms.Contains(Permissions.ApprovalStage1))
        {
            transaction.status = WorkflowStatuses.PendingAdmin;
            transaction.staff_inventory_notes = notes;
            transaction.approver_id = currentUserId;
            transaction.updated_at = DateTime.UtcNow;
            transaction.last_revision_stage = null;
            transaction.rejection_reason = null;
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            if (sendNotification)
            {
                await NotifyBatchResultAsync(new List<(Transaction, string, string?)> { (transaction, "APPROVE", null) });
            }
            return null;
        }

        // BORROW stage 2 (PendingAdmin / legacy Pending): persetujuan final -> potong stok + WaitingHandover.
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (transaction.type == "IN")
                {
                    var stockBefore = transaction.Product!.current_stock;
                    transaction.Product!.current_stock += transaction.quantity ?? 0;
                    _context.Products.Update(transaction.Product!);

                    if (transaction.product_variant_id.HasValue)
                    {
                        var variant = await _context.ProductVariants.FindAsync(transaction.product_variant_id.Value);
                        if (variant != null)
                        {
                            variant.stock += transaction.quantity ?? 0;
                            _context.ProductVariants.Update(variant);
                        }
                    }

                    var stockLog = new StockLog
                    {
                        transaction_id = transaction.id,
                        product_id = transaction.Product!.id,
                        stock_before = stockBefore,
                        stock_after = transaction.Product!.current_stock,
                        created_at = DateTime.UtcNow,
                        updated_at = DateTime.UtcNow
                    };
                    _context.StockLogs.Add(stockLog);
                }

                // Borrowing (OUT + BORROW) requires a handover step before it is considered
                // actively borrowed. Giveaways and stock-in are finalized immediately.
                if (transaction.type == "OUT" && transaction.request_type == "BORROW")
                {
                    transaction.status = WorkflowStatuses.WaitingHandover;
                }
                else
                {
                    transaction.status = WorkflowStatuses.Approved;
                    if (transaction.request_type == "GIVEAWAY")
                    {
                        transaction.returned_at = DateTime.UtcNow; // Mark finished
                    }
                }

                transaction.approver_id = currentUserId;
                transaction.updated_at = DateTime.UtcNow;

                if (perms.Contains(Permissions.ApprovalStage2))
                {
                    transaction.admin_notes = notes;
                }
                else if (perms.Contains(Permissions.ApprovalManager))
                {
                    transaction.manager_notes = notes;
                }

                _context.Transactions.Update(transaction);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                if (sendNotification)
                {
                    await NotifyBatchResultAsync(new List<(Transaction, string, string?)> { (transaction, "APPROVE", null) });
                }

                return null;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return "Approval failed: " + ex.Message;
            }
        });
    }

    public async Task<string?> RejectAsync(int id, string rejectionReason, int currentUserId, string? userRole, bool sendNotification = true)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions
            .Include(t => t.Product)
                .ThenInclude(p => p.Category)
            .Include(t => t.Requester)
            .FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!CanApprove(transaction, perms, userRole)) return "Transaction is no longer pending or is not assigned to your approval stage.";

        // Manager approval is giveaway-only; CanApprove() already blocks BORROW for Manager.

        var stageBefore = transaction.status;

        transaction.status = WorkflowStatuses.Rejected;
        transaction.approver_id = currentUserId;
        transaction.rejection_reason = rejectionReason;
        transaction.updated_at = DateTime.UtcNow;
        transaction.last_revision_stage = null;

        ApplyStageNotes(transaction, stageBefore, rejectionReason);

        if (transaction.type == "OUT" && transaction.Product != null)
        {
            var stockBefore = transaction.Product.current_stock;
            transaction.Product.current_stock += transaction.quantity ?? 0;
            _context.Products.Update(transaction.Product);

            if (transaction.product_variant_id.HasValue)
            {
                var variant = await _context.ProductVariants.FindAsync(transaction.product_variant_id.Value);
                if (variant != null)
                {
                    variant.stock += transaction.quantity ?? 0;
                    _context.ProductVariants.Update(variant);
                }
            }

            _context.StockLogs.Add(new StockLog
            {
                transaction_id = transaction.id,
                product_id = transaction.Product.id,
                stock_before = stockBefore,
                stock_after = transaction.Product.current_stock,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            });
        }

        if (transaction.type == "OUT" && transaction.Product != null && transaction.Requester != null)
        {
            if (transaction.request_type == "GIVEAWAY" && transaction.Product != null)
            {
                int pointsToRefund = (transaction.Product.value) * (transaction.quantity ?? 0);
                if (pointsToRefund > 0 && transaction.Requester != null)
                {
                    transaction.Requester.poin += pointsToRefund;
                    _context.Users.Update(transaction.Requester);
                }
            }
        }

        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        
        if (sendNotification)
        {
            await NotifyBatchResultAsync(new List<(Transaction, string, string?)> { (transaction, "REJECT", rejectionReason) });
        }
        
        return null;
    }

    public async Task<string?> RequestRevisionAsync(int id, string revisionReason, int currentUserId, string? userRole, bool sendNotification = true)
    {
        return await RequestRevisionAsync(id, revisionReason, currentUserId, userRole, null, null, sendNotification);
    }

    public async Task<string?> RequestRevisionAsync(int id, string revisionReason, int currentUserId, string? userRole, int? revisedQuantity, bool sendNotification = true)
    {
        return await RequestRevisionAsync(id, revisionReason, currentUserId, userRole, revisedQuantity, null, sendNotification);
    }

    public async Task<string?> RequestRevisionAsync(int id, string revisionReason, int currentUserId, string? userRole, int? revisedQuantity, DateTime? revisedPickupDate, bool sendNotification = true)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions
            .Include(t => t.Product)
                .ThenInclude(p => p.Category)
            .Include(t => t.Requester)
            .FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!CanApprove(transaction, perms, userRole)) return "Transaction is no longer pending or is not assigned to your approval stage.";

        // Manager approval is giveaway-only; CanApprove() already blocks BORROW for Manager.

        if (string.IsNullOrWhiteSpace(revisionReason)) return "Catatan revisi wajib diisi.";

        // Refund stock immediately back to inventory since it enters revision
        if (transaction.type == "OUT" && transaction.Product != null)
        {
            var stockBefore = transaction.Product.current_stock;
            transaction.Product.current_stock += transaction.quantity ?? 0;
            _context.Products.Update(transaction.Product);

            if (transaction.product_variant_id.HasValue)
            {
                var variant = await _context.ProductVariants.FindAsync(transaction.product_variant_id.Value);
                if (variant != null)
                {
                    variant.stock += transaction.quantity ?? 0;
                    _context.ProductVariants.Update(variant);
                }
            }

            _context.StockLogs.Add(new StockLog
            {
                transaction_id = transaction.id,
                product_id = transaction.Product.id,
                stock_before = stockBefore,
                stock_after = transaction.Product.current_stock,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            });
        }

        if (revisedQuantity.HasValue)
        {
            if (revisedQuantity.Value <= 0) return "Qty revisi harus lebih besar dari 0.";

            if (transaction.request_type == "GIVEAWAY" && transaction.Product != null && transaction.Requester != null)
            {
                var oldPoints = transaction.Product.value * (transaction.quantity ?? 0);
                var newPoints = transaction.Product.value * revisedQuantity.Value;
                var difference = newPoints - oldPoints;

                if (difference > 0 && transaction.Requester.poin < difference)
                    return "Poin user tidak mencukupi untuk qty revisi yang diusulkan.";

                transaction.Requester.poin -= difference;
                _context.Users.Update(transaction.Requester);
            }

            if (!transaction.original_quantity.HasValue)
            {
                transaction.original_quantity = transaction.quantity;
            }

            transaction.quantity = revisedQuantity.Value;
        }

        if (transaction.request_type == "GIVEAWAY" && revisedPickupDate.HasValue)
        {
            var pickupDate = revisedPickupDate.Value.Date;

            if (pickupDate < WibHelper.Today)
                return "Tanggal pengambilan revisi tidak boleh sebelum hari ini.";

            if (transaction.event_date.HasValue && transaction.event_date.Value.Date < pickupDate)
                return "Tanggal event tidak boleh lebih kecil dari tanggal pengambilan revisi.";

            if (!transaction.original_pickup_date.HasValue)
            {
                transaction.original_pickup_date = transaction.pickup_date;
            }

            transaction.pickup_date = pickupDate;
        }

        var stageBefore = transaction.status;

        transaction.status = GetRevisionStatusForStage(stageBefore);
        transaction.approver_id = currentUserId;
        transaction.rejection_reason = revisionReason;
        transaction.updated_at = DateTime.UtcNow;
        transaction.last_revision_stage = WorkflowStatuses.GetRevisionStage(transaction.status);

        ApplyStageNotes(transaction, stageBefore, revisionReason);

        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        
        if (sendNotification)
        {
            await NotifyBatchResultAsync(new List<(Transaction, string, string?)> { (transaction, "REVISE", revisionReason) });
        }
        
        return null;
    }

    public async Task<string?> ApproveReturnAsync(int id, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions.Include(t => t.Product).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";

        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.ApprovalHandover)) return "Unauthorized action.";
        
        if (transaction.status != WorkflowStatuses.Approved || transaction.pending_return_quantity <= 0 || transaction.is_return_draft != 0)
            return "Transaction is not pending return approval.";

        if (transaction.Product == null) return "Associated product not found.";

        try
        {
            // Semua perubahan disimpan dalam satu SaveChangesAsync (sudah atomik secara implisit),
            // sehingga tidak perlu transaksi manual + execution strategy yang rentan error.
            var qty = transaction.pending_return_quantity ?? 0;
            var stockBefore = transaction.Product.current_stock;
            bool isDamagedOrLost = transaction.return_status == "rusak" || transaction.return_status == "hilang" || transaction.return_condition == "rusak" || transaction.return_condition == "hilang";

            if (!isDamagedOrLost)
            {
                transaction.Product.current_stock += qty;

                if (transaction.product_variant_id.HasValue)
                {
                    var variant = await _context.ProductVariants.FindAsync(transaction.product_variant_id.Value);
                    if (variant != null)
                    {
                        variant.stock += qty;
                    }
                }

                _context.StockLogs.Add(new StockLog
                {
                    transaction_id = transaction.id,
                    product_id = transaction.Product.id,
                    stock_before = stockBefore,
                    stock_after = transaction.Product.current_stock,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                });
            }

            transaction.returned_quantity = (transaction.returned_quantity ?? 0) + qty;
            transaction.pending_return_quantity = 0;
            if (transaction.returned_quantity >= transaction.quantity)
            {
                transaction.returned_at = DateTime.UtcNow; // Tandai selesai dikembalikan
            }
            transaction.approver_id = currentUserId;
            transaction.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Kirim notifikasi email ke Admin (TL / PIC Studio)
            try
            {
                var admins = await _context.Users
                    .Where(u => u.role == "Team Leader Infrastructure" || u.role == "PIC Studio" || u.role == "admin")
                    .Select(u => u.email)
                    .ToListAsync();

                if (admins.Any())
                {
                    var product = transaction.Product?.name ?? "Barang";
                    var applicant = transaction.applicant_name ?? "User";
                    var condition = transaction.return_condition ?? "baik";
                    
                    string mailTitle = "Notifikasi Pengembalian Barang";
                    string mailMessage = $"Barang <strong>{product}</strong> sejumlah {qty} unit yang dipinjam oleh {applicant} telah dikembalikan dalam kondisi <strong>{condition}</strong>. Pengembalian telah diverifikasi oleh Staff Inventoris.";
                    string html = GetEmailTemplate(mailTitle, mailMessage);
                    _ = _emailService.SendEmailToMultipleAsync(admins, "Notifikasi Pengembalian Barang (WMS UT)", html);
                }
            }
            catch (Exception)
            {
                // Jangan menggagalkan transaksi jika pengiriman email notifikasi gagal
            }

            return null;
        }
        catch (Exception ex)
        {
            return "Approval failed: " + ex.Message;
        }
    }

    public async Task<string?> ApproveHandoverBatchAsync(string groupId, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.ApprovalHandoverFinal)) return "Unauthorized action.";

        var query = await _context.Transactions
            .Include(t => t.Product)
            .Where(t => t.status == WorkflowStatuses.WaitingAdminHandover && t.type == "OUT" && t.request_type == "BORROW")
            .ToListAsync();

        var matched = query.Where(t => t.group_id == groupId).ToList();

        if (matched.Count == 0) return "Tidak ada transaksi serah terima yang menunggu verifikasi pada event ini.";

        foreach (var item in matched)
        {
            item.status = WorkflowStatuses.Approved;
            item.approver_id = currentUserId;
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();

        if (matched.Any())
        {
            _ = NotifyUserAsync(matched.First(), "Serah Terima Selesai", "Proses serah terima barang peminjaman Anda telah diverifikasi oleh Admin.");
        }

        return null;
    }

    public async Task<string?> ConfirmHandoverBySiAsync(string groupId, int currentUserId, bool isApproved, string? rejectionReason)
    {
        using var _context = _factory.CreateDbContext();
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.ApprovalHandover)) return "Unauthorized action.";
        if (!isApproved && string.IsNullOrWhiteSpace(rejectionReason)) return "Alasan penolakan wajib diisi.";

        var query = await _context.Transactions
            .Where(t => t.status == WorkflowStatuses.WaitingHandoverConfirm && t.handover_uploaded_by == "USER" && t.type == "OUT" && t.request_type == "BORROW")
            .ToListAsync();

        var matched = query.Where(t => t.group_id == groupId).ToList();
        if (matched.Count == 0) return "Tidak ada transaksi yang menunggu konfirmasi serah terima Anda.";

        foreach (var item in matched)
        {
            if (isApproved)
            {
                item.status = WorkflowStatuses.Approved;
                item.rejection_reason = null;
            }
            else
            {
                item.status = WorkflowStatuses.WaitingHandover;
                item.rejection_reason = rejectionReason;
            }
            item.updated_at = DateTime.UtcNow;
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();

        if (matched.Any())
        {
            var firstMatched = matched.First();
            if (isApproved)
            {
                _ = NotifyUserAsync(firstMatched, "Konfirmasi Serah Terima Selesai", "Bukti serah terima Anda telah dikonfirmasi dan proses peminjaman telah disetujui.");
            }
            else
            {
                _ = NotifyUserAsync(firstMatched, "Penolakan Serah Terima", $"Bukti serah terima Anda ditolak. Alasan: {rejectionReason}");
            }
        }

        return null;
    }

    public async Task<string?> RejectHandoverBatchAsync(string groupId, string rejectionReason, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.ApprovalHandoverFinal)) return "Unauthorized action.";
        if (string.IsNullOrWhiteSpace(rejectionReason)) return "Rejection reason is required.";

        var query = await _context.Transactions
            .Where(t => t.status == WorkflowStatuses.WaitingAdminHandover && t.type == "OUT" && t.request_type == "BORROW")
            .ToListAsync();

        var matched = query.Where(t => t.group_id == groupId).ToList();

        if (matched.Count == 0) return "Tidak ada transaksi serah terima yang menunggu verifikasi pada event ini.";

        foreach (var item in matched)
        {
            item.status = WorkflowStatuses.WaitingHandover;
            item.rejection_reason = rejectionReason;
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();

        if (matched.Any())
        {
            _ = NotifyUserAsync(matched.First(), "Serah Terima Ditolak Admin", $"Proses serah terima Anda ditolak oleh Admin. Alasan: {rejectionReason}");
        }

        return null;
    }

    public async Task<string?> RejectReturnAsync(int id, string? rejectionReason, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";

        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.ApprovalHandover)) return "Unauthorized action.";
        
        if (transaction.status != WorkflowStatuses.Approved || transaction.pending_return_quantity <= 0 || transaction.is_return_draft != 0)
            return "Transaction is not pending return approval.";

        transaction.is_return_draft = 1; // Send back to draft
        transaction.return_rejection_reason = rejectionReason;
        transaction.updated_at = DateTime.UtcNow;

        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> ApproveProfileRequestAsync(int id, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var request = await _context.ProfileRequests.FirstOrDefaultAsync(pr => pr.id == id);
        if (request == null) return "Profile request not found.";

        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.UsersManage)) return "Unauthorized action.";
        
        if (request.status != "PENDING") return "Profile request is not pending.";

        var user = await _context.Users.FindAsync(request.user_id);
        if (user == null) return "User not found.";

        user.name = request.name;
        user.email = request.email;
        user.nrp = request.nrp;
        user.division_id = request.division_id;
        user.updated_at = DateTime.UtcNow;

        request.status = "APPROVED";
        request.updated_at = DateTime.UtcNow;

        _context.Users.Update(user);
        _context.ProfileRequests.Update(request);
        await _context.SaveChangesAsync();

        return null;
    }

    public async Task<string?> RejectProfileRequestAsync(int id, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var request = await _context.ProfileRequests.FirstOrDefaultAsync(pr => pr.id == id);
        if (request == null) return "Profile request not found.";

        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.UsersManage)) return "Unauthorized action.";
        
        if (request.status != "PENDING") return "Profile request is not pending.";

        request.status = "REJECTED";
        request.updated_at = DateTime.UtcNow;

        _context.ProfileRequests.Update(request);
        await _context.SaveChangesAsync();

        return null;
    }

    /// <summary>Resolusi permission efektif user (User.role -> AdminRole, dengan fallback role lama).</summary>
    private async Task<HashSet<string>> ResolvePermsAsync(ApplicationDbContext _context, int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return new HashSet<string>();
        var role = await _context.AdminRoles.FirstOrDefaultAsync(r => r.RoleName == user.role && r.IsActive);
        return Permissions.Resolve(user.role, role?.Permissions);
    }

    private static bool CanApprove(Transaction transaction, HashSet<string> perms, string? userRole)
    {
        if (transaction.status == WorkflowStatuses.WaitingHandover || 
            transaction.status == WorkflowStatuses.WaitingHandoverConfirm || 
            transaction.status == WorkflowStatuses.WaitingAdminHandover)
        {
            // Handover statuses are only for SI (Stage1/Handover) and TL (Stage2/HandoverFinal), NOT Manager.
            return perms.Contains(Permissions.ApprovalStage2) || perms.Contains(Permissions.ApprovalHandoverFinal) ||
                   perms.Contains(Permissions.ApprovalHandover) || perms.Contains(Permissions.ApprovalStage1);
        }

        if (transaction.request_type == "GIVEAWAY")
        {
            bool allowed = (transaction.status == WorkflowStatuses.PendingStaffInventory && perms.Contains(Permissions.ApprovalStage1)) ||
                           (transaction.status == WorkflowStatuses.PendingAdmin && perms.Contains(Permissions.ApprovalStage2)) ||
                           (transaction.status == WorkflowStatuses.PendingManager && perms.Contains(Permissions.ApprovalManager));
            if (!allowed) return false;

            if (transaction.status == WorkflowStatuses.PendingAdmin)
            {
                bool isSuper = string.Equals(userRole, "superadmin", StringComparison.OrdinalIgnoreCase) || 
                               string.Equals(userRole, "Super Admin", StringComparison.OrdinalIgnoreCase);
                if (!isSuper)
                {
                    var categoryName = transaction.Product?.Category?.name;
                    bool isElektronik = string.Equals(categoryName, "Elektronik", StringComparison.OrdinalIgnoreCase);
                    if (isElektronik)
                    {
                        return string.Equals(userRole, "PIC Studio", StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        return string.Equals(userRole, "Team Leader Infrastructure", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            return true;
        }

        // BORROW: Stage 1 (approval.stage1) -> Stage 2 (approval.stage2). Legacy PENDING boleh oleh keduanya.
        bool isPendingStage2 = transaction.status == WorkflowStatuses.PendingAdmin || 
                              (transaction.status == WorkflowStatuses.Pending && perms.Contains(Permissions.ApprovalStage2));

        bool canApproveBase = (transaction.status == WorkflowStatuses.PendingStaffInventory && perms.Contains(Permissions.ApprovalStage1)) ||
                              (transaction.status == WorkflowStatuses.PendingAdmin && perms.Contains(Permissions.ApprovalStage2)) ||
                              (transaction.status == WorkflowStatuses.Pending && (perms.Contains(Permissions.ApprovalStage1) || perms.Contains(Permissions.ApprovalStage2)));

        if (!canApproveBase) return false;

        if (isPendingStage2 && transaction.status != WorkflowStatuses.PendingStaffInventory)
        {
            bool isSuper = string.Equals(userRole, "superadmin", StringComparison.OrdinalIgnoreCase) || 
                           string.Equals(userRole, "Super Admin", StringComparison.OrdinalIgnoreCase);
            if (!isSuper)
            {
                var categoryName = transaction.Product?.Category?.name;
                bool isElektronik = string.Equals(categoryName, "Elektronik", StringComparison.OrdinalIgnoreCase);
                if (isElektronik)
                {
                    return string.Equals(userRole, "PIC Studio", StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    return string.Equals(userRole, "Team Leader Infrastructure", StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        return true;
    }

    private async Task<string?> ApproveGiveawayAsync(ApplicationDbContext _context, Transaction transaction, string? notes, int currentUserId, HashSet<string> perms, bool sendNotification = true)
    {
        switch (transaction.status)
        {
            case WorkflowStatuses.PendingStaffInventory when perms.Contains(Permissions.ApprovalStage1):
                transaction.status = WorkflowStatuses.PendingAdmin;
                transaction.staff_inventory_notes = notes;
                break;
            case WorkflowStatuses.PendingAdmin when perms.Contains(Permissions.ApprovalStage2):
                transaction.status = WorkflowStatuses.PendingManager;
                transaction.admin_notes = notes;
                break;
            case WorkflowStatuses.PendingManager when perms.Contains(Permissions.ApprovalManager):
                transaction.status = WorkflowStatuses.WaitingHandover;
                transaction.manager_notes = notes;
                transaction.rejection_reason = null;
                break;
            default:
                return "Transaction is not assigned to your approval stage.";
        }

        transaction.approver_id = currentUserId;
        transaction.updated_at = DateTime.UtcNow;
        transaction.last_revision_stage = null;
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();

        if (sendNotification)
        {
            await NotifyBatchResultAsync(new List<(Transaction, string, string?)> { (transaction, "APPROVE", null) });
        }

        return null;
    }

    private async Task NotifyUserAsync(Transaction transaction, string subject, string message)
    {
        var requesterEmail = transaction.Requester?.email;
        if (string.IsNullOrWhiteSpace(requesterEmail))
        {
            using var context = _factory.CreateDbContext();
            var user = await context.Users.FindAsync(transaction.requester_id);
            requesterEmail = user?.email;
        }

        if (!string.IsNullOrWhiteSpace(requesterEmail))
        {
            var htmlMessage = GetEmailTemplate(subject, message);
            await _emailService.SendEmailAsync(requesterEmail, subject, htmlMessage);
        }
    }

    public async Task NotifyBatchResultAsync(List<(Transaction item, string action, string? reason)> itemsProcessed)
    {
        if (itemsProcessed == null || !itemsProcessed.Any()) return;

        var grouped = itemsProcessed.GroupBy(x => new { x.item.requester_id, x.item.event_name, x.item.group_id });

        foreach (var group in grouped)
        {
            var firstItem = group.First().item;
            
            var requesterEmail = firstItem.Requester?.email;
            if (string.IsNullOrWhiteSpace(requesterEmail))
            {
                using var context = _factory.CreateDbContext();
                var user = await context.Users.FindAsync(firstItem.requester_id);
                requesterEmail = user?.email;
            }

            if (string.IsNullOrWhiteSpace(requesterEmail)) continue;

            string eventName = firstItem.event_name ?? "Tanpa Nama Event";
            
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<p>Berikut adalah hasil proses persetujuan untuk request Event: <strong>{eventName}</strong></p>");
            sb.AppendLine("<ul style='list-style-type: none; padding-left: 0;'>");
            
            foreach(var (item, action, reason) in group)
            {
                string productName = item.Product?.name ?? "Barang";
                string statusText = "";
                
                if (action == "APPROVE") 
                {
                    if (item.status == WorkflowStatuses.PendingAdmin || item.status == WorkflowStatuses.PendingManager)
                        statusText = "<span style='color: #1a6b8a; font-weight: bold;'>DISETUJUI (Menunggu Tahap Selanjutnya)</span>";
                    else
                        statusText = "<span style='color: #1a7a30; font-weight: bold;'>DISETUJUI FINAL</span>";
                }
                else if (action == "REJECT")
                {
                    statusText = $"<span style='color: #d94040; font-weight: bold;'>DITOLAK</span> (Alasan: {reason})";
                }
                else if (action == "REVISE")
                {
                    statusText = $"<span style='color: #e8a000; font-weight: bold;'>DIREVISI</span> (Catatan: {reason})";
                }
                else 
                {
                    statusText = action;
                }
                                    
                sb.AppendLine($"<li style='margin-bottom: 8px; border-bottom: 1px solid #eee; padding-bottom: 8px;'>");
                sb.AppendLine($"<strong>{productName}</strong> ({item.quantity} unit)<br/>Status: {statusText}");
                sb.AppendLine($"</li>");
            }
            sb.AppendLine("</ul>");

            var htmlMessage = GetEmailTemplate("Update Status Request WMS", sb.ToString());
            await _emailService.SendEmailAsync(requesterEmail, "Update Request WMS", htmlMessage);
        }
    }

    private string GetEmailTemplate(string title, string message)
    {
        return $@"
        <div style='font-family: ""Segoe UI"", Arial, sans-serif; background-color: #f4f4f5; padding: 40px 20px;'>
            <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05); border: 1px solid #e4e4e7;'>
                <div style='background-color: #fdc300; padding: 25px; text-align: center; border-bottom: 4px solid #e8a000;'>
                    <h1 style='color: #18181b; margin: 0; font-size: 24px; font-weight: 800; letter-spacing: 0.5px;'>UT WMS</h1>
                </div>
                <div style='padding: 35px 30px; color: #3f3f46; line-height: 1.6;'>
                    <h2 style='color: #18181b; margin-top: 0; font-size: 20px; font-weight: 600;'>{title}</h2>
                    <p style='font-size: 16px; margin-bottom: 0;'>{message}</p>
                </div>
                <div style='background-color: #fafafa; padding: 20px; text-align: center; font-size: 13px; color: #71717a; border-top: 1px solid #e4e4e7;'>
                    <p style='margin: 0;'>Pesan ini dikirim secara otomatis oleh Sistem Manajemen Inventaris United Tractors.</p>
                    <p style='margin: 5px 0 0 0;'>Mohon tidak membalas email ini.</p>
                </div>
            </div>
        </div>";
    }

    // Status revisi & catatan ditentukan oleh TAHAP (status sebelum aksi), bukan role.
    private static string GetRevisionStatusForStage(string? status) =>
        status switch
        {
            WorkflowStatuses.PendingStaffInventory => WorkflowStatuses.RevisionByStaffInventory,
            WorkflowStatuses.PendingAdmin => WorkflowStatuses.RevisionByAdmin,
            WorkflowStatuses.PendingManager => WorkflowStatuses.RevisionByManager,
            _ => WorkflowStatuses.Revision
        };

    private static void ApplyStageNotes(Transaction t, string? stageBefore, string? notes)
    {
        switch (stageBefore)
        {
            case WorkflowStatuses.PendingStaffInventory: t.staff_inventory_notes = notes; break;
            case WorkflowStatuses.PendingManager: t.manager_notes = notes; break;
            default: t.admin_notes = notes; break; // PendingAdmin / legacy Pending
        }
    }

    public async Task<string?> ApproveHandoverItemAsync(int transactionId, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.ApprovalHandover)) return "Unauthorized action.";

        var transaction = await _context.Transactions
            .Include(t => t.Product)
            .FirstOrDefaultAsync(t => t.id == transactionId && t.status == WorkflowStatuses.WaitingAdminHandover && t.type == "OUT" && t.request_type == "BORROW");

        if (transaction == null) return "Transaksi serah terima tidak ditemukan atau tidak sedang menunggu verifikasi.";

        transaction.status = WorkflowStatuses.Approved;
        transaction.approver_id = currentUserId;
        transaction.updated_at = DateTime.UtcNow;
        _context.Transactions.Update(transaction);

        await _context.SaveChangesAsync();
        _ = NotifyUserAsync(transaction, "Serah Terima Selesai", "Proses serah terima barang peminjaman Anda telah diverifikasi oleh Admin.");
        return null;
    }

    public async Task<string?> RejectHandoverItemAsync(int transactionId, string rejectionReason, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.ApprovalHandover)) return "Unauthorized action.";
        if (string.IsNullOrWhiteSpace(rejectionReason)) return "Rejection reason is required.";

        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.id == transactionId && t.status == WorkflowStatuses.WaitingAdminHandover && t.type == "OUT" && t.request_type == "BORROW");

        if (transaction == null) return "Transaksi serah terima tidak ditemukan atau tidak sedang menunggu verifikasi.";

        transaction.status = WorkflowStatuses.WaitingHandover;
        transaction.rejection_reason = rejectionReason;
        transaction.updated_at = DateTime.UtcNow;
        _context.Transactions.Update(transaction);

        await _context.SaveChangesAsync();
        _ = NotifyUserAsync(transaction, "Serah Terima Ditolak Admin", $"Proses serah terima Anda ditolak oleh Admin. Alasan: {rejectionReason}");
        return null;
    }

    public async Task<string?> ApproveGiveawayDocumentationBatchAsync(string groupId, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.ApprovalHandoverFinal) && !perms.Contains(Permissions.ApprovalHandover)) 
            return "Unauthorized action.";

        var query = await _context.Transactions
            .Include(t => t.Product)
            .Where(t => t.status == WorkflowStatuses.WaitingAdminDocumentation && t.type == "OUT" && t.request_type == "GIVEAWAY")
            .ToListAsync();

        var matched = query.Where(t => t.group_id == groupId).ToList();

        if (matched.Count == 0) return "Tidak ada transaksi dokumentasi yang menunggu verifikasi pada event ini.";

        foreach (var item in matched)
        {
            item.status = WorkflowStatuses.Completed;
            item.approver_id = currentUserId;
            item.updated_at = DateTime.UtcNow;
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();

        if (matched.Any())
        {
            _ = NotifyUserAsync(matched.First(), "Dokumentasi Giveaway Disetujui", "Dokumentasi foto penyerahan barang giveaway Anda telah disetujui oleh Admin.");
        }

        return null;
    }

    public async Task<string?> RejectGiveawayDocumentationBatchAsync(string groupId, string rejectionReason, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.ApprovalHandoverFinal) && !perms.Contains(Permissions.ApprovalHandover)) 
            return "Unauthorized action.";
        if (string.IsNullOrWhiteSpace(rejectionReason)) return "Rejection reason is required.";

        var query = await _context.Transactions
            .Where(t => t.status == WorkflowStatuses.WaitingAdminDocumentation && t.type == "OUT" && t.request_type == "GIVEAWAY")
            .ToListAsync();

        var matched = query.Where(t => t.group_id == groupId).ToList();

        if (matched.Count == 0) return "Tidak ada transaksi dokumentasi yang menunggu verifikasi pada event ini.";

        foreach (var item in matched)
        {
            item.status = WorkflowStatuses.WaitingDocumentation;
            item.rejection_reason = rejectionReason;
            item.updated_at = DateTime.UtcNow;
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();

        if (matched.Any())
        {
            _ = NotifyUserAsync(matched.First(), "Dokumentasi Giveaway Ditolak", $"Dokumentasi foto giveaway Anda ditolak oleh Admin. Alasan: {rejectionReason}");
        }

        return null;
    }

    public async Task<string?> ApproveGiveawayDocumentationItemAsync(int transactionId, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.ApprovalHandoverFinal) && !perms.Contains(Permissions.ApprovalHandover)) 
            return "Unauthorized action.";

        var transaction = await _context.Transactions
            .Include(t => t.Product)
            .FirstOrDefaultAsync(t => t.id == transactionId && t.status == WorkflowStatuses.WaitingAdminDocumentation && t.type == "OUT" && t.request_type == "GIVEAWAY");

        if (transaction == null) return "Transaksi dokumentasi tidak ditemukan atau tidak sedang menunggu verifikasi.";

        transaction.status = WorkflowStatuses.Completed;
        transaction.approver_id = currentUserId;
        transaction.updated_at = DateTime.UtcNow;
        _context.Transactions.Update(transaction);

        await _context.SaveChangesAsync();
        _ = NotifyUserAsync(transaction, "Dokumentasi Giveaway Disetujui", "Dokumentasi foto penyerahan barang giveaway Anda telah disetujui oleh Admin.");
        return null;
    }

    public async Task<string?> RejectGiveawayDocumentationItemAsync(int transactionId, string rejectionReason, int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var perms = await ResolvePermsAsync(_context, currentUserId);
        if (!perms.Contains(Permissions.ApprovalHandoverFinal) && !perms.Contains(Permissions.ApprovalHandover)) 
            return "Unauthorized action.";
        if (string.IsNullOrWhiteSpace(rejectionReason)) return "Rejection reason is required.";

        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.id == transactionId && t.status == WorkflowStatuses.WaitingAdminDocumentation && t.type == "OUT" && t.request_type == "GIVEAWAY");

        if (transaction == null) return "Transaksi dokumentasi tidak ditemukan atau tidak sedang menunggu verifikasi.";

        transaction.status = WorkflowStatuses.WaitingDocumentation;
        transaction.rejection_reason = rejectionReason;
        transaction.updated_at = DateTime.UtcNow;
        _context.Transactions.Update(transaction);

        await _context.SaveChangesAsync();
        _ = NotifyUserAsync(transaction, "Dokumentasi Giveaway Ditolak", $"Dokumentasi foto giveaway Anda ditolak oleh Admin. Alasan: {rejectionReason}");
        return null;
    }
}
