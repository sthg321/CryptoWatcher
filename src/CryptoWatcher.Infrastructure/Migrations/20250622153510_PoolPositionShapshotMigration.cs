using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PoolPositionShapshotMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoolPositionSnapshots",
                columns: table => new
                {
                    PoolPositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    IsInRange = table.Column<bool>(type: "boolean", nullable: false),
                    Token0_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolPositionSnapshots", x => new { x.PoolPositionId, x.NetworkName, x.Day });
                    table.ForeignKey(
                        name: "FK_PoolPositionSnapshots_PoolPositions_PoolPositionId_NetworkN~",
                        columns: x => new { x.PoolPositionId, x.NetworkName, x.Day },
                        principalTable: "PoolPositions",
                        principalColumns: new[] { "PositionId", "NetworkName", "Day" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoolPositionSnapshots");
        }
    }
}
