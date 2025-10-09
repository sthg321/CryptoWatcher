using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniswapCashFlowEventsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PoolPositions_Networks_NetworkName",
                table: "PoolPositions");

            migrationBuilder.DropTable(
                name: "Networks");

            migrationBuilder.DropIndex(
                name: "IX_PoolPositions_NetworkName",
                table: "PoolPositions");

            migrationBuilder.AddColumn<int>(
                name: "ProtocolVersion",
                table: "PoolPositions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "TickLower",
                table: "PoolPositions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TickUpper",
                table: "PoolPositions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "PoolPositionCashFlow",
                columns: table => new
                {
                    PositionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    NetworkName = table.Column<string>(type: "character varying(32)", nullable: false),
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
                    table.PrimaryKey("PK_PoolPositionCashFlow", x => new { x.PositionId, x.Date, x.Event });
                    table.ForeignKey(
                        name: "FK_PoolPositionCashFlow_PoolPositions_PositionId_NetworkName",
                        columns: x => new { x.PositionId, x.NetworkName },
                        principalTable: "PoolPositions",
                        principalColumns: new[] { "PositionId", "NetworkName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniswapChainConfigurations",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ProtocolVersion = table.Column<int>(type: "integer", nullable: false),
                    RpcUrl = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SmartContractAddresses_NftManager = table.Column<string>(type: "character varying(42)", maxLength: 42, nullable: false),
                    SmartContractAddresses_PoolFactory = table.Column<string>(type: "character varying(42)", maxLength: 42, nullable: false),
                    SmartContractAddresses_MultiCall = table.Column<string>(type: "character varying(42)", maxLength: 42, nullable: false),
                    LastProcessedBlock = table.Column<string>(type: "text", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniswapChainConfigurations", x => new { x.Name, x.ProtocolVersion });
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoolPositions_NetworkName_ProtocolVersion",
                table: "PoolPositions",
                columns: new[] { "NetworkName", "ProtocolVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_PoolPositionCashFlow_PositionId_NetworkName",
                table: "PoolPositionCashFlow",
                columns: new[] { "PositionId", "NetworkName" });

            migrationBuilder.AddForeignKey(
                name: "FK_PoolPositions_UniswapChainConfigurations_NetworkName_Protoc~",
                table: "PoolPositions",
                columns: new[] { "NetworkName", "ProtocolVersion" },
                principalTable: "UniswapChainConfigurations",
                principalColumns: new[] { "Name", "ProtocolVersion" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PoolPositions_UniswapChainConfigurations_NetworkName_Protoc~",
                table: "PoolPositions");

            migrationBuilder.DropTable(
                name: "PoolPositionCashFlow");

            migrationBuilder.DropTable(
                name: "UniswapChainConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_PoolPositions_NetworkName_ProtocolVersion",
                table: "PoolPositions");

            migrationBuilder.DropColumn(
                name: "ProtocolVersion",
                table: "PoolPositions");

            migrationBuilder.DropColumn(
                name: "TickLower",
                table: "PoolPositions");

            migrationBuilder.DropColumn(
                name: "TickUpper",
                table: "PoolPositions");

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

            migrationBuilder.CreateIndex(
                name: "IX_PoolPositions_NetworkName",
                table: "PoolPositions",
                column: "NetworkName");

            migrationBuilder.AddForeignKey(
                name: "FK_PoolPositions_Networks_NetworkName",
                table: "PoolPositions",
                column: "NetworkName",
                principalTable: "Networks",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
