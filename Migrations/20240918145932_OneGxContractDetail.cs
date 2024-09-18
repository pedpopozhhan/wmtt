using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    public partial class OneGxContractDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OneGxContractDetail",
                columns: table => new
                {
                    OneGxContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractWorkspace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContractManager = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorporateRegion = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchasingUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoldbackAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneGxContractDetail", x => x.OneGxContractId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OneGxContractDetail");
        }
    }
}
