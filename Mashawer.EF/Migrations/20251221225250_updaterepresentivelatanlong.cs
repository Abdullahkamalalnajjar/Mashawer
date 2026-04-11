using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mashawer.EF.Migrations
{
    /// <inheritdoc />
    public partial class updaterepresentivelatanlong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RepresentativeLongitude",
                table: "AspNetUsers",
                newName: "RepresentativeToLongitude");

            migrationBuilder.RenameColumn(
                name: "RepresentativeLatitude",
                table: "AspNetUsers",
                newName: "RepresentativeToLatitude");

            migrationBuilder.AddColumn<double>(
                name: "RepresentativeFromLatitude",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RepresentativeFromLongitude",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                columns: new[] { "RepresentativeFromLatitude", "RepresentativeFromLongitude" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepresentativeFromLatitude",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RepresentativeFromLongitude",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "RepresentativeToLongitude",
                table: "AspNetUsers",
                newName: "RepresentativeLongitude");

            migrationBuilder.RenameColumn(
                name: "RepresentativeToLatitude",
                table: "AspNetUsers",
                newName: "RepresentativeLatitude");
        }
    }
}
