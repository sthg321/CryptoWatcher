using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHyperliquidFromPublicSchemeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HyperliquidPositionCashFlows");

            migrationBuilder.DropTable(
                name: "HyperliquidPositionDailyPerformances");

            migrationBuilder.DropTable(
                name: "HyperliquidSynchronizationStates");

            migrationBuilder.DropTable(
                name: "HyperliquidVaultPeriod");

            migrationBuilder.DropTable(
                name: "HyperliquidVaultPositionSnapshots");

            migrationBuilder.DropTable(
                name: "HyperliquidVaultPositions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HyperliquidPositionDailyPerformances",
                columns: table => new
                {
                    VaultAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    BalanceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    ProfitInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperliquidPositionDailyPerformances", x => new { x.VaultAddress, x.WalletAddress, x.Day });
                });

            migrationBuilder.CreateTable(
                name: "HyperliquidSynchronizationStates",
                columns: table => new
                {
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    LastProcessedEventTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastTransactionHash = table.Column<string>(type: "character(66)", unicode: false, fixedLength: true, maxLength: 66, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperliquidSynchronizationStates", x => x.WalletAddress);
                });

            migrationBuilder.CreateTable(
                name: "HyperliquidVaultPositions",
                columns: table => new
                {
                    VaultAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    Token0_Address = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_Symbol = table.Column<string>(type: "character varying(16)", unicode: false, maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperliquidVaultPositions", x => new { x.VaultAddress, x.WalletAddress });
                    table.ForeignKey(
                        name: "FK_HyperliquidVaultPositions_Wallets_WalletAddress",
                        column: x => x.WalletAddress,
                        principalTable: "Wallets",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HyperliquidPositionCashFlows",
                columns: table => new
                {
                    VaultAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    TransactionHash = table.Column<string>(type: "character(66)", unicode: false, fixedLength: true, maxLength: 66, nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperliquidPositionCashFlows", x => new { x.VaultAddress, x.WalletAddress, x.Date });
                    table.ForeignKey(
                        name: "FK_HyperliquidPositionCashFlows_HyperliquidVaultPositions_Vaul~",
                        columns: x => new { x.VaultAddress, x.WalletAddress },
                        principalTable: "HyperliquidVaultPositions",
                        principalColumns: new[] { "VaultAddress", "WalletAddress" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HyperliquidVaultPeriod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VaultAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperliquidVaultPeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HyperliquidVaultPeriod_HyperliquidVaultPositions_VaultAddre~",
                        columns: x => new { x.VaultAddress, x.WalletAddress },
                        principalTable: "HyperliquidVaultPositions",
                        principalColumns: new[] { "VaultAddress", "WalletAddress" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HyperliquidVaultPositionSnapshots",
                columns: table => new
                {
                    VaultAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperliquidVaultPositionSnapshots", x => new { x.VaultAddress, x.WalletAddress, x.Day });
                    table.ForeignKey(
                        name: "FK_HyperliquidVaultPositionSnapshots_HyperliquidVaultPositions~",
                        columns: x => new { x.VaultAddress, x.WalletAddress },
                        principalTable: "HyperliquidVaultPositions",
                        principalColumns: new[] { "VaultAddress", "WalletAddress" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HyperliquidVaultPeriod_VaultAddress_WalletAddress",
                table: "HyperliquidVaultPeriod",
                columns: new[] { "VaultAddress", "WalletAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_HyperliquidVaultPositions_WalletAddress",
                table: "HyperliquidVaultPositions",
                column: "WalletAddress");
        }
    }
}
