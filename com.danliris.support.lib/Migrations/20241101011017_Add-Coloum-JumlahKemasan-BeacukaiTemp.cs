using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class AddColoumJumlahKemasanBeacukaiTemp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JumlahKemasan",
                table: "BEACUKAI_TEMP",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JumlahKemasan",
                table: "BEACUKAI_TEMP");
        }
    }
}
