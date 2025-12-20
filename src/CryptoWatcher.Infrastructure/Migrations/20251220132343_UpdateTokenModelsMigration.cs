using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTokenModelsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token0_Symbol",
                table: "UniswapPositionDailyPerformances");

            migrationBuilder.DropColumn(
                name: "Token1_Symbol",
                table: "UniswapPositionDailyPerformances");

            migrationBuilder.DropColumn(
                name: "Token0_Symbol",
                table: "UniswapLiquidityPositionSnapshots");

            migrationBuilder.DropColumn(
                name: "Token1_Symbol",
                table: "UniswapLiquidityPositionSnapshots");

            migrationBuilder.DropColumn(
                name: "Token0_FeeAmount",
                table: "UniswapLiquidityPositionCashFlows");

            migrationBuilder.DropColumn(
                name: "Token0_Symbol",
                table: "UniswapLiquidityPositionCashFlows");

            migrationBuilder.DropColumn(
                name: "Token1_FeeAmount",
                table: "UniswapLiquidityPositionCashFlows");

            migrationBuilder.DropColumn(
                name: "Token1_Symbol",
                table: "UniswapLiquidityPositionCashFlows");

            migrationBuilder.DropColumn(
                name: "Token_Symbol",
                table: "HyperliquidVaultPositionSnapshots");

            migrationBuilder.DropColumn(
                name: "Token_Symbol",
                table: "HyperliquidPositionCashFlows");

            migrationBuilder.DropColumn(
                name: "Token_Symbol",
                table: "AavePositionSnapshots");

            migrationBuilder.DropColumn(
                name: "Token_Symbol",
                table: "AavePositionCashFlows");

            migrationBuilder.RenameColumn(
                name: "Token1_FeeAmount",
                table: "UniswapPositionDailyPerformances",
                newName: "Token1_Fee");

            migrationBuilder.RenameColumn(
                name: "Token0_FeeAmount",
                table: "UniswapPositionDailyPerformances",
                newName: "Token0_Fee");

            migrationBuilder.RenameColumn(
                name: "Token1_FeeAmount",
                table: "UniswapLiquidityPositionSnapshots",
                newName: "Token1_Fee");

            migrationBuilder.RenameColumn(
                name: "Token0_FeeAmount",
                table: "UniswapLiquidityPositionSnapshots",
                newName: "Token0_Fee");

            migrationBuilder.RenameColumn(
                name: "Token_PriceInUsd",
                table: "HyperliquidVaultPositionSnapshots",
                newName: "Token0_PriceInUsd");

            migrationBuilder.RenameColumn(
                name: "Token_Amount",
                table: "HyperliquidVaultPositionSnapshots",
                newName: "Token0_Amount");

            migrationBuilder.RenameColumn(
                name: "Token_PriceInUsd",
                table: "HyperliquidPositionCashFlows",
                newName: "Token0_PriceInUsd");

            migrationBuilder.RenameColumn(
                name: "Token_Amount",
                table: "HyperliquidPositionCashFlows",
                newName: "Token0_Amount");

            migrationBuilder.RenameColumn(
                name: "Token_PriceInUsd",
                table: "AavePositionSnapshots",
                newName: "Token0_PriceInUsd");

            migrationBuilder.RenameColumn(
                name: "Token_Amount",
                table: "AavePositionSnapshots",
                newName: "Token0_Amount");

            migrationBuilder.RenameColumn(
                name: "Token_Symbol",
                table: "AavePositionDailyPerformances",
                newName: "Token0_Symbol");

            migrationBuilder.RenameColumn(
                name: "Token_PriceInUsd",
                table: "AavePositionDailyPerformances",
                newName: "Token0_PriceInUsd");

            migrationBuilder.RenameColumn(
                name: "Token_Amount",
                table: "AavePositionDailyPerformances",
                newName: "Token0_Amount");

            migrationBuilder.RenameColumn(
                name: "Token_PriceInUsd",
                table: "AavePositionCashFlows",
                newName: "Token0_PriceInUsd");

            migrationBuilder.RenameColumn(
                name: "Token_Amount",
                table: "AavePositionCashFlows",
                newName: "Token0_Amount");

            migrationBuilder.AddColumn<string>(
                name: "Token0_Address",
                table: "UniswapLiquidityPositions",
                type: "character(42)",
                unicode: false,
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token1_Address",
                table: "UniswapLiquidityPositions",
                type: "character(42)",
                unicode: false,
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token0_Address",
                table: "HyperliquidVaultPositions",
                type: "character(42)",
                unicode: false,
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Token0_Amount",
                table: "HyperliquidVaultPositions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Token0_PriceInUsd",
                table: "HyperliquidVaultPositions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Token0_Symbol",
                table: "HyperliquidVaultPositions",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token0_Address",
                table: "AavePositionDailyPerformances",
                type: "character(42)",
                unicode: false,
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                defaultValue: "");
            
            migrationBuilder.Sql("""
                                 UPDATE "HyperliquidVaultPositions"
                                 SET "Token0_Address" = '0xaf88d065e77c8cC2239327C5EDb3A432268e5831'
                                 """);
            
              migrationBuilder.Sql("""
                                 UPDATE "UniswapLiquidityPositions"
                                 SET "Token0_Address" = '0x0000000000000000000000000000000000000000'
                                 WHERE "NetworkName" = 'Arbitrum' AND "Token0_Symbol" = 'ETH'
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "UniswapLiquidityPositions"
                                 SET "Token1_Address" = '0xaf88d065e77c8cC2239327C5EDb3A432268e5831'
                                 WHERE "NetworkName" = 'Arbitrum' AND "Token1_Symbol" = 'USDC'
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "UniswapLiquidityPositions"
                                 SET "Token0_Address" = '0x82aF49447D8a07e3bd95BD0d56f35241523fBab1'
                                 WHERE "NetworkName" = 'Arbitrum' AND "Token0_Symbol" = 'WETH'
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "UniswapLiquidityPositions"
                                 SET "Token0_Address" = '0x2f2a2543B76A4166549F7aaB2e75Bef0aefC5B0f'
                                 WHERE "NetworkName" = 'Arbitrum' AND "Token0_Symbol" = 'WBTC'
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "UniswapLiquidityPositions"
                                 SET "Token0_Address" = '0x0000000000000000000000000000000000000000'
                                 WHERE "NetworkName" = 'Unichain' AND "Token0_Symbol" = 'ETH'
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "UniswapLiquidityPositions"
                                 SET "Token1_Address" = '0x927B51f251480a681271180DA4de28D44EC4AfB8'
                                 WHERE "NetworkName" = 'Unichain' AND "Token1_Symbol" = 'WBTC'
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "UniswapLiquidityPositions"
                                 SET "Token0_Address" = '0x078D782b760474a361dDA0AF3839290b0EF57AD6'
                                 WHERE "NetworkName" = 'Unichain' AND "Token0_Symbol" = 'USDC'
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token0_Address",
                table: "UniswapLiquidityPositions");

            migrationBuilder.DropColumn(
                name: "Token1_Address",
                table: "UniswapLiquidityPositions");

            migrationBuilder.DropColumn(
                name: "Token0_Address",
                table: "HyperliquidVaultPositions");

            migrationBuilder.DropColumn(
                name: "Token0_Amount",
                table: "HyperliquidVaultPositions");

            migrationBuilder.DropColumn(
                name: "Token0_PriceInUsd",
                table: "HyperliquidVaultPositions");

            migrationBuilder.DropColumn(
                name: "Token0_Symbol",
                table: "HyperliquidVaultPositions");

            migrationBuilder.DropColumn(
                name: "Token0_Address",
                table: "AavePositionDailyPerformances");

            migrationBuilder.RenameColumn(
                name: "Token1_Fee",
                table: "UniswapPositionDailyPerformances",
                newName: "Token1_FeeAmount");

            migrationBuilder.RenameColumn(
                name: "Token0_Fee",
                table: "UniswapPositionDailyPerformances",
                newName: "Token0_FeeAmount");

            migrationBuilder.RenameColumn(
                name: "Token1_Fee",
                table: "UniswapLiquidityPositionSnapshots",
                newName: "Token1_FeeAmount");

            migrationBuilder.RenameColumn(
                name: "Token0_Fee",
                table: "UniswapLiquidityPositionSnapshots",
                newName: "Token0_FeeAmount");

            migrationBuilder.RenameColumn(
                name: "Token0_PriceInUsd",
                table: "HyperliquidVaultPositionSnapshots",
                newName: "Token_PriceInUsd");

            migrationBuilder.RenameColumn(
                name: "Token0_Amount",
                table: "HyperliquidVaultPositionSnapshots",
                newName: "Token_Amount");

            migrationBuilder.RenameColumn(
                name: "Token0_PriceInUsd",
                table: "HyperliquidPositionCashFlows",
                newName: "Token_PriceInUsd");

            migrationBuilder.RenameColumn(
                name: "Token0_Amount",
                table: "HyperliquidPositionCashFlows",
                newName: "Token_Amount");

            migrationBuilder.RenameColumn(
                name: "Token0_PriceInUsd",
                table: "AavePositionSnapshots",
                newName: "Token_PriceInUsd");

            migrationBuilder.RenameColumn(
                name: "Token0_Amount",
                table: "AavePositionSnapshots",
                newName: "Token_Amount");

            migrationBuilder.RenameColumn(
                name: "Token0_Symbol",
                table: "AavePositionDailyPerformances",
                newName: "Token_Symbol");

            migrationBuilder.RenameColumn(
                name: "Token0_PriceInUsd",
                table: "AavePositionDailyPerformances",
                newName: "Token_PriceInUsd");

            migrationBuilder.RenameColumn(
                name: "Token0_Amount",
                table: "AavePositionDailyPerformances",
                newName: "Token_Amount");

            migrationBuilder.RenameColumn(
                name: "Token0_PriceInUsd",
                table: "AavePositionCashFlows",
                newName: "Token_PriceInUsd");

            migrationBuilder.RenameColumn(
                name: "Token0_Amount",
                table: "AavePositionCashFlows",
                newName: "Token_Amount");

            migrationBuilder.AddColumn<string>(
                name: "Token0_Symbol",
                table: "UniswapPositionDailyPerformances",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token1_Symbol",
                table: "UniswapPositionDailyPerformances",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token0_Symbol",
                table: "UniswapLiquidityPositionSnapshots",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token1_Symbol",
                table: "UniswapLiquidityPositionSnapshots",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Token0_FeeAmount",
                table: "UniswapLiquidityPositionCashFlows",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Token0_Symbol",
                table: "UniswapLiquidityPositionCashFlows",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Token1_FeeAmount",
                table: "UniswapLiquidityPositionCashFlows",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Token1_Symbol",
                table: "UniswapLiquidityPositionCashFlows",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token_Symbol",
                table: "HyperliquidVaultPositionSnapshots",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token_Symbol",
                table: "HyperliquidPositionCashFlows",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token_Symbol",
                table: "AavePositionSnapshots",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token_Symbol",
                table: "AavePositionCashFlows",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");
        }
    }
}
