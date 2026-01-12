using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMerklFieldsNameMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PendingAmout",
                table: "MerklCampaignSnapshots",
                newName: "PendingAmount");

            migrationBuilder.RenameColumn(
                name: "ClaimabelAmount",
                table: "MerklCampaignSnapshots",
                newName: "ClaimableAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PendingAmount",
                table: "MerklCampaignSnapshots",
                newName: "PendingAmout");

            migrationBuilder.RenameColumn(
                name: "ClaimableAmount",
                table: "MerklCampaignSnapshots",
                newName: "ClaimabelAmount");
        }
    }
}
