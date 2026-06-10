using System.Security.Claims;

namespace WMS_UnitedTracors_Blazor.Helpers;

public static class PermissionClaims
{
    /// <summary>True jika user punya permission tsb (claim "perm") atau role-nya superadmin.</summary>
    public static bool HasPermission(this ClaimsPrincipal? user, string permissionKey)
    {
        if (user?.Identity is not { IsAuthenticated: true }) return false;

        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        if (role == "superadmin" || string.Equals(role, "Super Admin", StringComparison.OrdinalIgnoreCase))
            return true;

        return user.Claims.Any(c => c.Type == "perm" && c.Value == permissionKey);
    }

    /// <summary>True jika user punya salah satu dari permission yang diberikan.</summary>
    public static bool HasAnyPermission(this ClaimsPrincipal? user, params string[] permissionKeys)
        => permissionKeys.Any(p => user.HasPermission(p));
}
