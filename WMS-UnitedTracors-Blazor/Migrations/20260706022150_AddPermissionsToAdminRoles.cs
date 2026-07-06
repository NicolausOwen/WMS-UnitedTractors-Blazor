using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ut_wms_asp.net.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionsToAdminRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kolom Permissions sudah ditambahkan via ALTER TABLE langsung ke DB
            // Migration ini hanya sebagai penanda di __EFMigrationsHistory
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE `admin_roles` DROP COLUMN IF EXISTS `Permissions`;");
        }
    }
}
