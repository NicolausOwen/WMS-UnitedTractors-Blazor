using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using WMS_UnitedTracors_Blazor.Components;
using Microsoft.AspNetCore.Authentication.Cookies;

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

// Database setup
builder.Services.AddDbContext<ApplicationDbContext>(options =>
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
        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = context => Task.CompletedTask
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

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
}).AllowAnonymous();

app.MapPost("/Logout", async (HttpContext context) =>
{
    await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(context, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).AllowAnonymous();

app.MapGet("/auth/microsoft", () =>
{
    var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = "/auth/microsoft/callback" };
    return Results.Challenge(properties, new[] { "Microsoft" });
}).AllowAnonymous();

app.MapGet("/auth/microsoft/callback", async (HttpContext context, WMS_UnitedTracors_Blazor.Services.AuthService authService) =>
{
    var result = await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.AuthenticateAsync(context, "ExternalCookie");
    if (!result.Succeeded)
    {
        return Results.Redirect("/login?errorMessage=Failed+to+authenticate+with+Microsoft.");
    }

    var claims = result.Principal?.Claims;
    var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value
             ?? claims?.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

    if (string.IsNullOrEmpty(email))
    {
        return Results.Redirect("/login?errorMessage=Gagal+mendapatkan+alamat+email+dari+akun+Microsoft+Anda.");
    }

    var (principal, error) = await authService.MicrosoftLoginAsync(email, claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value);
    
    if (principal == null)
    {
        await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(context, "ExternalCookie");
        return Results.Redirect($"/login?errorMessage={Uri.EscapeDataString(error ?? "Akses ditolak.")}");
    }

    await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(context, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, principal);
    await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(context, "ExternalCookie");

    return Results.Redirect("/");
}).AllowAnonymous();

// Seed database
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("RunMigrations", false))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var seeder = new UT_WMSDotnet.Data.DbSeeder(context, services.GetRequiredService<ILogger<UT_WMSDotnet.Data.DbSeeder>>());
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "FAILED STARTUP: Migration or Seeding failed.");
        }
    }
}

app.Run();
