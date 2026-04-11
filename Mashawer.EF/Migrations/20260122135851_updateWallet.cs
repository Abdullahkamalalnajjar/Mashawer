using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mashawer.EF.Migrations
{
    /// <inheritdoc />
    public partial class updateWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisable",
                table: "Wallets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisable",
                table: "Wallets");
        }
    }
}
