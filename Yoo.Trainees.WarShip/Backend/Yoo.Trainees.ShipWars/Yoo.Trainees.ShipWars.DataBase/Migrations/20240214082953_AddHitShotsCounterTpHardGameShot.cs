using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yoo.Trainees.ShipWars.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddHitShotsCounterTpHardGameShot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "hitShotsCounter",
                table: "HardGameShot",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hitShotsCounter",
                table: "HardGameShot");
        }
    }
}
