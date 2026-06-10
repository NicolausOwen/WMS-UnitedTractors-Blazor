using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UT_WMSDotnet.Models;

public class AdminRole
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(255)]
    public string RoleName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>JSON array of permission keys (see <c>WMS_UnitedTracors_Blazor.Helpers.Permissions</c>).</summary>
    [Column(TypeName = "text")]
    public string? Permissions { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    public virtual ICollection<UserAdminRole> UserAdminRoles { get; set; } = new List<UserAdminRole>();
}
