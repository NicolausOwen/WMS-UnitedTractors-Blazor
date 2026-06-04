using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UT_WMSDotnet.ViewModels;

public class TransactionItemViewModel
{
    [Required(ErrorMessage = "SKU wajib diisi.")]
    public string sku { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kuantitas wajib diisi.")]
    [Range(1, 1000, ErrorMessage = "Kuantitas harus antara 1 dan 1000.")]
    public int quantity { get; set; }

    [RegularExpression("^(BORROW|GIVEAWAY)$", ErrorMessage = "Tipe request tidak valid.")]
    public string? request_type { get; set; }

    [Range(1, 365, ErrorMessage = "Durasi peminjaman harus antara 1 dan 365 hari.")]
    public int? borrow_duration_days { get; set; }

    public DateTime? borrow_start_date { get; set; }
    public DateTime? expected_return_date { get; set; }
    public DateTime? pickup_date { get; set; }

    public int? product_variant_id { get; set; }
}

public class TransactionRequestViewModel
{
    [Required(ErrorMessage = "Tipe transaksi wajib diisi.")]
    [RegularExpression("^(IN|OUT)$", ErrorMessage = "Tipe transaksi harus IN atau OUT.")]
    public string type { get; set; } = string.Empty;

    public string? notes { get; set; }

    public List<TransactionItemViewModel>? items { get; set; }

    // Fallback if items not provided (single item request)
    public string? sku { get; set; }
    public int? quantity { get; set; }
    public string? request_type { get; set; }
    public int? product_variant_id { get; set; }

    // Applicant info
    [StringLength(255)]
    public string? applicant_name { get; set; }

    [StringLength(50)]
    public string? applicant_nrp { get; set; }

    public int? division_id { get; set; }

    // Required for OUT type
    [StringLength(255)]
    public string? event_name { get; set; }

    public DateTime? event_date { get; set; }

    [StringLength(500)]
    public string? documentation_link { get; set; }
}

public class ReturnItemViewModel
{
    [Required]
    public int transaction_id { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Kuantitas pengembalian minimal 1.")]
    public int return_quantity { get; set; }

    public IFormFile? return_photo { get; set; }

    [Required]
    [RegularExpression("^(baik|rusak|hilang)$", ErrorMessage = "Status pengembalian tidak valid.")]
    public string return_status { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? return_reason { get; set; }
}
