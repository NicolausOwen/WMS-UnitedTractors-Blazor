using System;
using System.ComponentModel.DataAnnotations;

namespace UT_WMSDotnet.ViewModels;

public class AdminRoleDto
{
    public Guid Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int TotalUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class AdminRoleViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Role Name wajib diisi.")]
    [StringLength(255, ErrorMessage = "Role Name maksimal 255 karakter.")]
    public string RoleName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description maksimal 500 karakter.")]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UserAssignmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Nrp { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? DivisionName { get; set; }
}
