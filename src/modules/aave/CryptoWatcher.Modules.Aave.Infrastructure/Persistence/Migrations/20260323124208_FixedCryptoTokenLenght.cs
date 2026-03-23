using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Modules.Aave.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixedCryptoTokenLenght : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token0_Symbol",
                schema: "aave",
                table: "AavePositions",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Token0_Symbol",
                schema: "aave",
                table: "AavePositionDailyPerformances",
                type: "character varying(16)",
                unicode: false,
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token0_Symbol",
                schema: "aave",
                table: "AavePositions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldUnicode: false,
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "Token0_Symbol",
                schema: "aave",
                table: "AavePositionDailyPerformances",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldUnicode: false,
                oldMaxLength: 16);
        }
    }
}
