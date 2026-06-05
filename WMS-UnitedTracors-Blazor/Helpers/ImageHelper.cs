using UT_WMSDotnet.Models;

namespace WMS_UnitedTracors_Blazor.Helpers;

/// <summary>
/// Resolusi path gambar produk/varian agar konsisten di semua halaman.
/// Stored value bisa berupa: "images/products/x.jpg", "/images/products/x.jpg",
/// "storage/products/x.jpg", nama file polos (gaya Laravel), atau URL penuh.
/// </summary>
public static class ImageHelper
{
    public const string DefaultPlaceholder = "https://placehold.co/100?text=No+Image";

    /// <summary>Ubah satu path gambar menjadi URL yang valid. Mengembalikan <paramref name="fallbackUrl"/> apa adanya jika kosong.</summary>
    public static string Resolve(string? image, string? fallbackUrl = null)
    {
        if (string.IsNullOrWhiteSpace(image))
            return fallbackUrl ?? DefaultPlaceholder;

        var normalized = image.Trim();
        while (normalized.StartsWith("/"))
            normalized = normalized[1..];

        if (string.IsNullOrEmpty(normalized))
            return fallbackUrl ?? DefaultPlaceholder;

        if (normalized.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            normalized.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return image.Trim();

        if (normalized.StartsWith("images/", StringComparison.OrdinalIgnoreCase) ||
            normalized.StartsWith("storage/", StringComparison.OrdinalIgnoreCase))
            return "/" + normalized;

        return "/storage/" + normalized;
    }

    /// <summary>Gambar utama produk: pakai <c>image</c>, fallback ke entri pertama <c>images</c> (JSON), lalu placeholder.</summary>
    public static string ResolveProduct(Product? product, string? fallbackUrl = null)
    {
        if (product == null)
            return fallbackUrl ?? DefaultPlaceholder;

        if (!string.IsNullOrEmpty(product.image))
            return Resolve(product.image, fallbackUrl);

        if (!string.IsNullOrEmpty(product.images))
        {
            try
            {
                var imgList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(product.images);
                var first = imgList?.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
                if (!string.IsNullOrEmpty(first))
                    return Resolve(first, fallbackUrl);
            }
            catch
            {
                // abaikan JSON yang tidak valid
            }
        }

        return fallbackUrl ?? DefaultPlaceholder;
    }
}
