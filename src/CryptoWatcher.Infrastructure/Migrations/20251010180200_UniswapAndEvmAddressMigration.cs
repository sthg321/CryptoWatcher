using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniswapAndEvmAddressMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AavePositionEvent_AavePositions_PositionId",
                table: "AavePositionEvent");

            migrationBuilder.DropTable(
                name: "PoolPositionSnapshots");

            migrationBuilder.DropTable(
                name: "PoolPositions");

            migrationBuilder.DropTable(
                name: "Networks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AavePositionEvent",
                table: "AavePositionEvent");

            migrationBuilder.RenameTable(
                name: "AavePositionEvent",
                newName: "AavePositionEvents");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Wallets",
                type: "character(42)",
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "WalletAddress",
                table: "HyperliquidVaultPositionSnapshots",
                type: "character(42)",
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "VaultAddress",
                table: "HyperliquidVaultPositionSnapshots",
                type: "character(42)",
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "WalletAddress",
                table: "HyperliquidVaultPositions",
                type: "character(42)",
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "VaultAddress",
                table: "HyperliquidVaultPositions",
                type: "character(42)",
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "WalletAddress",
                table: "HyperliquidVaultEvents",
                type: "character(42)",
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "VaultAddress",
                table: "HyperliquidVaultEvents",
                type: "character(42)",
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "WalletAddress",
                table: "AavePositions",
                type: "character(42)",
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "TokenAddress",
                table: "AavePositions",
                type: "character(42)",
                fixedLength: true,
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AavePositionEvents",
                table: "AavePositionEvents",
                columns: new[] { "PositionId", "Date", "Event" });

            migrationBuilder.CreateTable(
                name: "UniswapChainConfigurations",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ProtocolVersion = table.Column<int>(type: "integer", nullable: false),
                    RpcUrl = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SmartContractAddresses_NftManager = table.Column<string>(type: "character(42)", fixedLength: true, maxLength: 42, nullable: false),
                    SmartContractAddresses_PoolFactory = table.Column<string>(type: "character(42)", fixedLength: true, maxLength: 42, nullable: false),
                    SmartContractAddresses_MultiCall = table.Column<string>(type: "character(42)", fixedLength: true, maxLength: 42, nullable: false),
                    LastProcessedBlock = table.Column<string>(type: "text", nullable: false),
                    LastProcessedBlockUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapChainConfigurations", x => new { x.Name, x.ProtocolVersion });
                });

            migrationBuilder.CreateTable(
                name: "UniswapLiquidityPositions",
                columns: table => new
                {
                    PositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    TickLower = table.Column<long>(type: "bigint", nullable: false),
                    TickUpper = table.Column<long>(type: "bigint", nullable: false),
                    Token0_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", fixedLength: true, maxLength: 42, nullable: false),
                    ProtocolVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapLiquidityPositions", x => new { x.PositionId, x.NetworkName });
                    table.ForeignKey(
                        name: "FK_UniswapLiquidityPositions_UniswapChainConfigurations_Networ~",
                        columns: x => new { x.NetworkName, x.ProtocolVersion },
                        principalTable: "UniswapChainConfigurations",
                        principalColumns: new[] { "Name", "ProtocolVersion" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniswapLiquidityPositions_Wallets_WalletAddress",
                        column: x => x.WalletAddress,
                        principalTable: "Wallets",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LiquidityPositionCashFlows",
                columns: table => new
                {
                    PositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", nullable: false),
                    TransactionHash = table.Column<string>(type: "character varying(66)", maxLength: 66, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    Token0_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidityPositionCashFlows", x => new { x.PositionId, x.NetworkName, x.TransactionHash });
                    table.ForeignKey(
                        name: "FK_LiquidityPositionCashFlows_UniswapLiquidityPositions_Positi~",
                        columns: x => new { x.PositionId, x.NetworkName },
                        principalTable: "UniswapLiquidityPositions",
                        principalColumns: new[] { "PositionId", "NetworkName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniswapLiquidityPositionSnapshots",
                columns: table => new
                {
                    PoolPositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    IsInRange = table.Column<bool>(type: "boolean", nullable: false),
                    Token0_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapLiquidityPositionSnapshots", x => new { x.PoolPositionId, x.NetworkName, x.Day });
                    table.ForeignKey(
                        name: "FK_UniswapLiquidityPositionSnapshots_UniswapLiquidityPositions~",
                        columns: x => new { x.PoolPositionId, x.NetworkName },
                        principalTable: "UniswapLiquidityPositions",
                        principalColumns: new[] { "PositionId", "NetworkName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UniswapLiquidityPositions_NetworkName_ProtocolVersion",
                table: "UniswapLiquidityPositions",
                columns: new[] { "NetworkName", "ProtocolVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_UniswapLiquidityPositions_WalletAddress",
                table: "UniswapLiquidityPositions",
                column: "WalletAddress");

            migrationBuilder.AddForeignKey(
                name: "FK_AavePositionEvents_AavePositions_PositionId",
                table: "AavePositionEvents",
                column: "PositionId",
                principalTable: "AavePositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AavePositionEvents_AavePositions_PositionId",
                table: "AavePositionEvents");

            migrationBuilder.DropTable(
                name: "LiquidityPositionCashFlows");

            migrationBuilder.DropTable(
                name: "UniswapLiquidityPositionSnapshots");

            migrationBuilder.DropTable(
                name: "UniswapLiquidityPositions");

            migrationBuilder.DropTable(
                name: "UniswapChainConfigurations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AavePositionEvents",
                table: "AavePositionEvents");

            migrationBuilder.RenameTable(
                name: "AavePositionEvents",
                newName: "AavePositionEvent");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Wallets",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(42)",
                oldFixedLength: true,
                oldMaxLength: 42);

            migrationBuilder.AlterColumn<string>(
                name: "WalletAddress",
                table: "HyperliquidVaultPositionSnapshots",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(42)",
                oldFixedLength: true,
                oldMaxLength: 42);

            migrationBuilder.AlterColumn<string>(
                name: "VaultAddress",
                table: "HyperliquidVaultPositionSnapshots",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(42)",
                oldFixedLength: true,
                oldMaxLength: 42);

            migrationBuilder.AlterColumn<string>(
                name: "WalletAddress",
                table: "HyperliquidVaultPositions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(42)",
                oldFixedLength: true,
                oldMaxLength: 42);

            migrationBuilder.AlterColumn<string>(
                name: "VaultAddress",
                table: "HyperliquidVaultPositions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(42)",
                oldFixedLength: true,
                oldMaxLength: 42);

            migrationBuilder.AlterColumn<string>(
                name: "WalletAddress",
                table: "HyperliquidVaultEvents",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(42)",
                oldFixedLength: true,
                oldMaxLength: 42);

            migrationBuilder.AlterColumn<string>(
                name: "VaultAddress",
                table: "HyperliquidVaultEvents",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(42)",
                oldFixedLength: true,
                oldMaxLength: 42);

            migrationBuilder.AlterColumn<string>(
                name: "WalletAddress",
                table: "AavePositions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(42)",
                oldFixedLength: true,
                oldMaxLength: 42);

            migrationBuilder.AlterColumn<string>(
                name: "TokenAddress",
                table: "AavePositions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(42)",
                oldFixedLength: true,
                oldMaxLength: 42);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AavePositionEvent",
                table: "AavePositionEvent",
                columns: new[] { "PositionId", "Date", "Event" });

            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    MultiCallAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NftManagerAddress = table.Column<string>(type: "character varying(266)", maxLength: 266, nullable: false),
                    PoolFactoryAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ProtocolVersion = table.Column<int>(type: "integer", nullable: false),
                    RpcUrl = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "PoolPositions",
                columns: table => new
                {
                    PositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    WalletAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Symbol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolPositions", x => new { x.PositionId, x.NetworkName });
                    table.ForeignKey(
                        name: "FK_PoolPositions_Networks_NetworkName",
                        column: x => x.NetworkName,
                        principalTable: "Networks",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PoolPositions_Wallets_WalletAddress",
                        column: x => x.WalletAddress,
                        principalTable: "Wallets",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PoolPositionSnapshots",
                columns: table => new
                {
                    PoolPositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    IsInRange = table.Column<bool>(type: "boolean", nullable: false),
                    Token0_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token0_Symbol = table.Column<string>(type: "text", nullable: false),
                    Token1_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token1_Symbol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolPositionSnapshots", x => new { x.PoolPositionId, x.NetworkName, x.Day });
                    table.ForeignKey(
                        name: "FK_PoolPositionSnapshots_PoolPositions_PoolPositionId_NetworkN~",
                        columns: x => new { x.PoolPositionId, x.NetworkName },
                        principalTable: "PoolPositions",
                        principalColumns: new[] { "PositionId", "NetworkName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoolPositions_NetworkName",
                table: "PoolPositions",
                column: "NetworkName");

            migrationBuilder.CreateIndex(
                name: "IX_PoolPositions_WalletAddress",
                table: "PoolPositions",
                column: "WalletAddress");

            migrationBuilder.AddForeignKey(
                name: "FK_AavePositionEvent_AavePositions_PositionId",
                table: "AavePositionEvent",
                column: "PositionId",
                principalTable: "AavePositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
