using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AavePositionEventsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PreviousScaledAmount",
                table: "AavePositions",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AavePositionEvent",
                columns: table => new
                {
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    Token_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AavePositionEvent", x => new { x.PositionId, x.Date, x.EventType });
                    table.ForeignKey(
                        name: "FK_AavePositionEvent_AavePositions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "AavePositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AavePositionEvent");

            migrationBuilder.DropColumn(
                name: "PreviousScaledAmount",
                table: "AavePositions");
        }
    }
}
