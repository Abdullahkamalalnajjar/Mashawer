using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mashawer.EF.Migrations
{
    /// <inheritdoc />
    public partial class editOrderAddDeliveryDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryDescription",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DistanceKm",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryDescription",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DistanceKm",
                table: "Orders");
        }
    }
}
