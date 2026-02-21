using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveAaveHelathFactorToAaveAccountShapshotMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HealthFactor",
                table: "AavePositionSnapshots");

            migrationBuilder.CreateTable(
                name: "AaveAccountSnapshots",
                columns: table => new
                {
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    HealthFactor = table.Column<double>(type: "double precision", nullable: false),
                    TotalCollateralInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalDebtInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AaveAccountSnapshots", x => new { x.WalletAddress, x.NetworkName, x.Day });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AaveAccountSnapshots");

            migrationBuilder.AddColumn<double>(
                name: "HealthFactor",
                table: "AavePositionSnapshots",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
