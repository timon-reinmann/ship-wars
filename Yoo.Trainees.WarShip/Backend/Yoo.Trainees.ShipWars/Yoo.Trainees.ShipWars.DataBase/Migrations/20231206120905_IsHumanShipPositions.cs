﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yoo.Trainees.ShipWars.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class IsHumanShipPositions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHuman",
                table: "ShipPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHuman",
                table: "ShipPosition");
        }
    }
}
