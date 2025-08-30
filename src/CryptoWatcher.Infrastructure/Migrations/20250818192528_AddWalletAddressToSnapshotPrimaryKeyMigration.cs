using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletAddressToSnapshotPrimaryKeyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HyperliquidVaultPositionSnapshots",
                table: "HyperliquidVaultPositionSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_HyperliquidVaultPositionSnapshots_VaultAddress_WalletAddress",
                table: "HyperliquidVaultPositionSnapshots");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HyperliquidVaultPositionSnapshots",
                table: "HyperliquidVaultPositionSnapshots",
                columns: new[] { "VaultAddress", "WalletAddress", "Day" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HyperliquidVaultPositionSnapshots",
                table: "HyperliquidVaultPositionSnapshots");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HyperliquidVaultPositionSnapshots",
                table: "HyperliquidVaultPositionSnapshots",
                columns: new[] { "VaultAddress", "Day" });

            migrationBuilder.CreateIndex(
                name: "IX_HyperliquidVaultPositionSnapshots_VaultAddress_WalletAddress",
                table: "HyperliquidVaultPositionSnapshots",
                columns: new[] { "VaultAddress", "WalletAddress" });
        }
    }
}
