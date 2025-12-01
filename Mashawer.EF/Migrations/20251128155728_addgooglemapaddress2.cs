using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mashawer.EF.Migrations
{
    /// <inheritdoc />
    public partial class addgooglemapaddress2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GoogleMapAddress",
                table: "OrderTasks",
                newName: "GoogleMapAddressTo");

            migrationBuilder.AddColumn<string>(
                name: "GoogleMapAddressFrom",
                table: "OrderTasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleMapAddressFrom",
                table: "OrderTasks");

            migrationBuilder.RenameColumn(
                name: "GoogleMapAddressTo",
                table: "OrderTasks",
                newName: "GoogleMapAddress");
        }
    }
}
