using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLiquidationLltvToMorphoMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LiquidationLtv",
                table: "MorphoMarketPositionSnapshots",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.Sql("""
                                 UPDATE "MorphoMarketPositionSnapshots" 
                                 SET "LiquidationLtv" = 0.86
                                 WHERE "MorphoMarketPositionId" IN 
                                       (SELECT "Id" from "MorphoMarketPositions" 
                                                    WHERE "CollateralToken_Symbol" = 'ETH' or "CollateralToken_Symbol" = 'wstETH' or "CollateralToken_Symbol" = 'WETH');
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "MorphoMarketPositionSnapshots" 
                                 SET "LiquidationLtv" = 0.86
                                 WHERE "MorphoMarketPositionId" IN 
                                       (SELECT "Id" from "MorphoMarketPositions" 
                                                    WHERE "CollateralToken_Symbol" = 'WBTC');
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LiquidationLtv",
                table: "MorphoMarketPositionSnapshots");
        }
    }
}
