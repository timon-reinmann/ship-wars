using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yoo.Trainees.ShipWars.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddHardGameMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StartShotHumanLogic");

            migrationBuilder.CreateTable(
                name: "HardGameShot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    Hit = table.Column<bool>(type: "bit", nullable: false),
                    Step = table.Column<int>(type: "int", nullable: false),
                    MainShot = table.Column<bool>(type: "bit", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HardGameShot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HardGameShot_GamePlayer_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "GamePlayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HardGameShot_PlayerId",
                table: "HardGameShot",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HardGameShot");

            migrationBuilder.CreateTable(
                name: "StartShotHumanLogic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Hit = table.Column<bool>(type: "bit", nullable: false),
                    IsFirstShot = table.Column<bool>(type: "bit", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StartShotHumanLogic", x => x.Id);
                });
        }
    }
}
