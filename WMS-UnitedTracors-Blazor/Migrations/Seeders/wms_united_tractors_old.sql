-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Generation Time: Jun 03, 2026 at 10:00 PM
-- Server version: 8.0.45
-- PHP Version: 8.3.9

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `wms_united_tractors`
--

-- --------------------------------------------------------

--
-- Table structure for table `cache`
--

CREATE TABLE `cache` (
  `key` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `value` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `expiration` bigint NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `cache_locks`
--

CREATE TABLE `cache_locks` (
  `key` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `owner` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `expiration` bigint NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `categories`
--

CREATE TABLE `categories` (
  `id` bigint UNSIGNED NOT NULL,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `categories`
--

INSERT INTO `categories` (`id`, `name`, `description`, `created_at`, `updated_at`) VALUES
(1, 'Makanan', NULL, '2026-05-11 12:17:24', '2026-05-11 12:17:24'),
(3, 'Game', NULL, '2026-05-11 13:40:10', '2026-05-11 13:40:10'),
(4, 'Facility', NULL, '2026-05-11 13:40:10', '2026-05-11 13:40:10'),
(5, 'ATK', NULL, '2026-05-11 13:40:10', '2026-05-11 13:40:10'),
(6, 'Merchandise', NULL, '2026-05-11 13:40:10', '2026-05-11 13:40:10'),
(8, 'Alat Musik', NULL, '2026-05-11 13:40:11', '2026-05-11 13:40:11'),
(9, 'Elektronik', NULL, '2026-05-11 13:40:11', '2026-05-11 13:40:11');

-- --------------------------------------------------------

--
-- Table structure for table `divisions`
--

CREATE TABLE `divisions` (
  `id` bigint UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `divisions`
--

INSERT INTO `divisions` (`id`, `name`, `description`, `created_at`, `updated_at`) VALUES
(1, 'CCS', NULL, '2026-06-03 09:25:53', '2026-06-03 09:25:53'),
(2, 'CFA', NULL, '2026-06-03 09:25:53', '2026-06-03 09:25:53'),
(3, 'CHCU', NULL, '2026-06-03 09:25:53', '2026-06-03 09:25:53'),
(4, 'CRA', NULL, '2026-06-03 09:25:53', '2026-06-03 09:25:53'),
(5, 'CST', NULL, '2026-06-03 09:25:53', '2026-06-03 09:25:53'),
(6, 'DAD', NULL, '2026-06-03 09:25:53', '2026-06-03 09:25:53'),
(7, 'GLG', NULL, '2026-06-03 09:25:53', '2026-06-03 09:25:53'),
(8, 'MKT', NULL, '2026-06-03 09:25:53', '2026-06-03 09:25:53'),
(9, 'PIN', NULL, '2026-06-03 09:25:53', '2026-06-03 09:25:53'),
(10, 'PRT', NULL, '2026-06-03 09:25:53', '2026-06-03 09:25:53'),
(11, 'SOD', NULL, '2026-06-03 09:25:53', '2026-06-03 09:25:53'),
(12, 'SVC', NULL, '2026-06-03 09:25:54', '2026-06-03 09:25:54'),
(13, 'TMO', NULL, '2026-06-03 09:25:54', '2026-06-03 09:25:54'),
(14, 'TSO', NULL, '2026-06-03 09:25:54', '2026-06-03 09:25:54');

-- --------------------------------------------------------

--
-- Table structure for table `failed_jobs`
--

CREATE TABLE `failed_jobs` (
  `id` bigint UNSIGNED NOT NULL,
  `uuid` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `connection` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `queue` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `payload` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `exception` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `failed_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `jobs`
--

CREATE TABLE `jobs` (
  `id` bigint UNSIGNED NOT NULL,
  `queue` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `payload` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `attempts` smallint UNSIGNED NOT NULL,
  `reserved_at` int UNSIGNED DEFAULT NULL,
  `available_at` int UNSIGNED NOT NULL,
  `created_at` int UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `job_batches`
--

CREATE TABLE `job_batches` (
  `id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `total_jobs` int NOT NULL,
  `pending_jobs` int NOT NULL,
  `failed_jobs` int NOT NULL,
  `failed_job_ids` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `options` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `cancelled_at` int DEFAULT NULL,
  `created_at` int NOT NULL,
  `finished_at` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `locations`
--

CREATE TABLE `locations` (
  `id` bigint UNSIGNED NOT NULL,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `locations`
--

INSERT INTO `locations` (`id`, `name`, `description`, `created_at`, `updated_at`) VALUES
(1, 'Gudang', 'Gudang lt 1', '2026-05-11 12:57:18', '2026-05-11 12:57:18'),
(2, 'Storage Room', NULL, '2026-05-11 13:40:10', '2026-05-11 13:40:10'),
(3, 'ATK', NULL, '2026-05-11 13:40:10', '2026-05-11 13:40:10'),
(4, 'Makeup Room', NULL, '2026-05-11 13:40:10', '2026-05-11 13:40:10'),
(5, 'Merchandise', NULL, '2026-05-11 13:40:11', '2026-05-11 13:40:11'),
(6, '7.1.12.2', NULL, '2026-05-11 13:40:11', '2026-05-11 13:40:11'),
(7, '7.1.12.3', NULL, '2026-05-11 13:40:11', '2026-05-11 13:40:11'),
(8, '7.1.11.1', NULL, '2026-05-11 13:40:11', '2026-05-11 13:40:11'),
(9, '7.1.11.2', NULL, '2026-05-11 13:40:11', '2026-05-11 13:40:11'),
(10, '7.1.11.3', NULL, '2026-05-11 13:40:11', '2026-05-11 13:40:11'),
(11, '7.1.11.5', NULL, '2026-05-11 13:40:11', '2026-05-11 13:40:11');

-- --------------------------------------------------------

--
-- Table structure for table `migrations`
--

CREATE TABLE `migrations` (
  `id` int UNSIGNED NOT NULL,
  `migration` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `batch` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `migrations`
--

INSERT INTO `migrations` (`id`, `migration`, `batch`) VALUES
(1, '0001_01_01_000000_create_users_table', 1),
(2, '0001_01_01_000001_create_cache_table', 1),
(3, '0001_01_01_000002_create_jobs_table', 1),
(4, '2026_05_11_120115_create_products_table', 1),
(5, '2026_05_11_120123_create_transactions_table', 1),
(6, '2026_05_11_120125_create_stock_logs_table', 1),
(7, '2026_05_11_123206_add_barcode_type_to_products_table', 1),
(8, '2026_05_12_010000_create_categories_and_tracking', 1),
(9, '2026_05_12_020000_add_details_to_products_table', 2),
(10, '2026_05_12_025000_create_locations_table', 3),
(11, '2026_05_12_042900_create_divisions_table', 4),
(12, '2026_05_12_062303_add_division_id_to_users_and_transactions', 4),
(13, '2026_05_12_083559_add_borrowing_details_to_transactions_table', 4),
(14, '2026_05_12_085619_rename_division_string_column_in_transactions_table', 4),
(15, '2026_05_13_000000_add_rejection_reason_to_transactions_table', 4),
(16, '2026_05_17_110214_add_return_columns_to_products_and_transactions_tables', 4),
(17, '2026_05_18_005251_add_request_type_to_transactions_table', 4),
(18, '2026_05_18_032357_add_pending_return_quantity_to_transactions_table', 4),
(19, '2026_05_18_070319_modify_users_table_role_enum', 4),
(20, '2026_05_18_073237_add_return_condition_to_transactions_table', 4),
(21, '2026_05_18_074213_create_units_table', 4),
(22, '2026_05_18_081000_modify_products_table_unit_relation', 4),
(23, '2026_05_18_083710_add_return_details_to_transactions_table', 4),
(24, '2026_05_18_100000_add_nrp_to_users_table', 4),
(25, '2026_05_18_120000_add_borrow_duration_to_transactions_table', 4),
(26, '2026_05_19_151643_add_poin_to_users', 4),
(27, '2026_05_19_153542_add_value_to_products', 4),
(28, '2026_05_20_123704_add_is_return_draft_to_transactions_table', 4),
(29, '2026_05_21_085251_create_product_variants_table', 4),
(30, '2026_05_21_085252_add_transaction_type_and_images_to_products_table', 4),
(31, '2026_05_21_085252_update_transactions_for_variants_and_dates', 4),
(32, '2026_05_21_093908_add_image_to_product_variants_table', 4),
(33, '2026_05_21_161103_add_return_rejection_reason_to_transactions_table', 4),
(34, '2026_05_22_082602_create_profile_requests_table', 4),
(35, '2026_05_22_090000_add_multistage_approval_fields_to_transactions', 4),
(36, '2026_05_22_165747_add_revision_status_to_transactions', 4),
(37, '2026_05_26_083640_add_description_to_products_table', 4);

-- --------------------------------------------------------

--
-- Table structure for table `password_reset_tokens`
--

CREATE TABLE `password_reset_tokens` (
  `email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `token` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `products`
--

CREATE TABLE `products` (
  `id` bigint UNSIGNED NOT NULL,
  `sku` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `barcode_type` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'TYPE_CODE_128',
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `transaction_type` enum('BORROW','REQUEST') COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `value` decimal(10,2) DEFAULT NULL,
  `image` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `images` json DEFAULT NULL,
  `category_id` bigint UNSIGNED DEFAULT NULL,
  `location_id` bigint UNSIGNED DEFAULT NULL,
  `position_image` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `current_stock` int NOT NULL DEFAULT '0',
  `initial_stock` int NOT NULL DEFAULT '0',
  `unit_id` bigint UNSIGNED DEFAULT NULL,
  `is_returnable` tinyint(1) NOT NULL DEFAULT '1',
  `min_stock` int NOT NULL DEFAULT '0',
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `products`
--

INSERT INTO `products` (`id`, `sku`, `barcode_type`, `name`, `description`, `transaction_type`, `value`, `image`, `images`, `category_id`, `location_id`, `position_image`, `current_stock`, `initial_stock`, `unit_id`, `is_returnable`, `min_stock`, `created_at`, `updated_at`) VALUES
(1, 'GME-260511-0001', 'TYPE_CODE_128', 'Stick Ice Cream', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 8, 8, 1, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(2, 'GME-260511-0002', 'TYPE_CODE_128', 'Bola kecil', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 2, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(3, 'GME-260511-0003', 'TYPE_CODE_128', 'Caping', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 6, 6, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(4, 'GME-260511-0004', 'TYPE_CODE_128', 'Pompa Balon', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 6, 6, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(5, 'GME-260511-0005', 'TYPE_CODE_128', 'Keranjang Sampah', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 12, 12, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(6, 'FCY-260511-0001', 'TYPE_CODE_128', 'Kain Hitam', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 4, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(7, 'GME-260511-0006', 'TYPE_CODE_128', 'Sumpit', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 343, 343, 5, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(8, 'GME-260511-0007', 'TYPE_CODE_128', 'Bola Tenis', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 42, 42, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(9, 'FCY-260511-0002', 'TYPE_CODE_128', 'Masker Hitam', NULL, NULL, 50.00, NULL, NULL, 4, 2, NULL, 20, 20, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(10, 'GME-260511-0008', 'TYPE_CODE_128', 'Bola Pingpong', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 49, 49, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(11, 'GME-260511-0009', 'TYPE_CODE_128', 'Bola Golf', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 12, 12, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(12, 'GME-260511-0010', 'TYPE_CODE_128', 'Bola Kelereng', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 45, 45, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(13, 'MKN-260511-0001', 'TYPE_CODE_128', 'Tepung Maizena exp Nov 2026', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 6, 6, 6, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(14, 'MKN-260511-0002', 'TYPE_CODE_128', 'Tepung Roti exp 27 Nov 2026', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 6, 6, 6, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(15, 'MKN-260511-0003', 'TYPE_CODE_128', 'Santan exp April 2027', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 10, 10, 6, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(16, 'FCY-260511-0003', 'TYPE_CODE_128', 'Tissue', NULL, NULL, 50.00, NULL, NULL, 4, 2, NULL, 5, 5, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(17, 'MKN-260511-0004', 'TYPE_CODE_128', 'Gula exp 2027', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 5, 5, 6, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(18, 'MKN-260511-0005', 'TYPE_CODE_128', 'Gula halus rose brand exp nov 27', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 1, 1, 7, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(19, 'MKN-260511-0006', 'TYPE_CODE_128', 'Blue Band', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 6, 6, 6, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(20, 'MKN-260511-0007', 'TYPE_CODE_128', 'Dancow exp Maret 2027', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 8, 8, 8, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(21, 'MKN-260511-0008', 'TYPE_CODE_128', 'Frisian flag Exp Agustus 2026', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 18, 18, 8, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(22, 'MKN-260511-0009', 'TYPE_CODE_128', 'Ladaku Exp 2029', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 15, 15, 9, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(23, 'MKN-260511-0010', 'TYPE_CODE_128', 'Fermipan Exp 2027', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 2, 2, 10, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(24, 'MKN-260511-0011', 'TYPE_CODE_128', 'Pemanis buatan', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 1, 1, 8, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(25, 'GME-260511-0011', 'TYPE_CODE_128', 'Ember Merah', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(26, 'GME-260511-0012', 'TYPE_CODE_128', 'Mission Impossible (Permainan Bambu Biru Merah)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 48, 48, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(27, 'GME-260511-0013', 'TYPE_CODE_128', 'The Master of Risk', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(28, 'GME-260511-0014', 'TYPE_CODE_128', 'Spot The Difference', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 60, 60, 11, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(29, 'GME-260511-0015', 'TYPE_CODE_128', 'Puzzle taplak', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 32, 32, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(30, 'GME-260511-0016', 'TYPE_CODE_128', 'Cash drawer', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 12, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(31, 'GME-260511-0017', 'TYPE_CODE_128', 'Labble Scrabble (Board Only)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(32, 'GME-260511-0018', 'TYPE_CODE_128', 'Lego besar Hijau', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 13, 13, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(33, 'GME-260511-0019', 'TYPE_CODE_128', 'Lego kecil', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 10, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(34, 'MKN-260511-0012', 'TYPE_CODE_128', 'Minyak exp des 27', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 6, 6, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(35, 'MKN-260511-0013', 'TYPE_CODE_128', 'Tepung Terigu exp mei 27', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 2, 2, 6, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(36, 'MKN-260511-0014', 'TYPE_CODE_128', 'Tepung terigu', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 3, 3, 13, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(37, 'MKN-260511-0015', 'TYPE_CODE_128', 'Tepung Tapioka exp des 27', NULL, NULL, 50.00, NULL, NULL, 1, 2, NULL, 2, 2, 6, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(38, 'GME-260511-0020', 'TYPE_CODE_128', 'Lost in Paris', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(39, 'GME-260511-0021', 'TYPE_CODE_128', 'Tusuk sate', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(40, 'GME-260511-0022', 'TYPE_CODE_128', 'Kuas', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 23, 23, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(41, 'GME-260511-0023', 'TYPE_CODE_128', 'Set kuas', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 4, 4, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(42, 'GME-260511-0024', 'TYPE_CODE_128', 'Chess Set Game', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(43, 'GME-260511-0025', 'TYPE_CODE_128', 'Monopoli', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 5, 5, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(44, 'ATK-260511-0001', 'TYPE_CODE_128', 'Receipt Printer', NULL, NULL, 50.00, NULL, NULL, 5, 3, NULL, 1, 1, 12, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(45, 'FCY-260511-0004', 'TYPE_CODE_128', 'Tali tambang plastik', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 9, 9, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(46, 'GME-260511-0026', 'TYPE_CODE_128', 'Games Matras Nomor', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 25, 25, 11, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(47, 'FCY-260511-0005', 'TYPE_CODE_128', 'Tali rapia', NULL, NULL, 50.00, NULL, NULL, 4, 2, NULL, 3, 3, 12, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(48, 'FCY-260511-0006', 'TYPE_CODE_128', 'Jas ujan', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 8, 8, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(49, 'GME-260511-0027', 'TYPE_CODE_128', 'Sedotan warna warni', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 22, 22, 15, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(50, 'GME-260511-0028', 'TYPE_CODE_128', 'Bel warna warni', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 41, 41, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(51, 'FCY-260511-0007', 'TYPE_CODE_128', 'Hanger', NULL, NULL, 50.00, NULL, NULL, 4, 2, NULL, 18, 12, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(53, 'GME-260511-0029', 'TYPE_CODE_128', 'Balon Supporter', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 6, 6, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(54, 'GME-260511-0030', 'TYPE_CODE_128', 'Karung', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(55, 'FCY-260511-0008', 'TYPE_CODE_128', 'Tempat tissue', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(56, 'MHE-260511-0001', 'TYPE_CODE_128', 'Tumblr corpu', NULL, NULL, 50.00, NULL, NULL, 6, 2, NULL, 7, 7, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(57, 'MHE-260511-0002', 'TYPE_CODE_128', 'Tumblr IF', NULL, NULL, 50.00, NULL, NULL, 6, 2, NULL, 5, 5, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(58, 'GME-260511-0031', 'TYPE_CODE_128', 'Dangerous Crossing', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(59, 'GME-260511-0032', 'TYPE_CODE_128', 'Trompet', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(60, 'FCY-260511-0009', 'TYPE_CODE_128', 'Sarung Tangan', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 24, 24, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(61, 'GME-260511-0033', 'TYPE_CODE_128', 'Matras Puzzle Angry Bird', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 16, 16, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(62, 'GME-260511-0034', 'TYPE_CODE_128', 'Matras Puzzle Huruf', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 21, 21, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(63, 'UT-260512-0063', 'TYPE_CODE_128', 'Tas Hitam', NULL, NULL, 50.00, NULL, NULL, NULL, 4, NULL, 26, 26, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(64, 'UT-260512-0064', 'TYPE_CODE_128', 'Tas Hijau', NULL, NULL, 50.00, NULL, NULL, NULL, 4, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(67, 'MHE-260511-0003', 'TYPE_CODE_128', 'Topi Bucket', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 9, 9, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(68, 'MHE-260511-0004', 'TYPE_CODE_128', 'Cargo Rip Stop', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 6, 6, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(69, 'MHE-260511-0005', 'TYPE_CODE_128', 'Cap Coklat', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 4, 4, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(72, 'MSK-260511-0001', 'TYPE_CODE_128', 'Angklung', NULL, NULL, 50.00, NULL, NULL, 8, 2, NULL, 5, 5, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(75, 'FCY-260511-0010', 'TYPE_CODE_128', 'Piagam Generasi Muda 2015', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(76, 'FCY-260511-0011', 'TYPE_CODE_128', 'Photo Frame', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(77, 'FCY-260511-0012', 'TYPE_CODE_128', 'Piagam Instruktur Terbaik 2009', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(78, 'FCY-260511-0013', 'TYPE_CODE_128', 'Piagam Instruktur Terbaik 2010', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(79, 'FCY-260511-0014', 'TYPE_CODE_128', 'Skor Pelanggaran Tata Tertib', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(80, 'FCY-260511-0015', 'TYPE_CODE_128', 'Piagam Mektel 2012', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(81, 'FCY-260511-0016', 'TYPE_CODE_128', 'Piagam Mektel 2013', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(82, 'FCY-260511-0017', 'TYPE_CODE_128', 'Struktur Task Force 2011', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(83, 'FCY-260511-0018', 'TYPE_CODE_128', 'Photo Frame Kosong Hitam', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(84, 'FCY-260511-0019', 'TYPE_CODE_128', 'Kain Terpal', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(85, 'MHE-260511-0006', 'TYPE_CODE_128', 'Pulpen Corpu', NULL, NULL, 50.00, NULL, NULL, 6, 3, NULL, 7, 7, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(86, 'ATK-260511-0002', 'TYPE_CODE_128', 'Label Dot Orange', NULL, NULL, 50.00, NULL, NULL, 5, NULL, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(87, 'GME-260511-0035', 'TYPE_CODE_128', 'Huruf M', NULL, NULL, 50.00, NULL, NULL, 3, NULL, NULL, 10, 10, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(88, 'GME-260511-0036', 'TYPE_CODE_128', 'Huruf N', NULL, NULL, 50.00, NULL, NULL, 3, NULL, NULL, 6, 6, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(89, 'GME-260511-0037', 'TYPE_CODE_128', 'Game Bambu Size 1 Merah', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 16, 16, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(90, 'GME-260511-0038', 'TYPE_CODE_128', 'Game Bambu Size 2 Merah', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 16, 16, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(91, 'GME-260511-0039', 'TYPE_CODE_128', 'Game Bambu Size 3 Merah', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 8, 8, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(92, 'GME-260511-0040', 'TYPE_CODE_128', 'Game Bambu Size 4 Merah', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 8, 8, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(93, 'GME-260511-0041', 'TYPE_CODE_128', 'Game Bambu Size 1 Biru', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 15, 15, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(94, 'GME-260511-0042', 'TYPE_CODE_128', 'Game Bambu Size 2 Biru', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 16, 16, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(95, 'GME-260511-0043', 'TYPE_CODE_128', 'Game Bambu Size 3 Biru', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 9, 9, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(96, 'GME-260511-0044', 'TYPE_CODE_128', 'Game Bambu Size 4 Biru', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 8, 8, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(97, 'FCY-260511-0020', 'TYPE_CODE_128', 'Sertifikat PU 2017 with Frame', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 9, 9, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(98, 'FCY-260511-0021', 'TYPE_CODE_128', 'Miniatur Tiang Kecil', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(99, 'FCY-260511-0022', 'TYPE_CODE_128', 'Slayer Hijau', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(100, 'ATK-260511-0003', 'TYPE_CODE_128', 'Notebook Corpu', NULL, NULL, 50.00, NULL, NULL, 5, NULL, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(101, 'FCY-260511-0023', 'TYPE_CODE_128', 'Bingkai', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 37, 4, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(105, 'FCY-260511-0024', 'TYPE_CODE_128', 'Kompor Listrik', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(106, 'FCY-260511-0025', 'TYPE_CODE_128', 'Papan Jalan Biru + HVS', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(107, 'FCY-260511-0026', 'TYPE_CODE_128', 'Frame Foto Kecil Putih', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 8, 8, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(110, 'FCY-260511-0027', 'TYPE_CODE_128', 'Berkas Kegiatan PU (1 Bundle)', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 16, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(111, 'FCY-260511-0028', 'TYPE_CODE_128', 'Karikatur Kampus Merdeka', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(112, 'FCY-260511-0029', 'TYPE_CODE_128', 'Sertifikat sponsor', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(114, 'FCY-260511-0030', 'TYPE_CODE_128', 'Karikatur Best Prime Mover PUDP 2021', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 3, 3, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(115, 'GME-260511-0045', 'TYPE_CODE_128', 'How do you see it (Game)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 2, 2, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(116, 'GME-260511-0046', 'TYPE_CODE_128', 'Paradigm (Game)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 6, 6, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(117, 'GME-260511-0047', 'TYPE_CODE_128', 'SBS Challenge (Game)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(118, 'GME-260511-0048', 'TYPE_CODE_128', 'Productivity game', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(119, 'GME-260511-0049', 'TYPE_CODE_128', 'Guidance game', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 2, 2, 17, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(120, 'FCY-260511-0031', 'TYPE_CODE_128', 'Time Keeper', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(121, 'GME-260511-0050', 'TYPE_CODE_128', 'Find a match game', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 2, 2, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(122, 'GME-260511-0051', 'TYPE_CODE_128', 'Communicating for performance (Game)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(123, 'GME-260511-0052', 'TYPE_CODE_128', 'Party pooper', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(124, 'GME-260511-0053', 'TYPE_CODE_128', 'Airport Controller (Game)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(125, 'GME-260511-0054', 'TYPE_CODE_128', 'Puzzle balok', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(126, 'GME-260511-0055', 'TYPE_CODE_128', 'Category game', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 7, 7, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(127, 'GME-260511-0056', 'TYPE_CODE_128', 'Colaboration game', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 6, 6, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(128, 'GME-260511-0057', 'TYPE_CODE_128', 'Kartu cinta Indonesia', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(129, 'GME-260511-0058', 'TYPE_CODE_128', 'Puzzle jalan', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 6, 6, 18, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(130, 'GME-260511-0059', 'TYPE_CODE_128', 'Balon', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 4, 4, 19, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(131, 'GME-260511-0060', 'TYPE_CODE_128', 'The dangerous crossing', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 7, 7, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(132, 'GME-260511-0061', 'TYPE_CODE_128', 'Lets play music (Game)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(133, 'GME-260511-0062', 'TYPE_CODE_128', 'Stack them up (Game)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 6, 6, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(134, 'GME-260511-0063', 'TYPE_CODE_128', 'Ethics game', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(135, 'GME-260511-0064', 'TYPE_CODE_128', 'Ketapel', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 6, 6, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(136, 'GME-260511-0065', 'TYPE_CODE_128', 'Keyboard game', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(137, 'GME-260511-0066', 'TYPE_CODE_128', 'Out of my way (Game)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(138, 'GME-260511-0067', 'TYPE_CODE_128', 'Uno jenga', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(139, 'GME-260511-0068', 'TYPE_CODE_128', 'Jawaban devide es impera', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(140, 'GME-260511-0069', 'TYPE_CODE_128', 'Uang mainan', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 6, 6, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(141, 'GME-260511-0070', 'TYPE_CODE_128', 'Papan challenge ombak', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(142, 'GME-260511-0071', 'TYPE_CODE_128', 'Devide et impera (Game)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 23, 23, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(143, 'GME-260511-0072', 'TYPE_CODE_128', 'Who grows what (Game)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 2, 2, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(144, 'GME-260511-0073', 'TYPE_CODE_128', 'Botol kaca', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 3, 3, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(145, 'GME-260511-0074', 'TYPE_CODE_128', 'Human Leap', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 17, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(146, 'GME-260511-0075', 'TYPE_CODE_128', 'Ceforo challenge (Game)', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(147, 'GME-260511-0076', 'TYPE_CODE_128', 'Puzzle tetris', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(148, 'MSK-260511-0002', 'TYPE_CODE_128', 'Tamborin kerincing', NULL, NULL, 50.00, NULL, NULL, 8, 1, NULL, 1, 1, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(149, 'GME-260511-0077', 'TYPE_CODE_128', 'Mainboard', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(150, 'MSK-260511-0003', 'TYPE_CODE_128', 'Drum kecil', NULL, NULL, 50.00, NULL, NULL, 8, 1, NULL, 2, 2, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(151, 'MSK-260511-0004', 'TYPE_CODE_128', 'Stick drum', NULL, NULL, 50.00, NULL, NULL, 8, 1, NULL, 7, 7, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(152, 'ETK-260511-0001', 'TYPE_CODE_128', 'Cable manager', NULL, NULL, 50.00, NULL, NULL, 9, 1, NULL, 4, 4, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(153, 'ETK-260511-0002', 'TYPE_CODE_128', 'Kabel gulungan', NULL, NULL, 50.00, NULL, NULL, 9, 1, NULL, 3, 3, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(154, 'FCY-260511-0032', 'TYPE_CODE_128', 'Kawat gulungan', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(155, 'FCY-260511-0033', 'TYPE_CODE_128', 'Akrilik papan - nama ruangan', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 5, 5, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(156, 'FCY-260511-0034', 'TYPE_CODE_128', 'Silinder paralon', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 40, 40, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(157, 'FCY-260511-0035', 'TYPE_CODE_128', 'Pipa & paralon', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(158, 'FCY-260511-0036', 'TYPE_CODE_128', 'Canvas', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(159, 'FCY-260511-0037', 'TYPE_CODE_128', 'Penghargaan Astra Virtual Playday', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(160, 'FCY-260511-0038', 'TYPE_CODE_128', 'Penghargaan Astra Toll Road', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(161, 'GME-260511-0078', 'TYPE_CODE_128', 'Games Pie Face Showdown', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(162, 'FCY-260511-0039', 'TYPE_CODE_128', 'Remote Projector Epson', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 5, 5, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(163, 'FCY-260511-0040', 'TYPE_CODE_128', 'Remote Perfume Dispenser', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(164, 'ETK-260511-0003', 'TYPE_CODE_128', 'Commscope Port Unshuttered (LAN Port)', NULL, NULL, 50.00, NULL, NULL, 9, NULL, NULL, 5, 5, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(165, 'ETK-260511-0004', 'TYPE_CODE_128', 'Onfinity Wireless Interactive Whiteboard', NULL, NULL, 50.00, NULL, NULL, 9, NULL, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(166, 'FCY-260511-0041', 'TYPE_CODE_128', 'Case Pointer/Pulpen', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(167, 'GME-260511-0079', 'TYPE_CODE_128', 'Puzzle Kayu', NULL, NULL, 50.00, NULL, NULL, 3, NULL, NULL, 16, 16, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(168, 'FCY-260511-0042', 'TYPE_CODE_128', 'Aneka Kunci', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(169, 'FCY-260511-0043', 'TYPE_CODE_128', 'Laser Barcode Scanner', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(170, 'ATK-260511-0004', 'TYPE_CODE_128', 'Styrofoam', NULL, NULL, 50.00, NULL, NULL, 5, NULL, NULL, 6, 6, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(171, 'FCY-260511-0044', 'TYPE_CODE_128', 'Rangka X-Banner', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(172, 'FCY-260511-0045', 'TYPE_CODE_128', 'Raket Tennis Yonex', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(173, 'FCY-260511-0046', 'TYPE_CODE_128', 'Life Vest', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(174, 'FCY-260511-0047', 'TYPE_CODE_128', 'Plakat BPK Penabur Jakarta', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(175, 'FCY-260511-0048', 'TYPE_CODE_128', 'QCC Dashboard', NULL, NULL, 50.00, NULL, NULL, 4, NULL, NULL, 5, 5, 21, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(176, 'MHE-260511-0007', 'TYPE_CODE_128', 'Gelang Corpu', NULL, NULL, 50.00, NULL, NULL, 6, 2, NULL, 279, 30, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(180, 'FCY-260511-0049', 'TYPE_CODE_128', 'Gelas Plastik', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 69, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(183, 'FCY-260511-0050', 'TYPE_CODE_128', 'Cup', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 98, 20, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(185, 'FCY-260511-0051', 'TYPE_CODE_128', 'Benang', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 11, 5, 22, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(187, 'FCY-260511-0052', 'TYPE_CODE_128', 'Tali tambang putih', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 5, 5, 22, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(188, 'FCY-260511-0053', 'TYPE_CODE_128', 'Pita Dekorasi', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 4, 4, 22, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(189, 'FCY-260511-0054', 'TYPE_CODE_128', 'Kancing', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 5, 1, 23, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(193, 'FCY-260511-0055', 'TYPE_CODE_128', 'Jarum benang', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(194, 'GME-260511-0080', 'TYPE_CODE_128', 'Hulahop warna besar', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 23, 23, 24, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(195, 'GME-260511-0081', 'TYPE_CODE_128', 'Lilin malam', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 5, 5, 25, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(196, 'FCY-260511-0056', 'TYPE_CODE_128', 'Karet', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 1, 1, 19, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(197, 'MHE-260511-0008', 'TYPE_CODE_128', 'Jas Hitam Laki-laki', NULL, NULL, 50.00, NULL, NULL, 6, NULL, NULL, 5, 2, 26, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(200, 'MHE-260511-0009', 'TYPE_CODE_128', 'Kipas Handheld AHEMCEKECE', NULL, NULL, 50.00, NULL, NULL, 6, NULL, NULL, 4, 4, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(201, 'MHE-260511-0010', 'TYPE_CODE_128', 'Totebag AHEMCEKECE', NULL, NULL, 50.00, NULL, NULL, 6, NULL, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(202, 'GME-260511-0082', 'TYPE_CODE_128', 'Chips Karambol', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 42, 42, 10, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(203, 'GME-260511-0083', 'TYPE_CODE_128', 'Holahoop Kecil', NULL, NULL, 50.00, NULL, NULL, 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(204, 'FCY-260511-0057', 'TYPE_CODE_128', 'Cooking Gas Can', NULL, NULL, 50.00, NULL, NULL, 4, 1, NULL, 4, 4, 27, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(205, 'MHE-260511-0011', 'TYPE_CODE_128', 'Lanyard UTSMART Merah + Biru', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 90, 97, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(208, 'MHE-260511-0012', 'TYPE_CODE_128', 'Package UT Virtual Gathering - Kaos, Tumblr, Masket Mulut', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 6, 6, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(210, 'MHE-260511-0013', 'TYPE_CODE_128', 'Tas Reven', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 8, 4, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(213, 'MHE-260511-0014', 'TYPE_CODE_128', 'Sweater UT', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 113, 7, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(228, 'MHE-260511-0015', 'TYPE_CODE_128', 'Tumbler UT Smart', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 66, 73, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(229, 'MHE-260511-0016', 'TYPE_CODE_128', 'Tumbler Plastik UT Smart', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 367, 181, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(231, 'MHE-260511-0017', 'TYPE_CODE_128', 'Box Packaging Sweater UNTR', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 25, 25, 14, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(232, 'MHE-260511-0018', 'TYPE_CODE_128', 'Keranjang Anyaman', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 7, 7, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(233, 'MHE-260511-0019', 'TYPE_CODE_128', 'Stiker UT Smart Kuning Panjang', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 34, 34, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(234, 'FCY-260511-0058', 'TYPE_CODE_128', 'Good Detectors', NULL, NULL, 50.00, NULL, NULL, 4, 4, NULL, 3, 3, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(240, 'MHE-260511-0020', 'TYPE_CODE_128', 'Rompi PU 2025', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 14, 14, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(241, 'MHE-260511-0021', 'TYPE_CODE_128', 'Jersey UT', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 205, 5, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(253, 'MHE-260511-0022', 'TYPE_CODE_128', 'ID Card', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 768, 68, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(257, 'MHE-260511-0023', 'TYPE_CODE_128', 'ID Card Batik Tali', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 159, 97, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(263, 'MHE-260511-0024', 'TYPE_CODE_128', 'ID Card Holder', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 563, 508, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(265, 'MHE-260511-0025', 'TYPE_CODE_128', 'Giftset Mentor AFLP - Dus', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 155, 18, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(272, 'MHE-260511-0026', 'TYPE_CODE_128', 'Buku Batubara Indonesia', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 7, 7, 29, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(273, 'MHE-260511-0027', 'TYPE_CODE_128', 'Gift Box AFLP 2025 - Dus', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 94, 21, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(278, 'MHE-260511-0028', 'TYPE_CODE_128', 'Giftbox AALP Sleeve Laptop Sovlo', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 10, 10, 3, 0, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(279, 'MHE-260511-0029', 'TYPE_CODE_128', 'Giftbox besar AALP (kosong)', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 7, 7, 3, 0, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(280, 'MHE-260511-0030', 'TYPE_CODE_128', 'Giftbox Notebook PB + Pen - Dus', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 35, 20, 3, 0, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(282, 'MHE-260511-0031', 'TYPE_CODE_128', 'Merch Assessor - PB Baseus', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 5, 5, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(283, 'MHE-260511-0032', 'TYPE_CODE_128', 'Merch Assessor - TWS Baseus', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 20, 10, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(285, 'FCY-260511-0059', 'TYPE_CODE_128', 'Amplop Bubble', NULL, NULL, 50.00, NULL, NULL, 4, 4, NULL, 20, 20, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(286, 'ETK-260511-0005', 'TYPE_CODE_128', 'NS Accessories 18 in 1', NULL, NULL, 50.00, NULL, NULL, 9, 6, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(287, 'ETK-260511-0006', 'TYPE_CODE_128', 'NS Accesssories Ringfit Adventure', NULL, NULL, 50.00, NULL, NULL, 9, 6, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(288, 'ETK-260511-0007', 'TYPE_CODE_128', 'Kaset Game Nintendo', NULL, NULL, 50.00, NULL, NULL, 9, 6, NULL, 3, 3, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(289, 'GME-260511-0084', 'TYPE_CODE_128', 'Board Game Penguin', NULL, NULL, 50.00, NULL, NULL, 3, 6, NULL, 2, 2, 14, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(290, 'GME-260511-0085', 'TYPE_CODE_128', 'Klask - Board Game', NULL, NULL, 50.00, NULL, NULL, 3, 7, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(291, 'GME-260511-0086', 'TYPE_CODE_128', 'Board Game', NULL, NULL, 50.00, NULL, NULL, 3, 7, NULL, 2, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(293, 'FCY-260511-0060', 'TYPE_CODE_128', 'Bel Quiz', NULL, NULL, 50.00, NULL, NULL, 4, 8, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(294, 'GME-260511-0087', 'TYPE_CODE_128', '3D Puzzle Cube', NULL, NULL, 50.00, NULL, NULL, 3, 8, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(295, 'GME-260511-0088', 'TYPE_CODE_128', 'Rubik Speed Cube', NULL, NULL, 50.00, NULL, NULL, 3, 8, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(296, 'GME-260511-0089', 'TYPE_CODE_128', 'Puzzle Kayu', NULL, NULL, 50.00, NULL, NULL, 3, 8, NULL, 25, 25, 14, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(297, 'GME-260511-0090', 'TYPE_CODE_128', 'Games Balok Kayu Set', NULL, NULL, 50.00, NULL, NULL, 3, 8, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(298, 'GME-260511-0091', 'TYPE_CODE_128', 'Baffling Steel Puzzle', NULL, NULL, 50.00, NULL, NULL, 3, 8, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(299, 'FCY-260511-0061', 'TYPE_CODE_128', 'Properti Balon dan Sarung Tangan', NULL, NULL, 50.00, NULL, NULL, 4, 9, NULL, 1, 1, 30, 0, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(300, 'ETK-260511-0008', 'TYPE_CODE_128', 'Yi  CamCase', NULL, NULL, 50.00, NULL, NULL, 9, 7, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(301, 'ETK-260511-0009', 'TYPE_CODE_128', 'Canon G1X + 15-60mm', NULL, NULL, 50.00, NULL, NULL, 9, 9, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(302, 'ETK-260511-0010', 'TYPE_CODE_128', 'Microphone Podcast Set', NULL, NULL, 50.00, NULL, NULL, 9, 10, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(303, 'ETK-260511-0011', 'TYPE_CODE_128', 'TP Link Router', NULL, NULL, 50.00, NULL, NULL, 9, 9, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(304, 'ETK-260511-0012', 'TYPE_CODE_128', 'Handy Cam + Charger + Bag', NULL, NULL, 50.00, NULL, NULL, 9, 8, NULL, 3, 3, 14, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(305, 'ETK-260511-0013', 'TYPE_CODE_128', 'Videomic Rode', NULL, NULL, 50.00, NULL, NULL, 9, 11, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(306, 'ETK-260511-0014', 'TYPE_CODE_128', 'DJI Mavic Mini (NEW)', NULL, NULL, 50.00, NULL, NULL, 9, 11, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(307, 'FCY-260511-0062', 'TYPE_CODE_128', 'Zomei Professional Tripod', NULL, NULL, 50.00, NULL, NULL, 4, 11, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(308, 'ETK-260511-0015', 'TYPE_CODE_128', 'Camcorder AVCAM Panasonic', NULL, NULL, 50.00, NULL, NULL, 9, 9, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(309, 'ETK-260511-0016', 'TYPE_CODE_128', 'Microphone Podcast', NULL, NULL, 50.00, NULL, NULL, 9, 7, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(310, 'ETK-260511-0017', 'TYPE_CODE_128', 'Godox Minimaster', NULL, NULL, 50.00, NULL, NULL, 9, 7, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(311, 'ETK-260511-0018', 'TYPE_CODE_128', 'Studio Flash (Godox)', NULL, NULL, 50.00, NULL, NULL, 9, 7, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(312, 'ETK-260511-0019', 'TYPE_CODE_128', 'Alctron Audio Interface', NULL, NULL, 50.00, NULL, NULL, 9, 8, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-06-03 09:26:02'),
(400, 'MHE-260511-0033', 'TYPE_CODE_128', 'Baju Polo', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 27, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(410, 'MHE-260511-0034', 'TYPE_CODE_128', 'Celana Training', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 2, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(413, 'MHE-260511-0035', 'TYPE_CODE_128', 'Kemeja UT', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 110, 5, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(415, 'MHE-260511-0036', 'TYPE_CODE_128', 'Kaos UT', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 17, 3, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(429, 'MHE-260511-0037', 'TYPE_CODE_128', 'Merch Assessment - Tumbler Corkcilcke', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 4, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02'),
(431, 'MHE-260511-0038', 'TYPE_CODE_128', 'RACER', NULL, NULL, 50.00, NULL, NULL, 6, 4, NULL, 86, 12, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-03 09:26:02');

-- --------------------------------------------------------

--
-- Table structure for table `product_variants`
--

CREATE TABLE `product_variants` (
  `id` bigint UNSIGNED NOT NULL,
  `product_id` bigint UNSIGNED NOT NULL,
  `sku` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `color` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `size` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `image` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `stock` int NOT NULL DEFAULT '0',
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `product_variants`
--

INSERT INTO `product_variants` (`id`, `product_id`, `sku`, `color`, `size`, `image`, `stock`, `created_at`, `updated_at`) VALUES
(1, 197, 'MHE-260511-0008-V01', NULL, 'XL', NULL, 2, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(2, 197, 'MHE-260511-0008-V02', NULL, 'L', NULL, 1, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(3, 197, 'MHE-260511-0008-V03', NULL, 'M', NULL, 2, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(4, 213, 'MHE-260511-0014-V01', 'Hijau', 'L', NULL, 7, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(5, 213, 'MHE-260511-0014-V02', 'Hijau', '5XL', NULL, 6, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(6, 213, 'MHE-260511-0014-V03', 'Hijau', 'M', NULL, 6, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(7, 213, 'MHE-260511-0014-V04', 'Hijau', '4XL', NULL, 7, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(8, 213, 'MHE-260511-0014-V05', 'Hijau', 'XXXL', NULL, 5, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(9, 213, 'MHE-260511-0014-V06', 'Hijau', 'M', NULL, 1, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(10, 213, 'MHE-260511-0014-V07', 'Hijau', 'L', NULL, 1, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(11, 213, 'MHE-260511-0014-V08', 'Hijau', 'XXL', NULL, 1, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(12, 213, 'MHE-260511-0014-V09', 'Hitam', '5XL', NULL, 5, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(13, 213, 'MHE-260511-0014-V10', 'Hitam', 'XXXL', NULL, 5, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(14, 213, 'MHE-260511-0014-V11', 'Hitam', '4XL', NULL, 15, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(15, 213, 'MHE-260511-0014-V12', 'Abu-abu', 'XXXL', NULL, 10, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(16, 213, 'MHE-260511-0014-V13', 'Abu-abu', '4XL', NULL, 23, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(17, 213, 'MHE-260511-0014-V14', 'Abu-abu', 'XXL', NULL, 5, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(18, 213, 'MHE-260511-0014-V15', 'Abu-abu', '5XL', NULL, 8, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(19, 213, 'MHE-260511-0014-V16', 'UNTR', 'XXL', NULL, 5, '2026-06-03 09:25:54', '2026-06-03 09:26:00'),
(20, 213, 'MHE-260511-0014-V17', 'UNTR', 'L', NULL, 1, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(21, 213, 'MHE-260511-0014-V18', 'UNTR', 'M', NULL, 1, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(22, 213, 'MHE-260511-0014-V19', 'UNTR', 'XL', NULL, 1, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(23, 241, 'MHE-260511-0021-V01', 'Abu UT Fresh', 'XXL', NULL, 5, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(24, 241, 'MHE-260511-0021-V02', 'Abu UT Fresh', 'XXXL', NULL, 27, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(25, 241, 'MHE-260511-0021-V03', 'Abu UT Fresh', 'XL', NULL, 50, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(26, 241, 'MHE-260511-0021-V04', 'Abu UT Fresh', '4XL', NULL, 2, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(27, 241, 'MHE-260511-0021-V05', 'Abu UT Fresh', '3XL', NULL, 5, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(28, 241, 'MHE-260511-0021-V06', 'Abu UT Fresh', 'M', NULL, 12, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(29, 241, 'MHE-260511-0021-V07', 'Abu UT Fresh', 'L', NULL, 3, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(30, 241, 'MHE-260511-0021-V08', 'Hitam UT Fresh', 'M', NULL, 10, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(31, 241, 'MHE-260511-0021-V09', 'Hitam UT Fresh', 'XL', NULL, 34, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(32, 241, 'MHE-260511-0021-V10', 'Hitam UT Fresh', 'XXL', NULL, 5, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(33, 241, 'MHE-260511-0021-V11', 'Hitam UT Fresh', 'L', NULL, 40, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(34, 241, 'MHE-260511-0021-V12', 'Hitam UT Fresh', 'S', NULL, 10, '2026-06-03 09:25:54', '2026-06-03 09:26:01'),
(35, 291, 'GME-260511-0086-V01', 'Snake & Ladder', NULL, NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:25:59'),
(36, 291, 'GME-260511-0086-V02', 'Rebound Chess', NULL, NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:25:59'),
(37, 400, 'MHE-260511-0033-V01', 'Biru Panjang ASC', 'No Size', NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(38, 400, 'MHE-260511-0033-V02', 'Biru Panjang ASC', 'S', NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(39, 400, 'MHE-260511-0033-V03', 'Biru Panjang ASC', 'M', NULL, 2, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(40, 400, 'MHE-260511-0033-V04', 'Biru Panjang ASC', 'L', NULL, 8, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(41, 400, 'MHE-260511-0033-V05', 'Biru Panjang ASC', 'XXL', NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(42, 400, 'MHE-260511-0033-V06', 'Biru Panjang ASC', '4XL', NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(43, 400, 'MHE-260511-0033-V07', 'Biru Panjang Mechanic', 'S', NULL, 4, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(44, 400, 'MHE-260511-0033-V08', 'Biru Panjang Mechanic', 'M', NULL, 7, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(45, 400, 'MHE-260511-0033-V09', 'Biru Panjang Mechanic', '4XL', NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(46, 410, 'MHE-260511-0034-V01', NULL, '2XL', NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(47, 410, 'MHE-260511-0034-V02', NULL, '3XL', NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(48, 413, 'MHE-260511-0035-V01', 'Jeans', 'XL', NULL, 5, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(49, 413, 'MHE-260511-0035-V02', 'Jeans', 'XXL', NULL, 3, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(50, 415, 'MHE-260511-0036-V01', 'Corpu', 'M', NULL, 3, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(51, 415, 'MHE-260511-0036-V02', 'Corpu', 'L', NULL, 4, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(52, 415, 'MHE-260511-0036-V03', 'Corpu', 'XXL', NULL, 5, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(53, 413, 'MHE-260511-0035-V03', 'Hitam (PU) Lengan Pendek', 'S', NULL, 2, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(54, 413, 'MHE-260511-0035-V04', 'Hitam (PU) Lengan Pendek', 'M', NULL, 14, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(55, 413, 'MHE-260511-0035-V05', 'Hitam (PU) Lengan Pendek', 'L', NULL, 46, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(56, 413, 'MHE-260511-0035-V06', 'Hitam (PU) Lengan Pendek', 'XL', NULL, 28, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(57, 413, 'MHE-260511-0035-V07', 'Hitam (PU) Lengan Pendek', 'XXL', NULL, 6, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(58, 413, 'MHE-260511-0035-V08', 'Hitam (PU) Lengan Pendek', 'XXXL', NULL, 2, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(59, 413, 'MHE-260511-0035-V09', 'Hitam (PU) Lengan Pendek', '4XL', NULL, 2, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(60, 413, 'MHE-260511-0035-V10', 'Hitam (PU) Lengan Pendek', '5XL', NULL, 2, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(61, 210, 'MHE-260511-0013-V01', 'Coklat', NULL, NULL, 2, '2026-06-03 09:25:55', '2026-06-03 09:26:00'),
(62, 210, 'MHE-260511-0013-V02', 'Ijo', NULL, NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:26:00'),
(63, 210, 'MHE-260511-0013-V03', 'Hitam', NULL, NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:26:00'),
(64, 429, 'MHE-260511-0037-V01', 'Putih', NULL, NULL, 2, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(65, 429, 'MHE-260511-0037-V02', 'Biru', NULL, NULL, 2, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(66, 431, 'MHE-260511-0038-V01', 'Lanyard', NULL, NULL, 12, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(67, 431, 'MHE-260511-0038-V02', 'ID Card', NULL, NULL, 39, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(68, 431, 'MHE-260511-0038-V03', 'Stiker', NULL, NULL, 35, '2026-06-03 09:25:55', '2026-06-03 09:26:02'),
(69, 400, 'MHE-260511-0033-V10', 'Biru Panjang Mechanic Fasilitator', '3XL', NULL, 1, '2026-06-03 09:25:55', '2026-06-03 09:26:01'),
(70, 241, 'MHE-260511-0021-V13', 'Hijau Turbo', NULL, NULL, 2, '2026-06-03 09:25:56', '2026-06-03 09:26:01'),
(71, 415, 'MHE-260511-0036-V04', 'Solution PSDH', 'L', NULL, 4, '2026-06-03 09:25:56', '2026-06-03 09:26:01'),
(72, 415, 'MHE-260511-0036-V05', 'Moving As One Hitam', NULL, NULL, 1, '2026-06-03 09:25:56', '2026-06-03 09:26:01'),
(73, 101, 'FCY-260511-0023-V01', 'Hitam Polos Besar', NULL, NULL, 4, '2026-06-03 09:25:56', '2026-06-03 09:25:59'),
(74, 101, 'FCY-260511-0023-V02', 'Hitam Polos', NULL, NULL, 4, '2026-06-03 09:25:56', '2026-06-03 09:25:59'),
(75, 101, 'FCY-260511-0023-V03', 'Hitam Polos 21x30', NULL, NULL, 8, '2026-06-03 09:25:56', '2026-06-03 09:25:59'),
(76, 101, 'FCY-260511-0023-V04', 'Abu-abu Polos', NULL, NULL, 5, '2026-06-03 09:25:56', '2026-06-03 09:25:59'),
(77, 101, 'FCY-260511-0023-V05', 'Putih Polos 21x30', NULL, NULL, 14, '2026-06-03 09:25:56', '2026-06-03 09:25:59'),
(78, 101, 'FCY-260511-0023-V06', 'Dengan Isi Konten', NULL, NULL, 1, '2026-06-03 09:25:56', '2026-06-03 09:25:59'),
(79, 101, 'FCY-260511-0023-V07', 'Best Photo', NULL, NULL, 1, '2026-06-03 09:25:56', '2026-06-03 09:25:59'),
(80, 210, 'MHE-260511-0013-V04', 'Besar Coklat', NULL, NULL, 4, '2026-06-03 09:25:56', '2026-06-03 09:26:00'),
(81, 183, 'FCY-260511-0050-V01', 'Plastik Polkadot', NULL, NULL, 20, '2026-06-03 09:25:56', '2026-06-03 09:26:00'),
(82, 183, 'FCY-260511-0050-V02', 'Kertas Warna', NULL, NULL, 78, '2026-06-03 09:25:56', '2026-06-03 09:26:00'),
(83, 185, 'FCY-260511-0051-V01', 'Jahit', NULL, NULL, 5, '2026-06-03 09:25:56', '2026-06-03 09:26:00'),
(84, 185, 'FCY-260511-0051-V02', 'Kasur', NULL, NULL, 6, '2026-06-03 09:25:56', '2026-06-03 09:26:00'),
(85, 228, 'MHE-260511-0015-V01', 'Stainless', NULL, NULL, 66, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(86, 257, 'MHE-260511-0023-V01', 'Coklat', NULL, NULL, 97, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(87, 257, 'MHE-260511-0023-V02', 'Kuning', NULL, NULL, 62, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(88, 263, 'MHE-260511-0024-V01', 'UT Smart', NULL, NULL, 508, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(89, 263, 'MHE-260511-0024-V02', 'Hitam Polos', NULL, NULL, 55, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(90, 253, 'MHE-260511-0022-V01', 'Pink', NULL, NULL, 68, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(91, 253, 'MHE-260511-0022-V02', 'Ungu', NULL, NULL, 68, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(92, 253, 'MHE-260511-0022-V03', 'Hijau', NULL, NULL, 57, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(93, 253, 'MHE-260511-0022-V04', 'Coklat', NULL, NULL, 73, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(94, 253, 'MHE-260511-0022-V05', 'UT Smart (Besi) Kuning', NULL, NULL, 40, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(95, 253, 'MHE-260511-0022-V06', 'Besi', NULL, NULL, 401, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(96, 253, 'MHE-260511-0022-V07', 'Hitam UT Smart', NULL, NULL, 10, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(97, 253, 'MHE-260511-0022-V08', 'UT Smart - Warna Campur', NULL, NULL, 51, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(98, 176, 'MHE-260511-0007-V01', 'Hitam', NULL, NULL, 30, '2026-06-03 09:25:57', '2026-06-03 09:26:00'),
(99, 176, 'MHE-260511-0007-V02', 'Biru', NULL, NULL, 106, '2026-06-03 09:25:57', '2026-06-03 09:26:00'),
(100, 176, 'MHE-260511-0007-V03', 'Kuning', NULL, NULL, 60, '2026-06-03 09:25:57', '2026-06-03 09:26:00'),
(101, 176, 'MHE-260511-0007-V04', 'Pink', NULL, NULL, 83, '2026-06-03 09:25:57', '2026-06-03 09:26:00'),
(102, 189, 'FCY-260511-0054-V01', 'tosca', NULL, NULL, 1, '2026-06-03 09:25:57', '2026-06-03 09:26:00'),
(103, 189, 'FCY-260511-0054-V02', 'putih', NULL, NULL, 2, '2026-06-03 09:25:57', '2026-06-03 09:26:00'),
(104, 189, 'FCY-260511-0054-V03', 'merah', NULL, NULL, 1, '2026-06-03 09:25:57', '2026-06-03 09:26:00'),
(105, 189, 'FCY-260511-0054-V04', 'hitam coklat', NULL, NULL, 1, '2026-06-03 09:25:57', '2026-06-03 09:26:00'),
(106, 229, 'MHE-260511-0016-V01', 'Cream', NULL, NULL, 164, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(107, 229, 'MHE-260511-0016-V02', 'Hijau', NULL, NULL, 203, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(108, 265, 'MHE-260511-0025-V01', '1 (Gift Set IF Grade 1)', NULL, NULL, 18, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(109, 265, 'MHE-260511-0025-V02', '2 (Gift Set IF Grade 2)', NULL, NULL, 21, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(110, 265, 'MHE-260511-0025-V03', '3 (Gift Set IF Grade 2)', NULL, NULL, 21, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(111, 265, 'MHE-260511-0025-V04', '4', NULL, NULL, 24, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(112, 265, 'MHE-260511-0025-V05', '5', NULL, NULL, 24, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(113, 265, 'MHE-260511-0025-V06', '6', NULL, NULL, 24, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(114, 265, 'MHE-260511-0025-V07', '7', NULL, NULL, 23, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(115, 273, 'MHE-260511-0027-V01', '1', NULL, NULL, 21, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(116, 273, 'MHE-260511-0027-V02', '2', NULL, NULL, 21, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(117, 273, 'MHE-260511-0027-V03', '3', NULL, NULL, 21, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(118, 273, 'MHE-260511-0027-V04', '4', NULL, NULL, 20, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(119, 273, 'MHE-260511-0027-V05', '5', NULL, NULL, 11, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(120, 280, 'MHE-260511-0030-V01', 'Giftbox Notebook PB + Pen - Dus 1', NULL, NULL, 20, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(121, 280, 'MHE-260511-0030-V02', 'Giftbox Notebook PB + Pen - Dus 2', NULL, NULL, 15, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(122, 283, 'MHE-260511-0032-V01', 'Putih', NULL, NULL, 10, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(123, 283, 'MHE-260511-0032-V02', 'Black', NULL, NULL, 10, '2026-06-03 09:25:57', '2026-06-03 09:26:01'),
(124, 180, 'FCY-260511-0049-V01', 'Standard/Default', NULL, NULL, 2, '2026-06-03 09:25:57', '2026-06-03 09:26:00'),
(125, 180, 'FCY-260511-0049-V02', 'warna warni', NULL, NULL, 14, '2026-06-03 09:25:57', '2026-06-03 09:26:00'),
(126, 180, 'FCY-260511-0049-V03', 'bening', NULL, NULL, 53, '2026-06-03 09:25:57', '2026-06-03 09:26:00');

-- --------------------------------------------------------

--
-- Table structure for table `profile_requests`
--

CREATE TABLE `profile_requests` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` bigint UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nrp` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `division_id` bigint UNSIGNED DEFAULT NULL,
  `status` enum('PENDING','APPROVED','REJECTED') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDING',
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `sessions`
--

CREATE TABLE `sessions` (
  `id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `user_id` bigint UNSIGNED DEFAULT NULL,
  `ip_address` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `user_agent` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `payload` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `last_activity` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `sessions`
--

INSERT INTO `sessions` (`id`, `user_id`, `ip_address`, `user_agent`, `payload`, `last_activity`) VALUES
('da23vCAYl1uBu3Xn27VVcIUjqywUMa2roxRHrYtb', 1, '127.0.0.1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36', 'eyJfdG9rZW4iOiJQcFdXUm1IMU1oY213QXY1d242UW12a1N6V2JLRHNpQjVVVWdRUmZkIiwiX3ByZXZpb3VzIjp7InVybCI6Imh0dHA6XC9cL2dlbnRseS1kaXRjaC1wYXVwZXIubmdyb2stZnJlZS5kZXZcL3Byb2R1Y3RzP3NlYXJjaD14eGwiLCJyb3V0ZSI6InByb2R1Y3RzLmluZGV4In0sIl9mbGFzaCI6eyJvbGQiOltdLCJuZXciOltdfSwibG9naW5fd2ViXzU5YmEzNmFkZGMyYjJmOTQwMTU4MGYwMTRjN2Y1OGVhNGUzMDk4OWQiOjF9', 1778558387),
('G6d6rhPS0cpSp7vxFwdy2G3UvIqKtuad6QyjXqUE', 4, '127.0.0.1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36 OPR/131.0.0.0', 'eyJfdG9rZW4iOiJWcEZkSm1nOVljdlJkSXZWWWtaWE5xT1A4a0xISGJpcE1ET1Y5TUVNIiwiX3ByZXZpb3VzIjp7InVybCI6Imh0dHA6XC9cLzEyNy4wLjAuMTo4MDAwXC9wcm9kdWN0c1wvY3JlYXRlIiwicm91dGUiOiJwcm9kdWN0cy5jcmVhdGUifSwiX2ZsYXNoIjp7Im9sZCI6W10sIm5ldyI6W119LCJsb2dpbl93ZWJfNTliYTM2YWRkYzJiMmY5NDAxNTgwZjAxNGM3ZjU4ZWE0ZTMwOTg5ZCI6NH0=', 1780523937),
('hHoZpC7STsqZlhr1o9PD3R5qYkxRKuQ58EAFMT7N', 1, '127.0.0.1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/26.3.1 Safari/605.1.15', 'eyJfdG9rZW4iOiJ3MnZIVzFmaGJwRzFXWkkwMkk5dkowWm9pYmNpSzRSN2t5dzM3djJXIiwiX3ByZXZpb3VzIjp7InVybCI6Imh0dHA6XC9cL2dlbnRseS1kaXRjaC1wYXVwZXIubmdyb2stZnJlZS5kZXZcL3Byb2R1Y3RzP2NhdGVnb3J5PTkiLCJyb3V0ZSI6InByb2R1Y3RzLmluZGV4In0sIl9mbGFzaCI6eyJvbGQiOltdLCJuZXciOltdfSwibG9naW5fd2ViXzU5YmEzNmFkZGMyYjJmOTQwMTU4MGYwMTRjN2Y1OGVhNGUzMDk4OWQiOjF9', 1778558810),
('hy7VgDUSQp1mpzrutZ2qRiRTULfZpQD9ctmFKavJ', 1, '127.0.0.1', 'Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Mobile Safari/537.36', 'eyJfdG9rZW4iOiJPUHJCemxQd2o3dTJCZUlPTGpLVE1vVG1FV2VGM1U2QXBMb3dieXU0IiwiX3ByZXZpb3VzIjp7InVybCI6Imh0dHA6XC9cL2dlbnRseS1kaXRjaC1wYXVwZXIubmdyb2stZnJlZS5kZXZcL3NjYW4iLCJyb3V0ZSI6InNjYW5uZXIuaW5kZXgifSwiX2ZsYXNoIjp7Im9sZCI6W10sIm5ldyI6W119LCJsb2dpbl93ZWJfNTliYTM2YWRkYzJiMmY5NDAxNTgwZjAxNGM3ZjU4ZWE0ZTMwOTg5ZCI6MX0=', 1778552511),
('o7VEQC0xvT9MMP3jmg40MfOYOj1gOlwY4PZTI1ob', 1, '127.0.0.1', 'Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Mobile Safari/537.36', 'eyJfdG9rZW4iOiJGd2tSaVUzM1h0aHBad29NYlFGdzJhc2UzRUxRZ1A0anp2TzhBMnRTIiwiX3ByZXZpb3VzIjp7InVybCI6Imh0dHA6XC9cL2dlbnRseS1kaXRjaC1wYXVwZXIubmdyb2stZnJlZS5kZXZcL3Byb2R1Y3RzIiwicm91dGUiOiJwcm9kdWN0cy5pbmRleCJ9LCJfZmxhc2giOnsib2xkIjpbXSwibmV3IjpbXX0sImxvZ2luX3dlYl81OWJhMzZhZGRjMmIyZjk0MDE1ODBmMDE0YzdmNThlYTRlMzA5ODlkIjoxfQ==', 1778555401),
('thVaEI0HOzqAGPXc4alCPnGFcJkEOU9nZBLwuIg7', 1, '127.0.0.1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/146.0.0.0 Safari/537.36 OPR/130.0.0.0', 'eyJfdG9rZW4iOiJTM1ZyRldXS1lJeDhLMGlIcFVDdnBFWmdKeGowZ2lXTWtwV1VqUExQIiwiX2ZsYXNoIjp7Im9sZCI6W10sIm5ldyI6W119LCJfcHJldmlvdXMiOnsidXJsIjoiaHR0cDpcL1wvZ2VudGx5LWRpdGNoLXBhdXBlci5uZ3Jvay1mcmVlLmRldlwvY2F0ZWdvcmllcyIsInJvdXRlIjoiY2F0ZWdvcmllcy5pbmRleCJ9LCJsb2dpbl93ZWJfNTliYTM2YWRkYzJiMmY5NDAxNTgwZjAxNGM3ZjU4ZWE0ZTMwOTg5ZCI6MX0=', 1778558888);

-- --------------------------------------------------------

--
-- Table structure for table `stock_logs`
--

CREATE TABLE `stock_logs` (
  `id` bigint UNSIGNED NOT NULL,
  `transaction_id` bigint UNSIGNED NOT NULL,
  `product_id` bigint UNSIGNED NOT NULL,
  `stock_before` int NOT NULL,
  `stock_after` int NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `stock_logs`
--

INSERT INTO `stock_logs` (`id`, `transaction_id`, `product_id`, `stock_before`, `stock_after`, `created_at`, `updated_at`) VALUES
(1, 1, 1, 0, 5, '2026-05-11 12:16:20', '2026-05-11 12:16:20'),
(2, 2, 2, 0, 50, '2026-05-11 12:20:54', '2026-05-11 12:20:54'),
(3, 4, 2, 50, 53, '2026-05-11 12:20:57', '2026-05-11 12:20:57'),
(4, 3, 2, 53, 63, '2026-05-11 12:21:00', '2026-05-11 12:21:00');

-- --------------------------------------------------------

--
-- Table structure for table `transactions`
--

CREATE TABLE `transactions` (
  `id` bigint UNSIGNED NOT NULL,
  `product_id` bigint UNSIGNED NOT NULL,
  `product_variant_id` bigint UNSIGNED DEFAULT NULL,
  `type` enum('IN','OUT') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `request_type` enum('BORROW','GIVEAWAY') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'BORROW',
  `quantity` int NOT NULL,
  `returned_quantity` int NOT NULL DEFAULT '0',
  `pending_return_quantity` int NOT NULL DEFAULT '0',
  `returned_at` timestamp NULL DEFAULT NULL,
  `return_photo` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `return_status` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `return_reason` text COLLATE utf8mb4_unicode_ci,
  `is_return_draft` tinyint(1) NOT NULL DEFAULT '0',
  `return_condition` enum('BAIK','RUSAK','HILANG') COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` enum('PENDING','PENDING_MANAGER','APPROVED','REJECTED','REVISION') COLLATE utf8mb4_unicode_ci DEFAULT 'PENDING',
  `rejection_reason` text COLLATE utf8mb4_unicode_ci,
  `admin_notes` text COLLATE utf8mb4_unicode_ci,
  `manager_notes` text COLLATE utf8mb4_unicode_ci,
  `return_rejection_reason` text COLLATE utf8mb4_unicode_ci,
  `requester_id` bigint UNSIGNED NOT NULL,
  `approver_id` bigint UNSIGNED DEFAULT NULL,
  `notes` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `used_by` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `division_id` bigint UNSIGNED DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `applicant_name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `applicant_nrp` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `borrow_duration_days` int DEFAULT NULL,
  `borrow_start_date` date DEFAULT NULL,
  `expected_return_date` date DEFAULT NULL,
  `event_name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `event_date` date DEFAULT NULL,
  `documentation_link` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `transactions`
--

INSERT INTO `transactions` (`id`, `product_id`, `product_variant_id`, `type`, `request_type`, `quantity`, `returned_quantity`, `pending_return_quantity`, `returned_at`, `return_photo`, `return_status`, `return_reason`, `is_return_draft`, `return_condition`, `status`, `rejection_reason`, `admin_notes`, `manager_notes`, `return_rejection_reason`, `requester_id`, `approver_id`, `notes`, `used_by`, `division_id`, `created_at`, `updated_at`, `applicant_name`, `applicant_nrp`, `borrow_duration_days`, `borrow_start_date`, `expected_return_date`, `event_name`, `event_date`, `documentation_link`) VALUES
(1, 1, NULL, 'IN', 'BORROW', 5, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, 'APPROVED', NULL, NULL, NULL, NULL, 1, 1, NULL, NULL, NULL, '2026-05-11 12:16:11', '2026-05-11 12:16:20', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
(2, 2, NULL, 'IN', 'BORROW', 50, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, 'APPROVED', NULL, NULL, NULL, NULL, 1, 1, NULL, NULL, NULL, '2026-05-11 12:19:00', '2026-05-11 12:20:54', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
(3, 2, NULL, 'IN', 'BORROW', 10, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, 'APPROVED', NULL, NULL, NULL, NULL, 1, 1, NULL, NULL, NULL, '2026-05-11 12:20:13', '2026-05-11 12:21:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
(4, 2, NULL, 'IN', 'BORROW', 3, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, 'APPROVED', NULL, NULL, NULL, NULL, 1, 1, NULL, NULL, NULL, '2026-05-11 12:20:41', '2026-05-11 12:20:57', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `units`
--

CREATE TABLE `units` (
  `id` bigint UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `units`
--

INSERT INTO `units` (`id`, `name`, `created_at`, `updated_at`) VALUES
(1, 'Mix (4 Bungkus Kecil, 1 Pack Besar, 3 Roll Bekas)', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(2, 'Pack Jaring', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(3, 'Pcs', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(4, '2M X 1M', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(5, 'Pasang', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(6, 'Bungkus', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(7, 'Pack', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(8, 'Sachet', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(9, 'Sachet Kecil', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(10, 'Box', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(11, 'Lembar', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(12, 'Buah', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(13, 'Toples', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(14, 'Set', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(15, '7 Pack (Warna-warni), 1 Pack Besar (Biru), 14 Roll', '2026-06-03 09:25:47', '2026-06-03 09:25:47'),
(16, 'Bundle', '2026-06-03 09:25:48', '2026-06-03 09:25:48'),
(17, 'Map', '2026-06-03 09:25:48', '2026-06-03 09:25:48'),
(18, 'Ikat', '2026-06-03 09:25:48', '2026-06-03 09:25:48'),
(19, 'Plastik', '2026-06-03 09:25:48', '2026-06-03 09:25:48'),
(20, 'Unit', '2026-06-03 09:25:48', '2026-06-03 09:25:48'),
(21, 'Board', '2026-06-03 09:25:48', '2026-06-03 09:25:48'),
(22, 'Roll', '2026-06-03 09:25:49', '2026-06-03 09:25:49'),
(23, 'Pax', '2026-06-03 09:25:49', '2026-06-03 09:25:49'),
(24, 'Parts', '2026-06-03 09:25:49', '2026-06-03 09:25:49'),
(25, 'Blok', '2026-06-03 09:25:49', '2026-06-03 09:25:49'),
(26, 'Stel', '2026-06-03 09:25:49', '2026-06-03 09:25:49'),
(27, 'Kaleng', '2026-06-03 09:25:49', '2026-06-03 09:25:49'),
(28, '5', '2026-06-03 09:25:49', '2026-06-03 09:25:49'),
(29, 'Buku', '2026-06-03 09:25:50', '2026-06-03 09:25:50'),
(30, 'Kantong', '2026-06-03 09:25:50', '2026-06-03 09:25:50'),
(31, 'kg', '2026-06-03 09:25:54', '2026-06-03 09:25:54'),
(32, 'liter', '2026-06-03 09:25:54', '2026-06-03 09:25:54'),
(33, 'meter', '2026-06-03 09:25:54', '2026-06-03 09:25:54');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` bigint UNSIGNED NOT NULL,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nrp` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `poin` int NOT NULL DEFAULT '0',
  `email_verified_at` timestamp NULL DEFAULT NULL,
  `password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `role` enum('superadmin','admin','manager','staff') COLLATE utf8mb4_unicode_ci DEFAULT 'staff',
  `division_id` bigint UNSIGNED DEFAULT NULL,
  `remember_token` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `name`, `nrp`, `email`, `poin`, `email_verified_at`, `password`, `role`, `division_id`, `remember_token`, `created_at`, `updated_at`) VALUES
(1, 'Admin User', '18205138', 'admin@wms.com', 1000, '2026-05-11 11:30:04', '$2y$12$lMYeCaX50N2XMh4XWlV2Vugy0xjCtVUaS0r2cw5POFruB92zv8twu', 'admin', 3, 'tupohEJ7W6UHk61RcHxSCWsrMtF71Ut35MCUqFMMNMf3zv6Ykmtieo1jPEV6', '2026-05-11 11:30:04', '2026-06-03 09:26:03'),
(2, 'Manager User', '54857750', 'manager@wms.com', 1000, '2026-05-11 11:30:04', '$2y$12$02I3rs1esMRMHB9p3p0HFuOg74lwUnC2sspbS8V53v02rjryFrhj.', 'manager', 3, 'qxl2W9eeqk', '2026-05-11 11:30:04', '2026-06-03 09:26:03'),
(3, 'Staff User', '11016563', 'staff@wms.com', 1000, '2026-05-11 11:30:05', '$2y$12$it.pMlksHIksg7XDgctLY.1pvgefRP0hxJhkWbCDt5xXV76BXt8ki', 'staff', 3, 'ljf4NYEGgb', '2026-05-11 11:30:05', '2026-06-03 09:26:03'),
(4, 'Super Administrator', '82995012', 'superadmin@wms.com', 1000, NULL, '$2y$12$./UmwDwPf4CrjVOITU3PQOv2psm3WBSR6rqMD2yeiQs59.A3S9O6q', 'superadmin', 3, NULL, '2026-06-03 09:26:03', '2026-06-03 09:26:03');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `cache`
--
ALTER TABLE `cache`
  ADD PRIMARY KEY (`key`),
  ADD KEY `cache_expiration_index` (`expiration`);

--
-- Indexes for table `cache_locks`
--
ALTER TABLE `cache_locks`
  ADD PRIMARY KEY (`key`),
  ADD KEY `cache_locks_expiration_index` (`expiration`);

--
-- Indexes for table `categories`
--
ALTER TABLE `categories`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `divisions`
--
ALTER TABLE `divisions`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `failed_jobs`
--
ALTER TABLE `failed_jobs`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `failed_jobs_uuid_unique` (`uuid`);

--
-- Indexes for table `jobs`
--
ALTER TABLE `jobs`
  ADD PRIMARY KEY (`id`),
  ADD KEY `jobs_queue_index` (`queue`);

--
-- Indexes for table `job_batches`
--
ALTER TABLE `job_batches`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `locations`
--
ALTER TABLE `locations`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `migrations`
--
ALTER TABLE `migrations`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `password_reset_tokens`
--
ALTER TABLE `password_reset_tokens`
  ADD PRIMARY KEY (`email`);

--
-- Indexes for table `products`
--
ALTER TABLE `products`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `products_sku_unique` (`sku`),
  ADD KEY `products_category_id_foreign` (`category_id`),
  ADD KEY `products_location_id_foreign` (`location_id`),
  ADD KEY `products_unit_id_foreign` (`unit_id`);

--
-- Indexes for table `product_variants`
--
ALTER TABLE `product_variants`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `product_variants_sku_unique` (`sku`),
  ADD KEY `product_variants_product_id_foreign` (`product_id`);

--
-- Indexes for table `profile_requests`
--
ALTER TABLE `profile_requests`
  ADD PRIMARY KEY (`id`),
  ADD KEY `profile_requests_user_id_foreign` (`user_id`),
  ADD KEY `profile_requests_division_id_foreign` (`division_id`);

--
-- Indexes for table `sessions`
--
ALTER TABLE `sessions`
  ADD PRIMARY KEY (`id`),
  ADD KEY `sessions_user_id_index` (`user_id`),
  ADD KEY `sessions_last_activity_index` (`last_activity`);

--
-- Indexes for table `stock_logs`
--
ALTER TABLE `stock_logs`
  ADD PRIMARY KEY (`id`),
  ADD KEY `stock_logs_transaction_id_foreign` (`transaction_id`),
  ADD KEY `stock_logs_product_id_foreign` (`product_id`);

--
-- Indexes for table `transactions`
--
ALTER TABLE `transactions`
  ADD PRIMARY KEY (`id`),
  ADD KEY `transactions_product_id_foreign` (`product_id`),
  ADD KEY `transactions_requester_id_foreign` (`requester_id`),
  ADD KEY `transactions_approver_id_foreign` (`approver_id`),
  ADD KEY `transactions_division_id_foreign` (`division_id`),
  ADD KEY `transactions_product_variant_id_foreign` (`product_variant_id`);

--
-- Indexes for table `units`
--
ALTER TABLE `units`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `units_name_unique` (`name`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `users_email_unique` (`email`),
  ADD UNIQUE KEY `users_nrp_unique` (`nrp`),
  ADD KEY `users_division_id_foreign` (`division_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `categories`
--
ALTER TABLE `categories`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `divisions`
--
ALTER TABLE `divisions`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `failed_jobs`
--
ALTER TABLE `failed_jobs`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `jobs`
--
ALTER TABLE `jobs`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `locations`
--
ALTER TABLE `locations`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `migrations`
--
ALTER TABLE `migrations`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=38;

--
-- AUTO_INCREMENT for table `products`
--
ALTER TABLE `products`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=434;

--
-- AUTO_INCREMENT for table `product_variants`
--
ALTER TABLE `product_variants`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=127;

--
-- AUTO_INCREMENT for table `profile_requests`
--
ALTER TABLE `profile_requests`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `stock_logs`
--
ALTER TABLE `stock_logs`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `transactions`
--
ALTER TABLE `transactions`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `units`
--
ALTER TABLE `units`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=34;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `products`
--
ALTER TABLE `products`
  ADD CONSTRAINT `products_category_id_foreign` FOREIGN KEY (`category_id`) REFERENCES `categories` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `products_location_id_foreign` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `products_unit_id_foreign` FOREIGN KEY (`unit_id`) REFERENCES `units` (`id`) ON DELETE SET NULL;

--
-- Constraints for table `product_variants`
--
ALTER TABLE `product_variants`
  ADD CONSTRAINT `product_variants_product_id_foreign` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE CASCADE;

--
-- Constraints for table `profile_requests`
--
ALTER TABLE `profile_requests`
  ADD CONSTRAINT `profile_requests_division_id_foreign` FOREIGN KEY (`division_id`) REFERENCES `divisions` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `profile_requests_user_id_foreign` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE;

--
-- Constraints for table `stock_logs`
--
ALTER TABLE `stock_logs`
  ADD CONSTRAINT `stock_logs_product_id_foreign` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `stock_logs_transaction_id_foreign` FOREIGN KEY (`transaction_id`) REFERENCES `transactions` (`id`) ON DELETE CASCADE;

--
-- Constraints for table `transactions`
--
ALTER TABLE `transactions`
  ADD CONSTRAINT `transactions_approver_id_foreign` FOREIGN KEY (`approver_id`) REFERENCES `users` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `transactions_division_id_foreign` FOREIGN KEY (`division_id`) REFERENCES `divisions` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `transactions_product_id_foreign` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `transactions_product_variant_id_foreign` FOREIGN KEY (`product_variant_id`) REFERENCES `product_variants` (`id`) ON DELETE SET NULL,
  ADD CONSTRAINT `transactions_requester_id_foreign` FOREIGN KEY (`requester_id`) REFERENCES `users` (`id`) ON DELETE CASCADE;

--
-- Constraints for table `users`
--
ALTER TABLE `users`
  ADD CONSTRAINT `users_division_id_foreign` FOREIGN KEY (`division_id`) REFERENCES `divisions` (`id`) ON DELETE SET NULL;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
