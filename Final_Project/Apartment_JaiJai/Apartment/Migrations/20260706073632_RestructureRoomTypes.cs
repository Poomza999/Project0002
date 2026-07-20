using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Apartment.Migrations
{
    /// <inheritdoc />
    public partial class RestructureRoomTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dormitories_RoomTypes_RoomTypeID",
                table: "Dormitories");

            migrationBuilder.DropIndex(
                name: "IX_Dormitories_RoomTypeID",
                table: "Dormitories");

            migrationBuilder.DropColumn(
                name: "RoomTypeID",
                table: "Dormitories");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "RoomTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DormImages",
                columns: table => new
                {
                    ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DormID = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DormImages", x => x.ImageID);
                    table.ForeignKey(
                        name: "FK_DormImages_Dormitories_DormID",
                        column: x => x.DormID,
                        principalTable: "Dormitories",
                        principalColumn: "DormID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DormRoomTypes",
                columns: table => new
                {
                    DormRoomTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DormID = table.Column<int>(type: "int", nullable: false),
                    RoomTypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DormRoomTypes", x => x.DormRoomTypeID);
                    table.ForeignKey(
                        name: "FK_DormRoomTypes_Dormitories_DormID",
                        column: x => x.DormID,
                        principalTable: "Dormitories",
                        principalColumn: "DormID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DormRoomTypes_RoomTypes_RoomTypeID",
                        column: x => x.RoomTypeID,
                        principalTable: "RoomTypes",
                        principalColumn: "RoomTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 1,
                columns: new[] { "Category", "Name" },
                values: new object[] { "ในห้อง", "แอร์" });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 2,
                columns: new[] { "Category", "Name" },
                values: new object[] { "ในห้อง", "ทีวี" });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 3,
                columns: new[] { "Category", "Name" },
                values: new object[] { "ในห้อง", "ตู้เย็น" });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 4,
                columns: new[] { "Category", "Name" },
                values: new object[] { "ในห้อง", "เฟอร์นิเจอร์พร้อมอยู่" });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 5,
                columns: new[] { "Category", "Name" },
                values: new object[] { "อินเตอร์เน็ต", "อินเตอร์เน็ตฟรี" });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 6,
                columns: new[] { "Category", "Name" },
                values: new object[] { "อินเตอร์เน็ต", "อินเตอร์เน็ตมีค่าบริการ" });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 7,
                columns: new[] { "Category", "Name" },
                values: new object[] { "ซักผ้า", "เครื่องซักผ้าภายในหอพัก" });

            migrationBuilder.InsertData(
                table: "Amenities",
                columns: new[] { "AmenityID", "Category", "Name" },
                values: new object[,]
                {
                    { 8, "ความปลอดภัย", "กล้องวงจรปิด" },
                    { 9, "ความปลอดภัย", "คีย์การ์ด / รหัสเข้าห้อง" },
                    { 10, "ส่วนกลาง", "ที่จอดรถ" },
                    { 11, "ส่วนกลาง", "ฟิตเนส" },
                    { 12, "ส่วนกลาง", "สระว่ายน้ำ" }
                });

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "RoomTypeID",
                keyValue: 1,
                columns: new[] { "Category", "Name" },
                values: new object[] { "ประเภทเตียง", "เตียงเดี่ยว" });

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "RoomTypeID",
                keyValue: 2,
                columns: new[] { "Category", "Name" },
                values: new object[] { "ประเภทเตียง", "เตียงคู่" });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "RoomTypeID", "Category", "Name" },
                values: new object[,]
                {
                    { 3, "ประเภทสัญญา", "รายวัน" },
                    { 4, "ประเภทสัญญา", "รายเดือน" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DormImages_DormID",
                table: "DormImages",
                column: "DormID");

            migrationBuilder.CreateIndex(
                name: "IX_DormRoomTypes_DormID",
                table: "DormRoomTypes",
                column: "DormID");

            migrationBuilder.CreateIndex(
                name: "IX_DormRoomTypes_RoomTypeID",
                table: "DormRoomTypes",
                column: "RoomTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DormImages");

            migrationBuilder.DropTable(
                name: "DormRoomTypes");

            migrationBuilder.DeleteData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "RoomTypeID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "RoomTypeID",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "Category",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Amenities");

            migrationBuilder.AddColumn<int>(
                name: "RoomTypeID",
                table: "Dormitories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 1,
                column: "Name",
                value: "WiFi");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 2,
                column: "Name",
                value: "แอร์");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 3,
                column: "Name",
                value: "เครื่องซักผ้า");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 4,
                column: "Name",
                value: "ที่จอดรถ");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 5,
                column: "Name",
                value: "กล้องวงจรปิด");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 6,
                column: "Name",
                value: "ร้านสะดวกซื้อ");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityID",
                keyValue: 7,
                column: "Name",
                value: "ฟิตเนส");

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "RoomTypeID",
                keyValue: 1,
                column: "Name",
                value: "ห้องเดี่ยว");

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "RoomTypeID",
                keyValue: 2,
                column: "Name",
                value: "ห้องคู่");

            migrationBuilder.CreateIndex(
                name: "IX_Dormitories_RoomTypeID",
                table: "Dormitories",
                column: "RoomTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dormitories_RoomTypes_RoomTypeID",
                table: "Dormitories",
                column: "RoomTypeID",
                principalTable: "RoomTypes",
                principalColumn: "RoomTypeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
