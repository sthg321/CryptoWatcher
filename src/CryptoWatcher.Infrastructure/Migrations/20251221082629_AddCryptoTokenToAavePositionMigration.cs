using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCryptoTokenToAavePositionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenAddress",
                table: "AavePositions",
                newName: "Token0_Address");

            migrationBuilder.AddColumn<decimal>(
                name: "Token0_Amount",
                table: "AavePositions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Token0_PriceInUsd",
                table: "AavePositions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Token0_Symbol",
                table: "AavePositions",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                                 UPDATE "AavePositions"
                                 SET "Token0_Amount" = 1
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "AavePositions"
                                 SET "Token0_Symbol" = 'WETH'
                                 WHERE "Token0_Address" = '0x4200000000000000000000000000000000000006';
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "AavePositions"
                                 SET "Token0_Symbol" = 'CELO'
                                 WHERE "Token0_Address" = '0x471EcE3750Da237f93B8E339c536989b8978a438';
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "AavePositions"
                                 SET "Token0_Symbol" = 'WETH'
                                 WHERE "Token0_Address" = '0xD221812de1BD094f35587EE8E174B07B6167D9Af';
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "AavePositions"
                                 SET "Token0_Symbol" = 'USDT'
                                 WHERE "Token0_Address" = '0x48065fbBE25f71C9282ddf5e1cD6D6A887483D5e';
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "AavePositions"
                                 SET "Token0_Symbol" = 'SAVAX'
                                 WHERE "Token0_Address" = '0x2b2C81e08f1Af8835a78Bb2A90AE924ACE0eA4bE';
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "AavePositions"
                                 SET "Token0_Symbol" = 'USDT'
                                 WHERE "Token0_Address" = '0x9702230A8Ea53601f5cD2dc00fDBc13d4dF4A8c7';
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "AavePositions"
                                 SET "Token0_Symbol" = 'GHO'
                                 WHERE "Token0_Address" = '0xfc421aD3C883Bf9E7C4f42dE845C4e4405799e73';
                                 """);
            
            migrationBuilder.Sql("""
                                 UPDATE "AavePositions"
                                 SET "Token0_Symbol" = 'EURC'
                                 WHERE "Token0_Address" = '0xC891EB4cbdEFf6e073e859e987815Ed1505c2ACD';
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token0_Amount",
                table: "AavePositions");

            migrationBuilder.DropColumn(
                name: "Token0_PriceInUsd",
                table: "AavePositions");

            migrationBuilder.DropColumn(
                name: "Token0_Symbol",
                table: "AavePositions");

            migrationBuilder.RenameColumn(
                name: "Token0_Address",
                table: "AavePositions",
                newName: "TokenAddress");
        }
    }
}
