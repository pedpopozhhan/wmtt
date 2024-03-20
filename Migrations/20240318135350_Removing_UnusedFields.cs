using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class Removing_UnusedFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "CommunityCode",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "MaterialGroup",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "PurchaseGroup",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "Invoice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommunityCode",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialGroup",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Invoice",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseGroup",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
