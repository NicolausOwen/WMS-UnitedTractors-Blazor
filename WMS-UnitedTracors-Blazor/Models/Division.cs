using System.ComponentModel.DataAnnotations;

namespace UT_WMSDotnet.Models;

public class Division
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

    // Relasi: Satu divisi memiliki banyak User (One-to-Many)
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}