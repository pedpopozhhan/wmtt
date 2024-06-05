using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class Invoice_TimeReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceTimeReports",
                columns: table => new
                {
                    InvoiceTimeReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlightReportId = table.Column<int>(type: "int", nullable: false),
                    AuditCreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditLastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditLastUpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceTimeReports", x => x.InvoiceTimeReportId);
                    table.ForeignKey(
                        name: "FK_InvoiceTimeReports_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "InvoiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceTimeReports_InvoiceId",
                table: "InvoiceTimeReports",
                column: "InvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceTimeReports");
        }
    }
}
