using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class LocationService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public LocationService(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<(List<LocationDto> Locations, int TotalItems, int TotalPages)> GetLocationsAsync(int page = 1, int pageSize = 10)
    {
        using var _context = _factory.CreateDbContext();
        var query = _context.Locations.AsQueryable();

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var locations = await query
            .OrderBy(l => l.name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var locationList = new List<LocationDto>();
        foreach (var loc in locations)
        {
            var productsCount = await _context.Products.CountAsync(p => p.location_id == loc.id);
            locationList.Add(new LocationDto
            {
                Id = loc.id,
                Name = loc.name ?? "",
                Description = loc.description,
                ProductsCount = productsCount
            });
        }

        return (locationList, totalItems, totalPages);
    }

    public async Task<string?> CreateLocationAsync(LocationViewModel model)
    {
        using var _context = _factory.CreateDbContext();
        if (await _context.Locations.AnyAsync(l => l.name == model.name))
            return "Nama lokasi sudah ada.";

        var location = new Location
        {
            name = model.name,
            description = model.description,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateLocationAsync(int id, LocationViewModel model)
    {
        using var _context = _factory.CreateDbContext();
        if (await _context.Locations.AnyAsync(l => l.name == model.name && l.id != id))
            return "Nama lokasi sudah digunakan.";

        var location = await _context.Locations.FindAsync(id);
        if (location == null) return "Lokasi tidak ditemukan.";

        location.name = model.name;
        location.description = model.description;
        location.updated_at = DateTime.UtcNow;

        _context.Update(location);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<bool> DeleteLocationAsync(int id)
    {
        using var _context = _factory.CreateDbContext();
        var location = await _context.Locations.FindAsync(id);
        if (location != null)
        {
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}

public class LocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int ProductsCount { get; set; }
}
