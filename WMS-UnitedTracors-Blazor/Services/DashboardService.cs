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

    public async Task<(
        int WidgetTotalItems, 
        int PendingApprovals, 
        int TotalStock, 
        List<Category> Categories, 
        List<Division> Divisions, 
        List<Product> CatalogProducts, 
        int TotalItems, 
        int TotalPages)> 
        GetDashboardDataAsync(int? category, string? search = null, int page = 1, int pageSize = 15)
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

        return (widgetTotalItems, pendingApprovals, totalStock, categories, divisions, catalogProducts, catalogTotalItems, totalPages);
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
