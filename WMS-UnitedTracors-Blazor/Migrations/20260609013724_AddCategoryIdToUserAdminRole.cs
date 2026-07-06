using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ut_wms_asp.net.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIdToUserAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "user_admin_roles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_admin_roles_CategoryId",
                table: "user_admin_roles",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_admin_roles_categories_CategoryId",
                table: "user_admin_roles",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_admin_roles_categories_CategoryId",
                table: "user_admin_roles");

            migrationBuilder.DropIndex(
                name: "IX_user_admin_roles_CategoryId",
                table: "user_admin_roles");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "user_admin_roles");
        }
    }
}
