using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mashawer.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PremiumEndDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PremiumStartDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TrialEndDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TrialStartDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "isTrial",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SerialNumber",
                table: "AspNetUsers",
                newName: "ProfilePictureUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                newName: "SerialNumber");

            migrationBuilder.AddColumn<DateTime>(
                name: "PremiumEndDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PremiumStartDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrialEndDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrialStartDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isTrial",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                columns: new[] { "PremiumEndDate", "PremiumStartDate", "TrialEndDate", "TrialStartDate", "isTrial" },
                values: new object[] { null, null, null, null, false });
        }
    }
}
