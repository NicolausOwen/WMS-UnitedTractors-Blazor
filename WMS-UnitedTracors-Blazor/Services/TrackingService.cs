using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using WMS_UnitedTracors_Blazor.Helpers;

namespace WMS_UnitedTracors_Blazor.Services;

public class TrackingService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public TrackingService(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
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
        using var _context = _factory.CreateDbContext();
        var baseQuery = _context.Transactions
            .Include(t => t.Product).ThenInclude(p => p!.Category)
            .Include(t => t.Product).ThenInclude(p => p!.Unit)
            .Include(t => t.ProductVariant)
            .Include(t => t.Requester)
            .Include(t => t.Approver)
            .Include(t => t.Division)
            .Where(t => t.type == "OUT");

        // Active statuses shown to approvers/admins (in-progress work)
        var activeStatusFilter = baseQuery.Where(t =>
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
                t.status == WorkflowStatuses.WaitingHandoverConfirm ||
                t.status == WorkflowStatuses.WaitingAdminHandover ||
                (t.status == WorkflowStatuses.Approved && (t.returned_quantity == null || t.returned_quantity < t.quantity))
            )));

        // Resolve permissions
        var actor = await _context.Users.FindAsync(currentUserId);
        var actorRole = actor != null ? await _context.AdminRoles.FirstOrDefaultAsync(r => r.RoleName == actor.role && r.IsActive) : null;
        var perms = Permissions.Resolve(actor?.role, actorRole?.Permissions);

        bool isSuperAdmin = Permissions.All.All(p => perms.Contains(p));
        bool canManageOrApprove = perms.Contains(Permissions.ApprovalStage1) || perms.Contains(Permissions.ApprovalStage2) ||
                                  perms.Contains(Permissions.ApprovalHandover) ||
                                  perms.Contains(Permissions.ProductsManage);

        IQueryable<Transaction> query;

        if (!canManageOrApprove && perms.Contains(Permissions.ApprovalManager))
        {
            // Manager (only manager approval) -> giveaway in active states only + their own requests.
            query = baseQuery.Where(t => 
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
                t.requester_id == currentUserId
            );
        }
        else if (!canManageOrApprove)
        {
            // Regular user -> ALL of their own requests (active + full history).
            // This ensures the user can always track every request they ever made.
            query = baseQuery.Where(t => t.requester_id == currentUserId);
        }
        else
        {
            // Approver/Admin -> active in-progress items, scoped by category.
            query = activeStatusFilter;
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

        if (!string.IsNullOrWhiteSpace(productSearch))
        {
            query = query.Where(t => t.Product != null && t.Product.name.Contains(productSearch));
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
