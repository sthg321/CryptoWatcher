using System;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "wallet_ingestion");

            migrationBuilder.CreateTable(
                name: "WalletIngestionCheckpoints",
                schema: "wallet_ingestion",
                columns: table => new
                {
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    ChainId = table.Column<int>(type: "integer", nullable: false),
                    LastPublishedTransactionHash = table.Column<string>(type: "character(66)", unicode: false, fixedLength: true, maxLength: 66, nullable: true),
                    LastPublishedBlockNumber = table.Column<BigInteger>(type: "numeric", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletIngestionCheckpoints", x => new { x.WalletAddress, x.ChainId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletIngestionCheckpoints",
                schema: "wallet_ingestion");
        }
    }
}
