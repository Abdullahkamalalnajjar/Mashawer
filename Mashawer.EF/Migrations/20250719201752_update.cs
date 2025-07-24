using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mashawer.EF.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "admin@mashawer.com", "ADMIN@MASHAWER.COM", "ADMIN@MASHAWER.COM", "admin@mashawer.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "admin@ecommerce.com", "ADMIN@ECOMMERCE.COM", "ADMIN@ECOMMERCE.COM", "admin@ecommerce.com" });
        }
    }
}
