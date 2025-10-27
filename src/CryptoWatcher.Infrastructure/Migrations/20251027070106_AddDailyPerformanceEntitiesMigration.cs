using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyPerformanceEntitiesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token1_Symbol",
                table: "UniswapLiquidityPositionSnapshots",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Token0_Symbol",
                table: "UniswapLiquidityPositionSnapshots",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "AavePositionDailyPerformances",
                columns: table => new
                {
                    SnapshotPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    PositionType = table.Column<int>(type: "integer", nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", fixedLength: true, maxLength: 42, nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ProfitInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    ProfitInToken = table.Column<decimal>(type: "numeric", nullable: false),
                    Token_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token_Symbol = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AavePositionDailyPerformances", x => new { x.Day, x.PositionType, x.SnapshotPositionId });
                });

            migrationBuilder.CreateTable(
                name: "HyperliquidPositionDailyPerformances",
                columns: table => new
                {
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    VaultAddress = table.Column<string>(type: "character(42)", fixedLength: true, maxLength: 42, nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", fixedLength: true, maxLength: 42, nullable: false),
                    BalanceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    ProfitInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperliquidPositionDailyPerformances", x => new { x.VaultAddress, x.WalletAddress, x.Day });
                });

            migrationBuilder.CreateTable(
                name: "UniswapPositionDailyPerformances",
                columns: table => new
                {
                    PoolPositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    HoldValueInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    PositionValueInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    ProfitInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    CommissionInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    CumulativeCommissionInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    IsInRange = table.Column<bool>(type: "boolean", nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_Symbol = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Symbol = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapPositionDailyPerformances", x => new { x.Day, x.NetworkName, x.PoolPositionId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AavePositionDailyPerformances");

            migrationBuilder.DropTable(
                name: "HyperliquidPositionDailyPerformances");

            migrationBuilder.DropTable(
                name: "UniswapPositionDailyPerformances");

            migrationBuilder.AlterColumn<string>(
                name: "Token1_Symbol",
                table: "UniswapLiquidityPositionSnapshots",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "Token0_Symbol",
                table: "UniswapLiquidityPositionSnapshots",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);
        }
    }
}
