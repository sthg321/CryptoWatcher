using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMerklCampaignCashFlowMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MerklCampaignCashFlows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MerklCampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClaimedAmount_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    ClaimedAmount_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerklCampaignCashFlows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerklCampaignCashFlows_MerklCampaigns_MerklCampaignId",
                        column: x => x.MerklCampaignId,
                        principalTable: "MerklCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MerklCampaignCashFlows_MerklCampaignId",
                table: "MerklCampaignCashFlows",
                column: "MerklCampaignId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerklCampaignCashFlows");
        }
    }
}
