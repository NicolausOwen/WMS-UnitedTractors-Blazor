using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
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
        // 1. Eksekusi SQL Dump
        await ExecuteSqlDumpAsync();

        // 2. Eksekusi Seeder Tambahan (Division, Unit, Superadmin)
        await SeedDivisionsAsync();
        await SeedUnitsAsync();
        await SeedDefaultUsersAsync();
        await SeedDefaultAdminRolesAsync();
    }

    private async Task ExecuteSqlDumpAsync()
    {
        var sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Migrations", "Seeders", "wms_united_tractors_clean.sql");
        if (!File.Exists(sqlFilePath))
        {
            _logger.LogWarning($"File SQL dump tidak ditemukan di {sqlFilePath}");
            return;
        }

        // Jika tabel sudah ada isinya, kita lewati seeding dump agar tidak duplicate
        if (await _context.Set<Category>().AnyAsync())
        {
            _logger.LogInformation("Database sudah terisi, melewati eksekusi SQL dump.");
            return;
        }

        _logger.LogInformation("Mengeksekusi SQL dump...");
        var sqlContent = await File.ReadAllTextAsync(sqlFilePath);

        // Hapus CREATE TABLE, ALTER TABLE, DROP TABLE agar tidak bentrok dengan EF Core Migrations
        sqlContent = Regex.Replace(sqlContent, @"CREATE TABLE[\s\S]*?;\r?\n", "", RegexOptions.IgnoreCase);
        sqlContent = Regex.Replace(sqlContent, @"DROP TABLE[\s\S]*?;\r?\n", "", RegexOptions.IgnoreCase);
        sqlContent = Regex.Replace(sqlContent, @"ALTER TABLE[\s\S]*?;\r?\n", "", RegexOptions.IgnoreCase);

        // Hapus INSERT INTO untuk tabel-tabel Laravel internal yang tidak ada di EF Core
        var ignoredTables = new[] { "migrations", "sessions", "password_reset_tokens", "personal_access_tokens", "cache", "cache_locks", "jobs", "job_batches" };
        foreach (var table in ignoredTables)
        {
            // Match until ); at the end of the line to avoid matching semicolons inside strings
            sqlContent = Regex.Replace(sqlContent, $@"INSERT INTO `{table}`[\s\S]*?\);\r?\n", "", RegexOptions.IgnoreCase);
        }

        // Eksekusi sisa SQL (INSERT INTO)
        try
        {
            // Jalankan semua SQL mentah dengan menonaktifkan pengecekan Foreign Key sementara
            var finalSql = "SET FOREIGN_KEY_CHECKS = 0;\n" + sqlContent + "\nSET FOREIGN_KEY_CHECKS = 1;";
            await _context.Database.ExecuteSqlRawAsync(finalSql);
            _logger.LogInformation("Berhasil mengeksekusi SQL dump.");
            
            // 1. Masukkan divisions
            await SeedDivisionsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gagal mengeksekusi SQL dump.");
            throw;
        }
    }

    private async Task SeedDivisionsAsync()
    {
        var divisions = new[] { "CCS", "CFA", "CHCU", "CRA", "CST", "DAD", "GLG", "MKT", "PIN", "PRT", "SOD", "SVC", "TMO", "TSO" };

        foreach (var divisionName in divisions)
        {
            if (!await _context.Set<Division>().AnyAsync(d => d.name == divisionName))
            {
                _context.Set<Division>().Add(new Division { name = divisionName });
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task SeedUnitsAsync()
    {
        var units = new[] { "pcs", "box", "pack", "set", "kg", "liter", "roll", "meter" };

        foreach (var unitName in units)
        {
            if (!await _context.Set<Unit>().AnyAsync(u => u.name == unitName))
            {
                _context.Set<Unit>().Add(new Unit { name = unitName });
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task SeedDefaultUsersAsync()
    {
        var chcuDivision = await _context.Set<Division>().FirstOrDefaultAsync(d => d.name == "CHCU");
        int? divisionId = chcuDivision?.id;

        var usersToSeed = new List<User>
        {
            new User { email = "superadmin@wms.com", name = "Super Administrator", role = "superadmin" },
            new User { email = "admin@wms.com", name = "Admin User", role = "admin" },
            new User { email = "manager@wms.com", name = "Manager User", role = "manager" },
            new User { email = "staff@wms.com", name = "Staff User", role = "staff" }
        };

        foreach (var u in usersToSeed)
        {
            var exists = await _context.Set<User>().AnyAsync(x => x.email == u.email);
            if (!exists)
            {
                u.password = BCrypt.Net.BCrypt.HashPassword("password");
                u.poin = 1000;
                u.nrp = new Random().Next(10000000, 99999999).ToString();
                u.division_id = divisionId;
                _context.Set<User>().Add(u);
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task SeedDefaultAdminRolesAsync()
    {
        // roleName -> default permission keys
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
                    Permissions = System.Text.Json.JsonSerializer.Serialize(perms),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                });
            }
            else if (string.IsNullOrEmpty(existing.Permissions))
            {
                // Isi permission default hanya jika belum pernah diset.
                existing.Permissions = System.Text.Json.JsonSerializer.Serialize(perms);
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Idempotent merge for existing roles
        var allRoles = await _context.AdminRoles.ToListAsync();
        foreach (var role in allRoles)
        {
            if (!string.IsNullOrEmpty(role.Permissions))
            {
                try
                {
                    var rolePerms = System.Text.Json.JsonSerializer.Deserialize<List<string>>(role.Permissions) ?? new List<string>();
                    if (rolePerms.Contains(Perms.ApprovalStage2) && !rolePerms.Contains(Perms.ApprovalHandoverFinal))
                    {
                        rolePerms.Add(Perms.ApprovalHandoverFinal);
                        role.Permissions = System.Text.Json.JsonSerializer.Serialize(rolePerms);
                        role.UpdatedAt = DateTime.UtcNow;
                    }
                }
                catch { /* Ignore invalid JSON */ }
            }
        }

        await _context.SaveChangesAsync();
    }
}
