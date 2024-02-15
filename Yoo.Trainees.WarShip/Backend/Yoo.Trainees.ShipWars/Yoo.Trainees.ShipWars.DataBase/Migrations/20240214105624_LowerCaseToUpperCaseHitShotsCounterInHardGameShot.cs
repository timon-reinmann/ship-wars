using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yoo.Trainees.ShipWars.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class LowerCaseToUpperCaseHitShotsCounterInHardGameShot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "hitShotsCounter",
                table: "HardGameShot",
                newName: "HitShotsCounter");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HitShotsCounter",
                table: "HardGameShot",
                newName: "hitShotsCounter");
        }
    }
}
