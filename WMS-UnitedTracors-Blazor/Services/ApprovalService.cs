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

    public async Task<(Dictionary<string, List<Transaction>> GroupedApprovals, List<Transaction> PendingReturns, List<ProfileRequest> PendingProfileRequests)> GetApprovalsAsync(int currentUserId, string? userRole)
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
        else
        {
            query = query.Where(t => t.status == "PENDING");
            if (userRole == "manager")
            {
                query = query.Where(t => t.Product.is_returnable == 0);
            }
        }

        var transactions = await query.ToListAsync();
        var groupedApprovals = transactions
            .GroupBy(t => $"{t.created_at:yyyy-MM-dd HH:mm}_{t.requester_id}_{t.applicant_name}")
            .ToDictionary(g => g.Key, g => g.ToList());

        var pendingReturns = new List<Transaction>();
        var pendingProfileRequests = new List<ProfileRequest>();

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
        }

        return (groupedApprovals, pendingReturns, pendingProfileRequests);
    }

    public async Task<string?> ApproveAsync(int id, int currentUserId, string? userRole)
    {
        var transaction = await _context.Transactions.Include(t => t.Product).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        if (transaction.status != "PENDING") return "Transaction is no longer pending.";

        if (userRole == "manager" && transaction.Product.is_returnable == 1)
        {
            return "Managers cannot approve borrowing transactions. Only admins can.";
        }

        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var stockBefore = transaction.Product.current_stock;

            if (transaction.type == "IN")
            {
                transaction.Product.current_stock += transaction.quantity ?? 0;
            }
            else
            {
                if (transaction.Product.current_stock < transaction.quantity) throw new Exception("Insufficient stock");
                transaction.Product.current_stock -= transaction.quantity ?? 0;
            }

            _context.Products.Update(transaction.Product);

            if (transaction.product_variant_id.HasValue)
            {
                var variant = await _context.ProductVariants.FindAsync(transaction.product_variant_id.Value);
                if (variant != null)
                {
                    if (transaction.type == "IN") variant.stock += transaction.quantity ?? 0;
                    else
                    {
                        if (variant.stock < transaction.quantity) throw new Exception("Insufficient variant stock");
                        variant.stock -= transaction.quantity ?? 0;
                    }
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

            transaction.status = "APPROVED";
            transaction.approver_id = currentUserId;
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
    }

    public async Task<string?> RejectAsync(int id, string rejectionReason, int currentUserId, string? userRole)
    {
        var transaction = await _context.Transactions.Include(t => t.Product).Include(t => t.Requester).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";
        if (transaction.status != "PENDING") return "Transaction is no longer pending.";

        if (userRole == "manager" && transaction.Product.is_returnable == 1)
        {
            return "Managers cannot reject borrowing transactions. Only admins can.";
        }

        transaction.status = "REJECTED";
        transaction.approver_id = currentUserId;
        transaction.rejection_reason = rejectionReason;
        transaction.updated_at = DateTime.UtcNow;

        if (transaction.type == "OUT" && transaction.Product != null && transaction.Requester != null)
        {
            if (transaction.request_type == "GIVEAWAY")
            {
                int pointsToRefund = transaction.Product.value * (transaction.quantity ?? 0);
                if (pointsToRefund > 0)
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
        if (transaction.status != "PENDING") return "Transaction is no longer pending.";

        if (userRole == "manager" && transaction.Product.is_returnable == 1)
            return "Managers cannot request revision for borrowing transactions. Only admins can.";

        if (string.IsNullOrWhiteSpace(revisionReason)) return "Catatan revisi wajib diisi.";

        transaction.status = "REVISION";
        transaction.approver_id = currentUserId;
        transaction.rejection_reason = revisionReason; 
        transaction.updated_at = DateTime.UtcNow;

        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return null;
    }

    // Other batch methods omitted for brevity, but they follow the same pattern of wrapping the DbContext calls.
}
