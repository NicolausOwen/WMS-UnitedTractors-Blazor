using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UT_WMSDotnet.Models;
using Perms = WMS_UnitedTracors_Blazor.Helpers.Permissions;

namespace UT_WMSDotnet.Data;

public class DbSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(ApplicationDbContext context, ILogger<DbSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Memulai proses seeding database...");

        // Seed master data
        await SeedCategoriesAsync();
        await SeedDivisionsAsync();
        await SeedLocationsAsync();
        await SeedUnitsAsync();

        // Seed Users and Roles
        await SeedDefaultAdminRolesAsync();
        await SeedDefaultUsersAsync();
        await SeedUserAdminRoleAssignmentsAsync();

        // Seed Products
        await SeedProductsAsync();

        _logger.LogInformation("Proses seeding selesai.");
    }

    private async Task SeedCategoriesAsync()
    {
        var categories = new[]
        {
            new Category { name = "Makanan", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow },
            new Category { name = "Game", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow },
            new Category { name = "Facility", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow },
            new Category { name = "ATK", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow },
            new Category { name = "Merchandise", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow },
            new Category { name = "Alat Musik", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow },
            new Category { name = "Elektronik", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow }
        };

        foreach (var cat in categories)
        {
            if (!await _context.Categories.AnyAsync(c => c.name == cat.name))
            {
                _context.Categories.Add(cat);
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task SeedDivisionsAsync()
    {
        var divisions = new[] { "CCS", "CFA", "CHCU", "CRA", "CST", "DAD", "GLG", "MKT", "PIN", "PRT", "SOD", "SVC", "TMO", "TSO" };

        foreach (var divisionName in divisions)
        {
            if (!await _context.Divisions.AnyAsync(d => d.name == divisionName))
            {
                _context.Divisions.Add(new Division { name = divisionName, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow });
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task SeedLocationsAsync()
    {
        var locations = new[]
        {
            new Location { name = "Gudang", description = "Gudang lt 1" },
            new Location { name = "Storage Room" },
            new Location { name = "ATK" },
            new Location { name = "Makeup Room" },
            new Location { name = "Merchandise" },
            new Location { name = "7.1.12.2" },
            new Location { name = "7.1.12.3" },
            new Location { name = "7.1.11.1" },
            new Location { name = "7.1.11.2" },
            new Location { name = "7.1.11.3" },
            new Location { name = "7.1.11.5" }
        };

        foreach (var loc in locations)
        {
            if (!await _context.Locations.AnyAsync(l => l.name == loc.name))
            {
                loc.created_at = DateTime.UtcNow;
                loc.updated_at = DateTime.UtcNow;
                _context.Locations.Add(loc);
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task SeedUnitsAsync()
    {
        var units = new[] { "pcs", "box", "pack", "set", "kg", "liter", "roll", "meter" };

        foreach (var unitName in units)
        {
            if (!await _context.Units.AnyAsync(u => u.name == unitName))
            {
                _context.Units.Add(new Unit { name = unitName, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow });
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task SeedDefaultAdminRolesAsync()
    {
        // roleName -> default permission keys based on SQL dump
        var defaults = new (string Name, string[] Perms)[]
        {
            ("Super Admin", Perms.All),
            ("Staff Inventoris", new[] { Perms.DashboardView, Perms.TrackingView, Perms.ApprovalStage1, Perms.ApprovalHandover, Perms.ProductsManage }),
            ("PIC Studio", new[] { Perms.DashboardView, Perms.TrackingView, Perms.ApprovalStage2, Perms.ApprovalHandoverFinal, Perms.ApprovalReturn, Perms.ProductsManage, Perms.MasterDataManage }),
            ("Team Leader Infrastructure", new[] { Perms.DashboardView, Perms.TrackingView, Perms.ApprovalStage2, Perms.ApprovalHandoverFinal, Perms.ApprovalReturn, Perms.ProductsManage, Perms.MasterDataManage }),
            ("Manager", new[] { Perms.DashboardView, Perms.TrackingView, Perms.ApprovalManager, Perms.ReportsView }),
            ("User", new[] { Perms.DashboardView, Perms.RequestCreate, Perms.TrackingView }),
        };

        foreach (var (name, perms) in defaults)
        {
            var existing = await _context.AdminRoles.FirstOrDefaultAsync(r => r.RoleName == name);
            if (existing == null)
            {
                _context.AdminRoles.Add(new AdminRole
                {
                    Id = Guid.NewGuid(),
                    RoleName = name,
                    Description = $"Default role for {name}",
                    Permissions = JsonSerializer.Serialize(perms),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                });
            }
            else if (string.IsNullOrEmpty(existing.Permissions))
            {
                existing.Permissions = JsonSerializer.Serialize(perms);
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedDefaultUsersAsync()
    {
        var chcuDivision = await _context.Divisions.FirstOrDefaultAsync(d => d.name == "CHCU");
        int? divisionId = chcuDivision?.id;

        var usersToSeed = new List<User>
        {
            new User { email = "superadmin@wms.com",    name = "Super Administrator",        role = "superadmin" },
            new User { email = "si@wms.com",            name = "Ceri (Staff Inventoris)",    role = "Staff Inventoris" },
            new User { email = "admin@wms.com",         name = "Della (Admin)",              role = "Team Leader Infrastructure" },
            new User { email = "pic@wms.com",           name = "Taruna (PIC Studio)",        role = "PIC Studio" },
            new User { email = "manager@wms.com",       name = "Anwari (Manager)",           role = "Manager" },
            new User { email = "user@wms.com",          name = "User",                       role = "User" },
        };

        foreach (var u in usersToSeed)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.email == u.email);
            if (existingUser == null)
            {
                u.password = BCrypt.Net.BCrypt.HashPassword("password");
                u.poin = 1000;
                u.nrp = new Random().Next(10000000, 99999999).ToString();
                u.division_id = divisionId;
                _context.Users.Add(u);
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task SeedUserAdminRoleAssignmentsAsync()
    {
        var users = await _context.Users.ToListAsync();
        var adminRoles = await _context.AdminRoles.ToListAsync();

        foreach (var user in users)
        {
            if (string.IsNullOrEmpty(user.role)) continue;

            string targetRoleName = user.role switch
            {
                "superadmin" => "Super Admin",
                _ => user.role
            };

            var targetRole = adminRoles.FirstOrDefault(r => string.Equals(r.RoleName, targetRoleName, StringComparison.OrdinalIgnoreCase));
            if (targetRole != null)
            {
                var existingAssignment = await _context.UserAdminRoles
                    .FirstOrDefaultAsync(uar => uar.UserId == user.id && uar.AdminRoleId == targetRole.Id && uar.CategoryId == null);

                if (existingAssignment == null)
                {
                    _context.UserAdminRoles.Add(new UserAdminRole
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.id,
                        AdminRoleId = targetRole.Id,
                        CategoryId = null,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedProductsAsync()
    {
        if (await _context.Products.AnyAsync()) return; // Skip if products exist

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "products.json");
        if (!File.Exists(filePath))
        {
            _logger.LogWarning($"File products.json tidak ditemukan di {filePath}");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var products = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (products != null && products.Any())
        {
            _logger.LogInformation($"Menyisipkan {products.Count} produk dari products.json...");
            foreach (var p in products)
            {
                // Reset ID to auto increment
                p.id = 0; 
                _context.Products.Add(p);
            }
            await _context.SaveChangesAsync();
        }
    }
}
