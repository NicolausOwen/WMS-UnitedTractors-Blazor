using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class ProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductService(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<(List<Product> Products, int TotalItems, int TotalPages)> GetProductsAsync(int? category, string? search, int page = 1, int pageSize = 10)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Location)
            .Include(p => p.Unit)
            .OrderByDescending(p => p.created_at)
            .AsQueryable();

        if (category.HasValue)
        {
            query = query.Where(p => p.category_id == category.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.name.Contains(search) || p.sku.Contains(search));
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalItems, totalPages);
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products.Include(p => p.Unit).FirstOrDefaultAsync(p => p.id == id);
    }

    public async Task<string?> CreateProductAsync(ProductCreateViewModel model)
    {
        if (await _context.Products.AnyAsync(p => p.name == model.name))
        {
            return "Nama produk ini sudah ada di dalam sistem. Gunakan nama lain.";
        }

        if (!string.IsNullOrEmpty(model.sku) && await _context.Products.AnyAsync(p => p.sku == model.sku))
        {
            return "SKU atau Barcode ini sudah terdaftar pada produk lain.";
        }

        var unitName = model.unit.Trim().ToLower();
        var unit = await _context.Units.FirstOrDefaultAsync(u => u.name.ToLower() == unitName);
        if (unit == null)
        {
            unit = new Unit { name = model.unit.Trim(), created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow };
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();
        }

        var product = new Product
        {
            barcode_type = model.barcode_type,
            name = model.name,
            value = model.value ?? 0,
            category_id = model.category_id,
            location_id = model.location_id,
            unit_id = unit.id,
            is_returnable = model.is_returnable ? 1 : 0,
            initial_stock = model.initial_stock ?? 0,
            current_stock = model.initial_stock ?? 0,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        if (string.IsNullOrEmpty(model.sku))
        {
            var count = await _context.Products.CountAsync();
            product.sku = $"UT-{DateTime.Now:yyMMdd}-{(count + 1):D4}";
        }
        else
        {
            product.sku = model.sku;
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return null;
    }

    public async Task<string?> UpdateProductAsync(int id, ProductUpdateViewModel model)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return "Produk tidak ditemukan.";

        if (await _context.Products.AnyAsync(p => p.name == model.name && p.id != id))
        {
            return "Nama produk ini sudah ada di dalam sistem. Gunakan nama lain.";
        }

        if (await _context.Products.AnyAsync(p => p.sku == model.sku && p.id != id))
        {
            return "SKU atau Barcode ini sudah terdaftar pada produk lain.";
        }

        var unitName = model.unit.Trim().ToLower();
        var unit = await _context.Units.FirstOrDefaultAsync(u => u.name.ToLower() == unitName);
        if (unit == null)
        {
            unit = new Unit { name = model.unit.Trim(), created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow };
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();
        }

        product.sku = model.sku;
        product.barcode_type = model.barcode_type;
        product.name = model.name;
        product.value = model.value ?? 0;
        product.category_id = model.category_id;
        product.location_id = model.location_id;
        product.unit_id = unit.id;
        product.current_stock = model.current_stock;
        product.initial_stock = model.initial_stock;
        product.is_returnable = model.is_returnable ? 1 : 0;
        product.updated_at = DateTime.UtcNow;

        _context.Update(product);
        await _context.SaveChangesAsync();

        return null;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            if (!string.IsNullOrEmpty(product.image))
            {
                var path = Path.Combine(_env.ContentRootPath, "Storage", product.image);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            if (!string.IsNullOrEmpty(product.position_image))
            {
                var path = Path.Combine(_env.ContentRootPath, "Storage", product.position_image);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<List<Product>> GetProductsByTypeAsync(string type, int id)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Location)
            .Include(p => p.Unit)
            .AsQueryable();

        if (type == "category") query = query.Where(p => p.category_id == id);
        else if (type == "location") query = query.Where(p => p.location_id == id);
        else if (type == "unit") query = query.Where(p => p.unit_id == id);
        else if (type == "division")
        {
            var productIds = await _context.Transactions
                .Where(t => t.division_id == id)
                .Select(t => t.product_id)
                .Distinct()
                .ToListAsync();
            
            query = query.Where(p => productIds.Contains(p.id));
        }
        else
        {
            return new List<Product>();
        }

        return await query.ToListAsync();
    }
}
