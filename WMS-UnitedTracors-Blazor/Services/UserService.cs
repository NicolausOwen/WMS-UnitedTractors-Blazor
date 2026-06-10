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

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.Include(u => u.Division).FirstOrDefaultAsync(u => u.id == id);
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
            division_id = (model.division_id.HasValue && model.division_id.Value > 0) ? model.division_id : null,
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

        // Cegah self-lockout: tidak boleh mengubah role sendiri ke role tanpa izin Kelola User.
        if (user.id == currentUserId && model.role != user.role)
        {
            var newRole = await _context.AdminRoles.FirstOrDefaultAsync(r => r.RoleName == model.role && r.IsActive);
            var newPerms = WMS_UnitedTracors_Blazor.Helpers.Permissions.Resolve(model.role, newRole?.Permissions);
            if (!newPerms.Contains(WMS_UnitedTracors_Blazor.Helpers.Permissions.UsersManage))
                return "Anda tidak bisa mengubah role sendiri ke role tanpa izin Kelola User (mencegah terkunci dari menu User).";
        }

        user.name = model.name;
        user.nrp = model.nrp;
        user.email = model.email;
        user.role = model.role;
        user.division_id = (model.division_id.HasValue && model.division_id.Value > 0) ? model.division_id : null;
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
        if (user == null)
            return "User tidak ditemukan.";

        // Transaksi = riwayat/audit. User yang pernah jadi pemohon/penyetuju tidak boleh dihapus.
        bool hasTransactions = await _context.Transactions.AnyAsync(t => t.requester_id == id || t.approver_id == id);
        if (hasTransactions)
            return "User tidak bisa dihapus karena masih memiliki riwayat transaksi (sebagai pemohon/penyetuju). Hapus/selesaikan transaksinya dulu, atau biarkan akun tetap ada sebagai arsip.";

        // Bersihkan metadata per-user yang aman dihapus (assignment role/kategori & permintaan profil).
        var roleAssignments = await _context.UserAdminRoles.Where(uar => uar.UserId == id).ToListAsync();
        if (roleAssignments.Count > 0) _context.UserAdminRoles.RemoveRange(roleAssignments);

        var profileReqs = await _context.ProfileRequests.Where(pr => pr.user_id == id).ToListAsync();
        if (profileReqs.Count > 0) _context.ProfileRequests.RemoveRange(profileReqs);

        _context.Users.Remove(user);

        try
        {
            await _context.SaveChangesAsync();
            return null;
        }
        catch (Exception ex)
        {
            return "Gagal menghapus user: " + (ex.InnerException?.Message ?? ex.Message);
        }
    }
}
