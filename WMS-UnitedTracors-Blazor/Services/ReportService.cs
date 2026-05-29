using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;

namespace WMS_UnitedTracors_Blazor.Services;

public class ReportSummaryDto
{
    public int Count { get; set; }
    public int TotalQty { get; set; }
    public int Approved { get; set; }
    public int Pending { get; set; }
}

public class ReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    private Tuple<DateTime, DateTime> ApplyDateFilter(DateTime? startDateParam, DateTime? endDateParam)
    {
        var startDate = startDateParam ?? DateTime.Now.AddMonths(-1).Date;
        var endDate = endDateParam ?? DateTime.Now.Date;
        endDate = endDate.Date.AddDays(1).AddTicks(-1);
        return new Tuple<DateTime, DateTime>(startDate, endDate);
    }

    private IQueryable<Transaction> GetFilteredQuery(string type, string? requestType, DateTime startDate, DateTime endDate, int? divisionId, string? status)
    {
        var query = _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Requester)
            .Include(t => t.Approver)
            .Include(t => t.Division)
            .Where(t => t.type == type && t.created_at >= startDate && t.created_at <= endDate);

        if (!string.IsNullOrEmpty(requestType))
        {
            query = query.Where(t => t.request_type == requestType);
        }

        if (type == "OUT" && divisionId.HasValue)
        {
            query = query.Where(t => t.division_id == divisionId.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(t => t.status == status);
        }

        return query;
    }

    public async Task<Dictionary<string, ReportSummaryDto>> GetSummaryAsync(DateTime? startDateParam, DateTime? endDateParam)
    {
        var dates = ApplyDateFilter(startDateParam, endDateParam);
        var startDate = dates.Item1;
        var endDate = dates.Item2;

        var baseQuery = _context.Transactions
            .Where(t => t.created_at >= startDate && t.created_at <= endDate);

        var peminjaman = await baseQuery.Where(t => t.type == "OUT" && t.request_type == "BORROW").ToListAsync();
        var requestBarang = await baseQuery.Where(t => t.type == "OUT" && t.request_type == "GIVEAWAY").ToListAsync();
        var adjustBarang = await baseQuery.Where(t => t.type == "IN").ToListAsync();

        return new Dictionary<string, ReportSummaryDto>
        {
            { "peminjaman", new ReportSummaryDto { Count = peminjaman.Count, TotalQty = peminjaman.Sum(x => x.quantity ?? 0), Approved = peminjaman.Count(x => x.status == "APPROVED"), Pending = peminjaman.Count(x => x.status == "PENDING") } },
            { "request_barang", new ReportSummaryDto { Count = requestBarang.Count, TotalQty = requestBarang.Sum(x => x.quantity ?? 0), Approved = requestBarang.Count(x => x.status == "APPROVED"), Pending = requestBarang.Count(x => x.status == "PENDING") } },
            { "adjust_barang", new ReportSummaryDto { Count = adjustBarang.Count, TotalQty = adjustBarang.Sum(x => x.quantity ?? 0), Approved = adjustBarang.Count(x => x.status == "APPROVED"), Pending = adjustBarang.Count(x => x.status == "PENDING") } }
        };
    }

    public async Task<(List<Transaction> Transactions, int TotalItems, int TotalPages)> GetReportDataAsync(string type, string? requestType, DateTime? startDateParam, DateTime? endDateParam, int? divisionId, string? status, int page = 1, int pageSize = 20)
    {
        var dates = ApplyDateFilter(startDateParam, endDateParam);
        var query = GetFilteredQuery(type, requestType, dates.Item1, dates.Item2, divisionId, status);

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var transactions = await query
            .OrderByDescending(t => t.created_at)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (transactions, totalItems, totalPages);
    }
}
