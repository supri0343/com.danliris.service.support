using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class modifytabletpbfor261 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "disclaimer",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeValuta",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ndpbm",
                table: "TPBHeader",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "nilaiBarang",
                table: "TPBHeader",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "tempatStuffing",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "tglAkhirBerlaku",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "tglAwalBerlaku",
                table: "TPBHeader",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "totalDanaSawit",
                table: "TPBHeader",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "kodeFasilitas",
                table: "TPBDokumen",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeAsalBarang",
                table: "TPBBarang",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeKategoriBarang",
                table: "TPBBarang",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kodeNegaraAsal",
                table: "TPBBarang",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TPBBahanBaku",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    IdHeader = table.Column<long>(nullable: false),
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
                    table.PrimaryKey("PK_TPBBahanBaku", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBBahanBaku_TPBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "TPBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TPBBahanBakuTarif",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    IdHeader = table.Column<long>(nullable: false),
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
                    jumlahKemasan = table.Column<double>(nullable: false),
                    jumlahSatuan = table.Column<double>(nullable: false),
                    kodeAsalBahanBaku = table.Column<string>(nullable: true),
                    kodeFasilitasTarif = table.Column<string>(nullable: true),
                    kodeJenisPungutan = table.Column<string>(nullable: true),
                    kodeJenisTarif = table.Column<string>(nullable: true),
                    kodeSatuanBarang = table.Column<string>(nullable: true),
                    nilaiBayar = table.Column<double>(nullable: false),
                    nilaiFasilitas = table.Column<double>(nullable: false),
                    nilaiSudahDilunasi = table.Column<double>(nullable: false),
                    seriBahanBaku = table.Column<int>(nullable: false),
                    tarif = table.Column<double>(nullable: false),
                    tarifFasilitas = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBBahanBakuTarif", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBBahanBakuTarif_TPBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "TPBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TPBJaminan",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    IdHeader = table.Column<long>(nullable: false),
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
                    idJaminan = table.Column<string>(nullable: true),
                    kodeJenisJaminan = table.Column<string>(nullable: true),
                    nilaiJaminan = table.Column<double>(nullable: false),
                    nomorBpj = table.Column<string>(nullable: true),
                    nomorJaminan = table.Column<string>(nullable: true),
                    penjamin = table.Column<string>(nullable: true),
                    tanggalBpj = table.Column<string>(nullable: true),
                    tanggalJaminan = table.Column<string>(nullable: true),
                    tanggalJatuhTempo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBJaminan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBJaminan_TPBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "TPBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TPBBahanBaku_IdHeader",
                table: "TPBBahanBaku",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_TPBBahanBakuTarif_IdHeader",
                table: "TPBBahanBakuTarif",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_TPBJaminan_IdHeader",
                table: "TPBJaminan",
                column: "IdHeader");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
     
            migrationBuilder.DropTable(
                name: "TPBBahanBaku");

            migrationBuilder.DropTable(
                name: "TPBBahanBakuTarif");

            migrationBuilder.DropTable(
                name: "TPBJaminan");

            migrationBuilder.DropColumn(
                name: "disclaimer",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodeValuta",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "ndpbm",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "nilaiBarang",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "tempatStuffing",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "tglAkhirBerlaku",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "tglAwalBerlaku",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "totalDanaSawit",
                table: "TPBHeader");

            migrationBuilder.DropColumn(
                name: "kodeFasilitas",
                table: "TPBDokumen");

            migrationBuilder.DropColumn(
                name: "kodeAsalBarang",
                table: "TPBBarang");

            migrationBuilder.DropColumn(
                name: "kodeKategoriBarang",
                table: "TPBBarang");

            migrationBuilder.DropColumn(
                name: "kodeNegaraAsal",
                table: "TPBBarang");

          
        }
    }
}
