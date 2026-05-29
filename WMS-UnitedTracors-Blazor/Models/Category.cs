using System.ComponentModel.DataAnnotations;

namespace UT_WMSDotnet.Models;

public class Category
{
    [Key]
    public int id { get; set; }

    [Required]
    [StringLength(100)]
    public string name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? description { get; set; }

    public DateTime created_at { get; set; } = DateTime.UtcNow;

    public DateTime updated_at { get; set; } = DateTime.UtcNow;

    // Relasi ke Products
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}