using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiquidityPoolPositions_Networks_NetworkName",
                table: "LiquidityPoolPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_LiquidityPoolPositions_Wallets_WalletAddress",
                table: "LiquidityPoolPositions");

            migrationBuilder.DropTable(
                name: "LiquidityPoolPositionSnapshots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LiquidityPoolPositions",
                table: "LiquidityPoolPositions");

            migrationBuilder.RenameTable(
                name: "LiquidityPoolPositions",
                newName: "PoolPositions");

            migrationBuilder.RenameColumn(
                name: "SynchronizedAt",
                table: "PoolPositions",
                newName: "Day");

            migrationBuilder.RenameIndex(
                name: "IX_LiquidityPoolPositions_WalletAddress",
                table: "PoolPositions",
                newName: "IX_PoolPositions_WalletAddress");

            migrationBuilder.RenameIndex(
                name: "IX_LiquidityPoolPositions_NetworkName",
                table: "PoolPositions",
                newName: "IX_PoolPositions_NetworkName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PoolPositions",
                table: "PoolPositions",
                columns: new[] { "PositionId", "NetworkName", "Day" });

            migrationBuilder.CreateTable(
                name: "PoolPositionFees",
                columns: table => new
                {
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    LiquidityPoolPositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Token0Fee_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token0Fee_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0Fee_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1Fee_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token1Fee_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1Fee_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    IsInRange = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolPositionFees", x => new { x.LiquidityPoolPositionId, x.NetworkName, x.Day });
                    table.ForeignKey(
                        name: "FK_PoolPositionFees_PoolPositions_LiquidityPoolPositionId_Netw~",
                        columns: x => new { x.LiquidityPoolPositionId, x.NetworkName, x.Day },
                        principalTable: "PoolPositions",
                        principalColumns: new[] { "PositionId", "NetworkName", "Day" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_PoolPositions_Networks_NetworkName",
                table: "PoolPositions",
                column: "NetworkName",
                principalTable: "Networks",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PoolPositions_Wallets_WalletAddress",
                table: "PoolPositions",
                column: "WalletAddress",
                principalTable: "Wallets",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PoolPositions_Networks_NetworkName",
                table: "PoolPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_PoolPositions_Wallets_WalletAddress",
                table: "PoolPositions");

            migrationBuilder.DropTable(
                name: "PoolPositionFees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PoolPositions",
                table: "PoolPositions");

            migrationBuilder.RenameTable(
                name: "PoolPositions",
                newName: "LiquidityPoolPositions");

            migrationBuilder.RenameColumn(
                name: "Day",
                table: "LiquidityPoolPositions",
                newName: "SynchronizedAt");

            migrationBuilder.RenameIndex(
                name: "IX_PoolPositions_WalletAddress",
                table: "LiquidityPoolPositions",
                newName: "IX_LiquidityPoolPositions_WalletAddress");

            migrationBuilder.RenameIndex(
                name: "IX_PoolPositions_NetworkName",
                table: "LiquidityPoolPositions",
                newName: "IX_LiquidityPoolPositions_NetworkName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LiquidityPoolPositions",
                table: "LiquidityPoolPositions",
                columns: new[] { "PositionId", "NetworkName" });

            migrationBuilder.CreateTable(
                name: "LiquidityPoolPositionSnapshots",
                columns: table => new
                {
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    LiquidityPoolPositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    IsInRange = table.Column<bool>(type: "boolean", nullable: false),
                    Token0Fee_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0Fee_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0Fee_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token1Fee_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1Fee_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1Fee_Symbol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidityPoolPositionSnapshots", x => new { x.NetworkName, x.LiquidityPoolPositionId, x.Day });
                    table.ForeignKey(
                        name: "FK_LiquidityPoolPositionSnapshots_LiquidityPoolPositions_Liqui~",
                        columns: x => new { x.LiquidityPoolPositionId, x.NetworkName },
                        principalTable: "LiquidityPoolPositions",
                        principalColumns: new[] { "PositionId", "NetworkName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiquidityPoolPositionSnapshots_LiquidityPoolPositionId_Netw~",
                table: "LiquidityPoolPositionSnapshots",
                columns: new[] { "LiquidityPoolPositionId", "NetworkName" });

            migrationBuilder.AddForeignKey(
                name: "FK_LiquidityPoolPositions_Networks_NetworkName",
                table: "LiquidityPoolPositions",
                column: "NetworkName",
                principalTable: "Networks",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LiquidityPoolPositions_Wallets_WalletAddress",
                table: "LiquidityPoolPositions",
                column: "WalletAddress",
                principalTable: "Wallets",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
