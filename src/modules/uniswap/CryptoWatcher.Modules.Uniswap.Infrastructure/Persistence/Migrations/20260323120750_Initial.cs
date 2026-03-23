using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "uniswap");

            migrationBuilder.CreateTable(
                name: "UniswapChainConfigurations",
                schema: "uniswap",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ProtocolVersion = table.Column<int>(type: "integer", nullable: false),
                    ChainId = table.Column<int>(type: "integer", nullable: false),
                    BlockscoutUrl = table.Column<string>(type: "text", nullable: false),
                    SmartContractAddresses_MultiCall = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    SmartContractAddresses_PoolFactory = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    SmartContractAddresses_PositionManager = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    SmartContractAddresses_StateView = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: true),
                    RpcUrl = table.Column<string>(type: "text", nullable: false),
                    RpcAuthToken = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapChainConfigurations", x => new { x.Name, x.ProtocolVersion });
                });

            migrationBuilder.CreateTable(
                name: "UniswapLiquidityPositions",
                schema: "uniswap",
                columns: table => new
                {
                    PositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    TickLower = table.Column<long>(type: "bigint", nullable: false),
                    TickUpper = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    ClosedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    ProtocolVersion = table.Column<int>(type: "integer", nullable: false),
                    Token0_Address = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_Symbol = table.Column<string>(type: "character varying(16)", unicode: false, maxLength: 16, nullable: false),
                    Token1_Address = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Symbol = table.Column<string>(type: "character varying(16)", unicode: false, maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapLiquidityPositions", x => new { x.PositionId, x.NetworkName });
                });

            migrationBuilder.CreateTable(
                name: "UniswapPositionDailyPerformances",
                schema: "uniswap",
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
                    Token0_Fee = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Fee = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapPositionDailyPerformances", x => new { x.Day, x.NetworkName, x.PoolPositionId });
                });

            migrationBuilder.CreateTable(
                name: "UniswapLiquidityPositionCashFlows",
                schema: "uniswap",
                columns: table => new
                {
                    PositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", nullable: false),
                    TransactionHash = table.Column<string>(type: "character(66)", unicode: false, fixedLength: true, maxLength: 66, nullable: false),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapLiquidityPositionCashFlows", x => new { x.PositionId, x.NetworkName, x.TransactionHash, x.Event });
                    table.ForeignKey(
                        name: "FK_UniswapLiquidityPositionCashFlows_UniswapLiquidityPositions~",
                        columns: x => new { x.PositionId, x.NetworkName },
                        principalSchema: "uniswap",
                        principalTable: "UniswapLiquidityPositions",
                        principalColumns: new[] { "PositionId", "NetworkName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniswapLiquidityPositionSnapshots",
                schema: "uniswap",
                columns: table => new
                {
                    PoolPositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    IsInRange = table.Column<bool>(type: "boolean", nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_Fee = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Fee = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapLiquidityPositionSnapshots", x => new { x.PoolPositionId, x.NetworkName, x.Day });
                    table.ForeignKey(
                        name: "FK_UniswapLiquidityPositionSnapshots_UniswapLiquidityPositions~",
                        columns: x => new { x.PoolPositionId, x.NetworkName },
                        principalSchema: "uniswap",
                        principalTable: "UniswapLiquidityPositions",
                        principalColumns: new[] { "PositionId", "NetworkName" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniswapChainConfigurations",
                schema: "uniswap");

            migrationBuilder.DropTable(
                name: "UniswapLiquidityPositionCashFlows",
                schema: "uniswap");

            migrationBuilder.DropTable(
                name: "UniswapLiquidityPositionSnapshots",
                schema: "uniswap");

            migrationBuilder.DropTable(
                name: "UniswapPositionDailyPerformances",
                schema: "uniswap");

            migrationBuilder.DropTable(
                name: "UniswapLiquidityPositions",
                schema: "uniswap");
        }
    }
}
