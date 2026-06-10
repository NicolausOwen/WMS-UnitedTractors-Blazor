namespace WMS_UnitedTracors_Blazor.Helpers;

/// <summary>
/// Katalog izin (permission) aplikasi. Setiap role (AdminRole) menyimpan daftar key ini
/// (JSON) di kolom <c>permissions</c>. Login menaruh tiap key sebagai claim "perm".
/// </summary>
public static class Permissions
{
    // ── Alur Proses (urut sesuai proses request → approval → serah terima → return) ──
    public const string RequestCreate = "request.create";
    public const string TrackingView = "tracking.view";
    public const string ApprovalStage1 = "approval.stage1";   // Staff Inventoris
    public const string ApprovalStage2 = "approval.stage2";   // Admin
    public const string ApprovalManager = "approval.manager"; // Manager (giveaway)
    public const string ApprovalHandover = "approval.handover"; // Verifikasi serah terima
    public const string ApprovalReturn = "approval.return";   // Approve pengembalian

    // ── Kelola Data ──
    public const string DashboardView = "dashboard.view";
    public const string ProductsManage = "products.manage";
    public const string MasterDataManage = "masterdata.manage"; // kategori/lokasi/unit/divisi
    public const string ReportsView = "reports.view";
    public const string ScannerUse = "scanner.use";
    public const string UsersManage = "users.manage";
    public const string RolesManage = "roles.manage";

    public sealed record Def(string Key, string Label, string Group, int Order);

    /// <summary>Definisi izin lengkap, urut untuk ditampilkan di Permission page.</summary>
    public static readonly IReadOnlyList<Def> Catalog = new List<Def>
    {
        // Alur Proses
        new(RequestCreate,    "Buat Request (Borrow/Giveaway)",            "Alur Proses", 10),
        new(TrackingView,     "Lihat Tracking",                            "Alur Proses", 20),
        new(ApprovalStage1,   "Approve Tahap 1 (Staff Inventoris)",        "Alur Proses", 30),
        new(ApprovalStage2,   "Approve Tahap 2 (Admin)",                   "Alur Proses", 40),
        new(ApprovalManager,  "Approve Manager (Giveaway)",                "Alur Proses", 50),
        new(ApprovalHandover, "Verifikasi Serah Terima",                   "Alur Proses", 60),
        new(ApprovalReturn,   "Approve Pengembalian",                      "Alur Proses", 70),
        // Kelola Data
        new(DashboardView,    "Lihat Dashboard / Katalog",                 "Kelola Data", 110),
        new(ProductsManage,   "Kelola Produk",                             "Kelola Data", 120),
        new(MasterDataManage, "Kelola Master Data (Kategori/Lokasi/Unit/Divisi)", "Kelola Data", 130),
        new(ReportsView,      "Lihat Laporan",                             "Kelola Data", 140),
        new(ScannerUse,       "Gunakan Scanner",                           "Kelola Data", 150),
        new(UsersManage,      "Kelola User",                               "Kelola Data", 160),
        new(RolesManage,      "Kelola Role & Permission",                  "Kelola Data", 170),
    };

    /// <summary>Semua key (untuk superadmin).</summary>
    public static readonly string[] All = Catalog.Select(d => d.Key).ToArray();

    /// <summary>
    /// Resolusi permission untuk sebuah role: superadmin/Super Admin = semua;
    /// kalau ada JSON permissions pakai itu; kalau kosong pakai fallback role lama.
    /// </summary>
    public static HashSet<string> Resolve(string? roleName, string? permissionsJson)
    {
        if (string.IsNullOrWhiteSpace(roleName)) return new HashSet<string>();

        if (roleName == "superadmin" || string.Equals(roleName, "Super Admin", StringComparison.OrdinalIgnoreCase))
            return new HashSet<string>(All);

        if (!string.IsNullOrWhiteSpace(permissionsJson))
        {
            try
            {
                var list = System.Text.Json.JsonSerializer.Deserialize<List<string>>(permissionsJson);
                if (list != null) return new HashSet<string>(list);
            }
            catch { /* JSON rusak -> jatuh ke fallback */ }
        }

        return new HashSet<string>(LegacyRoleDefaults(roleName));
    }

    /// <summary>
    /// Fallback untuk role string lama (sebelum permission diatur). Menjaga akun lama tetap jalan.
    /// </summary>
    public static IReadOnlyList<string> LegacyRoleDefaults(string? role) => role switch
    {
        "superadmin" => All,
        "admin" => new[]
        {
            RequestCreate, TrackingView, ApprovalStage1, ApprovalStage2, ApprovalHandover,
            ApprovalReturn, DashboardView, ProductsManage, MasterDataManage, ReportsView, ScannerUse
        },
        "manager" => new[] { DashboardView, TrackingView, ApprovalManager, ReportsView },
        "staff" => new[] { DashboardView, RequestCreate, TrackingView },
        _ => new[] { DashboardView, RequestCreate, TrackingView },
    };
}
