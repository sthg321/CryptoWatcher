using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWatcher.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeLastTransactionNullabelMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastTransactionHash",
                table: "UniswapSynchronizationState",
                type: "character(66)",
                unicode: false,
                fixedLength: true,
                maxLength: 66,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(66)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 66);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastTransactionHash",
                table: "UniswapSynchronizationState",
                type: "character(66)",
                unicode: false,
                fixedLength: true,
                maxLength: 66,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character(66)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 66,
                oldNullable: true);
        }
    }
}
