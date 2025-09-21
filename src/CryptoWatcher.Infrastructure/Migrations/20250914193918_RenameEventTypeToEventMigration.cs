using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameEventTypeToEventMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventType",
                table: "HyperliquidVaultEvents",
                newName: "Event");

            migrationBuilder.RenameColumn(
                name: "EventType",
                table: "AavePositionEvent",
                newName: "Event");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Event",
                table: "HyperliquidVaultEvents",
                newName: "EventType");

            migrationBuilder.RenameColumn(
                name: "Event",
                table: "AavePositionEvent",
                newName: "EventType");
        }
    }
}
