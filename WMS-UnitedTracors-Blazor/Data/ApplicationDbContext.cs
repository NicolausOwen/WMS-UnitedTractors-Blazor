using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Models;

namespace UT_WMSDotnet.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Division> Divisions { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<StockLog> StockLogs { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProfileRequest> ProfileRequests { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AdminRole> AdminRoles { get; set; }
    public DbSet<UserAdminRole> UserAdminRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure restrictive delete behavior for relationships to prevent multiple cascade paths
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Map tables to match SQL dump
        modelBuilder.Entity<Category>().ToTable("categories");
        modelBuilder.Entity<Division>().ToTable("divisions");
        modelBuilder.Entity<Location>().ToTable("locations");
        modelBuilder.Entity<Product>().ToTable("products");
        modelBuilder.Entity<Transaction>().ToTable("transactions");
        modelBuilder.Entity<Unit>().ToTable("units");
        modelBuilder.Entity<ProductVariant>().ToTable("product_variants");
        modelBuilder.Entity<ProfileRequest>().ToTable("profile_requests");
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<AdminRole>().ToTable("admin_roles");
        modelBuilder.Entity<UserAdminRole>().ToTable("user_admin_roles");

        // Additional constraints based on models
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Requester)
            .WithMany()
            .HasForeignKey(t => t.requester_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Approver)
            .WithMany()
            .HasForeignKey(t => t.approver_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.StaffInventoryApprover)
            .WithMany()
            .HasForeignKey(t => t.staff_inventory_approver_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.AdminApprover)
            .WithMany()
            .HasForeignKey(t => t.admin_approver_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.ManagerApprover)
            .WithMany()
            .HasForeignKey(t => t.manager_approver_id)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Transactions)
            .WithOne(t => t.Product)
            .HasForeignKey(t => t.product_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.ProductVariant)
            .WithMany()
            .HasForeignKey(t => t.product_variant_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProfileRequest>()
            .HasOne(pr => pr.User)
            .WithMany()
            .HasForeignKey(pr => pr.user_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserAdminRole>()
            .HasOne(uar => uar.User)
            .WithMany(u => u.UserAdminRoles)
            .HasForeignKey(uar => uar.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserAdminRole>()
            .HasOne(uar => uar.AdminRole)
            .WithMany(ar => ar.UserAdminRoles)
            .HasForeignKey(uar => uar.AdminRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override int SaveChanges()
    {
        ConvertDateTimeToWib();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ConvertDateTimeToWib();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ConvertDateTimeToWib()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            foreach (var property in entry.Properties)
            {
                if (property.Metadata.ClrType == typeof(DateTime) || property.Metadata.ClrType == typeof(DateTime?))
                {
                    if (property.CurrentValue is DateTime dt)
                    {
                        if (dt.Kind == DateTimeKind.Utc)
                        {
                            property.CurrentValue = dt.AddHours(7);
                        }
                    }
                }
            }
        }
    }
}
