using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class UnitService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public UnitService(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<(List<UnitDto> Units, int TotalItems, int TotalPages)> GetUnitsAsync(int page = 1, int pageSize = 10)
    {
        using var _context = _factory.CreateDbContext();
        var query = _context.Units.AsQueryable();

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var units = await query
            .OrderBy(u => u.name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var unitList = new List<UnitDto>();
        foreach (var u in units)
        {
            var productsCount = await _context.Products.CountAsync(p => p.unit_id == u.id);
            unitList.Add(new UnitDto
            {
                Id = u.id,
                Name = u.name ?? "",
                ProductsCount = productsCount
            });
        }

        return (unitList, totalItems, totalPages);
    }

    public async Task<string?> CreateUnitAsync(UnitViewModel model)
    {
        using var _context = _factory.CreateDbContext();
        if (await _context.Units.AnyAsync(u => u.name == model.name))
            return "Nama satuan sudah ada.";

        var unit = new Unit
        {
            name = model.name,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        _context.Units.Add(unit);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateUnitAsync(int id, UnitViewModel model)
    {
        using var _context = _factory.CreateDbContext();
        if (await _context.Units.AnyAsync(u => u.name == model.name && u.id != id))
            return "Nama satuan sudah digunakan.";

        var unit = await _context.Units.FindAsync(id);
        if (unit == null) return "Satuan tidak ditemukan.";

        unit.name = model.name;
        unit.updated_at = DateTime.UtcNow;

        _context.Update(unit);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<bool> DeleteUnitAsync(int id)
    {
        using var _context = _factory.CreateDbContext();
        var unit = await _context.Units.FindAsync(id);
        if (unit != null)
        {
            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}

public class UnitDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int ProductsCount { get; set; }
}
