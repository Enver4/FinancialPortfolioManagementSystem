using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentPortfolioAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetTypeToPortfolio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetTypeId",
                table: "InvestmentPortfolios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPortfolios_AssetTypeId",
                table: "InvestmentPortfolios",
                column: "AssetTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPortfolios_AssetTypes_AssetTypeId",
                table: "InvestmentPortfolios",
                column: "AssetTypeId",
                principalTable: "AssetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentPortfolios_AssetTypes_AssetTypeId",
                table: "InvestmentPortfolios");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentPortfolios_AssetTypeId",
                table: "InvestmentPortfolios");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                table: "InvestmentPortfolios");
        }
    }
}
