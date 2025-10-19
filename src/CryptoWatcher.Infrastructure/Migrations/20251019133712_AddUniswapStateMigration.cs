using System;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniswapStateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TransactionHash",
                table: "UniswapLiquidityPositionCashFlows",
                type: "character(66)",
                fixedLength: true,
                maxLength: 66,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(66)",
                oldMaxLength: 66);

            migrationBuilder.CreateTable(
                name: "UniswapSynchronizationState",
                columns: table => new
                {
                    WalletAddress = table.Column<string>(type: "character(42)", fixedLength: true, maxLength: 42, nullable: false),
                    ChainName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UniswapProtocolVersion = table.Column<int>(type: "integer", nullable: false),
                    LastTransactionHash = table.Column<string>(type: "character(66)", fixedLength: true, maxLength: 66, nullable: false),
                    LastBlockNumber = table.Column<BigInteger>(type: "numeric", nullable: false),
                    SynchronizedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapSynchronizationState", x => new { x.ChainName, x.UniswapProtocolVersion, x.WalletAddress });
                    table.ForeignKey(
                        name: "FK_UniswapSynchronizationState_UniswapChainConfigurations_Chai~",
                        columns: x => new { x.ChainName, x.UniswapProtocolVersion },
                        principalTable: "UniswapChainConfigurations",
                        principalColumns: new[] { "Name", "ProtocolVersion" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniswapSynchronizationState_Wallets_WalletAddress",
                        column: x => x.WalletAddress,
                        principalTable: "Wallets",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UniswapSynchronizationState_WalletAddress",
                table: "UniswapSynchronizationState",
                column: "WalletAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniswapSynchronizationState");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionHash",
                table: "UniswapLiquidityPositionCashFlows",
                type: "character varying(66)",
                maxLength: 66,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(66)",
                oldFixedLength: true,
                oldMaxLength: 66);
        }
    }
}
