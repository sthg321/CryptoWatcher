using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUsdColumnSetMaxLengthForTokensMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token1_Symbol",
                table: "UniswapLiquidityPositions",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Token0_Symbol",
                table: "UniswapLiquidityPositions",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Token1_Symbol",
                table: "UniswapLiquidityPositionCashFlows",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Token0_Symbol",
                table: "UniswapLiquidityPositionCashFlows",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "Token_Amount",
                table: "HyperliquidVaultPositionSnapshots",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Token_PriceInUsd",
                table: "HyperliquidVaultPositionSnapshots",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Token_Symbol",
                table: "HyperliquidVaultPositionSnapshots",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AavePositionCashFlows",
                columns: table => new
                {
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    Token_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token_Symbol = table.Column<string>(type: "character varying(16)", unicode: false, maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AavePositionCashFlows", x => new { x.PositionId, x.Date, x.Event });
                    table.ForeignKey(
                        name: "FK_AavePositionCashFlows_AavePositions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "AavePositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HyperliquidPositionCashFlows",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VaultAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", unicode: false, fixedLength: true, maxLength: 42, nullable: false),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    Token_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token_Symbol = table.Column<string>(type: "character varying(16)", unicode: false, maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperliquidPositionCashFlows", x => new { x.VaultAddress, x.WalletAddress, x.Date });
                    table.ForeignKey(
                        name: "FK_HyperliquidPositionCashFlows_HyperliquidVaultPositions_Vaul~",
                        columns: x => new { x.VaultAddress, x.WalletAddress },
                        principalTable: "HyperliquidVaultPositions",
                        principalColumns: new[] { "VaultAddress", "WalletAddress" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HyperliquidPositionCashFlows_Wallets_WalletAddress",
                        column: x => x.WalletAddress,
                        principalTable: "Wallets",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HyperliquidPositionCashFlows_WalletAddress",
                table: "HyperliquidPositionCashFlows",
                column: "WalletAddress");
            
            migrationBuilder.Sql("""
                                 INSERT INTO "HyperliquidPositionCashFlows" ("VaultAddress", "WalletAddress", "Date", "Event", "Token_Amount", "Token_PriceInUsd", "Token_Symbol")
                                 SELECT "VaultAddress", "WalletAddress", "Date", "Event", "Usd", 1, 'USDC' FROM "HyperliquidVaultEvents"
                                 """);

            migrationBuilder.Sql("""
                                 INSERT INTO "AavePositionCashFlows" ("PositionId", "Date", "Event", "Token_Amount", "Token_PriceInUsd", "Token_Symbol" )
                                 SELECT "PositionId", "Date", "Event", "Token_Amount", "Token_PriceInUsd", "Token_Symbol" FROM "AavePositionEvents"
                                 """);

            migrationBuilder.Sql("""
                                 UPDATE "HyperliquidVaultPositionSnapshots" 
                                 SET "Token_Symbol" = 'USDC',
                                     "Token_PriceInUsd" = 1 ,
                                     "Token_Amount" = "Balance"
                                 """);
            
            migrationBuilder.DropTable(
                name: "AavePositionEvents");

            migrationBuilder.DropTable(
                name: "HyperliquidVaultEvents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AavePositionCashFlows");

            migrationBuilder.DropTable(
                name: "HyperliquidPositionCashFlows");

            migrationBuilder.DropColumn(
                name: "Token_Amount",
                table: "HyperliquidVaultPositionSnapshots");

            migrationBuilder.DropColumn(
                name: "Token_PriceInUsd",
                table: "HyperliquidVaultPositionSnapshots");

            migrationBuilder.DropColumn(
                name: "Token_Symbol",
                table: "HyperliquidVaultPositionSnapshots");

            migrationBuilder.AlterColumn<string>(
                name: "Token1_Symbol",
                table: "UniswapLiquidityPositions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldUnicode: false,
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "Token0_Symbol",
                table: "UniswapLiquidityPositions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldUnicode: false,
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "Token1_Symbol",
                table: "UniswapLiquidityPositionCashFlows",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldUnicode: false,
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "Token0_Symbol",
                table: "UniswapLiquidityPositionCashFlows",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldUnicode: false,
                oldMaxLength: 16);

            migrationBuilder.CreateTable(
                name: "AavePositionEvents",
                columns: table => new
                {
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    Token_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Token_PriceInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    Token_Symbol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AavePositionEvents", x => new { x.PositionId, x.Date, x.Event });
                    table.ForeignKey(
                        name: "FK_AavePositionEvents_AavePositions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "AavePositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HyperliquidVaultEvents",
                columns: table => new
                {
                    VaultAddress = table.Column<string>(type: "character(42)", fixedLength: true, maxLength: 42, nullable: false),
                    WalletAddress = table.Column<string>(type: "character(42)", fixedLength: true, maxLength: 42, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    Usd = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperliquidVaultEvents", x => new { x.VaultAddress, x.WalletAddress, x.Date });
                    table.ForeignKey(
                        name: "FK_HyperliquidVaultEvents_HyperliquidVaultPositions_VaultAddre~",
                        columns: x => new { x.VaultAddress, x.WalletAddress },
                        principalTable: "HyperliquidVaultPositions",
                        principalColumns: new[] { "VaultAddress", "WalletAddress" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HyperliquidVaultEvents_Wallets_WalletAddress",
                        column: x => x.WalletAddress,
                        principalTable: "Wallets",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HyperliquidVaultEvents_WalletAddress",
                table: "HyperliquidVaultEvents",
                column: "WalletAddress");
        }
    }
}
