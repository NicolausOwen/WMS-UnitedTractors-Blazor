using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS_UnitedTracors_Blazor.Migrations
{
    /// <inheritdoc />
    public partial class AddApproversPerStage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "admin_approver_id",
                table: "transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "manager_approver_id",
                table: "transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "staff_inventory_approver_id",
                table: "transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_admin_approver_id",
                table: "transactions",
                column: "admin_approver_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_manager_approver_id",
                table: "transactions",
                column: "manager_approver_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_staff_inventory_approver_id",
                table: "transactions",
                column: "staff_inventory_approver_id");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_admin_approver_id",
                table: "transactions",
                column: "admin_approver_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_manager_approver_id",
                table: "transactions",
                column: "manager_approver_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_staff_inventory_approver_id",
                table: "transactions",
                column: "staff_inventory_approver_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_admin_approver_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_manager_approver_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_staff_inventory_approver_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_admin_approver_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_manager_approver_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_staff_inventory_approver_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "admin_approver_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "manager_approver_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "staff_inventory_approver_id",
                table: "transactions");
        }
    }
}
