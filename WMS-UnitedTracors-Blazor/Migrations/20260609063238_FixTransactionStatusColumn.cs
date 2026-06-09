using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ut_wms_asp.net.Migrations
{
    /// <inheritdoc />
    public partial class FixTransactionStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kolom status sebelumnya adalah ENUM dengan nilai terbatas.
            // Diubah ke VARCHAR(50) agar semua WorkflowStatuses bisa disimpan
            // (PENDING_STAFF_INVENTORY, PENDING_ADMIN, REVISION_BY_STAFF_INVENTORY, dll).
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "transactions",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "PENDING",
                oldClrType: typeof(string),
                oldType: "enum('PENDING','PENDING_MANAGER','WAITING_HANDOVER','WAITING_ADMIN_HANDOVER','APPROVED','REJECTED','REVISION')",
                oldNullable: false,
                oldDefaultValue: "PENDING");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "transactions",
                type: "enum('PENDING','PENDING_MANAGER','WAITING_HANDOVER','WAITING_ADMIN_HANDOVER','APPROVED','REJECTED','REVISION')",
                nullable: false,
                defaultValue: "PENDING",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: false,
                oldDefaultValue: "PENDING");
        }
    }
}
