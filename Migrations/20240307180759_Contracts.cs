using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class Contracts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PeriodEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvoiceReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vendor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UniqueServiceSheetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunityCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.InvoiceId);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceOtherCostDetails",
                columns: table => new
                {
                    InvoiceOtherCostDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    From = table.Column<DateTime>(type: "datetime2", nullable: false),
                    To = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RateType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoOfUnits = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RateUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RatePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfitCentre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostCentre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FireNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fund = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceOtherCostDetails", x => x.InvoiceOtherCostDetailId);
                    table.ForeignKey(
                        name: "FK_InvoiceOtherCostDetails_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "InvoiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceTimeReportCostDetails",
                columns: table => new
                {
                    FlightReportCostDetailsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlightReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContractRegistrationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlightReportId = table.Column<int>(type: "int", nullable: false),
                    Ao02Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RateType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoOfUnits = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RateUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RatePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfitCenter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostCenter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FireNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fund = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceTimeReportCostDetails", x => x.FlightReportCostDetailsId);
                    table.ForeignKey(
                        name: "FK_InvoiceTimeReportCostDetails_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "InvoiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceOtherCostDetails_InvoiceId",
                table: "InvoiceOtherCostDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceTimeReportCostDetails_InvoiceId",
                table: "InvoiceTimeReportCostDetails",
                column: "InvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceOtherCostDetails");

            migrationBuilder.DropTable(
                name: "InvoiceTimeReportCostDetails");

            migrationBuilder.DropTable(
                name: "Invoice");
        }
    }
}
