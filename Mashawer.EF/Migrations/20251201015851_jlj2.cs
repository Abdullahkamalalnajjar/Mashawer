using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mashawer.EF.Migrations
{
    /// <inheritdoc />
    public partial class jlj2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepresentitiveCancelOrders_AspNetUsers_UserId",
                table: "RepresentitiveCancelOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_RepresentitiveCancelOrders_Orders_OrderId",
                table: "RepresentitiveCancelOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RepresentitiveCancelOrders",
                table: "RepresentitiveCancelOrders");

            migrationBuilder.RenameTable(
                name: "RepresentitiveCancelOrders",
                newName: "RepresentitiveCancel");

            migrationBuilder.RenameIndex(
                name: "IX_RepresentitiveCancelOrders_UserId",
                table: "RepresentitiveCancel",
                newName: "IX_RepresentitiveCancel_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RepresentitiveCancelOrders_OrderId",
                table: "RepresentitiveCancel",
                newName: "IX_RepresentitiveCancel_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RepresentitiveCancel",
                table: "RepresentitiveCancel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RepresentitiveCancel_AspNetUsers_UserId",
                table: "RepresentitiveCancel",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepresentitiveCancel_Orders_OrderId",
                table: "RepresentitiveCancel",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepresentitiveCancel_AspNetUsers_UserId",
                table: "RepresentitiveCancel");

            migrationBuilder.DropForeignKey(
                name: "FK_RepresentitiveCancel_Orders_OrderId",
                table: "RepresentitiveCancel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RepresentitiveCancel",
                table: "RepresentitiveCancel");

            migrationBuilder.RenameTable(
                name: "RepresentitiveCancel",
                newName: "RepresentitiveCancelOrders");

            migrationBuilder.RenameIndex(
                name: "IX_RepresentitiveCancel_UserId",
                table: "RepresentitiveCancelOrders",
                newName: "IX_RepresentitiveCancelOrders_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RepresentitiveCancel_OrderId",
                table: "RepresentitiveCancelOrders",
                newName: "IX_RepresentitiveCancelOrders_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RepresentitiveCancelOrders",
                table: "RepresentitiveCancelOrders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RepresentitiveCancelOrders_AspNetUsers_UserId",
                table: "RepresentitiveCancelOrders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepresentitiveCancelOrders_Orders_OrderId",
                table: "RepresentitiveCancelOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
