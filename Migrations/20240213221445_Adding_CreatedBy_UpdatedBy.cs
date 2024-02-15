using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class Adding_CreatedBy_UpdatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "InvoiceTimeReportCostDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedByDateTime",
                table: "InvoiceTimeReportCostDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "InvoiceOtherCostDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedByDateTime",
                table: "InvoiceOtherCostDetails",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "InvoiceTimeReportCostDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedByDateTime",
                table: "InvoiceTimeReportCostDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "InvoiceOtherCostDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedByDateTime",
                table: "InvoiceOtherCostDetails");
        }
    }
}
