using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class addColoumBarangBahanBakuTarif : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "jumlahKemasan",
                table: "TPBBarang_BahanBakuTarif",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "kodeAsalBahanBaku",
                table: "TPBBarang_BahanBakuTarif",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeSatuanBarang",
                table: "TPBBarang_BahanBakuTarif",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "jumlahKemasan",
                table: "TPBBarang_BahanBakuTarif");

            migrationBuilder.DropColumn(
                name: "kodeAsalBahanBaku",
                table: "TPBBarang_BahanBakuTarif");

            migrationBuilder.DropColumn(
                name: "kodeSatuanBarang",
                table: "TPBBarang_BahanBakuTarif");
        }
    }
}
