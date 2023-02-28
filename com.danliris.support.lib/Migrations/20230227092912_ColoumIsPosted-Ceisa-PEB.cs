using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class ColoumIsPostedCeisaPEB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isPosted",
                table: "PEBHeader",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "postedBy",
                table: "PEBHeader",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPosted",
                table: "PEBHeader");

            migrationBuilder.DropColumn(
                name: "postedBy",
                table: "PEBHeader");
        }
    }
}
