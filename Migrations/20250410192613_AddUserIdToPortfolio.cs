﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentPortfolioAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToPortfolio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "InvestmentPortfolios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPortfolios_UserId",
                table: "InvestmentPortfolios",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPortfolios_Users_UserId",
                table: "InvestmentPortfolios",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentPortfolios_Users_UserId",
                table: "InvestmentPortfolios");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentPortfolios_UserId",
                table: "InvestmentPortfolios");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InvestmentPortfolios");
        }
    }
}
