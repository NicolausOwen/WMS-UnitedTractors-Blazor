using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UT_WMSDotnet.Models;

public class ProfileRequest
{
    [Key]
    public int id { get; set; }

    public int user_id { get; set; }

    [Required]
    [StringLength(255)]
    public string name { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string nrp { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string email { get; set; } = string.Empty;

    public int? division_id { get; set; }

    [Required]
    [StringLength(50)]
    public string status { get; set; } = "PENDING";

    public DateTime? created_at { get; set; } = DateTime.UtcNow;

    public DateTime? updated_at { get; set; } = DateTime.UtcNow;

    [ForeignKey("user_id")]
    public virtual User? User { get; set; }

    [ForeignKey("division_id")]
    public virtual Division? Division { get; set; }
}
