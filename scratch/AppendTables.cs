using System;
using System.IO;

class Program
{
    static void Main()
    {
        var destPath = @"C:\Users\User\source\repos\WMS-UnitedTracors-Blazor\WMS-UnitedTracors-Blazor\Migrations\Seeders\wms_united_tractors_clean.sql";
        var sql = @"
-- ------------------------------------------------------------
-- Table: profile_requests
-- ------------------------------------------------------------
CREATE TABLE `profile_requests` (
  `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` bigint UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nrp` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `division_id` bigint UNSIGNED DEFAULT NULL,
  `status` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `profile_requests_user_id_foreign` (`user_id`),
  KEY `profile_requests_division_id_foreign` (`division_id`),
  CONSTRAINT `profile_requests_division_id_foreign` FOREIGN KEY (`division_id`) REFERENCES `divisions` (`id`) ON DELETE SET NULL,
  CONSTRAINT `profile_requests_user_id_foreign` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ------------------------------------------------------------
-- Table: stock_logs
-- ------------------------------------------------------------
CREATE TABLE `stock_logs` (
  `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT,
  `transaction_id` bigint UNSIGNED NOT NULL,
  `product_id` bigint UNSIGNED NOT NULL,
  `stock_before` int NOT NULL,
  `stock_after` int NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `stock_logs_transaction_id_foreign` (`transaction_id`),
  KEY `stock_logs_product_id_foreign` (`product_id`),
  CONSTRAINT `stock_logs_product_id_foreign` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE CASCADE,
  CONSTRAINT `stock_logs_transaction_id_foreign` FOREIGN KEY (`transaction_id`) REFERENCES `transactions` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
";
        File.AppendAllText(destPath, sql);
    }
}
