-- =============================================================
-- WMS United Tractors â€” Database Seeder (T-SQL / SQL Server)
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

-- (no seed data)

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
  (1 'Makanan'    NULL '2026-05-11 12:17:24' '2026-05-11 12:17:24'),
  (3 'Game'       NULL '2026-05-11 13:40:10' '2026-05-11 13:40:10'),
  (4 'Facility'   NULL '2026-05-11 13:40:10' '2026-05-11 13:40:10'),
  (5 'ATK'        NULL '2026-05-11 13:40:10' '2026-05-11 13:40:10'),
  (6 'Merchandise'NULL '2026-05-11 13:40:10' '2026-05-11 13:40:10'),
  (8 'Alat Musik' NULL '2026-05-11 13:40:11' '2026-05-11 13:40:11');


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
  (1'CCS' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (2'CFA' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (3'CHCU'NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (4'CRA' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (5'CST' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (6'DAD' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (7'GLG' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (8'MKT' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (9'PIN' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (10'PRT' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (11'SOD' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (12'SVC' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (13'TMO' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (14'TSO' NULL'2026-05-26 09:46:46''2026-05-26 09:46:46');


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
  (1'Gudang'      'Gudang lt 1''2026-05-11 12:57:18''2026-05-11 12:57:18'),
  (2'Storage Room'NULL         '2026-05-11 13:40:10''2026-05-11 13:40:10'),
  (3'ATK'         NULL         '2026-05-11 13:40:10''2026-05-11 13:40:10'),
  (4'Makeup Room' NULL         '2026-05-11 13:40:10''2026-05-11 13:40:10'),
  (5'Merchandise' NULL         '2026-05-11 13:40:11''2026-05-11 13:40:11'),
  (6'7.1.12.2'    NULL         '2026-05-11 13:40:11''2026-05-11 13:40:11'),
  (7'7.1.12.3'    NULL         '2026-05-11 13:40:11''2026-05-11 13:40:11'),
  (8'7.1.11.1'    NULL         '2026-05-11 13:40:11''2026-05-11 13:40:11'),
  (9'7.1.11.2'    NULL         '2026-05-11 13:40:11''2026-05-11 13:40:11'),
  (10'7.1.11.3'    NULL         '2026-05-11 13:40:11''2026-05-11 13:40:11'),
  (11'7.1.11.5'    NULL         '2026-05-11 13:40:11''2026-05-11 13:40:11');


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
  (1'Mix (4 Bungkus Kecil, 1 Pack Besar, 3 Roll Bekas)''2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (2'Pack Jaring'                                       '2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (3'Pcs'                                               '2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (4'2M X 1M'                                           '2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (5'Pasang'                                            '2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (6'Bungkus'                                           '2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (7'Pack'                                              '2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (8'Sachet'                                            '2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (9'Sachet Kecil'                                      '2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (10'Box'                                               '2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (11'Lembar'                                            '2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (12'Buah'                                              '2026-05-26 09:46:41''2026-05-26 09:46:41'),
  (13'Toples'                                            '2026-05-26 09:46:42''2026-05-26 09:46:42'),
  (14'Set'                                               '2026-05-26 09:46:42''2026-05-26 09:46:42'),
  (15'7 Pack (Warna-warni), 1 Pack Besar (Biru), 14 Roll''2026-05-26 09:46:42''2026-05-26 09:46:42'),
  (16'Bundle'                                            '2026-05-26 09:46:42''2026-05-26 09:46:42'),
  (17'Map'                                               '2026-05-26 09:46:42''2026-05-26 09:46:42'),
  (18'Ikat'                                              '2026-05-26 09:46:42''2026-05-26 09:46:42'),
  (19'Plastik'                                           '2026-05-26 09:46:42''2026-05-26 09:46:42'),
  (20'Unit'                                              '2026-05-26 09:46:42''2026-05-26 09:46:42'),
  (21'Board'                                             '2026-05-26 09:46:43''2026-05-26 09:46:43'),
  (22'Roll'                                              '2026-05-26 09:46:43''2026-05-26 09:46:43'),
  (23'Pax'                                               '2026-05-26 09:46:43''2026-05-26 09:46:43'),
  (24'Parts'                                             '2026-05-26 09:46:43''2026-05-26 09:46:43'),
  (25'Blok'                                              '2026-05-26 09:46:43''2026-05-26 09:46:43'),
  (26'Stel'                                              '2026-05-26 09:46:43''2026-05-26 09:46:43'),
  (27'Kaleng'                                            '2026-05-26 09:46:43''2026-05-26 09:46:43'),
  (28'5'                                                 '2026-05-26 09:46:43''2026-05-26 09:46:43'),
  (29'Buku'                                              '2026-05-26 09:46:43''2026-05-26 09:46:43'),
  (30'Kantong'                                           '2026-05-26 09:46:44''2026-05-26 09:46:44'),
  (31'kg'                                                '2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (32'liter'                                             '2026-05-26 09:46:46''2026-05-26 09:46:46'),
  (33'meter'                                             '2026-05-26 09:46:46''2026-05-26 09:46:46');


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
  `role`              VARCHAR(255) NULL DEFAULT 'User',
  `division_id`       bigint        NULL,
  `remember_token`    VARCHAR(100) NULL,
  `created_at`        DATETIME     NULL,
  `updated_at`        DATETIME     NULL,
  CONSTRAINT `PK_users`                PRIMARY KEY (`id`),
  CONSTRAINT `UQ_users_email`          UNIQUE      (`email`),
  CONSTRAINT `UQ_users_nrp`            UNIQUE      (`nrp`),
  -- role kini dinamis (nama AdminRole), tidak lagi dibatasi enum tetap.
  CONSTRAINT `FK_users_division_id`    FOREIGN KEY (`division_id`) REFERENCES `divisions` (`id`)
);

CREATE INDEX `IX_users_division_id` ON `users` (`division_id`);


INSERT INTO `users` (`id`,`name`,`nrp`,`email`,`poin`,`email_verified_at`,`password`,`role`,`division_id`,`remember_token`,`created_at`,`updated_at`) VALUES
  (1'Admin User'        '73216958''admin@wms.com'     1000'2026-05-11 11:30:04''$2y$12$6frRu6KhnRFsPL8sXW7ls.4QQk9rtkvkgxoFYp/tqp9/5zZHcEE.m''admin'      3'tupohEJ7W6UHk61RcHxSCWsrMtF71Ut35MCUqFMMNMf3zv6Ykmtieo1jPEV6''2026-05-11 11:30:04''2026-05-26 09:46:53'),
  (2'Manager User'      '44920801''manager@wms.com'   1000'2026-05-11 11:30:04''$2y$12$JCmEGC2gvkTljXwVWKizbuxsCoVxVOHmIdAtS7KwohAJAa7IeGuGK' 'manager'    3'qxl2W9eeqk'                                                   '2026-05-11 11:30:04''2026-05-26 09:46:53'),
  (3'Staff User'        '44727320''staff@wms.com'     1000'2026-05-11 11:30:05''$2y$12$n6yveeCHq3V/57rF7HbMyOAzuCa7WIXOkkQkBjkxT0B77Vaoa1Oy.''staff'      3'ljf4NYEGgb'                                                   '2026-05-11 11:30:05''2026-05-26 09:46:53'),
  (4'Super Administrator''35610885''superadmin@wms.com' 1000'2026-05-11 11:30:05''$2y$12$X2dRJ8Lk7hvYkgFj1MdVUe3QlpWWMxIs3PaSJV0M4oUuXz4H5rBS.''superadmin' 3NULL                                                           '2026-05-26 09:46:53''2026-05-26 09:46:53');


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
  `handover_photo`         VARCHAR(255) NULL,
  `handover_notes`         TEXT NULL,
  `handover_recipient_name`VARCHAR(255) NULL,
  `handover_timestamp`     DATETIME     NULL,
  `documentation_photo`    VARCHAR(255) NULL,
  `documentation_notes`    TEXT NULL,
  `documentation_uploaded_at` DATETIME  NULL,
  `return_status`          VARCHAR(255) NULL,
  `return_reason`          TEXT NULL,
  `is_return_draft`        TINYINT(1)           NOT NULL DEFAULT 0,
  `return_condition`       VARCHAR(6)   NULL,
  `status`                 VARCHAR(25)  NOT NULL DEFAULT 'PENDING',
  `rejection_reason`       TEXT NULL,
  `admin_notes`            TEXT NULL,
  `manager_notes`          TEXT NULL,
  `staff_inventory_notes`  TEXT NULL,
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
  CONSTRAINT `CK_transactions_status`                CHECK       (`status`           IN ('PENDING','PENDING_MANAGER','WAITING_HANDOVER','WAITING_ADMIN_HANDOVER','APPROVED','REJECTED','REVISION')),
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
  (11NULL'IN''BORROW'500NULLNULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'APPROVED'NULLNULLNULLNULLNULLNULL11NULLNULLNULL'2026-05-11 12:16:11''2026-05-11 12:16:20'NULLNULLNULLNULLNULLNULLNULLNULLNULL),
  (22NULL'IN''BORROW'5000NULLNULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'APPROVED'NULLNULLNULLNULLNULLNULL11NULLNULLNULL'2026-05-11 12:19:00''2026-05-11 12:20:54'NULLNULLNULLNULLNULLNULLNULLNULLNULL),
  (32NULL'IN''BORROW'1000NULLNULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'APPROVED'NULLNULLNULLNULLNULLNULL11NULLNULLNULL'2026-05-11 12:20:13''2026-05-11 12:21:00'NULLNULLNULLNULLNULLNULLNULLNULLNULL),
  (42NULL'IN''BORROW'300NULLNULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'APPROVED'NULLNULLNULLNULLNULLNULL11NULLNULLNULL'2026-05-11 12:20:41''2026-05-11 12:20:57'NULLNULLNULLNULLNULLNULLNULLNULLNULL),
  (5294NULL'OUT''BORROW'110'2026-06-04 01:09:38''images/return/48e38a67-f012-4238-870d-7320c7e2b043.jpg'NULLNULLNULLNULLNULLNULLNULL'baik'NULL0NULL'APPROVED'NULL'halo boleh ya'NULLNULLNULLNULL34'ok''Staff User'3'2026-06-02 01:10:48''2026-06-04 01:09:38''Staff User''44939834'NULL'2026-06-11'NULL'2026-06-11''Testing event''2026-06-16'NULL),
  (650NULL'OUT''BORROW'110'2026-06-04 01:20:18''images/return/06ccdaca-6200-4016-9933-4973629acf8b.jpeg'NULLNULLNULLNULLNULLNULLNULL'baik'NULL0NULL'APPROVED'NULLNULLNULLNULLNULLNULL34'oko''Staff User'3'2026-06-02 01:23:02''2026-06-04 01:20:18''Staff User''44939834'NULL'2026-06-16'NULL'2026-06-16''Testing event''2026-06-16'NULL),
  (7152NULL'OUT''BORROW'100NULLNULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'PENDING_MANAGER'NULLNULLNULLNULLNULLNULL3NULL'ok''Staff User'3'2026-06-02 01:37:30''2026-06-02 01:55:55''Staff User''44939834'NULL'2026-06-02'NULL'2026-06-02''Testing event''2026-06-02'NULL),
  (8231NULL'OUT''BORROW'110'2026-06-04 01:20:13''images/return/4f5ce0b3-d253-4863-ab05-14355afe9b59.jpg'NULLNULLNULLNULLNULLNULLNULL'baik'NULL0NULL'APPROVED'NULLNULLNULLNULLNULLNULL31'ok''Staff User'3'2026-06-02 01:49:16''2026-06-04 01:20:13''Staff User''44939834'NULL'2026-06-02'NULL'2026-06-02''Testing event''2026-06-02'NULL),
  (9400NULL'OUT''GIVEAWAY'100NULLNULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'APPROVED'NULLNULLNULLNULLNULLNULL31'ok''Staff User'3'2026-06-02 01:59:40''2026-06-02 02:00:10''Staff User''44939834'NULLNULLNULLNULL'Testing event''2026-06-02'NULL),
  (10400NULL'OUT''GIVEAWAY'100NULLNULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'APPROVED'NULLNULLNULLNULLNULLNULL32NULL'Staff User'3'2026-06-03 07:32:02''2026-06-03 07:49:02''Staff User''44939834'NULLNULLNULLNULL'testing''2026-06-04'NULL),
  (11294NULL'OUT''BORROW'110'2026-06-03 07:58:04''returns/jKCEN7viU0YJiSSP0h7KTcxRm9d4uJX7fUaH4ylR.jpg'NULLNULLNULLNULLNULLNULLNULL'baik'NULL0NULL'APPROVED'NULLNULLNULLNULLNULLNULL34NULL'Staff User'3'2026-06-03 07:32:02''2026-06-03 07:58:04''Staff User''44939834'NULL'2026-06-03'NULL'2026-06-05''testing''2026-06-04'NULL),
  (1272NULL'OUT''BORROW'110'2026-06-04 01:09:20''images/return/5d2d84b7-b14f-405c-9cda-f3d4b7aeab36.jpg'NULLNULLNULLNULLNULLNULLNULL'baik'NULL0NULL'APPROVED'NULL'bolehhhh'NULLNULLNULLNULL44'lklm''Super Administrator'3'2026-06-04 00:40:37''2026-06-04 01:09:20''Super Administrator''35610885'1'2026-06-04'NULL'2026-06-05''cghgjgjk''2026-06-04'NULL),
  (1372NULL'OUT''BORROW'220'2026-06-04 00:48:09''forced-by-admin'NULLNULLNULLNULLNULLNULLNULL'baik'NULL0NULL'APPROVED'NULL''NULLNULLNULLNULL44'pinjem ya''Super Administrator'3'2026-06-04 00:47:06''2026-06-04 00:48:09''Super Administrator''35610885'1'2026-06-04'NULL'2026-06-05''minjam''2026-06-04'NULL),
  (1440037'OUT''GIVEAWAY'100NULLNULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'REJECTED''ga cukup rek''ok''ga cukup rek'NULLNULLNULL42'yyyyy''Super Administrator'3'2026-06-04 00:48:32''2026-06-04 00:54:19''Super Administrator''35610885'0NULL'2026-06-04'NULL'iujknkkjjnn''2026-06-04'NULL),
  (1540044'OUT''GIVEAWAY'300'2026-06-04 01:08:52'NULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'APPROVED'NULLNULL'Ya boleh kok'NULLNULLNULL42'Mau bos boleh ga?''Super Administrator'3'2026-06-04 01:08:13''2026-06-04 01:08:52''Super Administrator''35610885'0NULL'2026-06-04'NULL'Mau''2026-06-04'NULL),
  (1740038'OUT''GIVEAWAY'100'2026-06-04 01:58:23'NULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'APPROVED'NULLNULLNULLNULLNULLNULL42NULL'Super Administrator'3'2026-06-04 01:51:29''2026-06-04 01:58:23''Super Administrator''35610885'0NULL'2026-06-04'NULL'asdadsad''2026-06-04'NULL),
  (1840040'OUT''GIVEAWAY'100'2026-06-04 02:08:15'NULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'APPROVED'NULLNULLNULLNULLNULLNULL44'asas''Super Administrator'3'2026-06-04 02:05:07''2026-06-04 02:08:15''Super Administrator''35610885'0NULL'2026-06-04'NULL'ASAASA''2026-06-04'NULL),
  (1940045'OUT''GIVEAWAY'100'2026-06-04 17:35:56'NULLNULLNULLNULLNULLNULLNULLNULLNULLNULL0NULL'APPROVED'NULL''NULLNULLNULLNULL44NULL'Super Administrator'3'2026-06-04 17:35:47''2026-06-04 17:35:56''Super Administrator''35610885'0NULL'2026-06-05'NULL'Yup''2026-06-06'NULL),
  (20124NULL'OUT''BORROW'100NULLNULL'images/handover/172e34c7-20cb-4385-b2d9-55192ed41b7f.jpg'NULLNULLNULLNULLNULLNULLNULLNULL0NULL'APPROVED'NULL''NULLNULLNULLNULL44'yaya''Super Administrator'3'2026-06-04 18:59:02''2026-06-04 19:00:04''Super Administrator''35610885'1'2026-06-05'NULL'2026-06-06''Program''2026-06-06'NULL);


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
  (1 5 294 2 1'2026-06-04 00:35:39''2026-06-04 00:35:39'),
  (212  72 5 4'2026-06-04 00:40:48''2026-06-04 00:40:48'),
  (313  72 4 2'2026-06-04 00:47:59''2026-06-04 00:47:59'),
  (413  72 2 4'2026-06-04 00:48:09''2026-06-04 00:48:09'),
  (514 4002726'2026-06-04 00:48:41''2026-06-04 00:48:41'),
  (615 4002623'2026-06-04 01:08:22''2026-06-04 01:08:22'),
  (715 4002320'2026-06-04 01:08:52''2026-06-04 01:08:52'),
  (812  72 4 5'2026-06-04 01:09:20''2026-06-04 01:09:20'),
  (9 5 294 1 2'2026-06-04 01:09:38''2026-06-04 01:09:38'),
  (10 8 2312526'2026-06-04 01:20:13''2026-06-04 01:20:13'),
  (11 6  504142'2026-06-04 01:20:18''2026-06-04 01:20:18'),
  (1317 4002019'2026-06-04 01:52:03''2026-06-04 01:52:03'),
  (1417 4001918'2026-06-04 01:58:22''2026-06-04 01:58:22'),
  (1518 4001817'2026-06-04 02:08:15''2026-06-04 02:08:15'),
  (1619 4001716'2026-06-04 17:35:56''2026-06-04 17:35:56'),
  (1720 124 1 0'2026-06-04 18:59:29''2026-06-04 18:59:29');


-- =============================================================
-- Normalisasi produk berdasarkan kategori
--   Merchandise, ATK, Makanan, Facility â†’ giveaway, value=50, is_returnable=0
--   Alat Musik, Elektronik, Game        â†’ pinjam, value=0, is_returnable=1
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
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
  `UpdatedAt` datetime(6) NOT NULL,
  `UpdatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
  CONSTRAINT `PK_admin_roles` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

INSERT INTO `admin_roles` (`Id`, `RoleName`, `Description`, `IsActive`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) VALUES
  ('063e920c-626c-4fa8-a8b9-35900bb7b7b7' 'Staff Inventoris' 'Default role for Staff Inventoris' 1 '2026-06-09 03:57:53.286081' 'System' '2026-06-09 03:57:53.286081' 'System'),
  ('6969448e-153b-43fc-8be0-1dda5927a8a1' 'Team Leader Infrastructure' 'Default role for Team Leader Infrastructure' 1 '2026-06-09 03:57:53.272759' 'System' '2026-06-09 03:57:53.272760' 'System'),
  ('d941584b-880a-4d31-aa4c-ca04a29a43d4' 'Manager' 'Default role for Manager' 1 '2026-06-09 03:57:53.280862' 'System' '2026-06-09 03:57:53.280862' 'System'),
  ('ec5f41d3-cec6-4d33-a220-81a18c9e1cfd' 'PIC Studio' 'Default role for PIC Studio' 1 '2026-06-09 03:57:53.187149' 'System' '2026-06-09 03:57:53.187244' 'System');

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

-- =============================================================
-- APPENDED FROM SEED
-- =============================================================
INSERT INTO `admin_roles` VALUES 
  ('0aa48329-85e7-480d-9e62-21cd0dec8e86','Super Admin','Default role for Super Admin','[\"request.create\",\"tracking.view\",\"approval.stage1\",\"approval.stage2\",\"approval.manager\",\"approval.handover\",\"approval.return\",\"dashboard.view\",\"products.manage\",\"masterdata.manage\",\"reports.view\",\"scanner.use\",\"users.manage\",\"roles.manage\"]',1,'2026-06-10 02:37:39.813699','System','2026-06-10 02:55:59.610173','Super Administrator'),
  ('e6ad3624-7c19-48cb-a746-b0f58f13ad31','User','Default role for User','[\"dashboard.view\",\"request.create\",\"tracking.view\"]',1,'2026-06-10 02:37:39.857361','System','2026-06-10 02:46:42.014613','Super Administrator');

INSERT INTO `products` VALUES 
  (295,'GME-260511-0088','TYPE_CODE_128','Rubik Speed Cube',NULL,NULL,0.00,NULL,NULL,3,8,NULL,2,2,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (296,'GME-260511-0089','TYPE_CODE_128','Puzzle Kayu',NULL,NULL,0.00,NULL,NULL,3,8,NULL,25,25,14,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (297,'GME-260511-0090','TYPE_CODE_128','Games Balok Kayu Set',NULL,NULL,0.00,NULL,NULL,3,8,NULL,1,1,14,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (298,'GME-260511-0091','TYPE_CODE_128','Baffling Steel Puzzle',NULL,NULL,0.00,NULL,NULL,3,8,NULL,1,1,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (299,'FCY-260511-0061','TYPE_CODE_128','Properti Balon dan Sarung Tangan',NULL,NULL,50.00,NULL,NULL,4,9,NULL,1,1,30,0,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (300,'ETK-260511-0008','TYPE_CODE_128','Yi CamCase',NULL,NULL,0.00,NULL,NULL,9,7,NULL,2,2,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (301,'ETK-260511-0009','TYPE_CODE_128','Canon G1X + 15-60mm',NULL,NULL,0.00,NULL,NULL,9,9,NULL,1,1,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (302,'ETK-260511-0010','TYPE_CODE_128','Microphone Podcast Set',NULL,NULL,0.00,NULL,NULL,9,10,NULL,1,1,14,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (303,'ETK-260511-0011','TYPE_CODE_128','TP Link Router',NULL,NULL,0.00,NULL,NULL,9,9,NULL,1,1,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (304,'ETK-260511-0012','TYPE_CODE_128','Handy Cam + Charger + Bag',NULL,NULL,0.00,NULL,NULL,9,8,NULL,3,3,14,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (305,'ETK-260511-0013','TYPE_CODE_128','Videomic Rode',NULL,NULL,0.00,NULL,NULL,9,11,NULL,2,2,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (306,'ETK-260511-0014','TYPE_CODE_128','DJI Mavic Mini (NEW)',NULL,NULL,0.00,NULL,NULL,9,11,NULL,2,2,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (307,'FCY-260511-0062','TYPE_CODE_128','Zomei Professional Tripod',NULL,NULL,50.00,NULL,NULL,4,11,NULL,2,2,3,0,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (308,'ETK-260511-0015','TYPE_CODE_128','Camcorder AVCAM Panasonic',NULL,NULL,0.00,NULL,NULL,9,9,NULL,1,1,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (309,'ETK-260511-0016','TYPE_CODE_128','Microphone Podcast',NULL,NULL,0.00,NULL,NULL,9,7,NULL,2,2,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (310,'ETK-260511-0017','TYPE_CODE_128','Godox Minimaster',NULL,NULL,0.00,NULL,NULL,9,7,NULL,2,2,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (311,'ETK-260511-0018','TYPE_CODE_128','Studio Flash (Godox)',NULL,NULL,0.00,NULL,NULL,9,7,NULL,1,1,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (312,'ETK-260511-0019','TYPE_CODE_128','Alctron Audio Interface',NULL,NULL,0.00,NULL,NULL,9,8,NULL,2,2,3,1,0,'2026-05-11 13:46:24','2026-05-26 09:46:52'),
  (400,'MHE-260511-0033','TYPE_CODE_128','Baju Polo',NULL,NULL,50.00,'/images/products/aObBuQFVHxEkNi17CtlIu3mnPT49HOnmBNMD8OFX.jpg','[\"/images/products/aObBuQFVHxEkNi17CtlIu3mnPT49HOnmBNMD8OFX.jpg\", \"/images/products/ceuQ2P6dcE8Ro4Sd97ulhldhS8i7ub5Cv4m06sWb.jpg\"]',6,4,NULL,16,1,3,0,0,'2026-05-11 13:46:23','2026-05-26 09:46:52'),
  (410,'MHE-260511-0034','TYPE_CODE_128','Celana Training',NULL,NULL,50.00,NULL,NULL,6,4,NULL,2,1,3,0,0,'2026-05-11 13:46:23','2026-05-26 09:46:52'),
  (413,'MHE-260511-0035','TYPE_CODE_128','Kemeja UT',NULL,NULL,50.00,'/images/products/HZpVxqrwZZOE9xef7B8aYouG1q7N4Lt0XzEq33LK.jpg','[\"/images/products/HZpVxqrwZZOE9xef7B8aYouG1q7N4Lt0XzEq33LK.jpg\"]',6,4,NULL,110,5,3,0,0,'2026-05-11 13:46:23','2026-05-26 09:46:52'),
  (415,'MHE-260511-0036','TYPE_CODE_128','Kaos UT',NULL,NULL,50.00,NULL,NULL,6,4,NULL,17,3,3,0,0,'2026-05-11 13:46:23','2026-05-26 09:46:52'),
  (429,'MHE-260511-0037','TYPE_CODE_128','Merch Assessment - Tumbler Corkcilcke',NULL,NULL,50.00,'/images/products/8jZaNApx2sjjOOF4A6sJCBwn54J1HvdX5CiqAtm1.jpg','[\"/images/products/8jZaNApx2sjjOOF4A6sJCBwn54J1HvdX5CiqAtm1.jpg\"]',6,4,NULL,4,2,3,0,0,'2026-05-11 13:46:23','2026-05-26 09:46:52'),
  (431,'MHE-260511-0038','TYPE_CODE_128','RACER',NULL,NULL,50.00,NULL,NULL,6,4,NULL,86,12,3,0,0,'2026-05-11 13:46:23','2026-05-26 09:46:52');

INSERT INTO `user_admin_roles` VALUES 
  ('36a2d501-7687-400c-a8e5-911a180e1bb6',5,'063e920c-626c-4fa8-a8b9-35900bb7b7b7','2026-06-10 01:10:41.028001','Super Administrator',NULL),
  ('6134fa61-54c8-4d16-8964-0b6ed05badbd',4,'0aa48329-85e7-480d-9e62-21cd0dec8e86','2026-06-10 02:42:39.861960','Super Administrator',NULL),
  ('976f60fb-4160-41fb-8cc6-399d20e352dd',1,'ec5f41d3-cec6-4d33-a220-81a18c9e1cfd','2026-06-10 01:13:52.888094','Super Administrator',NULL),
  ('ef3351b4-1c3a-4312-a707-f84f4cc17ef9',3,'e6ad3624-7c19-48cb-a746-b0f58f13ad31','2026-06-10 02:46:46.316230','Super Administrator',NULL);

INSERT INTO `users` VALUES 
  (5,'Dzaky','12098213','tester@tester.com',1000,NULL,'$2a$11$hWecE2pogdAr9UY28T/VP.RVf5ig13n4bjydfzaxTI587t6X8KLAm','admin',5,NULL,'2026-06-10 01:10:16','2026-06-10 02:50:04'),
  (6,'User','1340120938','user@wms.com',1000,NULL,'$2a$11$/YIY4ZhN/svsgPjWHwjyK.aA5Nqs9bXnsBTyN4uKy.fze4NXzUAzS','User',2,NULL,'2026-06-10 03:03:39','2026-06-10 03:03:39');



