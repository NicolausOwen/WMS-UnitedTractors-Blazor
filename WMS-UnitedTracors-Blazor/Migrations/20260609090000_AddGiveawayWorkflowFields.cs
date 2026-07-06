using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UT_WMSDotnet.Data;

#nullable disable

namespace ut_wms_asp.net.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260609090000_AddGiveawayWorkflowFields")]
    public partial class AddGiveawayWorkflowFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "documentation_notes",
                table: "transactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "documentation_photo",
                table: "transactions",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "documentation_uploaded_at",
                table: "transactions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "handover_recipient_name",
                table: "transactions",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "handover_timestamp",
                table: "transactions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_revision_stage",
                table: "transactions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "staff_inventory_notes",
                table: "transactions",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "documentation_notes",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "documentation_photo",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "documentation_uploaded_at",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "handover_recipient_name",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "handover_timestamp",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "last_revision_stage",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "staff_inventory_notes",
                table: "transactions");
        }
    }
}
