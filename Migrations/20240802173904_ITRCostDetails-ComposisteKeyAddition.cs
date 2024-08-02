using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class ITRCostDetailsComposisteKeyAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceTimeReportCostDetails",
                table: "InvoiceTimeReportCostDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceTimeReportCostDetails",
                table: "InvoiceTimeReportCostDetails",
                columns: new[] { "FlightReportCostDetailsId", "InvoiceId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceTimeReportCostDetails",
                table: "InvoiceTimeReportCostDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceTimeReportCostDetails",
                table: "InvoiceTimeReportCostDetails",
                column: "FlightReportCostDetailsId");
        }
    }
}
