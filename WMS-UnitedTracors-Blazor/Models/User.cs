using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UT_WMSDotnet.Models;

public class User
{
    [Key]
    public int id { get; set; }

    [Required]
    [StringLength(100)]
    public string name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? nrp { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string email { get; set; } = string.Empty;

    public int? poin { get; set; }

    public DateTime? email_verified_at { get; set; }

    [Required]
    [StringLength(255)]
    public string password { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string role { get; set; } = string.Empty;

    public int? division_id { get; set; }

    [ForeignKey("division_id")]
    public virtual Division? Division { get; set; }

    [StringLength(100)]
    public string? remember_token { get; set; }

    public DateTime created_at { get; set; } = DateTime.UtcNow;

    public DateTime updated_at { get; set; } = DateTime.UtcNow;

    public virtual ICollection<UserAdminRole> UserAdminRoles { get; set; } = new List<UserAdminRole>();
}