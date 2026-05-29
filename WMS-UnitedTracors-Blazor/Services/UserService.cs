using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<User> Users, List<Division> Divisions, int TotalItems, int TotalPages)> GetUsersAsync(string? searchQuery, int page = 1, int pageSize = 10)
    {
        var query = _context.Users.Include(u => u.Division).AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(u => u.name.Contains(searchQuery) || u.email.Contains(searchQuery));
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var users = await query
            .OrderBy(u => u.name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var divisions = await _context.Divisions.OrderBy(d => d.name).ToListAsync();

        return (users, divisions, totalItems, totalPages);
    }

    public async Task<string?> CreateUserAsync(UserCreateViewModel model)
    {
        if (await _context.Users.AnyAsync(u => u.email == model.email))
            return "Email sudah digunakan.";

        if (!string.IsNullOrEmpty(model.nrp) && await _context.Users.AnyAsync(u => u.nrp == model.nrp))
            return "NRP sudah digunakan.";

        var user = new User
        {
            name = model.name,
            nrp = model.nrp,
            email = model.email,
            password = BCrypt.Net.BCrypt.HashPassword(model.password),
            role = model.role,
            division_id = model.division_id ?? 0,
            poin = model.poin ?? 0,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateUserAsync(int id, UserUpdateViewModel model, int currentUserId, string? currentUserRole)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return "User tidak ditemukan.";

        if (await _context.Users.AnyAsync(u => u.email == model.email && u.id != id))
            return "Email sudah digunakan.";

        if (!string.IsNullOrEmpty(model.nrp) && await _context.Users.AnyAsync(u => u.nrp == model.nrp && u.id != id))
            return "NRP sudah digunakan.";

        if (user.id == currentUserId && currentUserRole == "admin" && model.role != "admin" && model.role != "superadmin")
            return "You cannot downgrade your own admin role.";

        if (user.id == currentUserId && currentUserRole == "superadmin" && model.role != "superadmin")
            return "You cannot downgrade your own superadmin role.";

        user.name = model.name;
        user.nrp = model.nrp;
        user.email = model.email;
        user.role = model.role;
        user.division_id = model.division_id ?? 0;
        user.poin = model.poin ?? 0;
        
        if (!string.IsNullOrEmpty(model.password))
        {
            user.password = BCrypt.Net.BCrypt.HashPassword(model.password);
        }

        user.updated_at = DateTime.UtcNow;

        _context.Update(user);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> DeleteUserAsync(int id, int currentUserId)
    {
        if (id == currentUserId)
            return "You cannot delete your own account.";

        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return null;
        }

        return "User tidak ditemukan.";
    }
}
