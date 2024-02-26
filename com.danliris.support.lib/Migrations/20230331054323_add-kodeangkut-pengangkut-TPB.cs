using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class addkodeangkutpengangkutTPB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "idPengangkut",
                table: "TPBPengangkut",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeCaraAngkut",
                table: "TPBPengangkut",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idPengangkut",
                table: "TPBPengangkut");

            migrationBuilder.DropColumn(
                name: "kodeCaraAngkut",
                table: "TPBPengangkut");
        }
    }
}
