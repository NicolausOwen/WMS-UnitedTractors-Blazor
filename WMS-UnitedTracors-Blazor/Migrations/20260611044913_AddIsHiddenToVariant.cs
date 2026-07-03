using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ut_wms_asp.net.Migrations
{
    /// <inheritdoc />
    public partial class AddIsHiddenToVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "is_hidden",
                table: "product_variants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_hidden",
                table: "product_variants");
        }
    }
}
