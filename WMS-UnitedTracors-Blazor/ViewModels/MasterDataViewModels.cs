using System.ComponentModel.DataAnnotations;

namespace UT_WMSDotnet.ViewModels;

public class CategoryViewModel
{
    public int id { get; set; }

    [Required(ErrorMessage = "Nama kategori wajib diisi.")]
    [StringLength(255, ErrorMessage = "Nama kategori maksimal 255 karakter.")]
    public string name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Deskripsi maksimal 500 karakter.")]
    public string? description { get; set; }
}

public class LocationViewModel
{
    public int id { get; set; }

    [Required(ErrorMessage = "Nama lokasi wajib diisi.")]
    [StringLength(255, ErrorMessage = "Nama lokasi maksimal 255 karakter.")]
    public string name { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Deskripsi maksimal 255 karakter.")]
    public string? description { get; set; }
}

public class UnitViewModel
{
    public int id { get; set; }

    [Required(ErrorMessage = "Nama unit wajib diisi.")]
    [StringLength(50, ErrorMessage = "Nama unit maksimal 50 karakter.")]
    public string name { get; set; } = string.Empty;
}

public class DivisionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UsersCount { get; set; }
    public int ProductsCount { get; set; }
}

public class DivisionViewModel
{
    public int id { get; set; }

    [Required(ErrorMessage = "Nama divisi wajib diisi.")]
    [StringLength(255, ErrorMessage = "Nama divisi maksimal 255 karakter.")]
    public string name { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Deskripsi maksimal 255 karakter.")]
    public string? description { get; set; }
}
