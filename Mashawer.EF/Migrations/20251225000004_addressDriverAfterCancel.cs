using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mashawer.EF.Migrations
{
    /// <inheritdoc />
    public partial class addressDriverAfterCancel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressDriverAfterCancel",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressDriverAfterCancel",
                table: "Orders");
        }
    }
}
