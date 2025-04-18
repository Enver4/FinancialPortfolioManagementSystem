using InvestmentPortfolioAPI.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentPortfolioAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentPortfolios_AssetTypes_AssetTypeId",
                table: "InvestmentPortfolios");

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                    
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPortfolios_Currencies_AssetTypeId",
                table: "InvestmentPortfolios",
                column: "AssetTypeId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentPortfolios_Currencies_AssetTypeId",
                table: "InvestmentPortfolios");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPortfolios_AssetTypes_AssetTypeId",
                table: "InvestmentPortfolios",
                column: "AssetTypeId",
                principalTable: "AssetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
