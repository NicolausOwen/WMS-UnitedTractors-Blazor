using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UT_WMSDotnet.Models;

public class ProductVariant
{
    [Key]
    public int id { get; set; }

    public int product_id { get; set; }

    [StringLength(255)]
    public string? sku { get; set; }

    [StringLength(255)]
    public string? color { get; set; }

    [StringLength(255)]
    public string? size { get; set; }

    [StringLength(255)]
    public string? image { get; set; }

    public int stock { get; set; }

    public int is_hidden { get; set; } = 0;

    public DateTime? created_at { get; set; } = DateTime.UtcNow;

    public DateTime? updated_at { get; set; } = DateTime.UtcNow;

    [ForeignKey("product_id")]
    public virtual Product? Product { get; set; }
}
