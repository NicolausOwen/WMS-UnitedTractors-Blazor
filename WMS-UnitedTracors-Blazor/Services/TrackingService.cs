using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using WMS_UnitedTracors_Blazor.Helpers;

namespace WMS_UnitedTracors_Blazor.Services;

public class TrackingService
{
    private readonly ApplicationDbContext _context;

    public TrackingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Transaction>> GetTrackingDataAsync(
        int? divisionId, 
        string? productSearch, 
        string? returnStatus, 
        DateTime? dateFrom, 
        DateTime? dateTo, 
        int currentUserId, 
        string? userRole)
    {
        var query = _context.Transactions
            .Include(t => t.Product).ThenInclude(p => p!.Category)
            .Include(t => t.Product).ThenInclude(p => p!.Unit)
            .Include(t => t.ProductVariant)
            .Include(t => t.Requester)
            .Include(t => t.Approver)
            .Include(t => t.Division)
            .Where(t => t.type == "OUT");

        query = query.Where(t =>
            (t.request_type == "GIVEAWAY" && (
                t.status == WorkflowStatuses.PendingStaffInventory ||
                t.status == WorkflowStatuses.PendingAdmin ||
                t.status == WorkflowStatuses.PendingManager ||
                t.status == WorkflowStatuses.Revision ||
                t.status == WorkflowStatuses.RevisionByStaffInventory ||
                t.status == WorkflowStatuses.RevisionByAdmin ||
                t.status == WorkflowStatuses.RevisionByManager ||
                t.status == WorkflowStatuses.WaitingHandover ||
                t.status == WorkflowStatuses.WaitingDocumentation ||
                t.status == WorkflowStatuses.DocumentationOverdue
            )) ||
            (t.request_type == "BORROW" && (
                t.status == WorkflowStatuses.Pending ||
                t.status == WorkflowStatuses.PendingStaffInventory ||
                t.status == WorkflowStatuses.PendingAdmin ||
                t.status == WorkflowStatuses.Revision ||
                t.status == WorkflowStatuses.RevisionByStaffInventory ||
                t.status == WorkflowStatuses.RevisionByAdmin ||
                t.status == WorkflowStatuses.WaitingHandover ||
                t.status == WorkflowStatuses.WaitingAdminHandover ||
                (t.status == WorkflowStatuses.Approved && (t.returned_quantity == null || t.returned_quantity < t.quantity))
            )));

        // Scope tracking berdasarkan permission (bukan role string), agar role custom tetap benar.
        var actor = await _context.Users.FindAsync(currentUserId);
        var actorRole = actor != null ? await _context.AdminRoles.FirstOrDefaultAsync(r => r.RoleName == actor.role && r.IsActive) : null;
        var perms = Permissions.Resolve(actor?.role, actorRole?.Permissions);

        bool isSuperAdmin = Permissions.All.All(p => perms.Contains(p));
        bool canManageOrApprove = perms.Contains(Permissions.ApprovalStage1) || perms.Contains(Permissions.ApprovalStage2) ||
                                  perms.Contains(Permissions.ApprovalHandover) || perms.Contains(Permissions.ApprovalReturn) ||
                                  perms.Contains(Permissions.ProductsManage);

        if (!canManageOrApprove && perms.Contains(Permissions.ApprovalManager))
        {
            // Manager (hanya approval manager) -> lihat giveaway saja.
            query = query.Where(t => t.request_type == "GIVEAWAY");
        }
        else if (!canManageOrApprove)
        {
            // Pemohon biasa -> hanya request miliknya sendiri.
            query = query.Where(t => t.requester_id == currentUserId);
        }
        else
        {
            // Approver/Admin -> semua, dengan scope kategori (jika admin per-kategori).
            var userAdminRoles = await _context.UserAdminRoles.Where(uar => uar.UserId == currentUserId).ToListAsync();
            bool isGlobalAdmin = isSuperAdmin || userAdminRoles.Any(uar => uar.CategoryId == null);
            if (!isGlobalAdmin)
            {
                var allowedCategoryIds = userAdminRoles.Where(uar => uar.CategoryId != null).Select(uar => uar.CategoryId!.Value).ToList();
                query = query.Where(t => t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value));
            }
        }

        if (divisionId.HasValue)
        {
            query = query.Where(t => t.division_id == divisionId.Value);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(t => t.updated_at >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            var dateToInclusive = dateTo.Value.Date.AddDays(1);
            query = query.Where(t => t.updated_at < dateToInclusive);
        }

        var overdueItems = await _context.Transactions
            .Where(t =>
                t.request_type == "GIVEAWAY" &&
                t.status == WorkflowStatuses.WaitingDocumentation &&
                t.event_date.HasValue &&
                t.event_date.Value.Date.AddDays(3) < DateTime.Today)
            .ToListAsync();

        if (overdueItems.Any())
        {
            foreach (var item in overdueItems)
            {
                item.status = WorkflowStatuses.DocumentationOverdue;
                item.updated_at = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        var allUsages = await query.OrderByDescending(t => t.updated_at).ToListAsync();

        return allUsages;
    }

    public string CreateTrackingGroupId(Transaction t)
    {
        var raw = $"{t.created_at:yyyy-MM-dd HH:mm}_{t.requester_id}_{t.applicant_name}_{t.request_type}";
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
