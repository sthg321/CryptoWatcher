using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    RpcUrl = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    NftManagerAddress = table.Column<string>(type: "character varying(266)", maxLength: 266, nullable: false),
                    PoolFactoryAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MultiCallAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ProtocolVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "LiquidityPoolPositions",
                columns: table => new
                {
                    PositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SynchronizedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    Token0_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsInRange = table.Column<bool>(type: "boolean", nullable: false),
                    WalletAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidityPoolPositions", x => new { x.PositionId, x.NetworkName });
                    table.ForeignKey(
                        name: "FK_LiquidityPoolPositions_Networks_NetworkName",
                        column: x => x.NetworkName,
                        principalTable: "Networks",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LiquidityPoolPositions_Wallets_WalletAddress",
                        column: x => x.WalletAddress,
                        principalTable: "Wallets",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LiquidityPoolPositionSnapshots",
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
                    table.PrimaryKey("PK_LiquidityPoolPositionSnapshots", x => new { x.NetworkName, x.LiquidityPoolPositionId, x.Day });
                    table.ForeignKey(
                        name: "FK_LiquidityPoolPositionSnapshots_LiquidityPoolPositions_Liqui~",
                        columns: x => new { x.LiquidityPoolPositionId, x.NetworkName },
                        principalTable: "LiquidityPoolPositions",
                        principalColumns: new[] { "PositionId", "NetworkName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiquidityPoolPositions_NetworkName",
                table: "LiquidityPoolPositions",
                column: "NetworkName");

            migrationBuilder.CreateIndex(
                name: "IX_LiquidityPoolPositions_WalletAddress",
                table: "LiquidityPoolPositions",
                column: "WalletAddress");

            migrationBuilder.CreateIndex(
                name: "IX_LiquidityPoolPositionSnapshots_LiquidityPoolPositionId_Netw~",
                table: "LiquidityPoolPositionSnapshots",
                columns: new[] { "LiquidityPoolPositionId", "NetworkName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiquidityPoolPositionSnapshots");

            migrationBuilder.DropTable(
                name: "LiquidityPoolPositions");

            migrationBuilder.DropTable(
                name: "Networks");

            migrationBuilder.DropTable(
                name: "Wallets");
        }
    }
}
