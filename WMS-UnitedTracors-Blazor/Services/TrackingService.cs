using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;

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
            .Include(t => t.Product).ThenInclude(p => p.Category)
            .Include(t => t.Product).ThenInclude(p => p.Unit)
            .Include(t => t.ProductVariant)
            .Include(t => t.Requester)
            .Include(t => t.Approver)
            .Include(t => t.Division)
            .Where(t => t.type == "OUT");

        query = query.Where(t => 
            (t.request_type == "GIVEAWAY" && (t.status == "PENDING" || t.status == "PENDING_MANAGER" || t.status == "REVISION")) ||
            (t.request_type == "BORROW" && (t.status == "PENDING" || t.status == "REVISION" || t.status == "APPROVED"))
        );

        if (userRole == "staff")
        {
            query = query.Where(t => t.requester_id == currentUserId);
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