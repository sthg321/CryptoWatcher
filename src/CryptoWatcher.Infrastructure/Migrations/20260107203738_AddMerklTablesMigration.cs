using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMerklTablesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MerklCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChainId = table.Column<int>(type: "integer", nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    TokenSymbol = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    TokenAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerklCampaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerklCampaignSnapshots",
                columns: table => new
                {
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    MerklCampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimabelAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ClaimedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PendingAmout = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerklCampaignSnapshots", x => new { x.Day, x.MerklCampaignId });
                    table.ForeignKey(
                        name: "FK_MerklCampaignSnapshots_MerklCampaigns_MerklCampaignId",
                        column: x => x.MerklCampaignId,
                        principalTable: "MerklCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MerklCampaignSnapshots_MerklCampaignId",
                table: "MerklCampaignSnapshots",
                column: "MerklCampaignId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerklCampaignSnapshots");

            migrationBuilder.DropTable(
                name: "MerklCampaigns");
        }
    }
}
