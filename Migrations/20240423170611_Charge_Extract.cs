using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class Charge_Extract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChargeExtractId",
                table: "Invoice",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChargeExtract",
                columns: table => new
                {
                    ChargeExtractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargeExtractDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChargeExtractFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditCreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditLastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditLastUpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeExtract", x => x.ChargeExtractId);
                });

            migrationBuilder.CreateTable(
                name: "ChargeExtractDetail",
                columns: table => new
                {
                    ChargeExtractDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargeExtractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuditCreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditLastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditLastUpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeExtractDetail", x => x.ChargeExtractDetailId);
                    table.ForeignKey(
                        name: "FK_ChargeExtractDetail_ChargeExtract_ChargeExtractId",
                        column: x => x.ChargeExtractId,
                        principalTable: "ChargeExtract",
                        principalColumn: "ChargeExtractId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargeExtractViewLog",
                columns: table => new
                {
                    ChargeExtractViewLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargeExtractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeExtractViewLog", x => x.ChargeExtractViewLogId);
                    table.ForeignKey(
                        name: "FK_ChargeExtractViewLog_ChargeExtract_ChargeExtractId",
                        column: x => x.ChargeExtractId,
                        principalTable: "ChargeExtract",
                        principalColumn: "ChargeExtractId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ChargeExtractId",
                table: "Invoice",
                column: "ChargeExtractId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeExtractDetail_ChargeExtractId",
                table: "ChargeExtractDetail",
                column: "ChargeExtractId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeExtractViewLog_ChargeExtractId",
                table: "ChargeExtractViewLog",
                column: "ChargeExtractId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_ChargeExtract_ChargeExtractId",
                table: "Invoice",
                column: "ChargeExtractId",
                principalTable: "ChargeExtract",
                principalColumn: "ChargeExtractId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_ChargeExtract_ChargeExtractId",
                table: "Invoice");

            migrationBuilder.DropTable(
                name: "ChargeExtractDetail");

            migrationBuilder.DropTable(
                name: "ChargeExtractViewLog");

            migrationBuilder.DropTable(
                name: "ChargeExtract");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_ChargeExtractId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "ChargeExtractId",
                table: "Invoice");
        }
    }
}
