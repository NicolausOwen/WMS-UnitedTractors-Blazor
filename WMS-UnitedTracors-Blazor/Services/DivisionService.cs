using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class DivisionService
{
    private readonly ApplicationDbContext _context;

    public DivisionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<DivisionDto> Divisions, int TotalItems, int TotalPages)> GetDivisionsAsync(int page = 1, int pageSize = 10)
    {
        var query = _context.Divisions.AsQueryable();

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var divisions = await query
            .OrderBy(d => d.name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var divisionList = new List<DivisionDto>();
        foreach (var div in divisions)
        {
            var productsCount = await _context.Transactions
                .Where(t => t.division_id == div.id)
                .Select(t => t.product_id)
                .Distinct()
                .CountAsync();

            divisionList.Add(new DivisionDto
            {
                Id = div.id,
                Name = div.name ?? "",
                Description = div.description,
                ProductsCount = productsCount
            });
        }

        return (divisionList, totalItems, totalPages);
    }

    public async Task<string?> CreateDivisionAsync(DivisionViewModel model)
    {
        if (await _context.Divisions.AnyAsync(d => d.name == model.name))
            return "Nama divisi sudah ada.";

        var division = new Division
        {
            name = model.name,
            description = model.description,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        _context.Divisions.Add(division);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateDivisionAsync(int id, DivisionViewModel model)
    {
        if (await _context.Divisions.AnyAsync(d => d.name == model.name && d.id != id))
            return "Nama divisi sudah digunakan.";

        var division = await _context.Divisions.FindAsync(id);
        if (division == null) return "Divisi tidak ditemukan.";

        division.name = model.name;
        division.description = model.description;
        division.updated_at = DateTime.UtcNow;

        _context.Update(division);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<bool> DeleteDivisionAsync(int id)
    {
        var division = await _context.Divisions.FindAsync(id);
        if (division != null)
        {
            _context.Divisions.Remove(division);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
