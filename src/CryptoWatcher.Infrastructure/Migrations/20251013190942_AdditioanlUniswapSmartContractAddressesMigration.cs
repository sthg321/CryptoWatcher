using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdditioanlUniswapSmartContractAddressesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockscoutUrl",
                table: "UniswapChainConfigurations",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RpcAuthToken",
                table: "UniswapChainConfigurations",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmartContractAddresses_PositionManager",
                table: "UniswapChainConfigurations",
                type: "character(42)",
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockscoutUrl",
                table: "UniswapChainConfigurations");

            migrationBuilder.DropColumn(
                name: "RpcAuthToken",
                table: "UniswapChainConfigurations");

            migrationBuilder.DropColumn(
                name: "SmartContractAddresses_PositionManager",
                table: "UniswapChainConfigurations");
        }
    }
}
