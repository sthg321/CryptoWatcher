using System;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniswapSyncStateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniswapSynchronizationState");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UniswapSynchronizationState",
                columns: table => new
                {
                    ChainName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UniswapProtocolVersion = table.Column<int>(type: "integer", nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    LastBlockNumber = table.Column<BigInteger>(type: "numeric", nullable: false),
                    LastTransactionHash = table.Column<string>(type: "character(66)", unicode: false, fixedLength: true, maxLength: 66, nullable: true),
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
    }
}
