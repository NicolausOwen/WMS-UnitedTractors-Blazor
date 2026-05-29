using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class CategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<CategoryDto> Categories, int TotalItems, int TotalPages)> GetCategoriesAsync(int page = 1, int pageSize = 10)
    {
        var query = _context.Categories.AsQueryable();
        
        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var categories = await query
            .OrderBy(c => c.name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CategoryDto
            {
                Id = c.id,
                Name = c.name,
                Description = c.description,
                ProductsCount = c.Products.Count()
            })
            .ToListAsync();

        return (categories, totalItems, totalPages);
    }

    public async Task<string?> CreateCategoryAsync(CategoryViewModel model)
    {
        if (await _context.Categories.AnyAsync(c => c.name == model.name))
        {
            return "Nama kategori sudah ada.";
        }

        var category = new Category
        {
            name = model.name,
            description = model.description,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return null; // Null means success
    }

    public async Task<string?> UpdateCategoryAsync(int id, CategoryViewModel model)
    {
        if (await _context.Categories.AnyAsync(c => c.name == model.name && c.id != id))
        {
            return "Nama kategori sudah digunakan oleh kategori lain.";
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null) return "Kategori tidak ditemukan.";

        category.name = model.name;
        category.description = model.description;
        category.updated_at = DateTime.UtcNow;

        _context.Update(category);
        await _context.SaveChangesAsync();

        return null;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProductsCount { get; set; }
}
