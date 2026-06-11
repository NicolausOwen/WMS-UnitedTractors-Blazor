using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UT_WMSDotnet.Data;

#nullable disable

namespace ut_wms_asp.net.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260611000000_AddEventEndDate")]
    public partial class AddEventEndDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "event_end_date",
                table: "transactions",
                type: "date",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "event_end_date",
                table: "transactions");
        }
    }
}
