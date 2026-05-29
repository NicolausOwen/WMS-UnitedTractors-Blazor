using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(ClaimsPrincipal? Principal, string? ErrorMessage)> LoginAsync(string email, string password)
    {
        var user = await _context.Users.Include(u => u.Division).FirstOrDefaultAsync(u => u.email == email);

        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.password))
        {
            var principal = CreateClaimsPrincipal(user);
            return (principal, null);
        }

        return (null, "Email atau password salah.");
    }

    public async Task<(ClaimsPrincipal? Principal, string? ErrorMessage)> MicrosoftLoginAsync(string email, string? name)
    {
        if (email.Contains("#EXT#"))
        {
            var beforeExt = email.Substring(0, email.IndexOf("#EXT#"));
            var lastUnderscore = beforeExt.LastIndexOf('_');
            if (lastUnderscore != -1)
            {
                email = beforeExt.Remove(lastUnderscore, 1).Insert(lastUnderscore, "@");
            }
            else
            {
                email = beforeExt;
            }
        }

        string emailLower = email.ToLower();
        if (!emailLower.EndsWith("@unitedtractors.com") && 
            !emailLower.EndsWith("@ut.co.id") && 
            !emailLower.EndsWith("@ww051205gmail.onmicrosoft.com"))
        {
            return (null, "Akses ditolak. Harap gunakan akun Microsoft berdomain resmi United Tractors untuk masuk.");
        }

        var user = await _context.Users.Include(u => u.Division).FirstOrDefaultAsync(u => u.email == email);

        if (user == null)
        {
            user = new User
            {
                name = name ?? "SSO User",
                email = email,
                password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                role = "staff",
                poin = 0,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        var principal = CreateClaimsPrincipal(user);
        return (principal, null);
    }

    private ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            new Claim(ClaimTypes.Name, user.name ?? ""),
            new Claim(ClaimTypes.Email, user.email ?? ""),
            new Claim(ClaimTypes.Role, user.role ?? ""),
            new Claim("nrp", user.nrp ?? ""),
            new Claim("poin", user.poin?.ToString() ?? "0"),
            new Claim("division", user.Division?.name ?? ""),
            new Claim("division_id", user.division_id?.ToString() ?? "")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}
