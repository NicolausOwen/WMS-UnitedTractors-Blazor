using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using UT_WMSDotnet.Data;
using System.IO;

namespace SchemaUpdater
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            var provider = services.BuildServiceProvider();
            var dbContext = provider.GetRequiredService<ApplicationDbContext>();

            Console.WriteLine("Applying schema updates...");

            // Each statement runs independently so an already-applied change does not block the rest.
            var statements = new (string Label, string Sql)[]
            {
                ("Expand transactions.status enum (add WAITING_HANDOVER / WAITING_ADMIN_HANDOVER)",
                 "ALTER TABLE transactions MODIFY COLUMN status ENUM('PENDING','PENDING_MANAGER','WAITING_HANDOVER','WAITING_ADMIN_HANDOVER','APPROVED','REJECTED','REVISION') NOT NULL DEFAULT 'PENDING';"),

                ("Add transactions.handover_photo",
                 "ALTER TABLE transactions ADD COLUMN handover_photo varchar(255) NULL AFTER return_photo;"),

                ("Add transactions.handover_notes",
                 "ALTER TABLE transactions ADD COLUMN handover_notes text NULL AFTER handover_photo;"),

                ("Set is_returnable = 0 for giveaway categories (Merchandise, ATK, Makanan, Facility)",
                 "UPDATE products p JOIN categories c ON c.id = p.category_id SET p.is_returnable = 0 WHERE c.name IN ('Merchandise','ATK','Makanan','Facility');"),

                ("Set is_returnable = 1 for borrowing categories (Alat Musik, Elektronik, Game)",
                 "UPDATE products p JOIN categories c ON c.id = p.category_id SET p.is_returnable = 1 WHERE c.name IN ('Alat Musik','Elektronik','Game');"),
            };

            foreach (var (label, sql) in statements)
            {
                try
                {
                    await dbContext.Database.ExecuteSqlRawAsync(sql);
                    Console.WriteLine($"[OK]   {label}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SKIP] {label} -> {ex.Message}");
                }
            }

            Console.WriteLine("Done.");
        }
    }
}
