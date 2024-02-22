using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class Updating_TimeReportCostDetails_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceTimeReportCostDetails",
                table: "InvoiceTimeReportCostDetails");

            migrationBuilder.DropColumn(
                name: "InvoiceTimeReportCostDetailId",
                table: "InvoiceTimeReportCostDetails");

            migrationBuilder.RenameColumn(
                name: "TimeReportCostDetailReferenceId",
                table: "InvoiceTimeReportCostDetails",
                newName: "FlightReportCostDetailsId");

            migrationBuilder.RenameColumn(
                name: "ReportNumber",
                table: "InvoiceTimeReportCostDetails",
                newName: "FlightReportId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceTimeReportCostDetails",
                table: "InvoiceTimeReportCostDetails",
                column: "FlightReportCostDetailsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceTimeReportCostDetails",
                table: "InvoiceTimeReportCostDetails");

            migrationBuilder.RenameColumn(
                name: "FlightReportId",
                table: "InvoiceTimeReportCostDetails",
                newName: "ReportNumber");

            migrationBuilder.RenameColumn(
                name: "FlightReportCostDetailsId",
                table: "InvoiceTimeReportCostDetails",
                newName: "TimeReportCostDetailReferenceId");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceTimeReportCostDetailId",
                table: "InvoiceTimeReportCostDetails",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceTimeReportCostDetails",
                table: "InvoiceTimeReportCostDetails",
                column: "InvoiceTimeReportCostDetailId");
        }
    }
}
