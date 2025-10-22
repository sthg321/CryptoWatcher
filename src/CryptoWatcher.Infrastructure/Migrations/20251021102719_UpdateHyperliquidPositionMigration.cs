using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHyperliquidPositionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ClosedAt",
                table: "HyperliquidVaultPositions",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CreatedAt",
                table: "HyperliquidVaultPositions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<decimal>(
                name: "InitialBalance",
                table: "HyperliquidVaultPositions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "HyperliquidVaultPositions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "HyperliquidVaultPositions");

            migrationBuilder.DropColumn(
                name: "InitialBalance",
                table: "HyperliquidVaultPositions");
        }
    }
}
