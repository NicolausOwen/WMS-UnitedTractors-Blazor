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
builder.Services.AddScoped<ReportService>();

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
