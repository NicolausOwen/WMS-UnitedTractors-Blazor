using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UT_WMSDotnet.Models;

public class UserAdminRole
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [Required]
    public Guid AdminRoleId { get; set; }

    [ForeignKey("AdminRoleId")]
    public virtual AdminRole? AdminRole { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? CreatedBy { get; set; }
}
