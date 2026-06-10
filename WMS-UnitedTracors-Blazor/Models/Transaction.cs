using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UT_WMSDotnet.Models;

public class Transaction
{
    [Key]
    public int id { get; set; }

    [NotMapped]
    public string group_id
    {
        get
        {
            var raw = $"{created_at:yyyy-MM-dd HH:mm}_{requester_id}_{applicant_name ?? ""}";
            using var md5 = System.Security.Cryptography.MD5.Create();
            var hashBytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    public int product_id { get; set; }

    public int? product_variant_id { get; set; }

    [Required]
    [StringLength(50)]
    public string type { get; set; } = string.Empty;

    [StringLength(50)]
    public string? request_type { get; set; }

    public int? quantity { get; set; } = 0;
 
    public int? returned_quantity { get; set; } = 0;
 
    public int? pending_return_quantity { get; set; } = 0;

    public DateTime? returned_at { get; set; }

    [StringLength(255)]
    public string? return_photo { get; set; }

    [Column(TypeName = "text")]
    public string? handover_photo { get; set; }

    [Column(TypeName = "text")]
    public string? handover_notes { get; set; }

    [StringLength(255)]
    public string? handover_recipient_name { get; set; }

    public DateTime? handover_timestamp { get; set; }

    [MaxLength(10)]
    public string? handover_uploaded_by { get; set; }

    public DateTime? handover_approved_at { get; set; }

    [Column(TypeName = "text")]
    public string? documentation_photo { get; set; }

    [Column(TypeName = "text")]
    public string? documentation_notes { get; set; }

    public DateTime? documentation_uploaded_at { get; set; }

    [StringLength(50)]
    public string? return_status { get; set; }

    [StringLength(500)]
    public string? return_reason { get; set; }

    public int? is_return_draft { get; set; } = 0;

    [StringLength(255)]
    public string? return_condition { get; set; }



    [Required]
    [StringLength(50)]
    public string status { get; set; } = string.Empty;

    [StringLength(500)]
    public string? rejection_reason { get; set; }

    [StringLength(500)]
    public string? return_rejection_reason { get; set; }

    [StringLength(500)]
    public string? admin_notes { get; set; }

    [StringLength(500)]
    public string? manager_notes { get; set; }

    [StringLength(500)]
    public string? staff_inventory_notes { get; set; }

    [StringLength(50)]
    public string? last_revision_stage { get; set; }

    public int requester_id { get; set; }

    public int? approver_id { get; set; }

    [StringLength(500)]
    public string? notes { get; set; }

    [StringLength(100)]
    public string? used_by { get; set; }

    public int? division_id { get; set; }

    public DateTime created_at { get; set; } = DateTime.UtcNow;

    public DateTime updated_at { get; set; } = DateTime.UtcNow;

    [StringLength(255)]
    public string? applicant_name { get; set; }

    [StringLength(50)]
    public string? applicant_nrp { get; set; }

    public int? borrow_duration_days { get; set; } = 0;

    [Column(TypeName = "date")]
    public DateTime? borrow_start_date { get; set; }

    [Column(TypeName = "date")]
    public DateTime? expected_return_date { get; set; }

    [Column(TypeName = "date")]
    public DateTime? pickup_date { get; set; }

    [StringLength(255)]
    public string? event_name { get; set; }

    [Column(TypeName = "date")]
    public DateTime? event_date { get; set; }

    [StringLength(500)]
    public string? documentation_link { get; set; }

    // Navigation Properties (Relasi)
    [ForeignKey("product_id")]
    public virtual Product? Product { get; set; }

    [ForeignKey("product_variant_id")]
    public virtual ProductVariant? ProductVariant { get; set; }

    [ForeignKey("requester_id")]
    public virtual User? Requester { get; set; }

    [ForeignKey("approver_id")]
    public virtual User? Approver { get; set; }

    [ForeignKey("division_id")]
    public virtual Division? Division { get; set; }

    public virtual ICollection<StockLog> StockLogs { get; set; } = new List<StockLog>();
}
