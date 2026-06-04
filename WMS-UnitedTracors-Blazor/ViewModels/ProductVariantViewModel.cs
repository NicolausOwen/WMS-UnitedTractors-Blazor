using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace UT_WMSDotnet.ViewModels;

public class ProductVariantViewModel
{
    public int? id { get; set; }
    public string? sku { get; set; }
    public string? color { get; set; }
    public string? size { get; set; }
    public int stock { get; set; }
    public IBrowserFile? image { get; set; }
    public string? existing_image { get; set; }
    
    // Properti sementara untuk preview file image di Blazor UI
    public string? image_preview_url { get; set; }
}