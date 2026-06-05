using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;

namespace WMS_UnitedTracors_Blazor.Services;

public class ApprovalService
{
    private readonly ApplicationDbContext _context;

    public ApprovalService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(Dictionary<string, List<Transaction>> GroupedApprovals, List<Transaction> PendingReturns, List<ProfileRequest> PendingProfileRequests, Dictionary<string, List<Transaction>> GroupedHandovers)> GetApprovalsAsync(int currentUserId, string? userRole)
    {
        var query = _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Requester)
            .Include(t => t.Division)
            .OrderByDescending(t => t.created_at)
            .AsQueryable();

        if (userRole == "staff")
        {
            query = query.Where(t => t.requester_id == currentUserId);
        }
        else if (userRole == "manager")
        {
            query = query.Where(t => t.status == "PENDING_MANAGER" && t.request_type == "GIVEAWAY");
        }
        else if (userRole == "admin" || userRole == "superadmin")
        {
            query = query.Where(t => t.status == "PENDING");
        }

        var transactions = await query.ToListAsync();
        var groupedApprovals = transactions
            .GroupBy(t => $"{t.created_at:yyyy-MM-dd HH:mm}_{t.requester_id}_{t.applicant_name}")
            .ToDictionary(g => g.Key, g => g.ToList());

        var pendingReturns = new List<Transaction>();
        var pendingProfileRequests = new List<ProfileRequest>();
        var groupedHandovers = new Dictionary<string, List<Transaction>>();

        if (userRole == "admin" || userRole == "superadmin")
        {
            pendingReturns = await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Include(t => t.Division)
                .Include(t => t.Approver)
                .Where(t => t.status == "APPROVED" && t.pending_return_quantity > 0 && t.is_return_draft == 0)
                .OrderByDescending(t => t.updated_at)
                .ToListAsync();

            pendingProfileRequests = await _context.ProfileRequests
                .Include(pr => pr.User)
                .Include(pr => pr.Division)
                .Where(pr => pr.status == "PENDING")
                .OrderByDescending(pr => pr.created_at)
                .ToListAsync();

            var handoversQuery = await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Include(t => t.Division)
                .Where(t => t.status == "WAITING_ADMIN_HANDOVER" && t.type == "OUT" && (t.request_type == "GIVEAWAY" || t.request_type == "BORROW"))
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
        var transaction = await _context.Transactions.Include(t => t.Product).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        if (transaction.status != "PENDING" && transaction.status != "PENDING_MANAGER") return "Transaction is no longer pending.";

        if (userRole == "manager" && transaction.Product != null && transaction.Product.is_returnable == 1)
        {
            return "Managers cannot approve borrowing transactions. Only admins can.";
        }

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
                    transaction.status = "WAITING_HANDOVER";
                }
                else
                {
                    transaction.status = "APPROVED";
                    if (transaction.request_type == "GIVEAWAY")
                    {
                        transaction.returned_at = DateTime.UtcNow; // Mark finished
                    }
                }

                transaction.approver_id = currentUserId;
                transaction.updated_at = DateTime.UtcNow;

                if (userRole == "admin" || userRole == "superadmin")
                {
                    transaction.admin_notes = notes;
                }
                else if (userRole == "manager")
                {
                    transaction.manager_notes = notes;
                }

                _context.Transactions.Update(transaction);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
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
        if (transaction.status != "PENDING" && transaction.status != "PENDING_MANAGER") return "Transaction is no longer pending.";

        if (userRole == "manager" && transaction.Product != null && transaction.Product.is_returnable == 1)
        {
            return "Managers cannot reject borrowing transactions. Only admins can.";
        }

        transaction.status = "REJECTED";
        transaction.approver_id = currentUserId;
        transaction.rejection_reason = rejectionReason;
        transaction.updated_at = DateTime.UtcNow;

        if (userRole == "admin" || userRole == "superadmin")
        {
            transaction.admin_notes = rejectionReason;
        }
        else if (userRole == "manager")
        {
            transaction.manager_notes = rejectionReason;
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
        return null;
    }

    public async Task<string?> RequestRevisionAsync(int id, string revisionReason, int currentUserId, string? userRole)
    {
        var transaction = await _context.Transactions.Include(t => t.Product).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        if (transaction.status != "PENDING" && transaction.status != "PENDING_MANAGER") return "Transaction is no longer pending.";

        if (userRole == "manager" && transaction.Product != null && transaction.Product.is_returnable == 1)
            return "Managers cannot request revision for borrowing transactions. Only admins can.";

        if (string.IsNullOrWhiteSpace(revisionReason)) return "Catatan revisi wajib diisi.";

        transaction.status = "REVISION";
        transaction.approver_id = currentUserId;
        transaction.rejection_reason = revisionReason;
        transaction.updated_at = DateTime.UtcNow;

        if (userRole == "admin" || userRole == "superadmin")
        {
            transaction.admin_notes = revisionReason;
        }
        else if (userRole == "manager")
        {
            transaction.manager_notes = revisionReason;
        }

        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> ApproveReturnAsync(int id, int currentUserId, string? userRole)
    {
        var transaction = await _context.Transactions.Include(t => t.Product).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        if (transaction.status != "APPROVED" || transaction.pending_return_quantity <= 0 || transaction.is_return_draft != 0)
            return "Transaction is not pending return approval.";

        if (transaction.Product == null) return "Associated product not found.";

        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var stockBefore = transaction.Product.current_stock;
                transaction.Product.current_stock += transaction.pending_return_quantity ?? 0;
                _context.Products.Update(transaction.Product);

                if (transaction.product_variant_id.HasValue)
                {
                    var variant = await _context.ProductVariants.FindAsync(transaction.product_variant_id.Value);
                    if (variant != null)
                    {
                        variant.stock += transaction.pending_return_quantity ?? 0;
                        _context.ProductVariants.Update(variant);
                    }
                }

                var stockLog = new StockLog
                {
                    transaction_id = transaction.id,
                    product_id = transaction.Product.id,
                    stock_before = stockBefore,
                    stock_after = transaction.Product.current_stock,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };
                _context.StockLogs.Add(stockLog);

                transaction.returned_quantity = (transaction.returned_quantity ?? 0) + (transaction.pending_return_quantity ?? 0);
                transaction.pending_return_quantity = 0;
                if (transaction.returned_quantity >= transaction.quantity)
                {
                    transaction.returned_at = DateTime.UtcNow; // Tandai selesai dikembalikan
                }
                transaction.updated_at = DateTime.UtcNow;
                _context.Transactions.Update(transaction);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return null;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return "Approval failed: " + ex.Message;
            }
        });
    }

    public async Task<string?> ApproveHandoverBatchAsync(string groupId, int currentUserId, string? userRole)
    {
        if (userRole != "admin" && userRole != "superadmin") return "Unauthorized action.";

        var query = await _context.Transactions
            .Where(t => t.status == "WAITING_ADMIN_HANDOVER" && t.type == "OUT" && (t.request_type == "GIVEAWAY" || t.request_type == "BORROW"))
            .ToListAsync();

        var matched = query.Where(t => t.group_id == groupId).ToList();

        if (matched.Count == 0) return "Tidak ada transaksi serah terima yang menunggu verifikasi pada event ini.";

        foreach (var item in matched)
        {
            item.status = "APPROVED";
            item.approver_id = currentUserId;
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> RejectHandoverBatchAsync(string groupId, string rejectionReason, int currentUserId, string? userRole)
    {
        if (userRole != "admin" && userRole != "superadmin") return "Unauthorized action.";
        if (string.IsNullOrWhiteSpace(rejectionReason)) return "Rejection reason is required.";

        var query = await _context.Transactions
            .Where(t => t.status == "WAITING_ADMIN_HANDOVER" && t.type == "OUT" && (t.request_type == "GIVEAWAY" || t.request_type == "BORROW"))
            .ToListAsync();

        var matched = query.Where(t => t.group_id == groupId).ToList();

        if (matched.Count == 0) return "Tidak ada transaksi serah terima yang menunggu verifikasi pada event ini.";

        foreach (var item in matched)
        {
            item.status = "WAITING_HANDOVER";
            item.rejection_reason = rejectionReason;
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> RejectReturnAsync(int id, string? rejectionReason, int currentUserId, string? userRole)
    {
        var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        if (transaction.status != "APPROVED" || transaction.pending_return_quantity <= 0 || transaction.is_return_draft != 0)
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
        if (request.status != "PENDING") return "Profile request is not pending.";

        request.status = "REJECTED";
        request.updated_at = DateTime.UtcNow;

        _context.ProfileRequests.Update(request);
        await _context.SaveChangesAsync();

        return null;
    }
}
