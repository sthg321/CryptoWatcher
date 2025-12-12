using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStartedAtAndClosedAtForUniswapPositionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ClosedAt",
                table: "UniswapLiquidityPositions",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CreatedAt",
                table: "UniswapLiquidityPositions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.Sql("""
                                 delete from "UniswapLiquidityPositions" 
                                 where (select count(*) from "UniswapLiquidityPositionSnapshots"
                                   	   where "UniswapLiquidityPositions"."PositionId" = "UniswapLiquidityPositionSnapshots"."PoolPositionId" and 
                                             "UniswapLiquidityPositions"."NetworkName" = "UniswapLiquidityPositionSnapshots"."NetworkName") = 0
                                 """);
            
            migrationBuilder.Sql("""
                                 update "UniswapLiquidityPositions"
                                 set "CreatedAt" = (select min("Day") 
                                                    from "UniswapLiquidityPositionSnapshots"
                                                    where "UniswapLiquidityPositions"."PositionId" = "UniswapLiquidityPositionSnapshots"."PoolPositionId" and 
                                                          "UniswapLiquidityPositions"."NetworkName" = "UniswapLiquidityPositionSnapshots"."NetworkName"
                                                    )
                                 where "PositionId" != 5148887
                                 """);
            
            migrationBuilder.Sql("""
                                 update "UniswapLiquidityPositions"
                                 set "ClosedAt" = (select MAX("Day") 
                                                    from "UniswapLiquidityPositionSnapshots"
                                                    where "UniswapLiquidityPositions"."PositionId" = "UniswapLiquidityPositionSnapshots"."PoolPositionId" and 
                                                          "UniswapLiquidityPositions"."NetworkName" = "UniswapLiquidityPositionSnapshots"."NetworkName" and
                                                          "UniswapLiquidityPositions"."IsActive" = false 
                                                    )
                                                    where "PositionId" != 5148887
                                 """);
            
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UniswapLiquidityPositions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "UniswapLiquidityPositions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UniswapLiquidityPositions");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UniswapLiquidityPositions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
