using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using WMS_UnitedTracors_Blazor.Helpers;

namespace WMS_UnitedTracors_Blazor.Services;

public class ApprovalService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public ApprovalService(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<(Dictionary<string, List<Transaction>> GroupedApprovals, List<Transaction> PendingReturns, List<ProfileRequest> PendingProfileRequests, Dictionary<string, List<Transaction>> GroupedHandovers)> GetApprovalsAsync(int currentUserId, string? userRole)
    {
        var query = _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Requester)
            .Include(t => t.Division)
            .OrderByDescending(t => t.created_at)
            .AsQueryable();

        var perms = await ResolvePermsAsync(currentUserId);
        
        List<int>? allowedCategoryIds = null;
        bool isGlobalAdmin = true;

        bool isSuperAdmin = Permissions.All.All(p => perms.Contains(p));
        bool hasApprovalPerms = perms.Contains(Permissions.ApprovalStage1) || 
                                perms.Contains(Permissions.ApprovalStage2) || 
                                perms.Contains(Permissions.ApprovalHandover) || 
                                perms.Contains(Permissions.ApprovalHandoverFinal) || 
                                perms.Contains(Permissions.ApprovalReturn) ||
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
        var groupedApprovals = transactions
            .GroupBy(t => $"{t.created_at:yyyy-MM-dd HH:mm}_{t.requester_id}_{t.applicant_name}")
            .ToDictionary(g => g.Key, g => g.ToList());

        var pendingReturns = new List<Transaction>();
        var pendingProfileRequests = new List<ProfileRequest>();
        var groupedHandovers = new Dictionary<string, List<Transaction>>();

        if (perms.Contains(Permissions.ApprovalReturn))
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

        return (groupedApprovals, pendingReturns, pendingProfileRequests, groupedHandovers);
    }

    public async Task<string?> ApproveAsync(int id, string? notes, int currentUserId, string? userRole)
    {
        var transaction = await _context.Transactions.Include(t => t.Product).Include(t => t.Requester).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        var perms = await ResolvePermsAsync(currentUserId);
        if (!CanApprove(transaction, perms)) return "Transaction is no longer pending or is not assigned to your approval stage.";

        if (transaction.request_type == "GIVEAWAY")
        {
            return await ApproveGiveawayAsync(transaction, notes, currentUserId, perms);
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
            _ = NotifyUserAsync(transaction, "Update Request WMS", "Request Anda telah disetujui pada tahap Staff Inventoris dan sedang menunggu tahap selanjutnya.");
            return null;
        }

        // BORROW stage 2 (PendingAdmin / legacy Pending): persetujuan final -> potong stok + WaitingHandover.
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var stockBefore = transaction.Product!.current_stock;

                if (transaction.type == "IN")
                {
                    transaction.Product!.current_stock += transaction.quantity ?? 0;
                }
                else
                {
                    if (transaction.Product!.current_stock < (transaction.quantity ?? 0)) throw new Exception("Insufficient stock");
                    transaction.Product!.current_stock -= transaction.quantity ?? 0;
                }

                _context.Products.Update(transaction.Product!);

                if (transaction.product_variant_id.HasValue)
                {
                    var variant = await _context.ProductVariants.FindAsync(transaction.product_variant_id.Value);
                    if (variant != null)
                    {
                        if (transaction.type == "IN") variant.stock += transaction.quantity ?? 0;
                        else
                        {
                            if (variant.stock < (transaction.quantity ?? 0)) throw new Exception("Insufficient variant stock");
                            variant.stock -= transaction.quantity ?? 0;
                        }
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

                var msg = transaction.type == "OUT" && transaction.request_type == "BORROW" 
                    ? "Request peminjaman Anda telah disetujui secara final. Silakan proses serah terima." 
                    : "Request Anda telah disetujui secara final.";
                _ = NotifyUserAsync(transaction, "Update Request WMS", msg);

                return null;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return "Approval failed: " + ex.Message;
            }
        });
    }

    public async Task<string?> RejectAsync(int id, string rejectionReason, int currentUserId, string? userRole)
    {
        var transaction = await _context.Transactions.Include(t => t.Product).Include(t => t.Requester).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        var perms = await ResolvePermsAsync(currentUserId);
        if (!CanApprove(transaction, perms)) return "Transaction is no longer pending or is not assigned to your approval stage.";

        if (perms.Contains(Permissions.ApprovalManager) && transaction.Product != null && transaction.Product.is_returnable == 1)
        {
            return "Managers cannot reject borrowing transactions. Only admins can.";
        }

        var stageBefore = transaction.status;

        transaction.status = WorkflowStatuses.Rejected;
        transaction.approver_id = currentUserId;
        transaction.rejection_reason = rejectionReason;
        transaction.updated_at = DateTime.UtcNow;
        transaction.last_revision_stage = null;

        ApplyStageNotes(transaction, stageBefore, rejectionReason);

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
        
        _ = NotifyUserAsync(transaction, "Request Ditolak WMS", $"Request Anda telah ditolak. Alasan: {rejectionReason}");
        
        return null;
    }

    public async Task<string?> RequestRevisionAsync(int id, string revisionReason, int currentUserId, string? userRole)
    {
        return await RequestRevisionAsync(id, revisionReason, currentUserId, userRole, null, null);
    }

    public async Task<string?> RequestRevisionAsync(int id, string revisionReason, int currentUserId, string? userRole, int? revisedQuantity)
    {
        return await RequestRevisionAsync(id, revisionReason, currentUserId, userRole, revisedQuantity, null);
    }

    public async Task<string?> RequestRevisionAsync(int id, string revisionReason, int currentUserId, string? userRole, int? revisedQuantity, DateTime? revisedPickupDate)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Requester)
            .FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        var perms = await ResolvePermsAsync(currentUserId);
        if (!CanApprove(transaction, perms)) return "Transaction is no longer pending or is not assigned to your approval stage.";

        if (perms.Contains(Permissions.ApprovalManager) && transaction.Product != null && transaction.Product.is_returnable == 1)
            return "Managers cannot request revision for borrowing transactions. Only admins can.";

        if (string.IsNullOrWhiteSpace(revisionReason)) return "Catatan revisi wajib diisi.";

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

            if (pickupDate < DateTime.Today)
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
        return null;
    }

    public async Task<string?> ApproveReturnAsync(int id, int currentUserId, string? userRole)
    {
        var transaction = await _context.Transactions.Include(t => t.Product).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        
        var perms = await ResolvePermsAsync(currentUserId);
        if (!perms.Contains(Permissions.ApprovalReturn)) return "Unauthorized action.";
        
        if (transaction.status != WorkflowStatuses.Approved || transaction.pending_return_quantity <= 0 || transaction.is_return_draft != 0)
            return "Transaction is not pending return approval.";

        if (transaction.Product == null) return "Associated product not found.";

        try
        {
            // Semua perubahan disimpan dalam satu SaveChangesAsync (sudah atomik secara implisit),
            // sehingga tidak perlu transaksi manual + execution strategy yang rentan error.
            var qty = transaction.pending_return_quantity ?? 0;
            var stockBefore = transaction.Product.current_stock;
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

            transaction.returned_quantity = (transaction.returned_quantity ?? 0) + qty;
            transaction.pending_return_quantity = 0;
            if (transaction.returned_quantity >= transaction.quantity)
            {
                transaction.returned_at = DateTime.UtcNow; // Tandai selesai dikembalikan
            }
            transaction.approver_id = currentUserId;
            transaction.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return null;
        }
        catch (Exception ex)
        {
            return "Approval failed: " + ex.Message;
        }
    }

    public async Task<string?> ApproveHandoverBatchAsync(string groupId, int currentUserId, string? userRole)
    {
        var perms = await ResolvePermsAsync(currentUserId);
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
        var perms = await ResolvePermsAsync(currentUserId);
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
                item.status = WorkflowStatuses.WaitingAdminHandover;
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
                _ = NotifyUserAsync(firstMatched, "Konfirmasi Serah Terima", "Bukti serah terima Anda telah dikonfirmasi dan sedang menunggu verifikasi final dari Admin.");
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
        var perms = await ResolvePermsAsync(currentUserId);
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
        var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        
        var perms = await ResolvePermsAsync(currentUserId);
        if (!perms.Contains(Permissions.ApprovalReturn)) return "Unauthorized action.";
        
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
        var request = await _context.ProfileRequests.FirstOrDefaultAsync(pr => pr.id == id);
        if (request == null) return "Profile request not found.";
        
        var perms = await ResolvePermsAsync(currentUserId);
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
        var request = await _context.ProfileRequests.FirstOrDefaultAsync(pr => pr.id == id);
        if (request == null) return "Profile request not found.";
        
        var perms = await ResolvePermsAsync(currentUserId);
        if (!perms.Contains(Permissions.UsersManage)) return "Unauthorized action.";
        
        if (request.status != "PENDING") return "Profile request is not pending.";

        request.status = "REJECTED";
        request.updated_at = DateTime.UtcNow;

        _context.ProfileRequests.Update(request);
        await _context.SaveChangesAsync();

        return null;
    }

    /// <summary>Resolusi permission efektif user (User.role -> AdminRole, dengan fallback role lama).</summary>
    private async Task<HashSet<string>> ResolvePermsAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return new HashSet<string>();
        var role = await _context.AdminRoles.FirstOrDefaultAsync(r => r.RoleName == user.role && r.IsActive);
        return Permissions.Resolve(user.role, role?.Permissions);
    }

    private static bool CanApprove(Transaction transaction, HashSet<string> perms)
    {
        if (transaction.request_type == "GIVEAWAY")
        {
            return (transaction.status == WorkflowStatuses.PendingStaffInventory && perms.Contains(Permissions.ApprovalStage1)) ||
                   (transaction.status == WorkflowStatuses.PendingAdmin && perms.Contains(Permissions.ApprovalStage2)) ||
                   (transaction.status == WorkflowStatuses.PendingManager && perms.Contains(Permissions.ApprovalManager));
        }

        // BORROW: Stage 1 (approval.stage1) -> Stage 2 (approval.stage2). Legacy PENDING boleh oleh keduanya.
        return (transaction.status == WorkflowStatuses.PendingStaffInventory && perms.Contains(Permissions.ApprovalStage1)) ||
               (transaction.status == WorkflowStatuses.PendingAdmin && perms.Contains(Permissions.ApprovalStage2)) ||
               (transaction.status == WorkflowStatuses.Pending && (perms.Contains(Permissions.ApprovalStage1) || perms.Contains(Permissions.ApprovalStage2)));
    }

    private async Task<string?> ApproveGiveawayAsync(Transaction transaction, string? notes, int currentUserId, HashSet<string> perms)
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

        string stage = transaction.status == WorkflowStatuses.PendingAdmin ? "Staff Inventoris" :
                       transaction.status == WorkflowStatuses.PendingManager ? "Admin" : "Manager (Final, silakan proses pengambilan/serah terima)";
        _ = NotifyUserAsync(transaction, "Update Request WMS", $"Request Giveaway Anda telah disetujui oleh {stage}.");

        return null;
    }

    private async Task NotifyUserAsync(Transaction transaction, string subject, string message)
    {
        if (transaction.Requester != null && !string.IsNullOrWhiteSpace(transaction.Requester.email))
        {
            await _emailService.SendEmailAsync(transaction.Requester.email, subject, message);
        }
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
        var perms = await ResolvePermsAsync(currentUserId);
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
        var perms = await ResolvePermsAsync(currentUserId);
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
}

