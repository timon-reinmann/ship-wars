using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yoo.Trainees.ShipWars.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFirstShot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFirstShot",
                table: "StartShotHumanLogic",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFirstShot",
                table: "StartShotHumanLogic");
        }
    }
}
