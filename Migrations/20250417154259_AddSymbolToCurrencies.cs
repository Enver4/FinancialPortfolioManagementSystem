﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentPortfolioAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSymbolToCurrencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Currencies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Currencies");
        }
    }
}
