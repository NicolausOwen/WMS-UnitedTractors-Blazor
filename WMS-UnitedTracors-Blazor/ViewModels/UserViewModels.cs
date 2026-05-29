using System.ComponentModel.DataAnnotations;

namespace UT_WMSDotnet.ViewModels;

public class UserCreateViewModel
{
    [Required(ErrorMessage = "Nama wajib diisi.")]
    [StringLength(255)]
    public string name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? nrp { get; set; }

    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    [StringLength(255)]
    public string email { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Poin tidak boleh negatif.")]
    public int? poin { get; set; }

    [Required(ErrorMessage = "Password wajib diisi.")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "Password minimal 8 karakter.")]
    public string password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role wajib dipilih.")]
    [RegularExpression("^(staff|manager|admin|superadmin)$", ErrorMessage = "Role tidak valid.")]
    public string role { get; set; } = string.Empty;

    public int? division_id { get; set; }
}

public class UserUpdateViewModel
{
    public int id { get; set; }

    [Required(ErrorMessage = "Nama wajib diisi.")]
    [StringLength(255)]
    public string name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? nrp { get; set; }

    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    [StringLength(255)]
    public string email { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Poin tidak boleh negatif.")]
    public int? poin { get; set; }

    [StringLength(255, MinimumLength = 8, ErrorMessage = "Password minimal 8 karakter.")]
    public string? password { get; set; }

    [Required(ErrorMessage = "Role wajib dipilih.")]
    [RegularExpression("^(staff|manager|admin|superadmin)$", ErrorMessage = "Role tidak valid.")]
    public string role { get; set; } = string.Empty;

    public int? division_id { get; set; }
}

public class ProfileUpdateViewModel
{
    [Required(ErrorMessage = "Nama wajib diisi.")]
    [StringLength(255)]
    public string name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    [StringLength(255)]
    public string email { get; set; } = string.Empty;

    [StringLength(255)]
    public string? nrp { get; set; }

    public int? division_id { get; set; }
}

public class PasswordUpdateViewModel
{
    [Required(ErrorMessage = "Password saat ini wajib diisi.")]
    public string current_password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password baru wajib diisi.")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "Password minimal 8 karakter.")]
    public string password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Konfirmasi password wajib diisi.")]
    [Compare("password", ErrorMessage = "Konfirmasi password tidak cocok.")]
    public string password_confirmation { get; set; } = string.Empty;
}

public class ProfileViewModel
{
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string? nrp { get; set; }
    public int division_id { get; set; }
}

public class ChangePasswordViewModel
{
    public string old_password { get; set; } = string.Empty;
    public string new_password { get; set; } = string.Empty;
    public string confirm_password { get; set; } = string.Empty;
}

public class UserViewModel
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string? nrp { get; set; }
    public string email { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;
    public int? division_id { get; set; }
    public int? poin { get; set; }
}
