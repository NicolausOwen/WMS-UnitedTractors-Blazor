-- =============================================================
-- WMS United Tractors — Database Seeder (T-SQL / SQL Server)
-- Database : ut_wms_db
-- Generated: 2026-06-05
-- =============================================================

-- Drop tables in reverse dependency order (dependents first)
DROP TABLE IF EXISTS `user_admin_roles`;
DROP TABLE IF EXISTS `admin_roles`;
DROP TABLE IF EXISTS `stock_logs`;
DROP TABLE IF EXISTS `transactions`;
DROP TABLE IF EXISTS `profile_requests`;
DROP TABLE IF EXISTS `product_variants`;
DROP TABLE IF EXISTS `products`;
DROP TABLE IF EXISTS `users`;
DROP TABLE IF EXISTS `__efmigrationshistory`;
DROP TABLE IF EXISTS `categories`;
DROP TABLE IF EXISTS `divisions`;
DROP TABLE IF EXISTS `locations`;
DROP TABLE IF EXISTS `units`;

-- =============================================================
-- TABLE: __efmigrationshistory
-- =============================================================

CREATE TABLE `__efmigrationshistory` (
  `MigrationId`    VARCHAR(150) NOT NULL,
  `ProductVersion` VARCHAR(32)  NOT NULL,
  CONSTRAINT `PK___efmigrationshistory` PRIMARY KEY (`MigrationId`)
);

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20260522095935_SyncWithSqlDump', '9.0.0'),
('20260527063537_AddProductDescriptionAndVariants', '9.0.0'),
('20260604021546_AddTransactionsTable', '9.0.0'),
('20260605013419_AddHandoverFields', '9.0.0'),
('20260608062936_UpdateDatabase', '9.0.0'),
('20260608081019_AddAdminRoles', '9.0.0'),
('20260609013724_AddCategoryIdToUserAdminRole', '9.0.0'),
('20260609063238_FixTransactionStatusColumn', '9.0.0'),
('20260609090000_AddGiveawayWorkflowFields', '9.0.0');

-- =============================================================
-- TABLE: categories
-- =============================================================

CREATE TABLE `categories` (
  `id`          bigint        NOT NULL AUTO_INCREMENT,
  `name`        VARCHAR(255) NOT NULL,
  `description` VARCHAR(255) NULL,
  `created_at`  DATETIME     NULL,
  `updated_at`  DATETIME     NULL,
  CONSTRAINT `PK_categories` PRIMARY KEY (`id`)
);


INSERT INTO `categories` (`id`,`name`,`description`,`created_at`,`updated_at`) VALUES
  (1, 'Makanan',    NULL, '2026-05-11 12:17:24', '2026-05-11 12:17:24'),
  (3, 'Game',       NULL, '2026-05-11 13:40:10', '2026-05-11 13:40:10'),
  (4, 'Facility',   NULL, '2026-05-11 13:40:10', '2026-05-11 13:40:10'),
  (5, 'ATK',        NULL, '2026-05-11 13:40:10', '2026-05-11 13:40:10'),
  (6, 'Merchandise',NULL, '2026-05-11 13:40:10', '2026-05-11 13:40:10'),
  (8, 'Alat Musik', NULL, '2026-05-11 13:40:11', '2026-05-11 13:40:11'),
  (9, 'Elektronik', NULL, '2026-05-11 13:40:11', '2026-05-11 13:40:11');


-- =============================================================
-- TABLE: divisions
-- =============================================================

CREATE TABLE `divisions` (
  `id`          bigint        NOT NULL AUTO_INCREMENT,
  `name`        VARCHAR(255) NOT NULL,
  `description` VARCHAR(255) NULL,
  `created_at`  DATETIME     NULL,
  `updated_at`  DATETIME     NULL,
  CONSTRAINT `PK_divisions` PRIMARY KEY (`id`)
);


INSERT INTO `divisions` (`id`,`name`,`description`,`created_at`,`updated_at`) VALUES
  ( 1,'CCS', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  ( 2,'CFA', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  ( 3,'CHCU',NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  ( 4,'CRA', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  ( 5,'CST', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  ( 6,'DAD', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  ( 7,'GLG', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  ( 8,'MKT', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  ( 9,'PIN', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  (10,'PRT', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  (11,'SOD', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  (12,'SVC', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  (13,'TMO', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46'),
  (14,'TSO', NULL,'2026-05-26 09:46:46','2026-05-26 09:46:46');


-- =============================================================
-- TABLE: locations
-- =============================================================

CREATE TABLE `locations` (
  `id`          bigint        NOT NULL AUTO_INCREMENT,
  `name`        VARCHAR(255) NOT NULL,
  `description` VARCHAR(255) NULL,
  `created_at`  DATETIME     NULL,
  `updated_at`  DATETIME     NULL,
  CONSTRAINT `PK_locations` PRIMARY KEY (`id`)
);


INSERT INTO `locations` (`id`,`name`,`description`,`created_at`,`updated_at`) VALUES
  ( 1,'Gudang',      'Gudang lt 1','2026-05-11 12:57:18','2026-05-11 12:57:18'),
  ( 2,'Storage Room',NULL,         '2026-05-11 13:40:10','2026-05-11 13:40:10'),
  ( 3,'ATK',         NULL,         '2026-05-11 13:40:10','2026-05-11 13:40:10'),
  ( 4,'Makeup Room', NULL,         '2026-05-11 13:40:10','2026-05-11 13:40:10'),
  ( 5,'Merchandise', NULL,         '2026-05-11 13:40:11','2026-05-11 13:40:11'),
  ( 6,'7.1.12.2',    NULL,         '2026-05-11 13:40:11','2026-05-11 13:40:11'),
  ( 7,'7.1.12.3',    NULL,         '2026-05-11 13:40:11','2026-05-11 13:40:11'),
  ( 8,'7.1.11.1',    NULL,         '2026-05-11 13:40:11','2026-05-11 13:40:11'),
  ( 9,'7.1.11.2',    NULL,         '2026-05-11 13:40:11','2026-05-11 13:40:11'),
  (10,'7.1.11.3',    NULL,         '2026-05-11 13:40:11','2026-05-11 13:40:11'),
  (11,'7.1.11.5',    NULL,         '2026-05-11 13:40:11','2026-05-11 13:40:11');


-- =============================================================
-- TABLE: units
-- =============================================================

CREATE TABLE `units` (
  `id`         bigint        NOT NULL AUTO_INCREMENT,
  `name`       VARCHAR(255) NOT NULL,
  `created_at` DATETIME     NULL,
  `updated_at` DATETIME     NULL,
  CONSTRAINT `PK_units`      PRIMARY KEY (`id`),
  CONSTRAINT `UQ_units_name` UNIQUE      (`name`)
);


INSERT INTO `units` (`id`,`name`,`created_at`,`updated_at`) VALUES
  ( 1,'Mix (4 Bungkus Kecil, 1 Pack Besar, 3 Roll Bekas)','2026-05-26 09:46:41','2026-05-26 09:46:41'),
  ( 2,'Pack Jaring',                                       '2026-05-26 09:46:41','2026-05-26 09:46:41'),
  ( 3,'Pcs',                                               '2026-05-26 09:46:41','2026-05-26 09:46:41'),
  ( 4,'2M X 1M',                                           '2026-05-26 09:46:41','2026-05-26 09:46:41'),
  ( 5,'Pasang',                                            '2026-05-26 09:46:41','2026-05-26 09:46:41'),
  ( 6,'Bungkus',                                           '2026-05-26 09:46:41','2026-05-26 09:46:41'),
  ( 7,'Pack',                                              '2026-05-26 09:46:41','2026-05-26 09:46:41'),
  ( 8,'Sachet',                                            '2026-05-26 09:46:41','2026-05-26 09:46:41'),
  ( 9,'Sachet Kecil',                                      '2026-05-26 09:46:41','2026-05-26 09:46:41'),
  (10,'Box',                                               '2026-05-26 09:46:41','2026-05-26 09:46:41'),
  (11,'Lembar',                                            '2026-05-26 09:46:41','2026-05-26 09:46:41'),
  (12,'Buah',                                              '2026-05-26 09:46:41','2026-05-26 09:46:41'),
  (13,'Toples',                                            '2026-05-26 09:46:42','2026-05-26 09:46:42'),
  (14,'Set',                                               '2026-05-26 09:46:42','2026-05-26 09:46:42'),
  (15,'7 Pack (Warna-warni), 1 Pack Besar (Biru), 14 Roll','2026-05-26 09:46:42','2026-05-26 09:46:42'),
  (16,'Bundle',                                            '2026-05-26 09:46:42','2026-05-26 09:46:42'),
  (17,'Map',                                               '2026-05-26 09:46:42','2026-05-26 09:46:42'),
  (18,'Ikat',                                              '2026-05-26 09:46:42','2026-05-26 09:46:42'),
  (19,'Plastik',                                           '2026-05-26 09:46:42','2026-05-26 09:46:42'),
  (20,'Unit',                                              '2026-05-26 09:46:42','2026-05-26 09:46:42'),
  (21,'Board',                                             '2026-05-26 09:46:43','2026-05-26 09:46:43'),
  (22,'Roll',                                              '2026-05-26 09:46:43','2026-05-26 09:46:43'),
  (23,'Pax',                                               '2026-05-26 09:46:43','2026-05-26 09:46:43'),
  (24,'Parts',                                             '2026-05-26 09:46:43','2026-05-26 09:46:43'),
  (25,'Blok',                                              '2026-05-26 09:46:43','2026-05-26 09:46:43'),
  (26,'Stel',                                              '2026-05-26 09:46:43','2026-05-26 09:46:43'),
  (27,'Kaleng',                                            '2026-05-26 09:46:43','2026-05-26 09:46:43'),
  (28,'5',                                                 '2026-05-26 09:46:43','2026-05-26 09:46:43'),
  (29,'Buku',                                              '2026-05-26 09:46:43','2026-05-26 09:46:43'),
  (30,'Kantong',                                           '2026-05-26 09:46:44','2026-05-26 09:46:44'),
  (31,'kg',                                                '2026-05-26 09:46:46','2026-05-26 09:46:46'),
  (32,'liter',                                             '2026-05-26 09:46:46','2026-05-26 09:46:46'),
  (33,'meter',                                             '2026-05-26 09:46:46','2026-05-26 09:46:46');


-- =============================================================
-- TABLE: products
-- Kolom: id, sku, barcode_type, name, description, transaction_type,
--        value, image, images, category_id, location_id, position_image,
--        current_stock, initial_stock, unit_id, is_returnable, min_stock,
--        created_at, updated_at
-- =============================================================

CREATE TABLE `products` (
  `id`               bigint         NOT NULL AUTO_INCREMENT,
  `sku`              VARCHAR(255)  NOT NULL,
  `barcode_type`     VARCHAR(255)  NOT NULL DEFAULT 'TYPE_CODE_128',
  `name`             VARCHAR(255)  NOT NULL,
  `description`      TEXT  NULL,
  `transaction_type` VARCHAR(7)    NULL,
  `value`            decimal(10,2)  NULL,
  `image`            VARCHAR(255)  NULL,
  `images`           TEXT  NULL,
  `category_id`      bigint         NULL,
  `location_id`      bigint         NULL,
  `position_image`   VARCHAR(255)  NULL,
  `current_stock`    int            NOT NULL DEFAULT 0,
  `initial_stock`    int            NOT NULL DEFAULT 0,
  `unit_id`          bigint         NULL,
  `is_returnable`    TINYINT(1)            NOT NULL DEFAULT 1,
  `min_stock`        int            NOT NULL DEFAULT 0,
  `created_at`       DATETIME      NULL,
  `updated_at`       DATETIME      NULL,
  CONSTRAINT `PK_products`                    PRIMARY KEY (`id`),
  CONSTRAINT `UQ_products_sku`                UNIQUE      (`sku`),
  CONSTRAINT `CK_products_transaction_type`   CHECK       (`transaction_type` IN ('BORROW','REQUEST')),
  CONSTRAINT `FK_products_category_id`        FOREIGN KEY (`category_id`) REFERENCES `categories` (`id`),
  CONSTRAINT `FK_products_location_id`        FOREIGN KEY (`location_id`) REFERENCES `locations`  (`id`),
  CONSTRAINT `FK_products_unit_id`            FOREIGN KEY (`unit_id`)     REFERENCES `units`      (`id`)
);

CREATE INDEX `IX_products_category_id` ON `products` (`category_id`);
CREATE INDEX `IX_products_location_id` ON `products` (`location_id`);
CREATE INDEX `IX_products_unit_id`     ON `products` (`unit_id`);

INSERT INTO `products` (`id`, `sku`, `barcode_type`, `name`, `description`, `transaction_type`, `value`, `image`, `images`, `category_id`, `location_id`, `position_image`, `current_stock`, `initial_stock`, `unit_id`, `is_returnable`, `min_stock`, `created_at`, `updated_at`) VALUES
(1, 'GME-260511-0001', 'TYPE_CODE_128', 'Stick Ice Cream', NULL, NULL, '0.00', 'images/products/97af8e4c-7d24-4a79-8359-fbfee5781bbc.jpg', NULL, 3, 1, NULL, 8, 8, 1, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(2, 'GME-260511-0002', 'TYPE_CODE_128', 'Bola kecil', NULL, NULL, '0.00', '/images/products/YFSOpvscJWF42hEyefx3qENxb8tM9DwDByqjx2gU.jpg', '[\"/images/products/YFSOpvscJWF42hEyefx3qENxb8tM9DwDByqjx2gU.jpg\"]', 3, 1, NULL, 1, 1, 2, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(3, 'GME-260511-0003', 'TYPE_CODE_128', 'Caping', NULL, NULL, '0.00', '/images/products/1CQOEPfy6r9zrGCCfMR22bzSI6uBeQbCZKryYqWc.jpg', '[\"/images/products/1CQOEPfy6r9zrGCCfMR22bzSI6uBeQbCZKryYqWc.jpg\"]', 3, 1, NULL, 6, 6, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(4, 'GME-260511-0004', 'TYPE_CODE_128', 'Pompa Balon', NULL, NULL, '0.00', 'images/products/416d7a73-1f37-4670-a291-41fa5faff681.jpg', NULL, 3, 1, NULL, 6, 6, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(5, 'GME-260511-0005', 'TYPE_CODE_128', 'Keranjang Sampah', NULL, NULL, '0.00', '/images/products/RSanEEGxn7ucC5TiFyBRGTirmVjgHlCBxAx6zkjM.jpg', '[\"/images/products/RSanEEGxn7ucC5TiFyBRGTirmVjgHlCBxAx6zkjM.jpg\"]', 3, 1, NULL, 12, 12, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(6, 'FCY-260511-0001', 'TYPE_CODE_128', 'Kain Hitam', NULL, NULL, '50.00', 'images/products/1dd3acf0-c756-40d1-82f6-c82b56884652.jpg', NULL, 4, 1, NULL, 1, 1, 4, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(7, 'GME-260511-0006', 'TYPE_CODE_128', 'Sumpit', NULL, NULL, '0.00', '/images/products/v4u7gSNCAueE1hczAsWFQ90tdJMq1cH9CcwcWzXF.jpg', '[\"/images/products/v4u7gSNCAueE1hczAsWFQ90tdJMq1cH9CcwcWzXF.jpg\"]', 3, 1, NULL, 343, 343, 5, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(8, 'GME-260511-0007', 'TYPE_CODE_128', 'Bola Tenis', NULL, NULL, '0.00', '/images/products/I46Pb7zcrKaf6sxggvhKPY9iaOXP3EVbMnlJpSuw.jpg', '[\"/images/products/I46Pb7zcrKaf6sxggvhKPY9iaOXP3EVbMnlJpSuw.jpg\"]', 3, 1, NULL, 42, 42, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(9, 'FCY-260511-0002', 'TYPE_CODE_128', 'Masker Hitam', NULL, NULL, '50.00', NULL, NULL, 4, 2, NULL, 20, 20, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(10, 'GME-260511-0008', 'TYPE_CODE_128', 'Bola Pingpong', NULL, NULL, '0.00', '/images/products/OYINvaOUZVzJO6Qif8XhPQuqenY83juAsGdmQKSR.jpg', '[\"/images/products/OYINvaOUZVzJO6Qif8XhPQuqenY83juAsGdmQKSR.jpg\"]', 3, 1, NULL, 49, 49, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(11, 'GME-260511-0009', 'TYPE_CODE_128', 'Bola Golf', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 12, 12, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(12, 'GME-260511-0010', 'TYPE_CODE_128', 'Bola Kelereng', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 45, 45, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(13, 'MKN-260511-0001', 'TYPE_CODE_128', 'Tepung Maizena exp Nov 2026', NULL, NULL, '50.00', '/images/products/fe1be027-140b-46f2-938b-3f9ddcbaab31.png', '[\"/images/products/fe1be027-140b-46f2-938b-3f9ddcbaab31.png\"]', 1, 2, NULL, 6, 6, 6, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(14, 'MKN-260511-0002', 'TYPE_CODE_128', 'Tepung Roti exp 27 Nov 2026', NULL, NULL, '50.00', '/images/products/6b7c71c3-7f64-47c9-81fc-76504f42ffa6.png', '[\"/images/products/6b7c71c3-7f64-47c9-81fc-76504f42ffa6.png\"]', 1, 2, NULL, 6, 6, 6, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(15, 'MKN-260511-0003', 'TYPE_CODE_128', 'Santan exp April 2027', NULL, NULL, '50.00', '/images/products/930e7a72-53c6-4e3f-83e4-2519085201ad.png', '[\"/images/products/930e7a72-53c6-4e3f-83e4-2519085201ad.png\"]', 1, 2, NULL, 10, 10, 6, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(16, 'FCY-260511-0003', 'TYPE_CODE_128', 'Tissue', NULL, NULL, '50.00', '/images/products/4b916ea2-06c8-4526-912c-9ddedc46460d.png', '[\"/images/products/4b916ea2-06c8-4526-912c-9ddedc46460d.png\"]', 4, 2, NULL, 5, 5, 7, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(17, 'MKN-260511-0004', 'TYPE_CODE_128', 'Gula exp 2027', NULL, NULL, '50.00', '/images/products/df3340e3-d01b-4777-979a-4a1c61983d07.png', '[\"/images/products/df3340e3-d01b-4777-979a-4a1c61983d07.png\"]', 1, 2, NULL, 5, 5, 6, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(18, 'MKN-260511-0005', 'TYPE_CODE_128', 'Gula halus rose brand exp nov 27', NULL, NULL, '50.00', '/images/products/77fcb040-6ce8-4c50-92dd-e96870fde9d8.png', '[\"/images/products/77fcb040-6ce8-4c50-92dd-e96870fde9d8.png\"]', 1, 2, NULL, 1, 1, 7, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(19, 'MKN-260511-0006', 'TYPE_CODE_128', 'Blue Band', NULL, NULL, '50.00', '/images/products/82bcff1c-2bb3-4322-927e-cb9ad6c5f234.png', '[\"/images/products/82bcff1c-2bb3-4322-927e-cb9ad6c5f234.png\"]', 1, 2, NULL, 6, 6, 6, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(20, 'MKN-260511-0007', 'TYPE_CODE_128', 'Dancow exp Maret 2027', NULL, NULL, '50.00', NULL, NULL, 1, 2, NULL, 8, 8, 8, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(21, 'MKN-260511-0008', 'TYPE_CODE_128', 'Frisian flag Exp Agustus 2026', NULL, NULL, '50.00', '/images/products/6c1c9b27-9735-46d3-95b6-f9e7d8081d72.png', '[\"/images/products/6c1c9b27-9735-46d3-95b6-f9e7d8081d72.png\"]', 1, 2, NULL, 18, 18, 8, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(22, 'MKN-260511-0009', 'TYPE_CODE_128', 'Ladaku Exp 2029', NULL, NULL, '50.00', NULL, NULL, 1, 2, NULL, 15, 15, 9, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(23, 'MKN-260511-0010', 'TYPE_CODE_128', 'Fermipan Exp 2027', NULL, NULL, '50.00', '/images/products/44368330-3f22-4f9a-963d-c5b6545a4bdf.png', '[\"/images/products/44368330-3f22-4f9a-963d-c5b6545a4bdf.png\"]', 1, 2, NULL, 2, 2, 10, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(24, 'MKN-260511-0011', 'TYPE_CODE_128', 'Pemanis buatan', NULL, NULL, '50.00', '/images/products/97d80166-579d-415d-be6e-a1668b6307c7.png', '[\"/images/products/97d80166-579d-415d-be6e-a1668b6307c7.png\"]', 1, 2, NULL, 1, 1, 8, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(25, 'GME-260511-0011', 'TYPE_CODE_128', 'Ember Merah', NULL, NULL, '0.00', '/images/products/DeOEcX3iAyaQXZ6GcoyTDhaggRpWemmck6yI0fyd.jpg', '[\"/images/products/DeOEcX3iAyaQXZ6GcoyTDhaggRpWemmck6yI0fyd.jpg\"]', 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(26, 'GME-260511-0012', 'TYPE_CODE_128', 'Mission Impossible (Permainan Bambu Biru Merah)', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 48, 48, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(27, 'GME-260511-0013', 'TYPE_CODE_128', 'The Master of Risk', NULL, NULL, '0.00', '/images/products/VMaADTsap8BDLFkiq3rt7zs96Xt30DirwaCTdTcJ.jpg', '[\"/images/products/VMaADTsap8BDLFkiq3rt7zs96Xt30DirwaCTdTcJ.jpg\"]', 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(28, 'GME-260511-0014', 'TYPE_CODE_128', 'Spot The Difference', NULL, NULL, '0.00', '/images/products/mJz9HRN3g518TBtMh0GMpbRInuoHbSv3Edxr6TAz.jpg', '[\"/images/products/mJz9HRN3g518TBtMh0GMpbRInuoHbSv3Edxr6TAz.jpg\"]', 3, 1, NULL, 60, 60, 11, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(29, 'GME-260511-0015', 'TYPE_CODE_128', 'Puzzle taplak', NULL, NULL, '0.00', 'images/products/cc4f6869-d783-4cc9-ad0a-d7a053050dda.jpg', NULL, 3, 1, NULL, 32, 32, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(30, 'GME-260511-0016', 'TYPE_CODE_128', 'Cash drawer', NULL, NULL, '0.00', '/images/products/30879695-01c0-4d5a-bfa5-d9683d6384fc.png', '[\"/images/products/30879695-01c0-4d5a-bfa5-d9683d6384fc.png\"]', 3, 1, NULL, 1, 1, 12, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(31, 'GME-260511-0017', 'TYPE_CODE_128', 'Labble Scrabble (Board Only)', NULL, NULL, '0.00', '/images/products/KqoOmKGaMsQEVdKoFb2jyPi8NtUU3YJp8PyHZNfd.jpg', '[\"/images/products/KqoOmKGaMsQEVdKoFb2jyPi8NtUU3YJp8PyHZNfd.jpg\", \"/images/products/P7FcFBkJEehWvJP6DrtvXDw7bH7xxqz2Y0Ss2Cn6.jpg\"]', 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(32, 'GME-260511-0018', 'TYPE_CODE_128', 'Lego besar Hijau', NULL, NULL, '0.00', '/images/products/Ph9VYHzVGJQGy0lQe5vXGTYm5Bc2IY5Su9nmXi8i.jpg', '[\"/images/products/Ph9VYHzVGJQGy0lQe5vXGTYm5Bc2IY5Su9nmXi8i.jpg\"]', 3, 1, NULL, 13, 13, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(33, 'GME-260511-0019', 'TYPE_CODE_128', 'Lego kecil', NULL, NULL, '0.00', '/images/products/9NRUks55CKuWbu7bM9VcirvmlM5RHNq0hJYg3lYd.jpg', '[\"/images/products/9NRUks55CKuWbu7bM9VcirvmlM5RHNq0hJYg3lYd.jpg\"]', 3, 1, NULL, 1, 1, 10, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(34, 'MKN-260511-0012', 'TYPE_CODE_128', 'Minyak exp des 27', NULL, NULL, '50.00', '/images/products/6cf2a55a-a34d-4992-8eda-da0cc2744cae.png', '[\"/images/products/6cf2a55a-a34d-4992-8eda-da0cc2744cae.png\"]', 1, 2, NULL, 6, 6, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(35, 'MKN-260511-0013', 'TYPE_CODE_128', 'Tepung Terigu exp mei 27', NULL, NULL, '50.00', '/images/products/f72f42e6-5094-4bd9-bf69-cf3a84a5406f.png', '[\"/images/products/f72f42e6-5094-4bd9-bf69-cf3a84a5406f.png\"]', 1, 2, NULL, 2, 2, 6, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(36, 'MKN-260511-0014', 'TYPE_CODE_128', 'Tepung terigu', NULL, NULL, '50.00', '/images/products/485606bf-554e-49d2-aef9-2e280effc972.png', '[\"/images/products/485606bf-554e-49d2-aef9-2e280effc972.png\"]', 1, 2, NULL, 3, 3, 13, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(37, 'MKN-260511-0015', 'TYPE_CODE_128', 'Tepung Tapioka exp des 27', NULL, NULL, '50.00', '/images/products/7bd1adef-0402-48a1-a4f7-29007f690f58.png', '[\"/images/products/7bd1adef-0402-48a1-a4f7-29007f690f58.png\"]', 1, 2, NULL, 2, 2, 6, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(38, 'GME-260511-0020', 'TYPE_CODE_128', 'Lost in Paris', NULL, NULL, '0.00', '/images/products/dKyXIzIEVGatN0CR7pOQ9FDGV67z5QEnN1yXTjzb.jpg', '[\"/images/products/dKyXIzIEVGatN0CR7pOQ9FDGV67z5QEnN1yXTjzb.jpg\"]', 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(39, 'GME-260511-0021', 'TYPE_CODE_128', 'Tusuk sate', NULL, NULL, '0.00', 'images/products/86564e04-428a-4234-b300-d8ffe0f2317c.jpg', NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(40, 'GME-260511-0022', 'TYPE_CODE_128', 'Kuas', NULL, NULL, '0.00', 'images/products/93940702-d699-4d45-8197-b56bd4e70fd4.jpg', NULL, 3, 1, NULL, 23, 23, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(41, 'GME-260511-0023', 'TYPE_CODE_128', 'Set kuas', NULL, NULL, '0.00', 'images/products/6273b228-35ae-4e45-b79c-9af2e7eb10f2.jpg', NULL, 3, 1, NULL, 4, 4, 14, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(42, 'GME-260511-0024', 'TYPE_CODE_128', 'Chess Set Game', NULL, NULL, '0.00', '/images/products/rov1rStN5CVYIyLvilFDUJuNzU3BTuTUkPHrBFfC.jpg', '[\"/images/products/rov1rStN5CVYIyLvilFDUJuNzU3BTuTUkPHrBFfC.jpg\", \"/images/products/H2KmMHGFz3dQ79KQtonWAbUYorNANgjejeVDF072.jpg\"]', 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(43, 'GME-260511-0025', 'TYPE_CODE_128', 'Monopoli', NULL, NULL, '0.00', 'images/products/7f81bd75-ef9e-42a3-b4db-0da832fae0f2.jpg', NULL, 3, 1, NULL, 5, 5, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(44, 'ATK-260511-0001', 'TYPE_CODE_128', 'Receipt Printer', NULL, NULL, '50.00', NULL, NULL, 5, 3, NULL, 1, 1, 12, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(45, 'FCY-260511-0004', 'TYPE_CODE_128', 'Tali tambang plastik', NULL, NULL, '50.00', 'images/products/f99d3dc9-e56a-4c7a-b5df-4da5ec4febf6.jpg', NULL, 4, 1, NULL, 9, 9, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(46, 'GME-260511-0026', 'TYPE_CODE_128', 'Games Matras Nomor', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 25, 25, 11, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(47, 'FCY-260511-0005', 'TYPE_CODE_128', 'Tali rapia', NULL, NULL, '50.00', '/images/products/a99f60ca-5db8-41fd-b828-909402f4fa5c.png', '[\"/images/products/a99f60ca-5db8-41fd-b828-909402f4fa5c.png\"]', 4, 2, NULL, 3, 3, 12, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(48, 'FCY-260511-0006', 'TYPE_CODE_128', 'Jas ujan', NULL, NULL, '50.00', 'images/products/5e548021-b3dc-4c2c-8f63-ae518a02d857.jpg', NULL, 4, 1, NULL, 8, 8, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(49, 'GME-260511-0027', 'TYPE_CODE_128', 'Sedotan warna warni', NULL, NULL, '0.00', 'images/products/88ae0e1d-7225-4448-9b85-0309bad950ce.jpg', NULL, 3, 1, NULL, 22, 22, 15, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(50, 'GME-260511-0028', 'TYPE_CODE_128', 'Bel warna warni', NULL, NULL, '0.00', 'images/products/235368be-3f89-43a9-9326-08e9679488d2.png', NULL, 3, 1, NULL, 42, 41, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(51, 'FCY-260511-0007', 'TYPE_CODE_128', 'Hanger', NULL, NULL, '50.00', '/images/products/34JWNSADVpxAkZED6eCtCh3MkVgej2vrDt0VrBs6.jpg', '[\"/images/products/34JWNSADVpxAkZED6eCtCh3MkVgej2vrDt0VrBs6.jpg\"]', 4, 2, NULL, 18, 12, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(53, 'GME-260511-0029', 'TYPE_CODE_128', 'Balon Supporter', NULL, NULL, '0.00', '/images/products/85782708-263a-4dc0-892e-51bac6ed5f19.png', '[\"/images/products/85782708-263a-4dc0-892e-51bac6ed5f19.png\"]', 3, 1, NULL, 6, 6, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(54, 'GME-260511-0030', 'TYPE_CODE_128', 'Karung', NULL, NULL, '0.00', 'images/products/9f77b090-0b5c-4c3c-bb9c-c2cdf3d02117.jpg', NULL, 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(55, 'FCY-260511-0008', 'TYPE_CODE_128', 'Tempat tissue', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 2, 2, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(56, 'MHE-260511-0001', 'TYPE_CODE_128', 'Tumblr corpu', NULL, NULL, '50.00', '/images/products/076002ca-0b80-443c-bebc-4d49fbc0c5a7.png', '[\"/images/products/076002ca-0b80-443c-bebc-4d49fbc0c5a7.png\"]', 6, 2, NULL, 7, 7, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(57, 'MHE-260511-0002', 'TYPE_CODE_128', 'Tumblr IF', NULL, NULL, '50.00', '/images/products/0e4ea1e1-6b40-418c-a1a4-f0246cd67927.png', '[\"/images/products/0e4ea1e1-6b40-418c-a1a4-f0246cd67927.png\"]', 6, 2, NULL, 5, 5, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(58, 'GME-260511-0031', 'TYPE_CODE_128', 'Dangerous Crossing', NULL, NULL, '0.00', '/images/products/sq0oAPx9GWcK7CmWgpJMYJJdUA4SGAnLsgVYoiL7.jpg', '[\"/images/products/sq0oAPx9GWcK7CmWgpJMYJJdUA4SGAnLsgVYoiL7.jpg\"]', 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(59, 'GME-260511-0032', 'TYPE_CODE_128', 'Trompet', NULL, NULL, '0.00', 'images/products/2d66993a-f836-4cbf-94f7-4d16ef467d08.jpg', NULL, 3, 1, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(60, 'FCY-260511-0009', 'TYPE_CODE_128', 'Sarung Tangan', NULL, NULL, '50.00', 'images/products/d7d0afd6-4028-41ed-b71f-3a33be0acc95.jpg', NULL, 4, 1, NULL, 24, 24, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(61, 'GME-260511-0033', 'TYPE_CODE_128', 'Matras Puzzle Angry Bird', NULL, NULL, '0.00', 'images/products/d58de4c1-8b23-4cc0-a6de-a5ba5146d9cd.jpg', NULL, 3, 1, NULL, 16, 16, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(62, 'GME-260511-0034', 'TYPE_CODE_128', 'Matras Puzzle Huruf', NULL, NULL, '0.00', 'images/products/34117537-c7ce-4265-813c-9e38e7cf1113.jpg', NULL, 3, 1, NULL, 21, 21, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(63, 'UT-260512-0063', 'TYPE_CODE_128', 'Tas Hitam', NULL, NULL, '50.00', NULL, NULL, NULL, 4, NULL, 26, 26, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(64, 'UT-260512-0064', 'TYPE_CODE_128', 'Tas Hijau', NULL, NULL, '50.00', NULL, NULL, NULL, 4, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(67, 'MHE-260511-0003', 'TYPE_CODE_128', 'Topi Bucket', NULL, NULL, '50.00', NULL, NULL, 6, 4, NULL, 9, 9, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(68, 'MHE-260511-0004', 'TYPE_CODE_128', 'Cargo Rip Stop', '', NULL, '50.00', '/images/products/a0a7ed7b-ec51-4445-9eb3-d66c44467f1c.jpg', '[\"/images/products/a0a7ed7b-ec51-4445-9eb3-d66c44467f1c.jpg\",\"/images/products/07724268-537a-4de7-807d-38acee202ca1.jpg\"]', 6, 4, NULL, 6, 6, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 01:27:11'),
(69, 'MHE-260511-0005', 'TYPE_CODE_128', 'Cap Coklat', NULL, NULL, '50.00', NULL, NULL, 6, 4, NULL, 4, 4, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(72, 'MSK-260511-0001', 'TYPE_CODE_128', 'Angklung', NULL, NULL, '0.00', '/images/products/MWqEgmHj0SIsEYzmpGwTCQwvZx0DmPZgpULgFihO.jpg', '[\"/images/products/MWqEgmHj0SIsEYzmpGwTCQwvZx0DmPZgpULgFihO.jpg\"]', 8, 2, NULL, 5, 5, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(75, 'FCY-260511-0010', 'TYPE_CODE_128', 'Piagam Generasi Muda 2015', '', NULL, '50.00', '/images/products/1e47ff2a-4831-4116-8ed1-960b4fe88402.png', '[\"/images/products/1e47ff2a-4831-4116-8ed1-960b4fe88402.png\"]', 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 01:51:48'),
(76, 'FCY-260511-0011', 'TYPE_CODE_128', 'Photo Frame', NULL, NULL, '50.00', '/images/products/7e4ce4db-a96b-4e0d-88f7-887070e7f294.png', '[\"/images/products/7e4ce4db-a96b-4e0d-88f7-887070e7f294.png\"]', 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(77, 'FCY-260511-0012', 'TYPE_CODE_128', 'Piagam Instruktur Terbaik 2009', NULL, NULL, '50.00', NULL, NULL, 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(78, 'FCY-260511-0013', 'TYPE_CODE_128', 'Piagam Instruktur Terbaik 2010', NULL, NULL, '50.00', NULL, NULL, 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(79, 'FCY-260511-0014', 'TYPE_CODE_128', 'Skor Pelanggaran Tata Tertib', NULL, NULL, '50.00', '/images/products/80977e71-155a-424f-bde1-c90cc1a6b445.png', '[\"/images/products/80977e71-155a-424f-bde1-c90cc1a6b445.png\"]', 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(80, 'FCY-260511-0015', 'TYPE_CODE_128', 'Piagam Mektel 2012', NULL, NULL, '50.00', NULL, NULL, 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(81, 'FCY-260511-0016', 'TYPE_CODE_128', 'Piagam Mektel 2013', NULL, NULL, '50.00', NULL, NULL, 4, 1, NULL, 2, 2, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(82, 'FCY-260511-0017', 'TYPE_CODE_128', 'Struktur Task Force 2011', NULL, NULL, '50.00', '/images/products/d50b94cf-4b91-464e-b512-54625f50ea7a.png', '[\"/images/products/d50b94cf-4b91-464e-b512-54625f50ea7a.png\"]', 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(83, 'FCY-260511-0018', 'TYPE_CODE_128', 'Photo Frame Kosong Hitam', NULL, NULL, '50.00', '/images/products/f7217a92-c24c-4a63-9e37-88a11b07c299.png', '[\"/images/products/f7217a92-c24c-4a63-9e37-88a11b07c299.png\"]', 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(84, 'FCY-260511-0019', 'TYPE_CODE_128', 'Kain Terpal', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(85, 'MHE-260511-0006', 'TYPE_CODE_128', 'Pulpen Corpu', NULL, NULL, '50.00', NULL, NULL, 6, 3, NULL, 7, 7, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(86, 'ATK-260511-0002', 'TYPE_CODE_128', 'Label Dot Orange', NULL, NULL, '50.00', NULL, NULL, 5, NULL, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(87, 'GME-260511-0035', 'TYPE_CODE_128', 'Huruf M', NULL, NULL, '0.00', NULL, NULL, 3, NULL, NULL, 10, 10, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(88, 'GME-260511-0036', 'TYPE_CODE_128', 'Huruf N', NULL, NULL, '0.00', NULL, NULL, 3, NULL, NULL, 6, 6, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(89, 'GME-260511-0037', 'TYPE_CODE_128', 'Game Bambu Size 1 Merah', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 16, 16, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(90, 'GME-260511-0038', 'TYPE_CODE_128', 'Game Bambu Size 2 Merah', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 16, 16, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(91, 'GME-260511-0039', 'TYPE_CODE_128', 'Game Bambu Size 3 Merah', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 8, 8, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(92, 'GME-260511-0040', 'TYPE_CODE_128', 'Game Bambu Size 4 Merah', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 8, 8, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(93, 'GME-260511-0041', 'TYPE_CODE_128', 'Game Bambu Size 1 Biru', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 15, 15, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(94, 'GME-260511-0042', 'TYPE_CODE_128', 'Game Bambu Size 2 Biru', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 16, 16, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(95, 'GME-260511-0043', 'TYPE_CODE_128', 'Game Bambu Size 3 Biru', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 9, 9, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(96, 'GME-260511-0044', 'TYPE_CODE_128', 'Game Bambu Size 4 Biru', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 8, 8, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(97, 'FCY-260511-0020', 'TYPE_CODE_128', 'Sertifikat PU 2017 with Frame', NULL, NULL, '50.00', 'images/products/2987bd3c-361b-49fc-9f52-51645cbc3d73.jpg', '[\"/images/products/ziBLRils3QsuPxLkAC98Fs3eHlg5bClL7FXjXsV5.jpg\"]', 4, 1, NULL, 9, 9, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(98, 'FCY-260511-0021', 'TYPE_CODE_128', 'Miniatur Tiang Kecil', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(99, 'FCY-260511-0022', 'TYPE_CODE_128', 'Slayer Hijau', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(100, 'ATK-260511-0003', 'TYPE_CODE_128', 'Notebook Corpu', NULL, NULL, '50.00', NULL, NULL, 5, NULL, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(101, 'FCY-260511-0023', 'TYPE_CODE_128', 'Bingkai', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 37, 4, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(105, 'FCY-260511-0024', 'TYPE_CODE_128', 'Kompor Listrik', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(106, 'FCY-260511-0025', 'TYPE_CODE_128', 'Papan Jalan Biru + HVS', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(107, 'FCY-260511-0026', 'TYPE_CODE_128', 'Frame Foto Kecil Putih', NULL, NULL, '50.00', 'images/products/f5ab8456-30f5-4cf7-b9eb-c5494c366253.jpg', NULL, 4, NULL, NULL, 8, 8, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(110, 'FCY-260511-0027', 'TYPE_CODE_128', 'Berkas Kegiatan PU (1 Bundle)', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 1, 1, 16, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(111, 'FCY-260511-0028', 'TYPE_CODE_128', 'Karikatur Kampus Merdeka', NULL, NULL, '50.00', '/images/products/ac461a9d-8df9-43d2-bedb-18060e46e33f.png', '[\"/images/products/ac461a9d-8df9-43d2-bedb-18060e46e33f.png\"]', 4, 1, NULL, 2, 2, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(112, 'FCY-260511-0029', 'TYPE_CODE_128', 'Sertifikat sponsor', NULL, NULL, '50.00', '/images/products/fd3138a1-ee58-4226-b0b9-69393050300e.png', '[\"/images/products/fd3138a1-ee58-4226-b0b9-69393050300e.png\"]', 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(114, 'FCY-260511-0030', 'TYPE_CODE_128', 'Karikatur Best Prime Mover PUDP 2021', NULL, NULL, '50.00', '/images/products/9f2228c1-1bee-471b-83c1-1cdd48461f7c.png', '[\"/images/products/9f2228c1-1bee-471b-83c1-1cdd48461f7c.png\"]', 4, 1, NULL, 3, 3, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(115, 'GME-260511-0045', 'TYPE_CODE_128', 'How do you see it (Game)', NULL, NULL, '0.00', 'images/products/1011778b-875d-41e2-b09d-62da06614f42.jpg', NULL, 3, 1, NULL, 2, 2, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(116, 'GME-260511-0046', 'TYPE_CODE_128', 'Paradigm (Game)', NULL, NULL, '0.00', 'images/products/f63a80a9-aebf-4e29-a6eb-ffea065bc559.jpg', NULL, 3, 1, NULL, 6, 6, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(117, 'GME-260511-0047', 'TYPE_CODE_128', 'SBS Challenge (Game)', NULL, NULL, '0.00', 'images/products/ad8d1a15-20ff-4a76-9fe8-5f47477125d6.jpg', NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(118, 'GME-260511-0048', 'TYPE_CODE_128', 'Productivity game', NULL, NULL, '0.00', '/images/products/01da93be-a520-48ef-855a-00ba36881ab6.jpg', '[\"/images/products/01da93be-a520-48ef-855a-00ba36881ab6.jpg\", \"/images/products/c8c647f2-4c17-4094-a240-276745c21ff7.jpg\"]', 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(119, 'GME-260511-0049', 'TYPE_CODE_128', 'Guidance game', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 2, 2, 17, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(120, 'FCY-260511-0031', 'TYPE_CODE_128', 'Time Keeper', NULL, NULL, '50.00', 'images/products/050aaba3-3805-4539-8f71-6ab4235b483d.jpg', NULL, 4, 1, NULL, 1, 1, 7, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(121, 'GME-260511-0050', 'TYPE_CODE_128', 'Find a match game', NULL, NULL, '0.00', '/images/products/2bXVoSfOvfCbc4iccQD9xSDddPP9xJjlveVbhXfJ.jpg', '[\"/images/products/2bXVoSfOvfCbc4iccQD9xSDddPP9xJjlveVbhXfJ.jpg\"]', 3, 1, NULL, 2, 2, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(122, 'GME-260511-0051', 'TYPE_CODE_128', 'Communicating for performance (Game)', NULL, NULL, '0.00', '/images/products/a1ff7e68-a806-436c-8dec-bc4a4ff5c396.png', '[\"/images/products/a1ff7e68-a806-436c-8dec-bc4a4ff5c396.png\"]', 3, 1, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(123, 'GME-260511-0052', 'TYPE_CODE_128', 'Party pooper', NULL, NULL, '0.00', 'images/products/f514ced3-a6af-4f70-8fd9-59bc8979d5a1.png', NULL, 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(124, 'GME-260511-0053', 'TYPE_CODE_128', 'Airport Controller (Game)', NULL, NULL, '0.00', '/images/products/7ab31872-88df-494d-9a69-02f4623978b9.png', '[\"/images/products/7ab31872-88df-494d-9a69-02f4623978b9.png\"]', 3, 1, NULL, 0, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(125, 'GME-260511-0054', 'TYPE_CODE_128', 'Puzzle balok', NULL, NULL, '0.00', 'images/products/76771ffc-3bde-4a01-b5b2-423b2c9e83e1.jpg', NULL, 3, 1, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(126, 'GME-260511-0055', 'TYPE_CODE_128', 'Category game', NULL, NULL, '0.00', 'images/products/eb0840bb-164d-4362-8289-9ad412db08db.jpg', NULL, 3, 1, NULL, 7, 7, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(127, 'GME-260511-0056', 'TYPE_CODE_128', 'Colaboration game', NULL, NULL, '0.00', 'images/products/1eac8e33-6839-4331-8989-4461b07568fc.jpg', NULL, 3, 1, NULL, 6, 6, 14, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(128, 'GME-260511-0057', 'TYPE_CODE_128', 'Kartu cinta Indonesia', NULL, NULL, '0.00', 'images/products/1dd80f77-1b7e-4336-9cc3-eb3e26532df6.jpg', NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(129, 'GME-260511-0058', 'TYPE_CODE_128', 'Puzzle jalan', NULL, NULL, '0.00', 'images/products/7bc7dd23-01ce-4f30-b1e5-b2eecb65add7.jpg', NULL, 3, 1, NULL, 6, 6, 18, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(130, 'GME-260511-0059', 'TYPE_CODE_128', 'Balon', NULL, NULL, '0.00', 'images/products/b4af5cc4-83f8-483c-bc40-fb916b72c2ba.jpg', NULL, 3, 1, NULL, 4, 4, 19, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(131, 'GME-260511-0060', 'TYPE_CODE_128', 'The dangerous crossing', '', NULL, '0.00', '/images/products/4889329b-dbd5-44e5-ac78-74166f264eb1.png', '[\"/images/products/4889329b-dbd5-44e5-ac78-74166f264eb1.png\"]', 3, 1, NULL, 7, 7, 3, 1, 0, '2026-05-11 13:46:23', '2026-06-10 02:04:35'),
(132, 'GME-260511-0061', 'TYPE_CODE_128', 'Lets play music (Game)', NULL, NULL, '0.00', 'images/products/439ecc23-f0a8-48ba-80df-9a2bbe1f71cf.jpg', NULL, 3, 1, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(133, 'GME-260511-0062', 'TYPE_CODE_128', 'Stack them up (Game)', NULL, NULL, '0.00', 'images/products/3ad12342-7caf-4699-bd7b-1f1a04d3fd86.jpg', NULL, 3, 1, NULL, 6, 6, 14, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(134, 'GME-260511-0063', 'TYPE_CODE_128', 'Ethics game', NULL, NULL, '0.00', 'images/products/9f2ae981-0060-4093-9ff3-9b1ec9a378cd.jpg', NULL, 3, 1, NULL, 1, 1, 20, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(135, 'GME-260511-0064', 'TYPE_CODE_128', 'Ketapel', NULL, NULL, '0.00', 'images/products/3568eebc-0b01-4684-bfdc-31f842fc36a3.jpg', NULL, 3, 1, NULL, 6, 6, 20, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(136, 'GME-260511-0065', 'TYPE_CODE_128', 'Keyboard game', '', NULL, '0.00', '/images/products/faddf499-b56f-4551-b876-699137ce4040.jpg', '[\"/images/products/faddf499-b56f-4551-b876-699137ce4040.jpg\"]', 3, 1, NULL, 1, 1, 20, 1, 0, '2026-05-11 13:46:23', '2026-06-10 01:30:24'),
(137, 'GME-260511-0066', 'TYPE_CODE_128', 'Out of my way (Game)', NULL, NULL, '0.00', '/images/products/563a1720-8e1f-4653-a3dd-0f4a933485af.png', '[\"/images/products/563a1720-8e1f-4653-a3dd-0f4a933485af.png\"]', 3, 1, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(138, 'GME-260511-0067', 'TYPE_CODE_128', 'Uno jenga', '', NULL, '0.00', '/images/products/72f0ed53-0eac-48fa-89d8-a9a867c0bc2c.png', '[\"/images/products/72f0ed53-0eac-48fa-89d8-a9a867c0bc2c.png\"]', 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-10 02:05:25'),
(139, 'GME-260511-0068', 'TYPE_CODE_128', 'Jawaban devide es impera', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(140, 'GME-260511-0069', 'TYPE_CODE_128', 'Uang mainan', NULL, NULL, '0.00', 'images/products/3d82539d-3a3a-47bb-b6ac-7b9c295a134f.jpg', NULL, 3, 1, NULL, 6, 6, 20, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(141, 'GME-260511-0070', 'TYPE_CODE_128', 'Papan challenge ombak', '', NULL, '0.00', '/images/products/80ec845e-272c-487b-b023-2c03b04db204.jpg', '[\"/images/products/80ec845e-272c-487b-b023-2c03b04db204.jpg\"]', 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-06-10 02:07:17'),
(142, 'GME-260511-0071', 'TYPE_CODE_128', 'Devide et impera (Game)', NULL, NULL, '0.00', '/images/products/michcKQcwIVglbgwVwZhuNqrxBwE5xuqOsw2w0aR.jpg', '[\"/images/products/michcKQcwIVglbgwVwZhuNqrxBwE5xuqOsw2w0aR.jpg\"]', 3, 1, NULL, 23, 23, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(143, 'GME-260511-0072', 'TYPE_CODE_128', 'Who grows what (Game)', NULL, NULL, '0.00', 'images/products/256582ed-db3a-4915-aa2a-94c0aff24ca3.jpg', NULL, 3, 1, NULL, 2, 2, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(144, 'GME-260511-0073', 'TYPE_CODE_128', 'Botol kaca', NULL, NULL, '0.00', '/images/products/fe08cf14-db20-40bb-b47f-a09acf0d1a61.png', '[\"/images/products/fe08cf14-db20-40bb-b47f-a09acf0d1a61.png\"]', 3, 1, NULL, 3, 3, 20, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(145, 'GME-260511-0074', 'TYPE_CODE_128', 'Human Leap', NULL, NULL, '0.00', 'images/products/3110ff19-9f63-4d50-947f-e33733cd7e07.jpg', NULL, 3, 1, NULL, 1, 1, 17, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(146, 'GME-260511-0075', 'TYPE_CODE_128', 'Ceforo challenge (Game)', NULL, NULL, '0.00', NULL, NULL, 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(147, 'GME-260511-0076', 'TYPE_CODE_128', 'Puzzle tetris', NULL, NULL, '0.00', '/images/products/2tMW75ykRWWoV6B2q4PMTu5OP8qayScb27cOLU6t.jpg', '[\"/images/products/2tMW75ykRWWoV6B2q4PMTu5OP8qayScb27cOLU6t.jpg\"]', 3, 1, NULL, 1, 1, 7, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(148, 'MSK-260511-0002', 'TYPE_CODE_128', 'Tamborin kerincing', NULL, NULL, '0.00', NULL, NULL, 8, 1, NULL, 1, 1, 20, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(149, 'GME-260511-0077', 'TYPE_CODE_128', 'Mainboard', NULL, NULL, '0.00', '/images/products/YKS0CHs1cJeX0FsrVaQFMvZCYLP0OAirscXlLsS9.jpg', '[\"/images/products/YKS0CHs1cJeX0FsrVaQFMvZCYLP0OAirscXlLsS9.jpg\"]', 3, 1, NULL, 1, 1, 20, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(150, 'MSK-260511-0003', 'TYPE_CODE_128', 'Drum kecil', NULL, NULL, '0.00', '/images/products/46kxifQTL6QWtn1VPw3NYR45Rw81CzwqOSqK78bH.jpg', '[\"/images/products/46kxifQTL6QWtn1VPw3NYR45Rw81CzwqOSqK78bH.jpg\"]', 8, 1, NULL, 2, 2, 20, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(151, 'MSK-260511-0004', 'TYPE_CODE_128', 'Stick drum', NULL, NULL, '0.00', NULL, NULL, 8, 1, NULL, 7, 7, 20, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(152, 'ETK-260511-0001', 'TYPE_CODE_128', 'Cable manager', NULL, NULL, '0.00', 'images/products/e3680bde-7ed5-4508-85cb-48c5e564b993.jpg', NULL, 9, 1, NULL, 4, 4, 20, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(153, 'ETK-260511-0002', 'TYPE_CODE_128', 'Kabel gulungan', NULL, NULL, '0.00', 'images/products/8d67307c-ed6a-42ce-a1fa-ab8ce3436f79.jpg', NULL, 9, 1, NULL, 3, 3, 20, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(154, 'FCY-260511-0032', 'TYPE_CODE_128', 'Kawat gulungan', NULL, NULL, '50.00', 'images/products/c038ecc7-79f8-4b06-af07-7b9567fbba3e.jpg', NULL, 4, 1, NULL, 1, 1, 20, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(155, 'FCY-260511-0033', 'TYPE_CODE_128', 'Akrilik papan - nama ruangan', NULL, NULL, '50.00', 'images/products/61e678cd-3425-4f2e-8303-d3aa448cfd97.jpg', NULL, 4, 1, NULL, 5, 5, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(156, 'FCY-260511-0034', 'TYPE_CODE_128', 'Silinder paralon', NULL, NULL, '50.00', NULL, NULL, 4, 1, NULL, 40, 40, 20, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(157, 'FCY-260511-0035', 'TYPE_CODE_128', 'Pipa & paralon', NULL, NULL, '50.00', NULL, NULL, 4, 1, NULL, 1, 1, 14, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(158, 'FCY-260511-0036', 'TYPE_CODE_128', 'Canvas', NULL, NULL, '50.00', 'images/products/50118487-ee9a-4ba7-b89b-79fc8e125b22.jpg', NULL, 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(159, 'FCY-260511-0037', 'TYPE_CODE_128', 'Penghargaan Astra Virtual Playday', NULL, NULL, '50.00', NULL, NULL, 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(160, 'FCY-260511-0038', 'TYPE_CODE_128', 'Penghargaan Astra Toll Road', '', NULL, '50.00', '/images/products/904414f2-ab47-4bef-a0f5-4b160a61d532.png', '[\"/images/products/904414f2-ab47-4bef-a0f5-4b160a61d532.png\"]', 4, 1, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 02:09:37'),
(161, 'GME-260511-0078', 'TYPE_CODE_128', 'Games Pie Face Showdown', NULL, NULL, '0.00', '/images/products/l4uh3qAaLxoqKJ0IVh3JNArVT4vWbVePnrzt2hoU.jpg', '[\"/images/products/l4uh3qAaLxoqKJ0IVh3JNArVT4vWbVePnrzt2hoU.jpg\"]', 3, 1, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(162, 'FCY-260511-0039', 'TYPE_CODE_128', 'Remote Projector Epson', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 5, 5, 20, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(163, 'FCY-260511-0040', 'TYPE_CODE_128', 'Remote Perfume Dispenser', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 1, 1, 20, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(164, 'ETK-260511-0003', 'TYPE_CODE_128', 'Commscope Port Unshuttered (LAN Port)', NULL, NULL, '0.00', NULL, NULL, 9, NULL, NULL, 5, 5, 20, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(165, 'ETK-260511-0004', 'TYPE_CODE_128', 'Onfinity Wireless Interactive Whiteboard', NULL, NULL, '0.00', NULL, NULL, 9, NULL, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(166, 'FCY-260511-0041', 'TYPE_CODE_128', 'Case Pointer/Pulpen', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 1, 1, 20, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(167, 'GME-260511-0079', 'TYPE_CODE_128', 'Puzzle Kayu', NULL, NULL, '0.00', NULL, NULL, 3, NULL, NULL, 16, 16, 14, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(168, 'FCY-260511-0042', 'TYPE_CODE_128', 'Aneka Kunci', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 1, 1, 14, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(169, 'FCY-260511-0043', 'TYPE_CODE_128', 'Laser Barcode Scanner', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 1, 1, 14, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(170, 'ATK-260511-0004', 'TYPE_CODE_128', 'Styrofoam', NULL, NULL, '50.00', NULL, NULL, 5, NULL, NULL, 6, 6, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(171, 'FCY-260511-0044', 'TYPE_CODE_128', 'Rangka X-Banner', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 2, 2, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(172, 'FCY-260511-0045', 'TYPE_CODE_128', 'Raket Tennis Yonex', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(173, 'FCY-260511-0046', 'TYPE_CODE_128', 'Life Vest', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 2, 2, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(174, 'FCY-260511-0047', 'TYPE_CODE_128', 'Plakat BPK Penabur Jakarta', NULL, NULL, '50.00', '/images/products/7dbf8e00-115f-4551-b785-2103ec487dd4.png', '[\"/images/products/7dbf8e00-115f-4551-b785-2103ec487dd4.png\"]', 4, NULL, NULL, 1, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(175, 'FCY-260511-0048', 'TYPE_CODE_128', 'QCC Dashboard', NULL, NULL, '50.00', NULL, NULL, 4, NULL, NULL, 5, 5, 21, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(176, 'MHE-260511-0007', 'TYPE_CODE_128', 'Gelang Corpu', NULL, NULL, '50.00', '/images/products/c3957bc5-bb05-437e-a306-644ddb6e6e9f.png', '[\"/images/products/c3957bc5-bb05-437e-a306-644ddb6e6e9f.png\"]', 6, 2, NULL, 279, 30, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(180, 'FCY-260511-0049', 'TYPE_CODE_128', 'Gelas Plastik', NULL, NULL, '50.00', 'images/products/8ed36c09-c097-4531-b0b4-1f2063e6eab9.jpg', NULL, 4, 1, NULL, 69, 2, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(183, 'FCY-260511-0050', 'TYPE_CODE_128', 'Cup', NULL, NULL, '50.00', 'images/products/c93cdbea-7efc-419b-94af-08568b686284.jpg', NULL, 4, 1, NULL, 98, 20, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(185, 'FCY-260511-0051', 'TYPE_CODE_128', 'Benang', NULL, NULL, '50.00', 'images/products/69104e83-cc7c-4b65-8aa2-fcab70d7f67a.jpg', NULL, 4, 1, NULL, 11, 5, 22, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(187, 'FCY-260511-0052', 'TYPE_CODE_128', 'Tali tambang putih', NULL, NULL, '50.00', 'images/products/787a6bb4-9846-44cd-a40e-e8f5419df25c.jpg', NULL, 4, 1, NULL, 5, 5, 22, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(188, 'FCY-260511-0053', 'TYPE_CODE_128', 'Pita Dekorasi', NULL, NULL, '50.00', 'images/products/4e742f1c-18dd-41ef-aff0-bd1153d46c44.jpg', NULL, 4, 1, NULL, 4, 4, 22, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(189, 'FCY-260511-0054', 'TYPE_CODE_128', 'Kancing', NULL, NULL, '50.00', 'images/products/a0aed741-cb88-4473-af12-a518ebc94d58.jpg', NULL, 4, 1, NULL, 5, 1, 23, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(193, 'FCY-260511-0055', 'TYPE_CODE_128', 'Jarum benang', NULL, NULL, '50.00', NULL, NULL, 4, 1, NULL, 1, 1, 14, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(194, 'GME-260511-0080', 'TYPE_CODE_128', 'Hulahop warna besar', NULL, NULL, '0.00', 'images/products/5ac6e8cc-cc8d-4078-a092-4b18c28366d2.jpg', NULL, 3, 1, NULL, 23, 23, 24, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(195, 'GME-260511-0081', 'TYPE_CODE_128', 'Lilin malam', NULL, NULL, '0.00', 'images/products/a918aae2-24fb-4084-8e24-506bcc980c7b.jpg', NULL, 3, 1, NULL, 5, 5, 25, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(196, 'FCY-260511-0056', 'TYPE_CODE_128', 'Karet', NULL, NULL, '50.00', 'images/products/3940831c-7e35-4e61-9176-b068f0b01de7.jpg', NULL, 4, 1, NULL, 1, 1, 19, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(197, 'MHE-260511-0008', 'TYPE_CODE_128', 'Jas Hitam Laki-laki', NULL, NULL, '50.00', NULL, NULL, 6, NULL, NULL, 5, 2, 26, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(200, 'MHE-260511-0009', 'TYPE_CODE_128', 'Kipas Handheld AHEMCEKECE', NULL, NULL, '50.00', NULL, NULL, 6, NULL, NULL, 4, 4, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(201, 'MHE-260511-0010', 'TYPE_CODE_128', 'Totebag AHEMCEKECE', NULL, NULL, '50.00', NULL, NULL, 6, NULL, NULL, 2, 2, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(202, 'GME-260511-0082', 'TYPE_CODE_128', 'Chips Karambol', NULL, NULL, '0.00', '/images/products/pMsTfBV4xfTgq8Zh7zL7814bPpAVQywXxf4Ahfqz.jpg', '[\"/images/products/pMsTfBV4xfTgq8Zh7zL7814bPpAVQywXxf4Ahfqz.jpg\"]', 3, 1, NULL, 42, 42, 10, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(203, 'GME-260511-0083', 'TYPE_CODE_128', 'Holahoop Kecil', NULL, NULL, '0.00', '/images/products/EE27aXrspLuCkBgAKjgLfO0vxTkRSTEgimM1QsaF.jpg', '[\"/images/products/EE27aXrspLuCkBgAKjgLfO0vxTkRSTEgimM1QsaF.jpg\"]', 3, 1, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(204, 'FCY-260511-0057', 'TYPE_CODE_128', 'Cooking Gas Can', NULL, NULL, '50.00', '/images/products/1a337488-01cf-4f3c-b69f-e25b648c9609.png', '[\"/images/products/1a337488-01cf-4f3c-b69f-e25b648c9609.png\"]', 4, 1, NULL, 4, 4, 27, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(205, 'MHE-260511-0011', 'TYPE_CODE_128', 'Lanyard UTSMART Merah + Biru', '', NULL, '50.00', '/images/products/ac0f07c0-519f-4f39-bed7-9edfe40e7484.jpg', '[\"/images/products/ac0f07c0-519f-4f39-bed7-9edfe40e7484.jpg\"]', 6, 4, NULL, 90, 97, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 01:43:48'),
(208, 'MHE-260511-0012', 'TYPE_CODE_128', 'Package UT Virtual Gathering - Kaos, Tumblr, Masket Mulut', NULL, NULL, '50.00', NULL, NULL, 6, 4, NULL, 6, 6, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(210, 'MHE-260511-0013', 'TYPE_CODE_128', 'Tas Reven', NULL, NULL, '50.00', '/images/products/Yt3zw1dAU8nAXLh4arGdQG9eAgvc77VFT1OOcqcs.jpg', '[\"/images/products/Yt3zw1dAU8nAXLh4arGdQG9eAgvc77VFT1OOcqcs.jpg\"]', 6, 4, NULL, 8, 4, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(213, 'MHE-260511-0014', 'TYPE_CODE_128', 'Sweater UT', '', NULL, '50.00', '/images/products/4dO3o7jk4h8AAwTE0MtqVAsBATECRbalrBA9vF61.jpg', '[\"/images/products/4dO3o7jk4h8AAwTE0MtqVAsBATECRbalrBA9vF61.jpg\"]', 6, 4, NULL, 113, 7, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 02:15:50'),
(228, 'MHE-260511-0015', 'TYPE_CODE_128', 'Tumbler UT Smart', NULL, NULL, '50.00', '/images/products/PvqIubSfSp7ootvD480EtqKrPw71BtY4kch5sYHE.jpg', '[\"/images/products/PvqIubSfSp7ootvD480EtqKrPw71BtY4kch5sYHE.jpg\"]', 6, 4, NULL, 66, 73, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(229, 'MHE-260511-0016', 'TYPE_CODE_128', 'Tumbler Plastik UT Smart', NULL, NULL, '50.00', '/images/products/ymVgSipIGBtEA6XLhpQMFbYbhITtaCCCw9xN0PTz.jpg', '[\"/images/products/ymVgSipIGBtEA6XLhpQMFbYbhITtaCCCw9xN0PTz.jpg\"]', 6, 4, NULL, 367, 181, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(231, 'MHE-260511-0017', 'TYPE_CODE_128', 'Box Packaging Sweater UNTR', NULL, NULL, '50.00', NULL, NULL, 6, 4, NULL, 26, 25, 14, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(232, 'MHE-260511-0018', 'TYPE_CODE_128', 'Keranjang Anyaman', NULL, NULL, '50.00', NULL, NULL, 6, 4, NULL, 7, 7, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(233, 'MHE-260511-0019', 'TYPE_CODE_128', 'Stiker UT Smart Kuning Panjang', '', NULL, '50.00', '/images/products/731d87fb-26b0-4726-b2aa-96fc7529801c.jpg', '[\"/images/products/731d87fb-26b0-4726-b2aa-96fc7529801c.jpg\"]', 6, 4, NULL, 34, 34, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 01:33:26'),
(234, 'FCY-260511-0058', 'TYPE_CODE_128', 'Good Detectors', NULL, NULL, '50.00', '/images/products/RU1FW4EPxjCEI5QO3jjE6JyibUEGFErBlfyb0us6.jpg', '[\"/images/products/RU1FW4EPxjCEI5QO3jjE6JyibUEGFErBlfyb0us6.jpg\", \"/images/products/VrD5WCm5a1C63yGbfF2u6QYwVMJ87kqYXVQ6YFWJ.jpg\", \"/images/products/5KvD4rwbFP1OQkbAw87a7KyDChgJblBpin6koDw2.jpg\", \"/images/products/cRBAWsnGT2MNbL9D8TtzCj6rCFW4eWstFXHB58AE.jpg\"]', 4, 4, NULL, 3, 3, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(240, 'MHE-260511-0020', 'TYPE_CODE_128', 'Rompi PU 2025', '', NULL, '50.00', '/images/products/95763618-84ef-43f8-af40-cfc24015ee63.jpg', '[\"/images/products/95763618-84ef-43f8-af40-cfc24015ee63.jpg\",\"/images/products/e066c0a7-94cf-4004-a8aa-8d18bd55bb40.jpg\"]', 6, 4, NULL, 14, 14, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 01:30:06'),
(241, 'MHE-260511-0021', 'TYPE_CODE_128', 'Jersey UT', '', NULL, '50.00', '/images/products/9w4qRI6MNgZ0kKxO1paGfJmby70g7MNAIdJIIwK3.jpg', '[\"/images/products/9w4qRI6MNgZ0kKxO1paGfJmby70g7MNAIdJIIwK3.jpg\"]', 6, 4, NULL, 205, 5, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 02:20:56'),
(253, 'MHE-260511-0022', 'TYPE_CODE_128', 'ID Card', '', NULL, '50.00', '/images/products/4d3941c2-eaf3-4b6a-b4bc-b3f435615ebb.jpg', '[\"/images/products/4d3941c2-eaf3-4b6a-b4bc-b3f435615ebb.jpg\",\"/images/products/564fb621-8cb1-4f4c-a9c6-18cb82828660.jpg\",\"/images/products/2c3e992b-21dc-468e-af3d-ea63794ea767.jpg\",\"/images/products/cebe5309-408f-4d0d-8989-2f766e33af5e.jpg\"]', 6, 4, NULL, 768, 68, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 01:43:15'),
(257, 'MHE-260511-0023', 'TYPE_CODE_128', 'ID Card Batik Tali', '', NULL, '50.00', '/images/products/dac46c80-aa4d-4d71-8dfd-eb0404cdf75d.jpg', '[\"/images/products/dac46c80-aa4d-4d71-8dfd-eb0404cdf75d.jpg\"]', 6, 4, NULL, 159, 97, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 01:42:01'),
(263, 'MHE-260511-0024', 'TYPE_CODE_128', 'ID Card Holder', '', NULL, '50.00', '/images/products/f0875326-e944-435e-965a-a90ef5908e99.jpg', '[\"/images/products/f0875326-e944-435e-965a-a90ef5908e99.jpg\"]', 6, 4, NULL, 563, 508, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 01:42:46'),
(265, 'MHE-260511-0025', 'TYPE_CODE_128', 'Giftset Mentor AFLP - Dus', NULL, NULL, '50.00', '/images/products/I9Q9a6IrwVGxsjh08PojcleWsXcAlQvbRwy3Q03F.jpg', '[\"/images/products/I9Q9a6IrwVGxsjh08PojcleWsXcAlQvbRwy3Q03F.jpg\", \"/images/products/eKNJtyYmrbsZEOlGieWl2WjZ3Xsm3e9sw9S9eDit.jpg\", \"/images/products/VPAzUeOZNA3LXbh4LP0SocPWhTjch58eXuA9pDuc.jpg\"]', 6, 4, NULL, 155, 18, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(272, 'MHE-260511-0026', 'TYPE_CODE_128', 'Buku Batubara Indonesia', NULL, NULL, '50.00', NULL, NULL, 6, 4, NULL, 7, 7, 29, 0, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(273, 'MHE-260511-0027', 'TYPE_CODE_128', 'Gift Box AFLP 2025 - Dus', NULL, NULL, '50.00', NULL, NULL, 6, 4, NULL, 94, 21, 3, 0, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(278, 'MHE-260511-0028', 'TYPE_CODE_128', 'Giftbox AALP Sleeve Laptop Sovlo', NULL, NULL, '50.00', '/images/products/6xEFpX4Y9W4gThatgZoaB9iEvgJaj58HQ2ajxsCH.jpg', '[\"/images/products/6xEFpX4Y9W4gThatgZoaB9iEvgJaj58HQ2ajxsCH.jpg\", \"/images/products/qV6RD5vJOL1yRCpJ6skoMK4lx6ySu56crAdK299V.jpg\"]', 6, 4, NULL, 10, 10, 3, 0, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(279, 'MHE-260511-0029', 'TYPE_CODE_128', 'Giftbox besar AALP (kosong)', '', NULL, '50.00', '/images/products/5d67ce7a-0541-471f-88a9-ca7a93450ca1.jpg', '[\"/images/products/5d67ce7a-0541-471f-88a9-ca7a93450ca1.jpg\"]', 6, 4, NULL, 7, 7, 3, 0, 0, '2026-05-11 13:46:24', '2026-06-10 01:36:39'),
(280, 'MHE-260511-0030', 'TYPE_CODE_128', 'Giftbox Notebook PB + Pen - Dus', NULL, NULL, '50.00', '/images/products/4gh6SKqTHq0jMUACqEhesrJDbYCLfx1mKTJi2sIL.jpg', '[\"/images/products/4gh6SKqTHq0jMUACqEhesrJDbYCLfx1mKTJi2sIL.jpg\", \"/images/products/uYzWlEszPscVilHlhvRmVbyprK2biIbHEXsfQLzi.jpg\", \"/images/products/lSXEMDYu7I5uYf26aTcoLnKHadhzIax0AvjW5V6b.jpg\", \"/images/products/chh4TkdWhWhbhWTX8OrrBmKIuYD0fodTuVOstxcp.jpg\"]', 6, 4, NULL, 35, 20, 3, 0, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(282, 'MHE-260511-0031', 'TYPE_CODE_128', 'Merch Assessor - PB Baseus', NULL, NULL, '50.00', '/images/products/aS09T1ym2S5miKgmLkTj7R263Jt6Wyfn7tbLQMlf.jpg', '[\"/images/products/aS09T1ym2S5miKgmLkTj7R263Jt6Wyfn7tbLQMlf.jpg\"]', 6, 4, NULL, 5, 5, 3, 0, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(283, 'MHE-260511-0032', 'TYPE_CODE_128', 'Merch Assessor - TWS Baseus', NULL, NULL, '50.00', '/images/products/MSeFMzMm2Z1f01p0RgN3ew2Fe5UgIDQQJZ1hOKqX.jpg', '[\"/images/products/MSeFMzMm2Z1f01p0RgN3ew2Fe5UgIDQQJZ1hOKqX.jpg\"]', 6, 4, NULL, 20, 10, 3, 0, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(285, 'FCY-260511-0059', 'TYPE_CODE_128', 'Amplop Bubble', '', NULL, '50.00', '/images/products/a9b13425-e88f-4dbf-8f6d-9a321cad5ddc.png', '[\"/images/products/a9b13425-e88f-4dbf-8f6d-9a321cad5ddc.png\"]', 4, 4, NULL, 20, 20, 3, 0, 0, '2026-05-11 13:46:24', '2026-06-10 01:38:49'),
(286, 'ETK-260511-0005', 'TYPE_CODE_128', 'NS Accessories 18 in 1', NULL, NULL, '0.00', NULL, NULL, 9, 6, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(287, 'ETK-260511-0006', 'TYPE_CODE_128', 'NS Accesssories Ringfit Adventure', NULL, NULL, '0.00', NULL, NULL, 9, 6, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(288, 'ETK-260511-0007', 'TYPE_CODE_128', 'Kaset Game Nintendo', NULL, NULL, '0.00', 'images/products/782c4ac5-0ce5-4991-a486-db0e56a8a0bd.png', '[\"images/products/782c4ac5-0ce5-4991-a486-db0e56a8a0bd.png\"]', 9, 6, NULL, 3, 3, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(289, 'GME-260511-0084', 'TYPE_CODE_128', 'Board Game Penguin', NULL, NULL, '0.00', NULL, NULL, 3, 6, NULL, 2, 2, 14, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(290, 'GME-260511-0085', 'TYPE_CODE_128', 'Klask - Board Game', NULL, NULL, '0.00', NULL, NULL, 3, 7, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(291, 'GME-260511-0086', 'TYPE_CODE_128', 'Board Game', NULL, NULL, '0.00', NULL, NULL, 3, 7, NULL, 2, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(293, 'FCY-260511-0060', 'TYPE_CODE_128', 'Bel Quiz', NULL, NULL, '50.00', NULL, NULL, 4, 8, NULL, 2, 2, 3, 0, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(294, 'GME-260511-0087', 'TYPE_CODE_128', '3D Puzzle Cube', NULL, NULL, '0.00', NULL, NULL, 3, 8, NULL, 1, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52');
INSERT INTO `products` (`id`, `sku`, `barcode_type`, `name`, `description`, `transaction_type`, `value`, `image`, `images`, `category_id`, `location_id`, `position_image`, `current_stock`, `initial_stock`, `unit_id`, `is_returnable`, `min_stock`, `created_at`, `updated_at`) VALUES
(295, 'GME-260511-0088', 'TYPE_CODE_128', 'Rubik Speed Cube', NULL, NULL, '0.00', NULL, NULL, 3, 8, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(296, 'GME-260511-0089', 'TYPE_CODE_128', 'Puzzle Kayu', NULL, NULL, '0.00', NULL, NULL, 3, 8, NULL, 25, 25, 14, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(297, 'GME-260511-0090', 'TYPE_CODE_128', 'Games Balok Kayu Set', NULL, NULL, '0.00', NULL, NULL, 3, 8, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(298, 'GME-260511-0091', 'TYPE_CODE_128', 'Baffling Steel Puzzle', NULL, NULL, '0.00', NULL, NULL, 3, 8, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(299, 'FCY-260511-0061', 'TYPE_CODE_128', 'Properti Balon dan Sarung Tangan', NULL, NULL, '50.00', NULL, NULL, 4, 9, NULL, 1, 1, 30, 0, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(300, 'ETK-260511-0008', 'TYPE_CODE_128', 'Yi CamCase', NULL, NULL, '0.00', NULL, NULL, 9, 7, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(301, 'ETK-260511-0009', 'TYPE_CODE_128', 'Canon G1X + 15-60mm', NULL, NULL, '0.00', NULL, NULL, 9, 9, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(302, 'ETK-260511-0010', 'TYPE_CODE_128', 'Microphone Podcast Set', NULL, NULL, '0.00', NULL, NULL, 9, 10, NULL, 1, 1, 14, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(303, 'ETK-260511-0011', 'TYPE_CODE_128', 'TP Link Router', NULL, NULL, '0.00', NULL, NULL, 9, 9, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(304, 'ETK-260511-0012', 'TYPE_CODE_128', 'Handy Cam + Charger + Bag', NULL, NULL, '0.00', NULL, NULL, 9, 8, NULL, 3, 3, 14, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(305, 'ETK-260511-0013', 'TYPE_CODE_128', 'Videomic Rode', NULL, NULL, '0.00', NULL, NULL, 9, 11, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(306, 'ETK-260511-0014', 'TYPE_CODE_128', 'DJI Mavic Mini (NEW)', NULL, NULL, '0.00', NULL, NULL, 9, 11, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(307, 'FCY-260511-0062', 'TYPE_CODE_128', 'Zomei Professional Tripod', NULL, NULL, '50.00', NULL, NULL, 4, 11, NULL, 2, 2, 3, 0, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(308, 'ETK-260511-0015', 'TYPE_CODE_128', 'Camcorder AVCAM Panasonic', NULL, NULL, '0.00', NULL, NULL, 9, 9, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(309, 'ETK-260511-0016', 'TYPE_CODE_128', 'Microphone Podcast', NULL, NULL, '0.00', NULL, NULL, 9, 7, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(310, 'ETK-260511-0017', 'TYPE_CODE_128', 'Godox Minimaster', NULL, NULL, '0.00', NULL, NULL, 9, 7, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(311, 'ETK-260511-0018', 'TYPE_CODE_128', 'Studio Flash (Godox)', NULL, NULL, '0.00', NULL, NULL, 9, 7, NULL, 1, 1, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(312, 'ETK-260511-0019', 'TYPE_CODE_128', 'Alctron Audio Interface', NULL, NULL, '0.00', NULL, NULL, 9, 8, NULL, 2, 2, 3, 1, 0, '2026-05-11 13:46:24', '2026-05-26 09:46:52'),
(400, 'MHE-260511-0033', 'TYPE_CODE_128', 'Baju Polo', NULL, NULL, '50.00', '/images/products/aObBuQFVHxEkNi17CtlIu3mnPT49HOnmBNMD8OFX.jpg', '[\"/images/products/aObBuQFVHxEkNi17CtlIu3mnPT49HOnmBNMD8OFX.jpg\", \"/images/products/ceuQ2P6dcE8Ro4Sd97ulhldhS8i7ub5Cv4m06sWb.jpg\"]', 6, 4, NULL, 16, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(410, 'MHE-260511-0034', 'TYPE_CODE_128', 'Celana Training', '', NULL, '50.00', '/images/products/9e3d6350-4439-4a9c-a304-9a7184184297.jpg', '[\"/images/products/9e3d6350-4439-4a9c-a304-9a7184184297.jpg\",\"/images/products/7cc0a0cc-c5e9-4b96-88a1-c4f862e03856.jpg\"]', 6, 4, NULL, 2, 1, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 01:26:47'),
(413, 'MHE-260511-0035', 'TYPE_CODE_128', 'Kemeja UT', NULL, NULL, '50.00', '/images/products/HZpVxqrwZZOE9xef7B8aYouG1q7N4Lt0XzEq33LK.jpg', '[\"/images/products/HZpVxqrwZZOE9xef7B8aYouG1q7N4Lt0XzEq33LK.jpg\"]', 6, 4, NULL, 110, 5, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(415, 'MHE-260511-0036', 'TYPE_CODE_128', 'Kaos UT', '', NULL, '50.00', '/images/products/e5a05cda-23d9-4dab-b9a6-42013007d511.jpg', '[\"/images/products/e5a05cda-23d9-4dab-b9a6-42013007d511.jpg\"]', 6, 4, NULL, 17, 3, 3, 0, 0, '2026-05-11 13:46:23', '2026-06-10 01:48:45'),
(429, 'MHE-260511-0037', 'TYPE_CODE_128', 'Merch Assessment - Tumbler Corkcilcke', NULL, NULL, '50.00', '/images/products/8jZaNApx2sjjOOF4A6sJCBwn54J1HvdX5CiqAtm1.jpg', '[\"/images/products/8jZaNApx2sjjOOF4A6sJCBwn54J1HvdX5CiqAtm1.jpg\"]', 6, 4, NULL, 4, 2, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52'),
(431, 'MHE-260511-0038', 'TYPE_CODE_128', 'RACER', NULL, NULL, '50.00', NULL, NULL, 6, 4, NULL, 86, 12, 3, 0, 0, '2026-05-11 13:46:23', '2026-05-26 09:46:52');

-- =============================================================
-- TABLE: product_variants
-- =============================================================

CREATE TABLE `product_variants` (
  `id`         bigint        NOT NULL AUTO_INCREMENT,
  `product_id` bigint        NOT NULL,
  `sku`        VARCHAR(255) NULL,
  `color`      VARCHAR(255) NULL,
  `size`       VARCHAR(255) NULL,
  `image`      VARCHAR(255) NULL,
  `stock`      int           NOT NULL DEFAULT 0,
  `created_at` DATETIME     NULL,
  `updated_at` DATETIME     NULL,
  CONSTRAINT `PK_product_variants`        PRIMARY KEY (`id`),
  CONSTRAINT `UQ_product_variants_sku`    UNIQUE      (`sku`),
  CONSTRAINT `FK_product_variants_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`)
);

CREATE INDEX `IX_product_variants_product_id` ON `product_variants` (`product_id`);

INSERT INTO `product_variants` (`id`, `product_id`, `sku`, `color`, `size`, `image`, `stock`, `created_at`, `updated_at`) VALUES
(1, 197, 'MHE-260511-0008-V01', NULL, 'XL', NULL, 2, '2026-05-26 09:46:46', '2026-05-26 09:46:51'),
(2, 197, 'MHE-260511-0008-V02', NULL, 'L', NULL, 1, '2026-05-26 09:46:46', '2026-05-26 09:46:51'),
(3, 197, 'MHE-260511-0008-V03', NULL, 'M', NULL, 2, '2026-05-26 09:46:46', '2026-05-26 09:46:51'),
(4, 213, 'MHE-260511-0014-V01', 'Hijau', 'L', '/images/products/variants/CKqH9jvgmlFJ9pUajspkwPCycgZ5RAWsMDtZh3wA.jpg', 7, '2026-05-26 09:46:46', '2026-06-10 02:15:50'),
(5, 213, 'MHE-260511-0014-V02', 'Hijau', '5XL', '/images/products/variants/R4ezf9NrTFUOKiByCEerefvoxtafP9CSXNaZVnz4.jpg', 6, '2026-05-26 09:46:46', '2026-06-10 02:15:50'),
(6, 213, 'MHE-260511-0014-V03', 'Hijau', 'M', '/images/products/variants/lmK67qH8Gj8rRQqfiNepwYBfK0kZXVv38ixzhk9S.jpg', 6, '2026-05-26 09:46:46', '2026-06-10 02:15:50'),
(7, 213, 'MHE-260511-0014-V04', 'Hijau', '4XL', '/images/products/variants/z8unAv4IE7Qer2NU7nItrl79zeGg5yU9E5Hqs8Rz.jpg', 7, '2026-05-26 09:46:46', '2026-06-10 02:15:50'),
(8, 213, 'MHE-260511-0014-V05', 'Hijau', 'XXXL', '/images/products/variants/vWEVzVHriuZ7QY2TUFiA8VBaeNbkTHxS6EdknmLt.jpg', 5, '2026-05-26 09:46:46', '2026-06-10 02:15:50'),
(9, 213, 'MHE-260511-0014-V06', 'Hijau', 'M', '/images/products/variants/accWkNPiwzwrTBFNnbV9JhDvJedYQ0QMv7q0R14O.jpg', 1, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(10, 213, 'MHE-260511-0014-V07', 'Hijau', 'L', '/images/products/variants/ozUyk2V27GVrZCyJbQMqTJfScVucQiCXJlh2U6I0.jpg', 1, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(11, 213, 'MHE-260511-0014-V08', 'Hijau', 'XXL', '/images/products/variants/ctA1Ps0Rklvg8vU3kIVPMNun4Py91WcZrD8YTxMK.jpg', 1, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(12, 213, 'MHE-260511-0014-V09', 'Hitam', '5XL', 'images/products/variants/9130e761-55c5-4fda-a252-20ca37c317cd.png', 5, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(13, 213, 'MHE-260511-0014-V10', 'Hitam', 'XXXL', 'images/products/variants/524899bb-953f-451e-9313-096f5b31e56f.png', 5, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(14, 213, 'MHE-260511-0014-V11', 'Hitam', '4XL', 'images/products/variants/334defab-4ceb-41a6-b656-834b0c1a1ec1.png', 15, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(15, 213, 'MHE-260511-0014-V12', 'Abu-abu', 'XXXL', '/images/products/variants/4tW5D7Z4FDVkOn0QhSjB1KEJCWf0oK8O6hro8Jff.jpg', 10, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(16, 213, 'MHE-260511-0014-V13', 'Abu-abu', '4XL', '/images/products/variants/XZtcUFXo34IpO1UGUzr5dtgiQI7RMUIHWV7H9mTd.jpg', 23, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(17, 213, 'MHE-260511-0014-V14', 'Abu-abu', 'XXL', '/images/products/variants/wAHFb6tQlfXSoUgkHlCePeSQGtnzVxFoywkUYUbh.jpg', 5, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(18, 213, 'MHE-260511-0014-V15', 'Abu-abu', '5XL', '/images/products/variants/AUoS5BrR8BlzQmIi0J630Bi1u2unlg3naTOV2qWk.jpg', 8, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(19, 213, 'MHE-260511-0014-V16', 'UNTR', 'XXL', 'images/products/variants/8b4db075-7a13-4c3e-9ec7-c5242f335d8c.jpg', 5, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(20, 213, 'MHE-260511-0014-V17', 'UNTR', 'L', 'images/products/variants/f70773f6-3a60-44eb-87e9-10bbd8b05ace.jpg', 1, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(21, 213, 'MHE-260511-0014-V18', 'UNTR', 'M', 'images/products/variants/17371903-45de-4855-bf50-273d8be0892f.jpg', 1, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(22, 213, 'MHE-260511-0014-V19', 'UNTR', 'XL', 'images/products/variants/db4e6e3f-8e3b-46f1-9726-98ec742bb613.jpg', 1, '2026-05-26 09:46:47', '2026-06-10 02:15:50'),
(23, 241, 'MHE-260511-0021-V01', 'Abu UT Fresh', 'XXL', '/images/products/variants/Lm94NYIwXVd6UtSVdan0jQnuoAesazmSbGu7t4LY.jpg', 5, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(24, 241, 'MHE-260511-0021-V02', 'Abu UT Fresh', 'XXXL', '/images/products/variants/EJsoqmzjRO4NbryAcxObTIOc8ppmcc7Wo0S45Nax.jpg', 27, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(25, 241, 'MHE-260511-0021-V03', 'Abu UT Fresh', 'XL', '/images/products/variants/9pnvz1snavwpql5M2tEtdWG1c5RtmeRaHpWa3opK.jpg', 50, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(26, 241, 'MHE-260511-0021-V04', 'Abu UT Fresh', '4XL', '/images/products/variants/a9Q1EjKvZ9MhH0CUeKuixQOQjeAkgR7OHgyGomzF.jpg', 2, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(27, 241, 'MHE-260511-0021-V05', 'Abu UT Fresh', '3XL', '/images/products/variants/WlWStKiBhHiIAriYTjKJOa9OCMCFhPpB2CxtgcFD.jpg', 5, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(28, 241, 'MHE-260511-0021-V06', 'Abu UT Fresh', 'M', '/images/products/variants/2vgtQ8VsNkx07mgTrsID0zQ6xLVxaoi5kc7cWo01.jpg', 12, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(29, 241, 'MHE-260511-0021-V07', 'Abu UT Fresh', 'L', '/images/products/variants/to3h2Fn9q1wVhBXk4lIhjA2EUR4aKWegzgdG7Heo.jpg', 3, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(30, 241, 'MHE-260511-0021-V08', 'Hitam UT Fresh', 'M', 'images/products/variants/f40c6b88-520d-4e1d-be17-85ede90953eb.jpg', 10, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(31, 241, 'MHE-260511-0021-V09', 'Hitam UT Fresh', 'XL', 'images/products/variants/f87c1de3-ddaf-4a00-82cc-da1b2c0036bf.jpg', 34, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(32, 241, 'MHE-260511-0021-V10', 'Hitam UT Fresh', 'XXL', 'images/products/variants/02d9ddf9-1359-4df8-8d3b-379af1193b0f.jpg', 5, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(33, 241, 'MHE-260511-0021-V11', 'Hitam UT Fresh', 'L', 'images/products/variants/6122e313-deb5-4057-91a7-2a578079cf24.jpg', 40, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(34, 241, 'MHE-260511-0021-V12', 'Hitam UT Fresh', 'S', 'images/products/variants/01c3da75-48b7-498f-8153-ac33ed77f9c0.jpg', 10, '2026-05-26 09:46:47', '2026-06-10 02:20:56'),
(35, 291, 'GME-260511-0086-V01', 'Snake & Ladder', NULL, NULL, 1, '2026-05-26 09:46:47', '2026-05-26 09:46:50'),
(36, 291, 'GME-260511-0086-V02', 'Rebound Chess', NULL, NULL, 1, '2026-05-26 09:46:47', '2026-05-26 09:46:50'),
(37, 400, 'MHE-260511-0033-V01', 'Biru Panjang ASC', 'No Size', NULL, 0, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(38, 400, 'MHE-260511-0033-V02', 'Biru Panjang ASC', 'S', NULL, 1, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(39, 400, 'MHE-260511-0033-V03', 'Biru Panjang ASC', 'M', NULL, 2, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(40, 400, 'MHE-260511-0033-V04', 'Biru Panjang ASC', 'L', NULL, 7, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(41, 400, 'MHE-260511-0033-V05', 'Biru Panjang ASC', 'XXL', NULL, 1, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(42, 400, 'MHE-260511-0033-V06', 'Biru Panjang ASC', '4XL', NULL, 1, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(43, 400, 'MHE-260511-0033-V07', 'Biru Panjang Mechanic', 'S', NULL, 4, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(44, 400, 'MHE-260511-0033-V08', 'Biru Panjang Mechanic', 'M', NULL, 1, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(45, 400, 'MHE-260511-0033-V09', 'Biru Panjang Mechanic', '4XL', NULL, 0, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(46, 410, 'MHE-260511-0034-V01', NULL, '2XL', NULL, 1, '2026-05-26 09:46:47', '2026-06-10 01:26:47'),
(47, 410, 'MHE-260511-0034-V02', NULL, '3XL', NULL, 1, '2026-05-26 09:46:47', '2026-06-10 01:26:48'),
(48, 413, 'MHE-260511-0035-V01', 'Jeans', 'XL', 'images/products/variants/fdbec916-887f-41a6-a6d0-b10dd9bd791a.png', 5, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(49, 413, 'MHE-260511-0035-V02', 'Jeans', 'XXL', 'images/products/variants/e18f7cae-f8be-4079-a04c-1546d4ed555c.png', 3, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(50, 415, 'MHE-260511-0036-V01', 'Corpu', 'M', NULL, 3, '2026-05-26 09:46:47', '2026-06-10 01:48:45'),
(51, 415, 'MHE-260511-0036-V02', 'Corpu', 'L', NULL, 4, '2026-05-26 09:46:47', '2026-06-10 01:48:45'),
(52, 415, 'MHE-260511-0036-V03', 'Corpu', 'XXL', NULL, 5, '2026-05-26 09:46:47', '2026-06-10 01:48:45'),
(53, 413, 'MHE-260511-0035-V03', 'Hitam (PU) Lengan Pendek', 'S', 'images/products/variants/adbba9d9-4fa0-4471-ba5a-6b725a32b04a.png', 2, '2026-05-26 09:46:47', '2026-06-09 00:32:38'),
(54, 413, 'MHE-260511-0035-V04', 'Hitam (PU) Lengan Pendek', 'M', 'images/products/variants/5e2e8151-ac9d-4f4d-b106-5e138bd69d92.png', 14, '2026-05-26 09:46:47', '2026-06-09 00:32:38'),
(55, 413, 'MHE-260511-0035-V05', 'Hitam (PU) Lengan Pendek', 'L', 'images/products/variants/eb36ca1b-77fa-4913-a95f-41566b62f257.png', 46, '2026-05-26 09:46:47', '2026-06-09 00:32:38'),
(56, 413, 'MHE-260511-0035-V06', 'Hitam (PU) Lengan Pendek', 'XL', 'images/products/variants/aa41cd1e-b178-4420-8b20-7c349100bc0d.png', 28, '2026-05-26 09:46:47', '2026-06-09 00:32:38'),
(57, 413, 'MHE-260511-0035-V07', 'Hitam (PU) Lengan Pendek', 'XXL', 'images/products/variants/a034806f-a5fa-4e9b-a3c1-b3bfc62dee74.png', 6, '2026-05-26 09:46:47', '2026-06-09 00:32:38'),
(58, 413, 'MHE-260511-0035-V08', 'Hitam (PU) Lengan Pendek', 'XXXL', 'images/products/variants/b29ba48f-a289-4dcb-abae-b834509495ed.png', 2, '2026-05-26 09:46:47', '2026-06-09 00:32:38'),
(59, 413, 'MHE-260511-0035-V09', 'Hitam (PU) Lengan Pendek', '4XL', 'images/products/variants/db8d3bdc-7a11-4efd-91f2-c1c4706f5f99.png', 2, '2026-05-26 09:46:47', '2026-06-09 00:32:38'),
(60, 413, 'MHE-260511-0035-V10', 'Hitam (PU) Lengan Pendek', '5XL', 'images/products/variants/0ad2c236-38b3-4f59-a7f2-1ad692aea924.png', 2, '2026-05-26 09:46:47', '2026-06-09 00:32:38'),
(61, 210, 'MHE-260511-0013-V01', 'Coklat', NULL, NULL, 2, '2026-05-26 09:46:47', '2026-05-26 09:46:51'),
(62, 210, 'MHE-260511-0013-V02', 'Ijo', NULL, NULL, 1, '2026-05-26 09:46:47', '2026-05-26 09:46:51'),
(63, 210, 'MHE-260511-0013-V03', 'Hitam', NULL, NULL, 1, '2026-05-26 09:46:47', '2026-05-26 09:46:51'),
(64, 429, 'MHE-260511-0037-V01', 'Putih', NULL, NULL, 2, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(65, 429, 'MHE-260511-0037-V02', 'Biru', NULL, NULL, 2, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(66, 431, 'MHE-260511-0038-V01', 'Lanyard', NULL, NULL, 12, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(67, 431, 'MHE-260511-0038-V02', 'ID Card', NULL, NULL, 39, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(68, 431, 'MHE-260511-0038-V03', 'Stiker', NULL, NULL, 35, '2026-05-26 09:46:47', '2026-05-26 09:46:52'),
(69, 400, 'MHE-260511-0033-V10', 'Biru Panjang Mechanic Fasilitator', '3XL', NULL, 1, '2026-05-26 09:46:48', '2026-05-26 09:46:52'),
(70, 241, 'MHE-260511-0021-V13', 'Hijau Turbo', NULL, 'images/products/variants/66ce59ec-dca5-4a95-9650-dadd2c6e9119.jpg', 2, '2026-05-26 09:46:48', '2026-06-10 02:20:56'),
(71, 415, 'MHE-260511-0036-V04', 'Solution PSDH', 'L', 'images/products/variants/d1e43b27-9273-486f-addb-752aa0f5149b.jpg', 4, '2026-05-26 09:46:48', '2026-06-10 01:48:45'),
(72, 415, 'MHE-260511-0036-V05', 'Moving As One Hitam', NULL, 'images/products/variants/50830a85-5ef9-41d6-a834-fde87ead6526.png', 1, '2026-05-26 09:46:48', '2026-06-10 01:48:45'),
(73, 101, 'FCY-260511-0023-V01', 'Hitam Polos Besar', NULL, NULL, 4, '2026-05-26 09:46:48', '2026-05-26 09:46:50'),
(74, 101, 'FCY-260511-0023-V02', 'Hitam Polos', NULL, NULL, 4, '2026-05-26 09:46:48', '2026-05-26 09:46:50'),
(75, 101, 'FCY-260511-0023-V03', 'Hitam Polos 21x30', NULL, NULL, 8, '2026-05-26 09:46:48', '2026-05-26 09:46:50'),
(76, 101, 'FCY-260511-0023-V04', 'Abu-abu Polos', NULL, NULL, 5, '2026-05-26 09:46:48', '2026-05-26 09:46:50'),
(77, 101, 'FCY-260511-0023-V05', 'Putih Polos 21x30', NULL, NULL, 14, '2026-05-26 09:46:48', '2026-05-26 09:46:50'),
(78, 101, 'FCY-260511-0023-V06', 'Dengan Isi Konten', NULL, NULL, 1, '2026-05-26 09:46:48', '2026-05-26 09:46:50'),
(79, 101, 'FCY-260511-0023-V07', 'Best Photo', NULL, NULL, 1, '2026-05-26 09:46:48', '2026-05-26 09:46:50'),
(80, 210, 'MHE-260511-0013-V04', 'Besar Coklat', NULL, NULL, 4, '2026-05-26 09:46:48', '2026-05-26 09:46:51'),
(81, 183, 'FCY-260511-0050-V01', 'Plastik Polkadot', NULL, 'images/products/variants/88f68ee8-139c-4123-bd9c-24b64558ad24.jpg', 20, '2026-05-26 09:46:48', '2026-06-08 20:16:48'),
(82, 183, 'FCY-260511-0050-V02', 'Kertas Warna', NULL, 'images/products/variants/d401c9ee-ba07-44c9-9178-6588cc510528.jpg', 78, '2026-05-26 09:46:48', '2026-06-08 20:16:48'),
(83, 185, 'FCY-260511-0051-V01', 'Jahit', NULL, 'images/products/variants/f28d398b-3519-4f93-814a-933ecfd840e4.jpg', 5, '2026-05-26 09:46:48', '2026-06-08 20:07:55'),
(84, 185, 'FCY-260511-0051-V02', 'Kasur', NULL, NULL, 6, '2026-05-26 09:46:48', '2026-05-26 09:46:51'),
(85, 228, 'MHE-260511-0015-V01', 'Stainless', NULL, NULL, 66, '2026-05-26 09:46:48', '2026-05-26 09:46:51'),
(86, 257, 'MHE-260511-0023-V01', 'Coklat', NULL, NULL, 97, '2026-05-26 09:46:48', '2026-06-10 01:42:01'),
(87, 257, 'MHE-260511-0023-V02', 'Kuning', NULL, NULL, 62, '2026-05-26 09:46:48', '2026-06-10 01:42:01'),
(88, 263, 'MHE-260511-0024-V01', 'UT Smart', NULL, NULL, 508, '2026-05-26 09:46:48', '2026-06-10 01:42:46'),
(89, 263, 'MHE-260511-0024-V02', 'Hitam Polos', NULL, NULL, 55, '2026-05-26 09:46:48', '2026-06-10 01:42:46'),
(90, 253, 'MHE-260511-0022-V01', 'Pink', NULL, NULL, 68, '2026-05-26 09:46:49', '2026-06-10 01:43:15'),
(91, 253, 'MHE-260511-0022-V02', 'Ungu', NULL, NULL, 68, '2026-05-26 09:46:49', '2026-06-10 01:43:15'),
(92, 253, 'MHE-260511-0022-V03', 'Hijau', NULL, NULL, 57, '2026-05-26 09:46:49', '2026-06-10 01:43:15'),
(93, 253, 'MHE-260511-0022-V04', 'Coklat', NULL, NULL, 73, '2026-05-26 09:46:49', '2026-06-10 01:43:15'),
(94, 253, 'MHE-260511-0022-V05', 'UT Smart (Besi) Kuning', NULL, NULL, 40, '2026-05-26 09:46:49', '2026-06-10 01:43:15'),
(95, 253, 'MHE-260511-0022-V06', 'Besi', NULL, NULL, 401, '2026-05-26 09:46:49', '2026-06-10 01:43:15'),
(96, 253, 'MHE-260511-0022-V07', 'Hitam UT Smart', NULL, NULL, 10, '2026-05-26 09:46:49', '2026-06-10 01:43:15'),
(97, 253, 'MHE-260511-0022-V08', 'UT Smart - Warna Campur', NULL, NULL, 51, '2026-05-26 09:46:49', '2026-06-10 01:43:15'),
(98, 176, 'MHE-260511-0007-V01', 'Hitam', NULL, 'images/products/variants/3cb2a09d-6b61-45b4-bbfc-dd0440155548.png', 30, '2026-05-26 09:46:49', '2026-06-09 00:27:16'),
(99, 176, 'MHE-260511-0007-V02', 'Biru', NULL, 'images/products/variants/8eae815d-c691-4a09-a344-d6514025cded.png', 106, '2026-05-26 09:46:49', '2026-06-09 00:27:16'),
(100, 176, 'MHE-260511-0007-V03', 'Kuning', NULL, 'images/products/variants/797fe61d-2c36-4fc9-b459-10c0c8b22251.png', 60, '2026-05-26 09:46:49', '2026-06-09 00:27:16'),
(101, 176, 'MHE-260511-0007-V04', 'Pink', NULL, 'images/products/variants/0b5612f9-63b9-42d8-beb7-bc94437559b7.png', 83, '2026-05-26 09:46:49', '2026-06-09 00:27:16'),
(102, 189, 'FCY-260511-0054-V01', 'tosca', NULL, NULL, 1, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(103, 189, 'FCY-260511-0054-V02', 'putih', NULL, NULL, 2, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(104, 189, 'FCY-260511-0054-V03', 'merah', NULL, NULL, 1, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(105, 189, 'FCY-260511-0054-V04', 'hitam coklat', NULL, NULL, 1, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(106, 229, 'MHE-260511-0016-V01', 'Cream', NULL, NULL, 164, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(107, 229, 'MHE-260511-0016-V02', 'Hijau', NULL, NULL, 203, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(108, 265, 'MHE-260511-0025-V01', '1 (Gift Set IF Grade 1)', NULL, NULL, 18, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(109, 265, 'MHE-260511-0025-V02', '2 (Gift Set IF Grade 2)', NULL, NULL, 21, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(110, 265, 'MHE-260511-0025-V03', '3 (Gift Set IF Grade 2)', NULL, NULL, 21, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(111, 265, 'MHE-260511-0025-V04', '4', NULL, NULL, 24, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(112, 265, 'MHE-260511-0025-V05', '5', NULL, NULL, 24, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(113, 265, 'MHE-260511-0025-V06', '6', NULL, NULL, 24, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(114, 265, 'MHE-260511-0025-V07', '7', NULL, NULL, 23, '2026-05-26 09:46:49', '2026-05-26 09:46:51'),
(115, 273, 'MHE-260511-0027-V01', '1', NULL, NULL, 21, '2026-05-26 09:46:49', '2026-05-26 09:46:52'),
(116, 273, 'MHE-260511-0027-V02', '2', NULL, NULL, 21, '2026-05-26 09:46:49', '2026-05-26 09:46:52'),
(117, 273, 'MHE-260511-0027-V03', '3', NULL, NULL, 21, '2026-05-26 09:46:49', '2026-05-26 09:46:52'),
(118, 273, 'MHE-260511-0027-V04', '4', NULL, NULL, 20, '2026-05-26 09:46:49', '2026-05-26 09:46:52'),
(119, 273, 'MHE-260511-0027-V05', '5', NULL, NULL, 11, '2026-05-26 09:46:49', '2026-05-26 09:46:52'),
(120, 280, 'MHE-260511-0030-V01', 'Giftbox Notebook PB + Pen - Dus 1', NULL, NULL, 20, '2026-05-26 09:46:49', '2026-05-26 09:46:52'),
(121, 280, 'MHE-260511-0030-V02', 'Giftbox Notebook PB + Pen - Dus 2', NULL, NULL, 15, '2026-05-26 09:46:49', '2026-05-26 09:46:52'),
(122, 283, 'MHE-260511-0032-V01', 'Putih', NULL, NULL, 10, '2026-05-26 09:46:49', '2026-05-26 09:46:52'),
(123, 283, 'MHE-260511-0032-V02', 'Black', NULL, NULL, 10, '2026-05-26 09:46:49', '2026-05-26 09:46:52'),
(124, 180, 'FCY-260511-0049-V01', 'Standard/Default', NULL, 'images/products/variants/9e3c1400-cc43-497b-b95d-e634614b36bb.jpg', 2, '2026-05-26 09:46:49', '2026-06-08 19:13:43'),
(125, 180, 'FCY-260511-0049-V02', 'warna warni', NULL, 'images/products/variants/87e90818-ee12-44e3-979d-3f8033736782.jpg', 14, '2026-05-26 09:46:49', '2026-06-08 19:13:43'),
(126, 180, 'FCY-260511-0049-V03', 'bening', NULL, 'images/products/variants/32a324a5-4b71-470e-a95e-9d19ac9b48e2.jpg', 53, '2026-05-26 09:46:49', '2026-06-08 19:13:43');

-- =============================================================
-- TABLE: users
-- =============================================================

CREATE TABLE `users` (
  `id`                bigint        NOT NULL AUTO_INCREMENT,
  `name`              VARCHAR(255) NOT NULL,
  `nrp`               VARCHAR(255) NULL,
  `email`             VARCHAR(255) NOT NULL,
  `poin`              int           NOT NULL DEFAULT 0,
  `email_verified_at` DATETIME     NULL,
  `password`          VARCHAR(255) NOT NULL,
  `role`              VARCHAR(10)  NULL DEFAULT 'staff',
  `division_id`       bigint        NULL,
  `remember_token`    VARCHAR(100) NULL,
  `created_at`        DATETIME     NULL,
  `updated_at`        DATETIME     NULL,
  CONSTRAINT `PK_users`                PRIMARY KEY (`id`),
  CONSTRAINT `UQ_users_email`          UNIQUE      (`email`),
  CONSTRAINT `UQ_users_nrp`            UNIQUE      (`nrp`),
  CONSTRAINT `CK_users_role`           CHECK       (`role` IN ('superadmin','admin','manager','staff')),
  CONSTRAINT `FK_users_division_id`    FOREIGN KEY (`division_id`) REFERENCES `divisions` (`id`)
);

CREATE INDEX `IX_users_division_id` ON `users` (`division_id`);


INSERT INTO `users` (`id`,`name`,`nrp`,`email`,`poin`,`email_verified_at`,`password`,`role`,`division_id`,`remember_token`,`created_at`,`updated_at`) VALUES
  (1,'Admin User',        '73216958','admin@wms.com',     1000,'2026-05-11 11:30:04','$2y$12$6frRu6KhnRFsPL8sXW7ls.4QQk9rtkvkgxoFYp/tqp9/5zZHcEE.m','admin',      3,'tupohEJ7W6UHk61RcHxSCWsrMtF71Ut35MCUqFMMNMf3zv6Ykmtieo1jPEV6','2026-05-11 11:30:04','2026-05-26 09:46:53'),
  (2,'Manager User',      '44920801','manager@wms.com',   1000,'2026-05-11 11:30:04','$2y$12$JCmEGC2gvkTljXwVWKizbuxsCoVxVOHmIdAtS7KwohAJAa7IeGuGK', 'manager',    3,'qxl2W9eeqk',                                                   '2026-05-11 11:30:04','2026-05-26 09:46:53'),
  (3,'Staff User',        '44727320','staff@wms.com',     1000,'2026-05-11 11:30:05','$2y$12$n6yveeCHq3V/57rF7HbMyOAzuCa7WIXOkkQkBjkxT0B77Vaoa1Oy.','staff',      3,'ljf4NYEGgb',                                                   '2026-05-11 11:30:05','2026-05-26 09:46:53'),
  (4,'Super Administrator','35610885','superadmin@wms.com', 1000,'2026-05-11 11:30:05','$2y$12$X2dRJ8Lk7hvYkgFj1MdVUe3QlpWWMxIs3PaSJV0M4oUuXz4H5rBS.','superadmin', 3,NULL,                                                           '2026-05-26 09:46:53','2026-05-26 09:46:53');


-- =============================================================
-- TABLE: profile_requests
-- =============================================================

CREATE TABLE `profile_requests` (
  `id`          bigint        NOT NULL AUTO_INCREMENT,
  `user_id`     bigint        NOT NULL,
  `name`        VARCHAR(255) NOT NULL,
  `nrp`         VARCHAR(255) NOT NULL,
  `email`       VARCHAR(255) NOT NULL,
  `division_id` bigint        NULL,
  `status`      VARCHAR(50)  NOT NULL DEFAULT 'PENDING',
  `created_at`  DATETIME     NULL,
  `updated_at`  DATETIME     NULL,
  CONSTRAINT `PK_profile_requests`              PRIMARY KEY (`id`),
  CONSTRAINT `FK_profile_requests_user_id`      FOREIGN KEY (`user_id`)     REFERENCES `users`     (`id`),
  CONSTRAINT `FK_profile_requests_division_id`  FOREIGN KEY (`division_id`) REFERENCES `divisions` (`id`)
);

CREATE INDEX `IX_profile_requests_user_id`     ON `profile_requests` (`user_id`);
CREATE INDEX `IX_profile_requests_division_id` ON `profile_requests` (`division_id`);

-- (no seed data)

-- =============================================================
-- TABLE: transactions
-- =============================================================

CREATE TABLE `transactions` (
  `id`                     bigint        NOT NULL AUTO_INCREMENT,
  `product_id`             bigint        NOT NULL,
  `product_variant_id`     bigint        NULL,
  `type`                   VARCHAR(3)   NOT NULL,
  `request_type`           VARCHAR(8)   NOT NULL DEFAULT 'BORROW',
  `quantity`               int           NOT NULL,
  `returned_quantity`      int           NOT NULL DEFAULT 0,
  `pending_return_quantity`int           NOT NULL DEFAULT 0,
  `returned_at`            DATETIME     NULL,
  `return_photo`           VARCHAR(255) NULL,
  `handover_photo`         TEXT NULL,
  `handover_notes`         TEXT NULL,
  `handover_recipient_name`VARCHAR(255) NULL,
  `handover_timestamp`     DATETIME     NULL,
  `documentation_photo`    TEXT NULL,
  `documentation_notes`    TEXT NULL,
  `documentation_uploaded_at` DATETIME  NULL,
  `return_status`          VARCHAR(255) NULL,
  `return_reason`          TEXT NULL,
  `is_return_draft`        TINYINT(1)           NOT NULL DEFAULT 0,
  `return_condition`       VARCHAR(6)   NULL,
  `status`                 VARCHAR(50)  NOT NULL DEFAULT 'PENDING',
  `rejection_reason`       TEXT NULL,
  `admin_notes`            TEXT NULL,
  `manager_notes`          TEXT NULL,
  `staff_inventory_notes`  VARCHAR(500) NULL,
  `last_revision_stage`    VARCHAR(50)  NULL,
  `return_rejection_reason`TEXT NULL,
  `requester_id`           bigint        NOT NULL,
  `approver_id`            bigint        NULL,
  `notes`                  TEXT NULL,
  `used_by`                VARCHAR(255) NULL,
  `division_id`            bigint        NULL,
  `created_at`             DATETIME     NULL,
  `updated_at`             DATETIME     NULL,
  `applicant_name`         VARCHAR(255) NULL,
  `applicant_nrp`          VARCHAR(255) NULL,
  `borrow_duration_days`   int           NULL,
  `borrow_start_date`      date          NULL,
  `pickup_date`            date          NULL,
  `expected_return_date`   date          NULL,
  `event_name`             VARCHAR(255) NULL,
  `event_date`             date          NULL,
  `documentation_link`     VARCHAR(255) NULL,
  CONSTRAINT `PK_transactions`                       PRIMARY KEY (`id`),
  CONSTRAINT `CK_transactions_type`                  CHECK       (`type`             IN ('IN','OUT')),
  CONSTRAINT `CK_transactions_request_type`          CHECK       (`request_type`     IN ('BORROW','GIVEAWAY')),
  CONSTRAINT `CK_transactions_return_condition`      CHECK       (`return_condition`  IN ('BAIK','RUSAK','HILANG')),
  CONSTRAINT `CK_transactions_status`                CHECK       (`status`           IN ('PENDING','PENDING_STAFF_INVENTORY','PENDING_ADMIN','PENDING_MANAGER','WAITING_HANDOVER','WAITING_ADMIN_HANDOVER','WAITING_DOCUMENTATION','DOCUMENTATION_OVERDUE','APPROVED','COMPLETED','REJECTED','REVISION','REVISION_BY_STAFF_INVENTORY','REVISION_BY_ADMIN','REVISION_BY_MANAGER')),
  CONSTRAINT `FK_transactions_product_id`            FOREIGN KEY (`product_id`)         REFERENCES `products`         (`id`),
  CONSTRAINT `FK_transactions_product_variant_id`    FOREIGN KEY (`product_variant_id`) REFERENCES `product_variants` (`id`),
  CONSTRAINT `FK_transactions_requester_id`          FOREIGN KEY (`requester_id`)       REFERENCES `users`            (`id`),
  CONSTRAINT `FK_transactions_approver_id`           FOREIGN KEY (`approver_id`)        REFERENCES `users`            (`id`),
  CONSTRAINT `FK_transactions_division_id`           FOREIGN KEY (`division_id`)        REFERENCES `divisions`        (`id`)
);

CREATE INDEX `IX_transactions_product_id`         ON `transactions` (`product_id`);
CREATE INDEX `IX_transactions_requester_id`       ON `transactions` (`requester_id`);
CREATE INDEX `IX_transactions_approver_id`        ON `transactions` (`approver_id`);
CREATE INDEX `IX_transactions_division_id`        ON `transactions` (`division_id`);
CREATE INDEX `IX_transactions_product_variant_id` ON `transactions` (`product_variant_id`);


INSERT INTO `transactions` (`id`,`product_id`,`product_variant_id`,`type`,`request_type`,`quantity`,`returned_quantity`,`pending_return_quantity`,`returned_at`,`return_photo`,`handover_photo`,`handover_notes`,`handover_recipient_name`,`handover_timestamp`,`documentation_photo`,`documentation_notes`,`documentation_uploaded_at`,`return_status`,`return_reason`,`is_return_draft`,`return_condition`,`status`,`rejection_reason`,`admin_notes`,`manager_notes`,`staff_inventory_notes`,`last_revision_stage`,`return_rejection_reason`,`requester_id`,`approver_id`,`notes`,`used_by`,`division_id`,`created_at`,`updated_at`,`applicant_name`,`applicant_nrp`,`borrow_duration_days`,`borrow_start_date`,`pickup_date`,`expected_return_date`,`event_name`,`event_date`,`documentation_link`) VALUES
  (1,1,NULL,'IN','BORROW',5,0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'APPROVED',NULL,NULL,NULL,NULL,NULL,NULL,1,1,NULL,NULL,NULL,'2026-05-11 12:16:11','2026-05-11 12:16:20',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),
  (2,2,NULL,'IN','BORROW',50,0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'APPROVED',NULL,NULL,NULL,NULL,NULL,NULL,1,1,NULL,NULL,NULL,'2026-05-11 12:19:00','2026-05-11 12:20:54',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),
  (3,2,NULL,'IN','BORROW',10,0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'APPROVED',NULL,NULL,NULL,NULL,NULL,NULL,1,1,NULL,NULL,NULL,'2026-05-11 12:20:13','2026-05-11 12:21:00',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),
  (4,2,NULL,'IN','BORROW',3,0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'APPROVED',NULL,NULL,NULL,NULL,NULL,NULL,1,1,NULL,NULL,NULL,'2026-05-11 12:20:41','2026-05-11 12:20:57',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),
  (5,294,NULL,'OUT','BORROW',1,1,0,'2026-06-04 01:09:38','images/return/48e38a67-f012-4238-870d-7320c7e2b043.jpg',NULL,NULL,NULL,NULL,NULL,NULL,NULL,'baik',NULL,0,NULL,'APPROVED',NULL,'halo boleh ya',NULL,NULL,NULL,NULL,3,4,'ok','Staff User',3,'2026-06-02 01:10:48','2026-06-04 01:09:38','Staff User','44939834',NULL,'2026-06-11',NULL,'2026-06-11','Testing event','2026-06-16',NULL),
  (6,50,NULL,'OUT','BORROW',1,1,0,'2026-06-04 01:20:18','images/return/06ccdaca-6200-4016-9933-4973629acf8b.jpeg',NULL,NULL,NULL,NULL,NULL,NULL,NULL,'baik',NULL,0,NULL,'APPROVED',NULL,NULL,NULL,NULL,NULL,NULL,3,4,'oko','Staff User',3,'2026-06-02 01:23:02','2026-06-04 01:20:18','Staff User','44939834',NULL,'2026-06-16',NULL,'2026-06-16','Testing event','2026-06-16',NULL),
  (7,152,NULL,'OUT','BORROW',1,0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'PENDING_MANAGER',NULL,NULL,NULL,NULL,NULL,NULL,3,NULL,'ok','Staff User',3,'2026-06-02 01:37:30','2026-06-02 01:55:55','Staff User','44939834',NULL,'2026-06-02',NULL,'2026-06-02','Testing event','2026-06-02',NULL),
  (8,231,NULL,'OUT','BORROW',1,1,0,'2026-06-04 01:20:13','images/return/4f5ce0b3-d253-4863-ab05-14355afe9b59.jpg',NULL,NULL,NULL,NULL,NULL,NULL,NULL,'baik',NULL,0,NULL,'APPROVED',NULL,NULL,NULL,NULL,NULL,NULL,3,1,'ok','Staff User',3,'2026-06-02 01:49:16','2026-06-04 01:20:13','Staff User','44939834',NULL,'2026-06-02',NULL,'2026-06-02','Testing event','2026-06-02',NULL),
  (9,400,NULL,'OUT','GIVEAWAY',1,0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'APPROVED',NULL,NULL,NULL,NULL,NULL,NULL,3,1,'ok','Staff User',3,'2026-06-02 01:59:40','2026-06-02 02:00:10','Staff User','44939834',NULL,NULL,NULL,NULL,'Testing event','2026-06-02',NULL),
  (10,400,NULL,'OUT','GIVEAWAY',1,0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'APPROVED',NULL,NULL,NULL,NULL,NULL,NULL,3,2,NULL,'Staff User',3,'2026-06-03 07:32:02','2026-06-03 07:49:02','Staff User','44939834',NULL,NULL,NULL,NULL,'testing','2026-06-04',NULL),
  (11,294,NULL,'OUT','BORROW',1,1,0,'2026-06-03 07:58:04','returns/jKCEN7viU0YJiSSP0h7KTcxRm9d4uJX7fUaH4ylR.jpg',NULL,NULL,NULL,NULL,NULL,NULL,NULL,'baik',NULL,0,NULL,'APPROVED',NULL,NULL,NULL,NULL,NULL,NULL,3,4,NULL,'Staff User',3,'2026-06-03 07:32:02','2026-06-03 07:58:04','Staff User','44939834',NULL,'2026-06-03',NULL,'2026-06-05','testing','2026-06-04',NULL),
  (12,72,NULL,'OUT','BORROW',1,1,0,'2026-06-04 01:09:20','images/return/5d2d84b7-b14f-405c-9cda-f3d4b7aeab36.jpg',NULL,NULL,NULL,NULL,NULL,NULL,NULL,'baik',NULL,0,NULL,'APPROVED',NULL,'bolehhhh',NULL,NULL,NULL,NULL,4,4,'lklm','Super Administrator',3,'2026-06-04 00:40:37','2026-06-04 01:09:20','Super Administrator','35610885',1,'2026-06-04',NULL,'2026-06-05','cghgjgjk','2026-06-04',NULL),
  (13,72,NULL,'OUT','BORROW',2,2,0,'2026-06-04 00:48:09','forced-by-admin',NULL,NULL,NULL,NULL,NULL,NULL,NULL,'baik',NULL,0,NULL,'APPROVED',NULL,'',NULL,NULL,NULL,NULL,4,4,'pinjem ya','Super Administrator',3,'2026-06-04 00:47:06','2026-06-04 00:48:09','Super Administrator','35610885',1,'2026-06-04',NULL,'2026-06-05','minjam','2026-06-04',NULL),
  (14,400,37,'OUT','GIVEAWAY',1,0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'REJECTED','ga cukup rek','ok','ga cukup rek',NULL,NULL,NULL,4,2,'yyyyy','Super Administrator',3,'2026-06-04 00:48:32','2026-06-04 00:54:19','Super Administrator','35610885',0,NULL,'2026-06-04',NULL,'iujknkkjjnn','2026-06-04',NULL),
  (15,400,44,'OUT','GIVEAWAY',3,0,0,'2026-06-04 01:08:52',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'APPROVED',NULL,NULL,'Ya boleh kok',NULL,NULL,NULL,4,2,'Mau bos boleh ga?','Super Administrator',3,'2026-06-04 01:08:13','2026-06-04 01:08:52','Super Administrator','35610885',0,NULL,'2026-06-04',NULL,'Mau','2026-06-04',NULL),
  (17,400,38,'OUT','GIVEAWAY',1,0,0,'2026-06-04 01:58:23',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'APPROVED',NULL,NULL,NULL,NULL,NULL,NULL,4,2,NULL,'Super Administrator',3,'2026-06-04 01:51:29','2026-06-04 01:58:23','Super Administrator','35610885',0,NULL,'2026-06-04',NULL,'asdadsad','2026-06-04',NULL),
  (18,400,40,'OUT','GIVEAWAY',1,0,0,'2026-06-04 02:08:15',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'APPROVED',NULL,NULL,NULL,NULL,NULL,NULL,4,4,'asas','Super Administrator',3,'2026-06-04 02:05:07','2026-06-04 02:08:15','Super Administrator','35610885',0,NULL,'2026-06-04',NULL,'ASAASA','2026-06-04',NULL),
  (19,400,45,'OUT','GIVEAWAY',1,0,0,'2026-06-04 17:35:56',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'APPROVED',NULL,'',NULL,NULL,NULL,NULL,4,4,NULL,'Super Administrator',3,'2026-06-04 17:35:47','2026-06-04 17:35:56','Super Administrator','35610885',0,NULL,'2026-06-05',NULL,'Yup','2026-06-06',NULL),
  (20,124,NULL,'OUT','BORROW',1,0,0,NULL,NULL,'images/handover/172e34c7-20cb-4385-b2d9-55192ed41b7f.jpg',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,'APPROVED',NULL,'',NULL,NULL,NULL,NULL,4,4,'yaya','Super Administrator',3,'2026-06-04 18:59:02','2026-06-04 19:00:04','Super Administrator','35610885',1,'2026-06-05',NULL,'2026-06-06','Program','2026-06-06',NULL);


-- =============================================================
-- TABLE: stock_logs
-- =============================================================

CREATE TABLE `stock_logs` (
  `id`             bigint    NOT NULL AUTO_INCREMENT,
  `transaction_id` bigint    NOT NULL,
  `product_id`     bigint    NOT NULL,
  `stock_before`   int       NOT NULL,
  `stock_after`    int       NOT NULL,
  `created_at`     DATETIME NULL,
  `updated_at`     DATETIME NULL,
  CONSTRAINT `PK_stock_logs`                  PRIMARY KEY (`id`),
  CONSTRAINT `FK_stock_logs_transaction_id`   FOREIGN KEY (`transaction_id`) REFERENCES `transactions` (`id`),
  CONSTRAINT `FK_stock_logs_product_id`       FOREIGN KEY (`product_id`)     REFERENCES `products`     (`id`)
);

CREATE INDEX `IX_stock_logs_transaction_id` ON `stock_logs` (`transaction_id`);
CREATE INDEX `IX_stock_logs_product_id`     ON `stock_logs` (`product_id`);


INSERT INTO `stock_logs` (`id`,`transaction_id`,`product_id`,`stock_before`,`stock_after`,`created_at`,`updated_at`) VALUES
  ( 1, 5, 294, 2, 1,'2026-06-04 00:35:39','2026-06-04 00:35:39'),
  ( 2,12,  72, 5, 4,'2026-06-04 00:40:48','2026-06-04 00:40:48'),
  ( 3,13,  72, 4, 2,'2026-06-04 00:47:59','2026-06-04 00:47:59'),
  ( 4,13,  72, 2, 4,'2026-06-04 00:48:09','2026-06-04 00:48:09'),
  ( 5,14, 400,27,26,'2026-06-04 00:48:41','2026-06-04 00:48:41'),
  ( 6,15, 400,26,23,'2026-06-04 01:08:22','2026-06-04 01:08:22'),
  ( 7,15, 400,23,20,'2026-06-04 01:08:52','2026-06-04 01:08:52'),
  ( 8,12,  72, 4, 5,'2026-06-04 01:09:20','2026-06-04 01:09:20'),
  ( 9, 5, 294, 1, 2,'2026-06-04 01:09:38','2026-06-04 01:09:38'),
  (10, 8, 231,25,26,'2026-06-04 01:20:13','2026-06-04 01:20:13'),
  (11, 6,  50,41,42,'2026-06-04 01:20:18','2026-06-04 01:20:18'),
  (13,17, 400,20,19,'2026-06-04 01:52:03','2026-06-04 01:52:03'),
  (14,17, 400,19,18,'2026-06-04 01:58:22','2026-06-04 01:58:22'),
  (15,18, 400,18,17,'2026-06-04 02:08:15','2026-06-04 02:08:15'),
  (16,19, 400,17,16,'2026-06-04 17:35:56','2026-06-04 17:35:56'),
  (17,20, 124, 1, 0,'2026-06-04 18:59:29','2026-06-04 18:59:29');


-- =============================================================
-- Normalisasi produk berdasarkan kategori
--   Merchandise, ATK, Makanan, Facility → giveaway, value=50, is_returnable=0
--   Alat Musik, Elektronik, Game        → pinjam, value=0, is_returnable=1
-- =============================================================

UPDATE `products` p
JOIN `categories` c ON c.`id` = p.`category_id`
SET p.`is_returnable` = 0, p.`value` = 50
WHERE c.`name` IN ('Merchandise','ATK','Makanan','Facility');

UPDATE `products` p
JOIN `categories` c ON c.`id` = p.`category_id`
SET p.`is_returnable` = 1, p.`value` = 0
WHERE c.`name` IN ('Alat Musik','Elektronik','Game');

-- =============================================================


-- =============================================================
-- TABLE: admin_roles and user_admin_roles
-- =============================================================

CREATE TABLE `admin_roles` (
  `Id` char(36) COLLATE ascii_general_ci NOT NULL,
  `RoleName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
  `Permissions` text CHARACTER SET utf8mb4 NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
  `UpdatedAt` datetime(6) NOT NULL,
  `UpdatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
  CONSTRAINT `PK_admin_roles` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

INSERT INTO `admin_roles` (`Id`, `RoleName`, `Description`, `IsActive`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) VALUES
('063e920c-626c-4fa8-a8b9-35900bb7b7b7', 'Staff Inventoris', 'Default role for Staff Inventoris', 1, '2026-06-09 03:57:53.286081', 'System', '2026-06-09 03:57:53.286081', 'System'),
('6969448e-153b-43fc-8be0-1dda5927a8a1', 'Team Leader Infrastructure', 'Default role for Team Leader Infrastructure', 1, '2026-06-09 03:57:53.272759', 'System', '2026-06-09 03:57:53.272760', 'System'),
('d941584b-880a-4d31-aa4c-ca04a29a43d4', 'Manager', 'Default role for Manager', 1, '2026-06-09 03:57:53.280862', 'System', '2026-06-09 03:57:53.280862', 'System'),
('ec5f41d3-cec6-4d33-a220-81a18c9e1cfd', 'PIC Studio', 'Default role for PIC Studio', 1, '2026-06-09 03:57:53.187149', 'System', '2026-06-09 03:57:53.187244', 'System');

CREATE TABLE `user_admin_roles` (
  `Id` char(36) COLLATE ascii_general_ci NOT NULL,
  `UserId` bigint(20) NOT NULL,
  `AdminRoleId` char(36) COLLATE ascii_general_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
  `CategoryId` bigint NULL,
  CONSTRAINT `PK_user_admin_roles` PRIMARY KEY (`Id`),
  CONSTRAINT `FK_user_admin_roles_admin_roles_AdminRoleId` FOREIGN KEY (`AdminRoleId`) REFERENCES `admin_roles` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_user_admin_roles_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_user_admin_roles_categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `categories` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_user_admin_roles_AdminRoleId` ON `user_admin_roles` (`AdminRoleId`);
CREATE INDEX `IX_user_admin_roles_UserId` ON `user_admin_roles` (`UserId`);
CREATE INDEX `IX_user_admin_roles_CategoryId` ON `user_admin_roles` (`CategoryId`);
