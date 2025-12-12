using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStartedAtAndClosedAtForUniswapPositionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UniswapLiquidityPositions");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ClosedAt",
                table: "UniswapLiquidityPositions",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CreatedAt",
                table: "UniswapLiquidityPositions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "UniswapLiquidityPositions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UniswapLiquidityPositions");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UniswapLiquidityPositions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
