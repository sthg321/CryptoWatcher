using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHyperliquidTablesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HyperliquidVaultPositions",
                columns: table => new
                {
                    VaultAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    WalletAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
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
                name: "HyperliquidVaultEvents",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VaultAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    WalletAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Usd = table.Column<decimal>(type: "numeric", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperliquidVaultEvents", x => new { x.VaultAddress, x.WalletAddress, x.Date });
                    table.ForeignKey(
                        name: "FK_HyperliquidVaultEvents_HyperliquidVaultPositions_VaultAddre~",
                        columns: x => new { x.VaultAddress, x.WalletAddress },
                        principalTable: "HyperliquidVaultPositions",
                        principalColumns: new[] { "VaultAddress", "WalletAddress" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HyperliquidVaultEvents_Wallets_WalletAddress",
                        column: x => x.WalletAddress,
                        principalTable: "Wallets",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HyperliquidVaultPositionSnapshots",
                columns: table => new
                {
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    VaultAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    WalletAddress = table.Column<string>(type: "character varying(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperliquidVaultPositionSnapshots", x => new { x.VaultAddress, x.Day });
                    table.ForeignKey(
                        name: "FK_HyperliquidVaultPositionSnapshots_HyperliquidVaultPositions~",
                        columns: x => new { x.VaultAddress, x.WalletAddress },
                        principalTable: "HyperliquidVaultPositions",
                        principalColumns: new[] { "VaultAddress", "WalletAddress" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HyperliquidVaultPositionSnapshots_Wallets_WalletAddress",
                        column: x => x.WalletAddress,
                        principalTable: "Wallets",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HyperliquidVaultEvents_WalletAddress",
                table: "HyperliquidVaultEvents",
                column: "WalletAddress");

            migrationBuilder.CreateIndex(
                name: "IX_HyperliquidVaultPositions_WalletAddress",
                table: "HyperliquidVaultPositions",
                column: "WalletAddress");

            migrationBuilder.CreateIndex(
                name: "IX_HyperliquidVaultPositionSnapshots_VaultAddress_WalletAddress",
                table: "HyperliquidVaultPositionSnapshots",
                columns: new[] { "VaultAddress", "WalletAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_HyperliquidVaultPositionSnapshots_WalletAddress",
                table: "HyperliquidVaultPositionSnapshots",
                column: "WalletAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HyperliquidVaultEvents");

            migrationBuilder.DropTable(
                name: "HyperliquidVaultPositionSnapshots");

            migrationBuilder.DropTable(
                name: "HyperliquidVaultPositions");
        }
    }
}
