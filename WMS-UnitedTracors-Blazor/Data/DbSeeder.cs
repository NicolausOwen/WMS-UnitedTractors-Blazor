using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using UT_WMSDotnet.Models;

namespace UT_WMSDotnet.Data;

public class DbSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(ApplicationDbContext context, ILogger<DbSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Memulai proses seeding database...");

        // Disable foreign key checks for MySQL during seeding if needed, but since we order them, it should be fine.
        
        // Seed order is critical to avoid FK constraints
        await SeedTableAsync<Category>("categories.json");
        await SeedTableAsync<Division>("divisions.json");
        await SeedTableAsync<Location>("locations.json");
        await SeedTableAsync<Unit>("units.json");

        await SeedTableAsync<AdminRole>("admin_roles.json");

        await SeedTableAsync<Product>("products.json");
        await SeedTableAsync<ProductVariant>("product_variants.json");

        await SeedTableAsync<User>("users.json");
        await SeedTableAsync<UserAdminRole>("user_admin_roles.json");

        // await SeedTableAsync<Transaction>("transactions.json");
        // await SeedTableAsync<StockLog>("stock_logs.json");

        _logger.LogInformation("Proses seeding selesai.");
    }

    private async Task SeedTableAsync<T>(string filename) where T : class
    {
        var dbSet = _context.Set<T>();
        
        if (await dbSet.AnyAsync()) return; // Skip if data exists

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", filename);
        if (!File.Exists(filePath))
        {
            _logger.LogWarning($"File {filename} tidak ditemukan di {filePath}");
            return;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                Converters = { 
                    new BoolConverter(),
                    new CustomDateTimeConverter(),
                    new NullableCustomDateTimeConverter()
                }
            };
            var data = JsonSerializer.Deserialize<List<T>>(json, options);

            if (data != null && data.Any())
            {
                // Auto-hash plain text passwords for User model
                if (data is List<User> users)
                {
                    foreach (var u in users)
                    {
                        if (!string.IsNullOrEmpty(u.password) && !u.password.StartsWith("$2"))
                        {
                            u.password = BCrypt.Net.BCrypt.HashPassword(u.password);
                        }
                    }
                }

                // Kembalikan current_stock ke initial_stock untuk produk agar clean
                if (data is List<Product> products)
                {
                    foreach (var p in products)
                    {
                        p.current_stock = p.initial_stock;
                    }
                }

                // Reverse transaksi untuk stok varian agar kembali utuh
                if (data is List<ProductVariant> variants)
                {
                    var txPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "transactions.json");
                    if (File.Exists(txPath))
                    {
                        var txJson = await File.ReadAllTextAsync(txPath);
                        var txs = JsonSerializer.Deserialize<List<Transaction>>(txJson, options);
                        if (txs != null)
                        {
                            var validStatuses = new[] { "APPROVED", "COMPLETED", "HANDOVER" };
                            foreach (var tx in txs.Where(t => t.product_variant_id.HasValue && validStatuses.Contains(t.status)))
                            {
                                var variant = variants.FirstOrDefault(v => v.id == tx.product_variant_id);
                                if (variant != null)
                                {
                                    variant.stock += tx.quantity ?? 0;
                                }
                            }
                        }
                    }
                }

                _logger.LogInformation($"Menyisipkan {data.Count} baris dari {filename}...");
                
                // Allow explicit ID insert in EF Core for MySQL by just adding them.
                // MySQL EF provider usually handles this if Identity is not explicitly locked,
                // but if we encounter errors, we may need to adjust.
                await dbSet.AddRangeAsync(data);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saat menyisipkan data dari {filename}");
            throw; // Re-throw to stop seeding if one critical table fails
        }
    }
}

public class BoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32() == 1;
        }
        if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
        {
            return reader.GetBoolean();
        }
        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString() == "1" || reader.GetString()?.ToLower() == "true";
        }
        return false;
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string[] _formats = new[] {
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm:ss.ffffff",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-dd"
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        if (string.IsNullOrWhiteSpace(stringValue))
        {
            return DateTime.MinValue;
        }

        if (DateTime.TryParseExact(stringValue, _formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }

        if (DateTime.TryParse(stringValue, out date))
        {
            return date;
        }

        return DateTime.MinValue;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}

public class NullableCustomDateTimeConverter : JsonConverter<DateTime?>
{
    private readonly string[] _formats = new[] {
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm:ss.ffffff",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-dd"
    };

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        if (string.IsNullOrWhiteSpace(stringValue))
        {
            return null;
        }

        if (DateTime.TryParseExact(stringValue, _formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }

        if (DateTime.TryParse(stringValue, out date))
        {
            return date;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
