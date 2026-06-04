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

public class ExecutiveSummaryDto
{
    public int TotalBorrowed { get; set; }
    public int TotalReturned { get; set; }
    public int MasihDipinjam { get; set; }
    public int TotalItemsOut { get; set; }
    public int PendingReturns { get; set; }
    
    public List<string> ChartLabels { get; set; } = new();
    public List<int> ChartPeminjaman { get; set; } = new();
    public List<int> ChartBarangKeluar { get; set; } = new();
    
    public List<Transaction> RecentTransactions { get; set; } = new();
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
            { "peminjaman", new ReportSummaryDto { Count = peminjaman.Count, TotalQty = peminjaman.Select(x => x.quantity ?? 0).Sum(), Approved = peminjaman.Count(x => x.status == "APPROVED"), Pending = peminjaman.Count(x => x.status == "PENDING") } },
            { "request_barang", new ReportSummaryDto { Count = requestBarang.Count, TotalQty = requestBarang.Select(x => x.quantity ?? 0).Sum(), Approved = requestBarang.Count(x => x.status == "APPROVED"), Pending = requestBarang.Count(x => x.status == "PENDING") } },
            { "adjust_barang", new ReportSummaryDto { Count = adjustBarang.Count, TotalQty = adjustBarang.Select(x => x.quantity ?? 0).Sum(), Approved = adjustBarang.Count(x => x.status == "APPROVED"), Pending = adjustBarang.Count(x => x.status == "PENDING") } }
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
    
    public async Task<ExecutiveSummaryDto> GetExecutiveSummaryAsync(DateTime? startDateParam, DateTime? endDateParam)
    {
        var dates = ApplyDateFilter(startDateParam, endDateParam);
        var startDate = dates.Item1;
        var endDate = dates.Item2;
        
        var summary = new ExecutiveSummaryDto();
        
        summary.TotalBorrowed = await _context.Transactions.Where(t => t.type == "OUT" && t.request_type == "BORROW" && t.status == "APPROVED").SumAsync(t => (int?)t.quantity) ?? 0;
        summary.TotalReturned = await _context.Transactions.Where(t => t.type == "OUT" && t.request_type == "BORROW" && t.status == "APPROVED").SumAsync(t => (int?)t.returned_quantity) ?? 0;
        summary.TotalItemsOut = await _context.Transactions.Where(t => t.type == "OUT" && t.status == "APPROVED").SumAsync(t => (int?)t.quantity) ?? 0;
        summary.PendingReturns = await _context.Transactions.Where(t => t.status == "APPROVED" && t.pending_return_quantity > 0).CountAsync();
        summary.MasihDipinjam = summary.TotalBorrowed - summary.TotalReturned;
        
        var recentTransactions = await _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Division)
            .Where(t => t.created_at >= startDate && t.created_at <= endDate)
            .OrderByDescending(t => t.created_at)
            .Take(10)
            .ToListAsync();
            
        summary.RecentTransactions = recentTransactions;
        
        var elevenMonthsAgo = DateTime.Now.AddMonths(-11);
        var startOfElevenMonthsAgo = new DateTime(elevenMonthsAgo.Year, elevenMonthsAgo.Month, 1);

        var peminjamanDataRaw = await _context.Transactions
            .Where(t => t.type == "OUT" && t.request_type == "BORROW" && t.status == "APPROVED" && t.created_at >= startOfElevenMonthsAgo)
            .GroupBy(t => new { t.created_at.Year, t.created_at.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(x => x.quantity) })
            .ToListAsync();

        var barangKeluarDataRaw = await _context.Transactions
            .Where(t => t.type == "OUT" && t.status == "APPROVED" && t.created_at >= startOfElevenMonthsAgo)
            .GroupBy(t => new { t.created_at.Year, t.created_at.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(x => x.quantity) })
            .ToListAsync();

        for (int i = 11; i >= 0; i--)
        {
            var date = DateTime.Now.AddMonths(-i);
            summary.ChartLabels.Add(date.ToString("MMM yyyy", System.Globalization.CultureInfo.InvariantCulture));
            
            var pemTotal = peminjamanDataRaw.FirstOrDefault(p => p.Year == date.Year && p.Month == date.Month)?.Total ?? 0;
            var bkTotal = barangKeluarDataRaw.FirstOrDefault(p => p.Year == date.Year && p.Month == date.Month)?.Total ?? 0;
            
            summary.ChartPeminjaman.Add(pemTotal);
            summary.ChartBarangKeluar.Add(bkTotal);
        }
        
        return summary;
    }
}
