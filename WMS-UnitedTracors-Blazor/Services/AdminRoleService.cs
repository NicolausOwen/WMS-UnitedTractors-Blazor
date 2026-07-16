using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class AdminRoleService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public AdminRoleService(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<(List<AdminRoleDto> Roles, int TotalItems, int TotalPages)> GetAdminRolesAsync(
        string? searchQuery, 
        string? sortColumn = "RoleName", 
        bool sortDescending = false, 
        int page = 1,
        int pageSize = 10)
    {
        using var _context = _factory.CreateDbContext();
        var query = _context.AdminRoles.AsQueryable();

        // Search
        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(r => r.RoleName.Contains(searchQuery) || (r.Description != null && r.Description.Contains(searchQuery)));
        }

        // Sorting
        query = sortColumn switch
        {
            "RoleName" => sortDescending ? query.OrderByDescending(r => r.RoleName) : query.OrderBy(r => r.RoleName),
            "Description" => sortDescending ? query.OrderByDescending(r => r.Description) : query.OrderBy(r => r.Description),
            "IsActive" => sortDescending ? query.OrderByDescending(r => r.IsActive) : query.OrderBy(r => r.IsActive),
            "CreatedAt" => sortDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
            _ => query.OrderBy(r => r.RoleName)
        };

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var roles = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtoList = new List<AdminRoleDto>();
        foreach (var role in roles)
        {
            int totalUser = await _context.UserAdminRoles
                .Where(uar => uar.AdminRoleId == role.Id)
                .Select(uar => uar.UserId)
                .Distinct()
                .CountAsync();
            
            dtoList.Add(new AdminRoleDto
            {
                Id = role.Id,
                RoleName = role.RoleName,
                Description = role.Description,
                IsActive = role.IsActive,
                TotalUser = totalUser,
                CreatedAt = role.CreatedAt,
                CreatedBy = role.CreatedBy,
                UpdatedAt = role.UpdatedAt,
                UpdatedBy = role.UpdatedBy
            });
        }

        return (dtoList, totalItems, totalPages);
    }

    public async Task<AdminRoleDto?> GetAdminRoleByIdAsync(Guid id)
    {
        using var _context = _factory.CreateDbContext();
        var role = await _context.AdminRoles.FindAsync(id);
        if (role == null) return null;

        int totalUser = await _context.UserAdminRoles
            .Where(uar => uar.AdminRoleId == role.Id)
            .Select(uar => uar.UserId)
            .Distinct()
            .CountAsync();

        return new AdminRoleDto
        {
            Id = role.Id,
            RoleName = role.RoleName,
            Description = role.Description,
            IsActive = role.IsActive,
            TotalUser = totalUser,
            CreatedAt = role.CreatedAt,
            CreatedBy = role.CreatedBy,
            UpdatedAt = role.UpdatedAt,
            UpdatedBy = role.UpdatedBy
        };
    }

    public async Task<List<UserAssignmentDto>> GetAssignedUsersAsync(Guid roleId)
    {
        using var _context = _factory.CreateDbContext();
        return await _context.UserAdminRoles
            .Where(uar => uar.AdminRoleId == roleId)
            .Include(uar => uar.User)
            .ThenInclude(u => u!.Division)
            .Include(uar => uar.Category)
            .Select(uar => new UserAssignmentDto
            {
                Id = uar.User!.id,
                Name = uar.User!.name,
                Email = uar.User!.email,
                Nrp = uar.User!.nrp,
                DivisionName = uar.User!.Division != null ? uar.User.Division.name : null,
                CategoryId = uar.CategoryId,
                CategoryName = uar.Category != null ? uar.Category.name : null
            })
            .ToListAsync();
    }

    public async Task<List<UserAssignmentDto>> GetUnassignedUsersAsync(Guid roleId)
    {
        using var _context = _factory.CreateDbContext();
        var assignedUserIds = await _context.UserAdminRoles
            .Where(uar => uar.AdminRoleId == roleId)
            .Select(uar => uar.UserId)
            .ToListAsync();

        return await _context.Users
            .Where(u => !assignedUserIds.Contains(u.id))
            .Include(u => u.Division)
            .Select(u => new UserAssignmentDto
            {
                Id = u.id,
                Name = u.name,
                Email = u.email,
                Nrp = u.nrp,
                DivisionName = u.Division != null ? u.Division.name : null
            })
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<string?> CreateAdminRoleAsync(AdminRoleViewModel model, string createdBy)
    {
        using var _context = _factory.CreateDbContext();
        if (await _context.AdminRoles.AnyAsync(r => r.RoleName == model.RoleName))
        {
            return "Role Name sudah digunakan.";
        }

        var role = new AdminRole
        {
            Id = Guid.NewGuid(),
            RoleName = model.RoleName,
            Description = model.Description,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = createdBy
        };

        _context.AdminRoles.Add(role);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateAdminRoleAsync(Guid id, AdminRoleViewModel model, string updatedBy)
    {
        using var _context = _factory.CreateDbContext();
        if (await _context.AdminRoles.AnyAsync(r => r.RoleName == model.RoleName && r.Id != id))
        {
            return "Role Name sudah digunakan.";
        }

        var role = await _context.AdminRoles.FindAsync(id);
        if (role == null) return "Role tidak ditemukan.";

        role.RoleName = model.RoleName;
        role.Description = model.Description;
        role.IsActive = model.IsActive;
        role.UpdatedAt = DateTime.UtcNow;
        role.UpdatedBy = updatedBy;

        _context.Update(role);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> DeleteAdminRoleAsync(Guid id)
    {
        using var _context = _factory.CreateDbContext();
        var role = await _context.AdminRoles.FindAsync(id);
        if (role == null) return "Role tidak ditemukan.";

        var hasAssignedUsers = await _context.UserAdminRoles.AnyAsync(uar => uar.AdminRoleId == id);
        if (hasAssignedUsers)
        {
            return "Role cannot be deleted because it is assigned to one or more users.";
        }

        _context.AdminRoles.Remove(role);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> AssignUserToRoleAsync(Guid roleId, int userId, List<int>? categoryIds, string createdBy)
    {
        using var _context = _factory.CreateDbContext();
        if (categoryIds == null || !categoryIds.Any())
        {
            var isAssigned = await _context.UserAdminRoles.AnyAsync(uar => uar.AdminRoleId == roleId && uar.UserId == userId && uar.CategoryId == null);
            if (isAssigned) return "User sudah terasosiasi dengan Akses Global di Role ini.";

            _context.UserAdminRoles.Add(new UserAdminRole
            {
                Id = Guid.NewGuid(),
                AdminRoleId = roleId,
                UserId = userId,
                CategoryId = null,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            });
        }
        else
        {
            foreach (var catId in categoryIds)
            {
                var isAssigned = await _context.UserAdminRoles.AnyAsync(uar => uar.AdminRoleId == roleId && uar.UserId == userId && uar.CategoryId == catId);
                if (!isAssigned)
                {
                    _context.UserAdminRoles.Add(new UserAdminRole
                    {
                        Id = Guid.NewGuid(),
                        AdminRoleId = roleId,
                        UserId = userId,
                        CategoryId = catId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = createdBy
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> RemoveUserFromRoleAsync(Guid roleId, int userId, int? categoryId)
    {
        using var _context = _factory.CreateDbContext();
        var mapping = await _context.UserAdminRoles.FirstOrDefaultAsync(uar => uar.AdminRoleId == roleId && uar.UserId == userId && uar.CategoryId == categoryId);
        if (mapping == null) return "User tidak terasosiasi dengan Role dan Kategori ini.";

        _context.UserAdminRoles.Remove(mapping);
        await _context.SaveChangesAsync();
        return null;
    }

    /// <summary>
    /// Edit category assignment for an already-assigned user.
    /// Removes the old assignment and creates the new one(s) atomically.
    /// </summary>
    public async Task<string?> UpdateUserCategoryInRoleAsync(Guid roleId, int userId, int? oldCategoryId, List<int>? newCategoryIds, string updatedBy)
    {
        using var _context = _factory.CreateDbContext();

        // Remove the existing assignment
        var existing = await _context.UserAdminRoles
            .FirstOrDefaultAsync(uar => uar.AdminRoleId == roleId && uar.UserId == userId && uar.CategoryId == oldCategoryId);
        if (existing != null)
            _context.UserAdminRoles.Remove(existing);

        // Create the new assignment(s)
        if (newCategoryIds == null || !newCategoryIds.Any())
        {
            // Global (no category restriction)
            _context.UserAdminRoles.Add(new UserAdminRole
            {
                Id = Guid.NewGuid(),
                AdminRoleId = roleId,
                UserId = userId,
                CategoryId = null,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = updatedBy
            });
        }
        else
        {
            foreach (var catId in newCategoryIds)
            {
                _context.UserAdminRoles.Add(new UserAdminRole
                {
                    Id = Guid.NewGuid(),
                    AdminRoleId = roleId,
                    UserId = userId,
                    CategoryId = catId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = updatedBy
                });
            }
        }

        await _context.SaveChangesAsync();
        return null;
    }

    /// <summary>Removes ALL category assignments for a given user from a role.</summary>
    public async Task<string?> RemoveAllUserAssignmentsFromRoleAsync(Guid roleId, int userId)
    {
        using var _context = _factory.CreateDbContext();
        var mappings = await _context.UserAdminRoles
            .Where(uar => uar.AdminRoleId == roleId && uar.UserId == userId)
            .ToListAsync();
        if (!mappings.Any()) return "User tidak terasosiasi dengan Role ini.";

        _context.UserAdminRoles.RemoveRange(mappings);
        await _context.SaveChangesAsync();
        return null;
    }

    /// <summary>
    /// Replaces ALL category assignments for a user in one atomic operation:
    /// removes every existing entry then inserts the new selection.
    /// </summary>
    public async Task<string?> ReplaceUserAssignmentsInRoleAsync(Guid roleId, int userId, List<int>? newCategoryIds, string updatedBy)
    {
        using var _context = _factory.CreateDbContext();

        // Remove all existing
        var existing = await _context.UserAdminRoles
            .Where(uar => uar.AdminRoleId == roleId && uar.UserId == userId)
            .ToListAsync();
        _context.UserAdminRoles.RemoveRange(existing);

        // Insert new
        if (newCategoryIds == null || !newCategoryIds.Any())
        {
            _context.UserAdminRoles.Add(new UserAdminRole
            {
                Id = Guid.NewGuid(),
                AdminRoleId = roleId,
                UserId = userId,
                CategoryId = null,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = updatedBy
            });
        }
        else
        {
            foreach (var catId in newCategoryIds)
            {
                _context.UserAdminRoles.Add(new UserAdminRole
                {
                    Id = Guid.NewGuid(),
                    AdminRoleId = roleId,
                    UserId = userId,
                    CategoryId = catId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = updatedBy
                });
            }
        }

        await _context.SaveChangesAsync();
        return null;
    }

    // ── Permissions ──

    /// <summary>Daftar permission key yang dimiliki sebuah role (dari kolom JSON).</summary>
    public async Task<List<string>> GetRolePermissionsAsync(Guid roleId)
    {
        using var _context = _factory.CreateDbContext();
        var role = await _context.AdminRoles.FindAsync(roleId);
        if (role == null || string.IsNullOrWhiteSpace(role.Permissions)) return new List<string>();
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(role.Permissions) ?? new List<string>();
        }
        catch { return new List<string>(); }
    }

    public async Task<string?> UpdateRolePermissionsAsync(Guid roleId, List<string> permissions, string updatedBy)
    {
        using var _context = _factory.CreateDbContext();
        var role = await _context.AdminRoles.FindAsync(roleId);
        if (role == null) return "Role tidak ditemukan.";

        // Hanya simpan key yang valid.
        var valid = permissions.Where(p => WMS_UnitedTracors_Blazor.Helpers.Permissions.All.Contains(p)).Distinct().ToList();
        role.Permissions = System.Text.Json.JsonSerializer.Serialize(valid);
        role.UpdatedAt = DateTime.UtcNow;
        role.UpdatedBy = updatedBy;
        _context.Update(role);
        await _context.SaveChangesAsync();
        return null;
    }

    /// <summary>Resolusi permission efektif seorang user (berdasarkan User.role → AdminRole).</summary>
    public async Task<HashSet<string>> GetUserPermissionsAsync(int userId)
    {
        using var _context = _factory.CreateDbContext();
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return new HashSet<string>();
        var role = await _context.AdminRoles.FirstOrDefaultAsync(r => r.RoleName == user.role && r.IsActive);
        return WMS_UnitedTracors_Blazor.Helpers.Permissions.Resolve(user.role, role?.Permissions);
    }

    /// <summary>Nama role aktif untuk dropdown (Manage Users).</summary>
    public async Task<List<string>> GetActiveRoleNamesAsync()
    {
        using var _context = _factory.CreateDbContext();
        return await _context.AdminRoles
            .Where(r => r.IsActive)
            .OrderBy(r => r.RoleName)
            .Select(r => r.RoleName)
            .ToListAsync();
    }
}
