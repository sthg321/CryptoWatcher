using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMorphoTablesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MorphoMarketPositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketExternalId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChainId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    CollateralToken_Address = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    CollateralToken_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CollateralToken_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    CollateralToken_Symbol = table.Column<string>(type: "character varying(16)", unicode: false, maxLength: 16, nullable: false),
                    LoanToken_Address = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    LoanToken_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanToken_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanToken_Symbol = table.Column<string>(type: "character varying(16)", unicode: false, maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MorphoMarketPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MorphoMarketPositionCashFlow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    MorphoMarketPositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MorphoMarketPositionCashFlow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MorphoMarketPositionCashFlow_MorphoMarketPositions_MorphoMa~",
                        column: x => x.MorphoMarketPositionId,
                        principalTable: "MorphoMarketPositions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MorphoMarketPositionSnapshots",
                columns: table => new
                {
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    MorphoMarketPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    HealthFactor = table.Column<double>(type: "double precision", nullable: false),
                    CollateralToken_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CollateralToken_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    LoadToken_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    LoadToken_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MorphoMarketPositionSnapshots", x => new { x.Day, x.MorphoMarketPositionId });
                    table.ForeignKey(
                        name: "FK_MorphoMarketPositionSnapshots_MorphoMarketPositions_MorphoM~",
                        column: x => x.MorphoMarketPositionId,
                        principalTable: "MorphoMarketPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MorphoMarketPositionCashFlow_MorphoMarketPositionId",
                table: "MorphoMarketPositionCashFlow",
                column: "MorphoMarketPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_MorphoMarketPositionSnapshots_MorphoMarketPositionId",
                table: "MorphoMarketPositionSnapshots",
                column: "MorphoMarketPositionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MorphoMarketPositionCashFlow");

            migrationBuilder.DropTable(
                name: "MorphoMarketPositionSnapshots");

            migrationBuilder.DropTable(
                name: "MorphoMarketPositions");
        }
    }
}
