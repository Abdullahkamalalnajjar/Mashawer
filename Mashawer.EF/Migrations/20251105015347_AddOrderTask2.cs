using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mashawer.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTask2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Orders_OrderId",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "DeliveryDescription",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryLocation_EntranceNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryLocation_PhoneNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryLocation_StreetName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DistanceKm",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FromLatitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FromLongitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsClientPaidForItems",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsDriverReimbursed",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ItemPhotoAfter",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ItemPhotoBefore",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PickupLocation_EntranceNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PickupLocation_PhoneNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PickupLocation_StreetName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ToLatitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ToLongitude",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "PurchaseItems",
                newName: "OrderTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItems_OrderId",
                table: "PurchaseItems",
                newName: "IX_PurchaseItems_OrderTaskId");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDeliveryPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalDistanceKm",
                table: "Orders",
                type: "float",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromLatitude = table.Column<double>(type: "float", nullable: false),
                    FromLongitude = table.Column<double>(type: "float", nullable: false),
                    ToLatitude = table.Column<double>(type: "float", nullable: false),
                    ToLongitude = table.Column<double>(type: "float", nullable: false),
                    PickupLocation_StreetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupLocation_EntranceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupLocation_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryLocation_StreetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryLocation_EntranceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryLocation_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DistanceKm = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DeliveryDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClientPaidForItems = table.Column<bool>(type: "bit", nullable: false),
                    IsDriverReimbursed = table.Column<bool>(type: "bit", nullable: false),
                    ItemPhotoBefore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemPhotoAfter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTasks_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderTasks_OrderId",
                table: "OrderTasks",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_OrderTasks_OrderTaskId",
                table: "PurchaseItems",
                column: "OrderTaskId",
                principalTable: "OrderTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_OrderTasks_OrderTaskId",
                table: "PurchaseItems");

            migrationBuilder.DropTable(
                name: "OrderTasks");

            migrationBuilder.DropColumn(
                name: "TotalDeliveryPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalDistanceKm",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "OrderTaskId",
                table: "PurchaseItems",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItems_OrderTaskId",
                table: "PurchaseItems",
                newName: "IX_PurchaseItems_OrderId");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethod",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryDescription",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryLocation_EntranceNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryLocation_PhoneNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryLocation_StreetName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "DistanceKm",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FromLatitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FromLongitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsClientPaidForItems",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDriverReimbursed",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ItemPhotoAfter",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemPhotoBefore",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupLocation_EntranceNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupLocation_PhoneNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupLocation_StreetName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ToLatitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ToLongitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Orders_OrderId",
                table: "PurchaseItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
