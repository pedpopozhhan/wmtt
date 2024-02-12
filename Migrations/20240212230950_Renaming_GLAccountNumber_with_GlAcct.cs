using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class Renaming_GLAccountNumber_with_GlAcct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Account",
                table: "InvoiceTimeReportCostDetails",
                newName: "GlAcct");

            migrationBuilder.RenameColumn(
                name: "Account",
                table: "InvoiceOtherCostDetails",
                newName: "GlAcct");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GlAcct",
                table: "InvoiceTimeReportCostDetails",
                newName: "Account");

            migrationBuilder.RenameColumn(
                name: "GlAcct",
                table: "InvoiceOtherCostDetails",
                newName: "Account");
        }
    }
}
