using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLltvToAaveSnapshotsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LiquidationLtv",
                table: "AavePositionSnapshots",
                type: "double precision",
                nullable: true);


            migrationBuilder.Sql("""
                                 UPDATE "AavePositionSnapshots" 
                                 SET "LiquidationLtv" = 0.84
                                 WHERE "PositionId" IN 
                                       (SELECT "Id" from "AavePositions" 
                                                    WHERE "Token0_Symbol" = 'ETH' or "Token0_Symbol" = 'wstETH' or "Token0_Symbol" = 'WETH');
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "AavePositionSnapshots" 
                                 SET "LiquidationLtv" = 0.78
                                 WHERE "PositionId" IN 
                                       (SELECT "Id" from "AavePositions" 
                                                    WHERE "Token0_Symbol" = 'WBTC');
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LiquidationLtv",
                table: "AavePositionSnapshots");
        }
    }
}
