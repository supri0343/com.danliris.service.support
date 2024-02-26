using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class modifytableforbc23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "fob",
                table: "TPBHeader",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "kodeAsuransi",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeIncoterm",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeKantorBongkar",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeKenaPajak",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodePelBongkar",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodePelMuat",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodePelTransit",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeTps",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeTujuanTpb",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeTutupPu",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nomorBc11",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "posBc11",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "subposBc11",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "tanggalBc11",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "tanggalTiba",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "fob",
                table: "TPBBarang",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "freight",
                table: "TPBBarang",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "kodePerhitungan",
                table: "TPBBarang",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "nilaiTambah",
                table: "TPBBarang",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "TPBBarang_Dokumen",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    IdBarang = table.Column<long>(nullable: false),
                    _CreatedAgent = table.Column<string>(nullable: true),
                    _CreatedBy = table.Column<string>(nullable: true),
                    _CreatedUtc = table.Column<DateTime>(nullable: false),
                    _DeletedAgent = table.Column<string>(nullable: true),
                    _DeletedBy = table.Column<string>(nullable: true),
                    _DeletedUtc = table.Column<DateTime>(nullable: false),
                    _IsDeleted = table.Column<bool>(nullable: false),
                    _LastModifiedAgent = table.Column<string>(nullable: true),
                    _LastModifiedBy = table.Column<string>(nullable: true),
                    _LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    seriDokumen = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBBarang_Dokumen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBBarang_Dokumen_TPBBarang_IdBarang",
                        column: x => x.IdBarang,
                        principalTable: "TPBBarang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TPBBarang_Dokumen_IdBarang",
                table: "TPBBarang_Dokumen",
                column: "IdBarang");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TPBBarang_Dokumen");

            migrationBuilder.DropColumn(
                name: "fob",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodeAsuransi",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodeIncoterm",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodeKantorBongkar",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodeKenaPajak",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodePelBongkar",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodePelMuat",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodePelTransit",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodeTps",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodeTujuanTpb",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodeTutupPu",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "nomorBc11",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "posBc11",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "subposBc11",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "tanggalBc11",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "tanggalTiba",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "fob",
                table: "TPBBarang");

            migrationBuilder.DropColumn(
                name: "freight",
                table: "TPBBarang");

            migrationBuilder.DropColumn(
                name: "kodePerhitungan",
                table: "TPBBarang");

            migrationBuilder.DropColumn(
                name: "nilaiTambah",
                table: "TPBBarang");
        }
    }
}
