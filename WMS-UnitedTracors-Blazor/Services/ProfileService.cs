using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class ProfileService
{
    private readonly ApplicationDbContext _context;

    public ProfileService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserProfileAsync(int userId)
    {
        return await _context.Users.Include(u => u.Division).FirstOrDefaultAsync(u => u.id == userId);
    }

    public async Task<string?> SubmitProfileRequestAsync(int userId, ProfileViewModel model)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return "User not found.";

        var existingPending = await _context.ProfileRequests
            .FirstOrDefaultAsync(pr => pr.user_id == userId && pr.status == "PENDING");

        if (existingPending != null)
        {
            return "Anda sudah memiliki pengajuan perubahan profil yang sedang menunggu persetujuan.";
        }

        var pr = new ProfileRequest
        {
            user_id = userId,
            name = model.name,
            email = model.email,
            nrp = model.nrp,
            division_id = model.division_id,
            status = "PENDING",
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        _context.ProfileRequests.Add(pr);
        await _context.SaveChangesAsync();

        return null;
    }

    public async Task<string?> ChangePasswordAsync(int userId, ChangePasswordViewModel model)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return "User not found.";

        if (!BCrypt.Net.BCrypt.Verify(model.old_password, user.password))
        {
            return "Password lama tidak sesuai.";
        }

        user.password = BCrypt.Net.BCrypt.HashPassword(model.new_password);
        _context.Update(user);
        await _context.SaveChangesAsync();

        return null;
    }
}
