using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UT_WMSDotnet.ViewModels;

public class ProductCreateViewModel
{
    [StringLength(255, ErrorMessage = "SKU maksimal 255 karakter.")]
    public string? sku { get; set; }

    [Required(ErrorMessage = "Tipe barcode wajib diisi.")]
    [RegularExpression("^(TYPE_CODE_128|TYPE_CODE_39|TYPE_EAN_13|TYPE_UPC_A)$", ErrorMessage = "Tipe barcode tidak valid.")]
    public string barcode_type { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nama produk wajib diisi.")]
    [StringLength(255, ErrorMessage = "Nama produk maksimal 255 karakter.")]
    public string name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Unit wajib diisi.")]
    [StringLength(50, ErrorMessage = "Unit maksimal 50 karakter.")]
    public string unit { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Nilai harus lebih besar atau sama dengan 0.")]
    public int? value { get; set; }

    [Required(ErrorMessage = "Kategori wajib dipilih.")]
    public int category_id { get; set; }

    public int? location_id { get; set; }

    public IFormFile? image { get; set; }

    public IFormFile? position_image { get; set; }

    [Range(0, 10000, ErrorMessage = "Stok awal harus antara 0 dan 10000.")]
    public int? initial_stock { get; set; }

    public bool is_returnable { get; set; }

    public string? description { get; set; }
    
    public string? transaction_type { get; set; }
}
