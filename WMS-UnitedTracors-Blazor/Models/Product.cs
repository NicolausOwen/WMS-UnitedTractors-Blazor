using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UT_WMSDotnet.Models;

public class Product
{
    [Key]
    public int id { get; set; }

    [Required]
    [StringLength(100)]
    public string sku { get; set; } = string.Empty;

    [StringLength(100)]
    public string? barcode_type { get; set; }

    [Required]
    [StringLength(255)]
    public string name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? transaction_type { get; set; }

    public int value { get; set; }

    [StringLength(255)]
    public string? image { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? description { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? images { get; set; }

    public int? category_id { get; set; }

    public int? location_id { get; set; }

    [StringLength(255)]
    public string? position_image { get; set; }

    public int current_stock { get; set; }

    public int initial_stock { get; set; }

    public int? unit_id { get; set; }

    public int is_returnable { get; set; }

    public int min_stock { get; set; }

    public DateTime created_at { get; set; } = DateTime.UtcNow;

    public DateTime updated_at { get; set; } = DateTime.UtcNow;

    // Navigation Properties (Relasi)
    [ForeignKey("category_id")]
    public virtual Category? Category { get; set; }

    [ForeignKey("location_id")]
    public virtual Location? Location { get; set; }

    [ForeignKey("unit_id")]
    public virtual Unit? Unit { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<StockLog> StockLogs { get; set; } = new List<StockLog>();
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}