using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class addBarangbahanBakuBC25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "kodeCaraBayar",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "niperEntitas",
                table: "TPBEntitas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeDokAsal",
                table: "TPBBarang",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeGunaBarang",
                table: "TPBBarang",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeKondisiBarang",
                table: "TPBBarang",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TPBBarang_BahanBaku",
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
                    cif = table.Column<double>(nullable: false),
                    cifRupiah = table.Column<double>(nullable: false),
                    flagTis = table.Column<string>(nullable: true),
                    hargaPenyerahan = table.Column<double>(nullable: false),
                    hargaPerolehan = table.Column<double>(nullable: false),
                    isiPerKemasan = table.Column<double>(nullable: false),
                    jumlahSatuan = table.Column<double>(nullable: false),
                    kodeAsalBahanBaku = table.Column<string>(nullable: true),
                    kodeBarang = table.Column<string>(nullable: true),
                    kodeDokAsal = table.Column<string>(nullable: true),
                    kodeDokumen = table.Column<string>(nullable: true),
                    kodeKantor = table.Column<string>(nullable: true),
                    kodeSatuanBarang = table.Column<string>(nullable: true),
                    merkBarang = table.Column<string>(nullable: true),
                    ndpbm = table.Column<double>(nullable: false),
                    netto = table.Column<double>(nullable: false),
                    nilaiJasa = table.Column<double>(nullable: false),
                    nomorAjuDokAsal = table.Column<string>(nullable: true),
                    nomorDaftarDokAsal = table.Column<string>(nullable: true),
                    nomorDokumen = table.Column<string>(nullable: true),
                    posTarif = table.Column<string>(nullable: true),
                    seriBahanBaku = table.Column<int>(nullable: false),
                    seriBarang = table.Column<int>(nullable: false),
                    seriBarangDokAsal = table.Column<int>(nullable: false),
                    seriIjin = table.Column<int>(nullable: false),
                    spesifikasiLainBarang = table.Column<string>(nullable: true),
                    tanggalDaftarDokAsal = table.Column<string>(nullable: true),
                    tipeBarang = table.Column<string>(nullable: true),
                    ukuranBarang = table.Column<string>(nullable: true),
                    uraianBarang = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBBarang_BahanBaku", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBBarang_BahanBaku_TPBBarang_IdBarang",
                        column: x => x.IdBarang,
                        principalTable: "TPBBarang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TPBBarang_BahanBakuTarif",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    IdBarang_BahanBaku = table.Column<long>(nullable: false),
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
                    jumlahSatuan = table.Column<double>(nullable: false),
                    kodeFasilitasTarif = table.Column<string>(nullable: true),
                    kodeJenisPungutan = table.Column<string>(nullable: true),
                    kodeJenisTarif = table.Column<string>(nullable: true),
                    nilaiBayar = table.Column<double>(nullable: false),
                    nilaiFasilitas = table.Column<double>(nullable: false),
                    nilaiSudahDilunasi = table.Column<double>(nullable: false),
                    seriBahanBaku = table.Column<int>(nullable: false),
                    tarif = table.Column<double>(nullable: false),
                    tarifFasilitas = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBBarang_BahanBakuTarif", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBBarang_BahanBakuTarif_TPBBarang_BahanBaku_IdBarang_BahanBaku",
                        column: x => x.IdBarang_BahanBaku,
                        principalTable: "TPBBarang_BahanBaku",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TPBBarang_BahanBaku_IdBarang",
                table: "TPBBarang_BahanBaku",
                column: "IdBarang");

            migrationBuilder.CreateIndex(
                name: "IX_TPBBarang_BahanBakuTarif_IdBarang_BahanBaku",
                table: "TPBBarang_BahanBakuTarif",
                column: "IdBarang_BahanBaku");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TPBBarang_BahanBakuTarif");

            migrationBuilder.DropTable(
                name: "TPBBarang_BahanBaku");

            migrationBuilder.DropColumn(
                name: "kodeCaraBayar",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "niperEntitas",
                table: "TPBEntitas");

            migrationBuilder.DropColumn(
                name: "kodeDokAsal",
                table: "TPBBarang");

            migrationBuilder.DropColumn(
                name: "kodeGunaBarang",
                table: "TPBBarang");

            migrationBuilder.DropColumn(
                name: "kodeKondisiBarang",
                table: "TPBBarang");
        }
    }
}
