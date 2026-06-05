using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;

namespace WMS_UnitedTracors_Blazor.Services;

public class DashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public class DashboardData
    {
        public int WidgetTotalItems { get; set; }
        public int PendingApprovals { get; set; }
        public int TotalStock { get; set; }
        public List<Category> Categories { get; set; } = new();
        public List<Division> Divisions { get; set; } = new();
        public List<Product> CatalogProducts { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public List<Product> LowStockProducts { get; set; } = new();
        public int DonutApproved { get; set; }
        public int DonutRejected { get; set; }
        public int DonutPending { get; set; }
        public int MerchAvailable { get; set; }
        public int MerchLowStock { get; set; }
        public int MerchOutOfStock { get; set; }
        public int BorrowOverdue { get; set; }
        public int BorrowAvailable { get; set; }
        public int BorrowBorrowed { get; set; }
    }

    public async Task<DashboardData> GetDashboardDataAsync(int? category, string? search = null, int page = 1, int pageSize = 15)
    {
        var widgetTotalItems = await _context.Products.CountAsync();
        var pendingApprovals = await _context.Transactions.CountAsync(t => t.status == "PENDING");
        var totalStock = await _context.Products.SumAsync(p => (int?)p.current_stock) ?? 0;
        
        var categories = await _context.Categories.OrderBy(c => c.name).ToListAsync();
        var divisions = await _context.Divisions.OrderBy(d => d.name).ToListAsync();

        var catalogQuery = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Location)
            .Include(p => p.Unit)
            .Include(p => p.ProductVariants)
            .OrderBy(p => p.name)
            .AsQueryable();

        if (category.HasValue)
        {
            catalogQuery = catalogQuery.Where(p => p.category_id == category.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            catalogQuery = catalogQuery.Where(p => p.name.Contains(search) || p.sku.Contains(search));
        }

        int catalogTotalItems = await catalogQuery.CountAsync();
        int totalPages = (int)Math.Ceiling(catalogTotalItems / (double)pageSize);

        var catalogProducts = await catalogQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var data = new DashboardData
        {
            WidgetTotalItems = widgetTotalItems,
            PendingApprovals = pendingApprovals,
            TotalStock = totalStock,
            Categories = categories,
            Divisions = divisions,
            CatalogProducts = catalogProducts,
            TotalItems = catalogTotalItems,
            TotalPages = totalPages,
            LowStockProducts = await _context.Products.Include(p => p.Unit).Where(p => p.current_stock <= 2 && p.current_stock >= 0).OrderBy(p => p.current_stock).Take(10).ToListAsync(),
            DonutApproved = await _context.Transactions.CountAsync(t => t.status == "APPROVED"),
            DonutRejected = await _context.Transactions.CountAsync(t => t.status == "REJECTED"),
            DonutPending = await _context.Transactions.CountAsync(t => t.status == "PENDING" || t.status == "PENDING_MANAGER"),
            MerchAvailable = await _context.Products.Where(p => p.is_returnable != 1 && p.current_stock > 2).CountAsync(),
            MerchLowStock = await _context.Products.Where(p => p.is_returnable != 1 && p.current_stock > 0 && p.current_stock <= 2).CountAsync(),
            MerchOutOfStock = await _context.Products.Where(p => p.is_returnable != 1 && p.current_stock <= 0).CountAsync(),
            BorrowOverdue = await _context.Transactions.Where(t => t.request_type == "BORROW" && t.status == "APPROVED" && t.returned_quantity < t.quantity && t.expected_return_date < DateTime.Today).CountAsync(),
            BorrowAvailable = await _context.Products.Where(p => p.is_returnable == 1 && p.current_stock > 0).CountAsync(),
            BorrowBorrowed = await _context.Transactions.Where(t => t.request_type == "BORROW" && t.status == "APPROVED" && t.returned_quantity < t.quantity).CountAsync()
        };

        return data;
    }

    public async Task<List<Transaction>> GetActiveBorrowsAsync(int currentUserId, string? userRole)
    {
        var activeBorrowsQuery = _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Requester)
            .Where(t => t.type == "OUT" && t.status == "APPROVED" && 
                        t.Product != null && t.Product.is_returnable == 1 &&
                        (t.quantity - (t.returned_quantity) - t.pending_return_quantity) > 0)
            .AsQueryable();

        if (userRole != "superadmin")
        {
            activeBorrowsQuery = activeBorrowsQuery.Where(t => t.requester_id == currentUserId);
        }

        return await activeBorrowsQuery.ToListAsync();
    }
}
