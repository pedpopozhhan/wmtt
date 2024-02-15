using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class Adding_CreatedBy_UpdatedBy_Name_DateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "InvoiceTimeReportCostDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedByDateTime",
                table: "InvoiceTimeReportCostDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "InvoiceServiceSheet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedByDateTime",
                table: "InvoiceServiceSheet",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "InvoiceServiceSheet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedByDateTime",
                table: "InvoiceServiceSheet",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "InvoiceOtherCostDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedByDateTime",
                table: "InvoiceOtherCostDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedByDateTime",
                table: "Invoice",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedByDateTime",
                table: "Invoice",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InvoiceTimeReportCostDetails");

            migrationBuilder.DropColumn(
                name: "CreatedByDateTime",
                table: "InvoiceTimeReportCostDetails");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InvoiceServiceSheet");

            migrationBuilder.DropColumn(
                name: "CreatedByDateTime",
                table: "InvoiceServiceSheet");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "InvoiceServiceSheet");

            migrationBuilder.DropColumn(
                name: "UpdatedByDateTime",
                table: "InvoiceServiceSheet");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InvoiceOtherCostDetails");

            migrationBuilder.DropColumn(
                name: "CreatedByDateTime",
                table: "InvoiceOtherCostDetails");

            migrationBuilder.DropColumn(
                name: "CreatedByDateTime",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "UpdatedByDateTime",
                table: "Invoice");
        }
    }
}
