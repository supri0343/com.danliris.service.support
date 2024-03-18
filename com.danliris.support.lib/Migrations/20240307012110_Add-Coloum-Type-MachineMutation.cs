using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class AddColoumTypeMachineMutation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "MachineMutation",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuyerCode",
                table: "Machine",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BuyerId",
                table: "Machine",
                maxLength: 255,
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "BuyerName",
                table: "Machine",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "MachineMutation");

            migrationBuilder.DropColumn(
                name: "BuyerCode",
                table: "Machine");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "Machine");

            migrationBuilder.DropColumn(
                name: "BuyerName",
                table: "Machine");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TglDaftarAju",
                table: "BEACUKAI_TEMP",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
