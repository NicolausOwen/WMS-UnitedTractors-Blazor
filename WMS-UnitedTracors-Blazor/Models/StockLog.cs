using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UT_WMSDotnet.Models;

[Table("stock_logs")]
public class StockLog
{
    [Key]
    public int id { get; set; }

    public int transaction_id { get; set; }

    public int product_id { get; set; }

    public int stock_before { get; set; }

    public int stock_after { get; set; }

    public DateTime created_at { get; set; } = DateTime.UtcNow;

    public DateTime updated_at { get; set; } = DateTime.UtcNow;

    // Navigation Properties (Relasi)
    [ForeignKey("transaction_id")]
    public virtual Transaction? Transaction { get; set; }

    [ForeignKey("product_id")]
    public virtual Product? Product { get; set; }
}