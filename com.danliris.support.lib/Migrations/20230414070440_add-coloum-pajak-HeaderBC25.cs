using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class addcoloumpajakHeaderBC25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "diskon",
                table: "TPBHeader",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ppnPajak",
                table: "TPBHeader",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ppnbmPajak",
                table: "TPBHeader",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "tarifPpnPajak",
                table: "TPBHeader",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "tarifPpnbmPajak",
                table: "TPBHeader",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "diskon",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "ppnPajak",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "ppnbmPajak",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "tarifPpnPajak",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "tarifPpnbmPajak",
                table: "TPBHeader");
        }
    }
}
