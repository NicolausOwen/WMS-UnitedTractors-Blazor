using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using WMS_UnitedTracors_Blazor.Components;

// Trigger watch restart

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.CategoryService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.ProductService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.TransactionService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.ApprovalService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.DashboardService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.AuthService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.DivisionService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.LocationService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.UnitService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.UserService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.CartService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.ProfileService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.CategoryService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.ReportService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.TrackingService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.PdfReportService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.PdfReceiptService>();
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.AdminRoleService>();

builder.Services.Configure<WMS_UnitedTracors_Blazor.Services.SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<WMS_UnitedTracors_Blazor.Services.IEmailService, WMS_UnitedTracors_Blazor.Services.EmailService>();

// Database setup
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.30-mysql"),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null));
});

// Authentication setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    })
    .AddCookie("ExternalCookie")
    .AddOpenIdConnect("Microsoft", options =>
    {
        options.SignInScheme = "ExternalCookie";
        options.ClientId = builder.Configuration["AzureAd:ClientId"] ?? "placeholder-client-id";
        options.ClientSecret = builder.Configuration["AzureAd:ClientSecret"] ?? "placeholder-client-secret";
        options.Authority = $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"] ?? "common"}/v2.0";
        options.CallbackPath = "/auth/microsoft/callback-oidc";
        options.ResponseType = "code";
        options.SaveTokens = true;
        options.Scope.Add("email");
        options.Scope.Add("profile");

        options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false
        };
        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = context => Task.CompletedTask
        };
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseCookiePolicy();

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

// Map Storage folder
var storagePath = Path.Combine(app.Environment.ContentRootPath, "Storage");
if (!Directory.Exists(storagePath))
{
    Directory.CreateDirectory(storagePath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(storagePath),
    RequestPath = "/storage"
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Auth Endpoints
app.MapPost("/api/auth/login", async (HttpContext context, [Microsoft.AspNetCore.Mvc.FromForm] string email, [Microsoft.AspNetCore.Mvc.FromForm] string password, [Microsoft.AspNetCore.Mvc.FromForm] string? remember_me, WMS_UnitedTracors_Blazor.Services.AuthService authService) =>
{
    var (principal, error) = await authService.LoginAsync(email, password);
    if (principal != null)
    {
        var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
        {
            IsPersistent = remember_me == "true",
            ExpiresUtc = remember_me == "true" ? DateTimeOffset.UtcNow.AddDays(30) : null
        };
        await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(context, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        return Results.Redirect("/");
    }
    return Results.Redirect("/login?error=1");
}).AllowAnonymous().DisableAntiforgery();

app.MapPost("/Logout", async (HttpContext context) =>
{
    await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(context, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).AllowAnonymous().DisableAntiforgery();

app.MapGet("/auth/microsoft", () =>
{
    var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = "/auth/microsoft/callback" };
    return Results.Challenge(properties, new[] { "Microsoft" });
}).AllowAnonymous();

app.MapGet("/auth/microsoft/callback", async (
    HttpContext context,
    WMS_UnitedTracors_Blazor.Services.AuthService authService) =>
{
    var result = await context.AuthenticateAsync("ExternalCookie");

    if (!result.Succeeded)
    {
        return Results.Redirect("/login?errorMessage=Failed+to+authenticate+with+Microsoft.");
    }

    var claims = result.Principal?.Claims;

    var email = claims?.FirstOrDefault(c =>
        c.Type == System.Security.Claims.ClaimTypes.Email)?.Value
        ?? claims?.FirstOrDefault(c =>
        c.Type == "preferred_username")?.Value;

    if (string.IsNullOrEmpty(email))
    {
        return Results.Redirect("/login?errorMessage=Gagal+mendapatkan+alamat+email+dari+akun+Microsoft+Anda.");
    }

    var (principal, error) =
        await authService.MicrosoftLoginAsync(
            email,
            claims?.FirstOrDefault(c =>
                c.Type == System.Security.Claims.ClaimTypes.Name)?.Value);

    if (principal == null)
    {
        await context.SignOutAsync("ExternalCookie");

        return Results.Redirect(
            $"/login?errorMessage={Uri.EscapeDataString(error ?? "Akses ditolak.")}");
    }

    await context.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal);

    await context.SignOutAsync("ExternalCookie");

    return Results.Redirect("/");
}).AllowAnonymous();


// =======================
// Migration & Seeder
// =======================
if (app.Environment.IsDevelopment() ||
    builder.Configuration.GetValue<bool>("RunMigrations", false))
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var dbFactory = services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        var context = dbFactory.CreateDbContext();

        // 1. Cek apakah ada command flag --reset-db
        var isResetRequested = args.Contains("--reset-db");
        var logger = services.GetRequiredService<ILogger<Program>>();

        if (isResetRequested)
        {
            logger.LogWarning("Menghapus database (FRESH REWRITE) karena flag --reset-db aktif...");
            await context.Database.EnsureDeletedAsync();
            logger.LogInformation("Menjalankan migrasi database baru...");
            await context.Database.MigrateAsync();
        }
        else
        {
            logger.LogInformation("Memeriksa status database existing...");
            
            // Pengamanan agar EF Core tidak menabrak tabel yang sudah ada (karena file migrasi baru saja disquash)
            try
            {
                var tablesExist = false;
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT 1 FROM `categories` LIMIT 1;");
                    tablesExist = true;
                }
                catch { }

                if (tablesExist)
                {
                    await context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
                            `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
                            `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
                            CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
                        ) CHARACTER SET=utf8mb4;
                    ");
                    // Insert file migrasi terbaru agar EF Core tahu DB sudah up-to-date dengan InitialCreate
                    await context.Database.ExecuteSqlRawAsync(@"
                        INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) 
                        VALUES ('20260713032839_InitialCreate', '9.0.0');
                    ");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Gagal mensinkronisasi riwayat migrasi.");
            }

            logger.LogInformation("Menjalankan migrasi database...");
            await context.Database.MigrateAsync();
        }

        // 2. Run the C# Seeder to populate the database
        logger.LogInformation("Menjalankan database seeder...");
        var seeder = new UT_WMSDotnet.Data.DbSeeder(
            context,
            services.GetRequiredService<ILogger<UT_WMSDotnet.Data.DbSeeder>>());

        await seeder.SeedAsync();
        
        logger.LogInformation("Setup database dan seeding selesai.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "FAILED STARTUP: Migration or Seeding failed.");
    }
}

// =======================
// API PDF Endpoints
// =======================
app.MapGet("/api/pdf/peminjaman", async (string start, string end, WMS_UnitedTracors_Blazor.Services.ReportService reportService, WMS_UnitedTracors_Blazor.Services.PdfReportService pdfService, IWebHostEnvironment env) =>
{
    var startDate = DateTime.Parse(start);
    var endDate = DateTime.Parse(end);
    var transactions = await reportService.GetTransactionsForReportAsync(startDate, endDate, "BORROW");
    var pdf = pdfService.GeneratePeminjamanReport(transactions, startDate, endDate, env.WebRootPath);
    return Results.File(pdf, "application/pdf", $"Laporan_Peminjaman_{DateTime.Now:yyyyMMdd}.pdf");
});

app.MapGet("/api/pdf/request", async (string start, string end, WMS_UnitedTracors_Blazor.Services.ReportService reportService, WMS_UnitedTracors_Blazor.Services.PdfReportService pdfService, IWebHostEnvironment env) =>
{
    var startDate = DateTime.Parse(start);
    var endDate = DateTime.Parse(end);
    var transactions = await reportService.GetTransactionsForReportAsync(startDate, endDate, "GIVEAWAY");
    var pdf = pdfService.GenerateRequestReport(transactions, startDate, endDate, env.WebRootPath);
    return Results.File(pdf, "application/pdf", $"Laporan_Request_{DateTime.Now:yyyyMMdd}.pdf");
});

app.MapGet("/api/pdf/adjust", async (string start, string end, WMS_UnitedTracors_Blazor.Services.ReportService reportService, WMS_UnitedTracors_Blazor.Services.PdfReportService pdfService, IWebHostEnvironment env) =>
{
    var startDate = DateTime.Parse(start);
    var endDate = DateTime.Parse(end);
    var transactions = await reportService.GetTransactionsForReportAsync(startDate, endDate, "ADJUST");
    var pdf = pdfService.GenerateAdjustReport(transactions, startDate, endDate, env.WebRootPath);
    return Results.File(pdf, "application/pdf", $"Laporan_UpdateStok_{DateTime.Now:yyyyMMdd}.pdf");
});

app.MapGet("/api/pdf/receipt/{groupId}", async (string groupId, WMS_UnitedTracors_Blazor.Services.TransactionService transactionService, WMS_UnitedTracors_Blazor.Services.PdfReceiptService pdfService, IWebHostEnvironment env) =>
{
    var transactions = await transactionService.GetTransactionsByGroupIdAsync(groupId);
    if (!transactions.Any()) return Results.NotFound();
    
    var pdfBytes = pdfService.GenerateReceipt(transactions, env.WebRootPath);
    return Results.File(pdfBytes, "application/pdf", enableRangeProcessing: true);
});

app.MapGet("/api/pdf/receipt-request/{groupId}", async (string groupId, WMS_UnitedTracors_Blazor.Services.TransactionService transactionService, WMS_UnitedTracors_Blazor.Services.PdfReceiptService pdfService, IWebHostEnvironment env) =>
{
    var transactions = await transactionService.GetTransactionsByGroupIdAsync(groupId);
    if (!transactions.Any()) return Results.NotFound();
    
    var pdfBytes = pdfService.GenerateReceiptRequest(transactions, env.WebRootPath);
    return Results.File(pdfBytes, "application/pdf", enableRangeProcessing: true);
});

app.MapGet("/api/pdf/receipt-approval/{groupId}", async (string groupId, WMS_UnitedTracors_Blazor.Services.TransactionService transactionService, WMS_UnitedTracors_Blazor.Services.PdfReceiptService pdfService, IWebHostEnvironment env) =>
{
    var transactions = await transactionService.GetTransactionsByGroupIdAsync(groupId);
    if (!transactions.Any()) return Results.NotFound();
    
    var pdfBytes = pdfService.GenerateReceiptApproval(transactions, env.WebRootPath);
    return Results.File(pdfBytes, "application/pdf", enableRangeProcessing: true);
});

app.MapGet("/api/pdf/receipt-handover/{groupId}", async (string groupId, WMS_UnitedTracors_Blazor.Services.TransactionService transactionService, WMS_UnitedTracors_Blazor.Services.PdfReceiptService pdfService, IWebHostEnvironment env) =>
{
    var transactions = await transactionService.GetTransactionsByGroupIdAsync(groupId);
    if (!transactions.Any()) return Results.NotFound();
    
    var pdfBytes = pdfService.GenerateReceiptHandover(transactions, env.WebRootPath);
    return Results.File(pdfBytes, "application/pdf", enableRangeProcessing: true);
});

app.MapGet("/api/pdf/receipt-documentation/{groupId}", async (string groupId, WMS_UnitedTracors_Blazor.Services.TransactionService transactionService, WMS_UnitedTracors_Blazor.Services.PdfReceiptService pdfService, IWebHostEnvironment env) =>
{
    var transactions = await transactionService.GetTransactionsByGroupIdAsync(groupId);
    if (!transactions.Any()) return Results.NotFound();

    var pdfBytes = pdfService.GenerateReceiptDocumentation(transactions, env.WebRootPath);
    return Results.File(pdfBytes, "application/pdf", enableRangeProcessing: true);
});

app.MapGet("/api/pdf/receipt-return/{groupId}", async (string groupId, WMS_UnitedTracors_Blazor.Services.TransactionService transactionService, WMS_UnitedTracors_Blazor.Services.PdfReceiptService pdfService, IWebHostEnvironment env) =>
{
    var transactions = await transactionService.GetTransactionsByGroupIdAsync(groupId);
    if (!transactions.Any()) return Results.NotFound();
    
    var pdfBytes = pdfService.GenerateReturnReceipt(transactions, env.WebRootPath);
    return Results.File(pdfBytes, "application/pdf", enableRangeProcessing: true);
});

// =======================
// Admin Roles API
// =======================
var adminRolesGroup = app.MapGroup("/api/admin-roles");

adminRolesGroup.MapGet("/", async (string? search, string? sortColumn, bool? sortDescending, int? page, int? pageSize, WMS_UnitedTracors_Blazor.Services.AdminRoleService service) =>
{
    var (roles, totalItems, totalPages) = await service.GetAdminRolesAsync(search, sortColumn ?? "RoleName", sortDescending ?? false, page ?? 1, pageSize ?? 10);
    return Results.Ok(new { roles, totalItems, totalPages });
});

adminRolesGroup.MapGet("/{id:guid}", async (Guid id, WMS_UnitedTracors_Blazor.Services.AdminRoleService service) =>
{
    var role = await service.GetAdminRoleByIdAsync(id);
    if (role == null) return Results.NotFound("Role tidak ditemukan.");
    var assignedUsers = await service.GetAssignedUsersAsync(id);
    return Results.Ok(new { role, assignedUsers });
});

adminRolesGroup.MapPost("/", async (UT_WMSDotnet.ViewModels.AdminRoleViewModel model, HttpContext context, WMS_UnitedTracors_Blazor.Services.AdminRoleService service) =>
{
    var username = context.User.Identity?.Name ?? "System";
    var error = await service.CreateAdminRoleAsync(model, username);
    if (error != null) return Results.BadRequest(error);
    return Results.Ok("Role berhasil ditambahkan.");
});

adminRolesGroup.MapPut("/{id:guid}", async (Guid id, UT_WMSDotnet.ViewModels.AdminRoleViewModel model, HttpContext context, WMS_UnitedTracors_Blazor.Services.AdminRoleService service) =>
{
    var username = context.User.Identity?.Name ?? "System";
    var error = await service.UpdateAdminRoleAsync(id, model, username);
    if (error != null) return Results.BadRequest(error);
    return Results.Ok("Role berhasil diupdate.");
});

adminRolesGroup.MapDelete("/{id:guid}", async (Guid id, WMS_UnitedTracors_Blazor.Services.AdminRoleService service) =>
{
    var error = await service.DeleteAdminRoleAsync(id);
    if (error != null) return Results.BadRequest(error);
    return Results.Ok("Role berhasil dihapus.");
});

adminRolesGroup.MapPost("/{id:guid}/assign", async (Guid id, [Microsoft.AspNetCore.Mvc.FromBody] int userId, HttpContext context, WMS_UnitedTracors_Blazor.Services.AdminRoleService service) =>
{
    var username = context.User.Identity?.Name ?? "System";
    var error = await service.AssignUserToRoleAsync(id, userId, null, username);
    if (error != null) return Results.BadRequest(error);
    return Results.Ok("User berhasil ditambahkan ke role.");
});

adminRolesGroup.MapDelete("/{id:guid}/assign/{userId:int}", async (Guid id, int userId, WMS_UnitedTracors_Blazor.Services.AdminRoleService service) =>
{
    var error = await service.RemoveUserFromRoleAsync(id, userId, null);
    if (error != null) return Results.BadRequest(error);
    return Results.Ok("User berhasil dihapus dari role.");
});

// =======================
// Start Application
// =======================
app.Run();
