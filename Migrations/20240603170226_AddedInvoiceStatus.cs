using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class AddedInvoiceStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceStatus",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceStatus",
                table: "Invoice");
        }
    }
}
