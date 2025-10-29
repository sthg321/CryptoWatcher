using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAavePositionPeriodMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAtDay",
                table: "AavePositions");

            migrationBuilder.DropColumn(
                name: "CreatedAtDay",
                table: "AavePositions");

            migrationBuilder.CreateTable(
                name: "AavePositionPeriod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAtDay = table.Column<DateOnly>(type: "date", nullable: false),
                    ClosedAtDay = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AavePositionPeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AavePositionPeriod_AavePositions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "AavePositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AavePositionPeriod_PositionId_ClosedAtDay",
                table: "AavePositionPeriod",
                columns: new[] { "PositionId", "ClosedAtDay" },
                unique: true,
                filter: " \"ClosedAtDay\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AavePositionPeriod");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ClosedAtDay",
                table: "AavePositions",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CreatedAtDay",
                table: "AavePositions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
