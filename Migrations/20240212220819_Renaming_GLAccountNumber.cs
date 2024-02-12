using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class Renaming_GLAccountNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GlAccountNumber",
                table: "InvoiceTimeReportCostDetails",
                newName: "Account");

            migrationBuilder.RenameColumn(
                name: "GlAccountNumber",
                table: "InvoiceOtherCostDetails",
                newName: "Account");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Account",
                table: "InvoiceTimeReportCostDetails",
                newName: "GlAccountNumber");

            migrationBuilder.RenameColumn(
                name: "Account",
                table: "InvoiceOtherCostDetails",
                newName: "GlAccountNumber");
        }
    }
}
