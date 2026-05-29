using System.ComponentModel.DataAnnotations;

namespace UT_WMSDotnet.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    public string email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password wajib diisi.")]
    [DataType(DataType.Password)]
    public string password { get; set; } = string.Empty;

    public bool remember_me { get; set; }
}
