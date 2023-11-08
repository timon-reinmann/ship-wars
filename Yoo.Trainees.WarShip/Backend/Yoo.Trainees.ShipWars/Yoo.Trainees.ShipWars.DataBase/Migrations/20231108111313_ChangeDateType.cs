using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Yoo.Trainees.ShipWars.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ship",
                keyColumn: "Id",
                keyValue: new Guid("45652d4e-10be-4b26-a8a8-c6de9a4e49af"));

            migrationBuilder.DeleteData(
                table: "Ship",
                keyColumn: "Id",
                keyValue: new Guid("985497b3-353c-49d3-97a0-79232a270da0"));

            migrationBuilder.DeleteData(
                table: "Ship",
                keyColumn: "Id",
                keyValue: new Guid("a735b8ec-c868-45fa-81bc-942eda165a8f"));

            migrationBuilder.DeleteData(
                table: "Ship",
                keyColumn: "Id",
                keyValue: new Guid("eee6167b-6588-42d9-b657-7698a2f5ca40"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Message",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "smalldatetime");

            migrationBuilder.InsertData(
                table: "Ship",
                columns: new[] { "Id", "Length", "Name" },
                values: new object[,]
                {
                    { new Guid("2d54d923-62db-4949-88cc-9d01904aed7e"), 3, "Cruiser" },
                    { new Guid("4310e2d7-db12-473f-ac98-da27457b629e"), 2, "Destroyer" },
                    { new Guid("4dfc0747-1b3a-4b9e-b478-4df9899abde6"), 1, "Submarine" },
                    { new Guid("b1e25182-425a-4bdf-a35e-4860edcbba7e"), 4, "Warship" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ship",
                keyColumn: "Id",
                keyValue: new Guid("2d54d923-62db-4949-88cc-9d01904aed7e"));

            migrationBuilder.DeleteData(
                table: "Ship",
                keyColumn: "Id",
                keyValue: new Guid("4310e2d7-db12-473f-ac98-da27457b629e"));

            migrationBuilder.DeleteData(
                table: "Ship",
                keyColumn: "Id",
                keyValue: new Guid("4dfc0747-1b3a-4b9e-b478-4df9899abde6"));

            migrationBuilder.DeleteData(
                table: "Ship",
                keyColumn: "Id",
                keyValue: new Guid("b1e25182-425a-4bdf-a35e-4860edcbba7e"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Message",
                type: "smalldatetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.InsertData(
                table: "Ship",
                columns: new[] { "Id", "Length", "Name" },
                values: new object[,]
                {
                    { new Guid("45652d4e-10be-4b26-a8a8-c6de9a4e49af"), 1, "Submarine" },
                    { new Guid("985497b3-353c-49d3-97a0-79232a270da0"), 3, "Cruiser" },
                    { new Guid("a735b8ec-c868-45fa-81bc-942eda165a8f"), 2, "Destroyer" },
                    { new Guid("eee6167b-6588-42d9-b657-7698a2f5ca40"), 4, "Warship" }
                });
        }
    }
}
