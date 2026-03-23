using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniswapMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniswapLiquidityPositionCashFlows");

            migrationBuilder.DropTable(
                name: "UniswapLiquidityPositionSnapshots");

            migrationBuilder.DropTable(
                name: "UniswapPositionDailyPerformances");

            migrationBuilder.DropTable(
                name: "UniswapLiquidityPositions");

            migrationBuilder.DropTable(
                name: "UniswapChainConfigurations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UniswapChainConfigurations",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ProtocolVersion = table.Column<int>(type: "integer", nullable: false),
                    BlockscoutUrl = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ChainId = table.Column<int>(type: "integer", nullable: false),
                    LastProcessedBlock = table.Column<string>(type: "text", nullable: false),
                    LastProcessedBlockUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RpcAuthToken = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    RpcUrl = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SmartContractAddresses_MultiCall = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    SmartContractAddresses_PoolFactory = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    SmartContractAddresses_PositionManager = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    SmartContractAddresses_StateView = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapChainConfigurations", x => new { x.Name, x.ProtocolVersion });
                });

            migrationBuilder.CreateTable(
                name: "UniswapPositionDailyPerformances",
                columns: table => new
                {
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PoolPositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CommissionInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    CumulativeCommissionInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    HoldValueInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    IsInRange = table.Column<bool>(type: "boolean", nullable: false),
                    PositionValueInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    ProfitInUsd = table.Column<decimal>(type: "numeric", nullable: false),
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
                name: "UniswapLiquidityPositions",
                columns: table => new
                {
                    PositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    ClosedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    ProtocolVersion = table.Column<int>(type: "integer", nullable: false),
                    TickLower = table.Column<long>(type: "bigint", nullable: false),
                    TickUpper = table.Column<long>(type: "bigint", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_UniswapLiquidityPositions_UniswapChainConfigurations_Networ~",
                        columns: x => new { x.NetworkName, x.ProtocolVersion },
                        principalTable: "UniswapChainConfigurations",
                        principalColumns: new[] { "Name", "ProtocolVersion" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniswapLiquidityPositions_Wallets_WalletAddress",
                        column: x => x.WalletAddress,
                        principalTable: "Wallets",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniswapLiquidityPositionCashFlows",
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
                        principalTable: "UniswapLiquidityPositions",
                        principalColumns: new[] { "PositionId", "NetworkName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniswapLiquidityPositionSnapshots",
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
                        principalTable: "UniswapLiquidityPositions",
                        principalColumns: new[] { "PositionId", "NetworkName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UniswapLiquidityPositions_NetworkName_ProtocolVersion",
                table: "UniswapLiquidityPositions",
                columns: new[] { "NetworkName", "ProtocolVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_UniswapLiquidityPositions_WalletAddress",
                table: "UniswapLiquidityPositions",
                column: "WalletAddress");
        }
    }
}
