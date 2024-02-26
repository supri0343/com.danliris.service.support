using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class addtablefortpbBC40 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "TPBHeader",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
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
                    asalData = table.Column<string>(nullable: true),
                    asuransi = table.Column<double>(nullable: false),
                    biayaPengurang = table.Column<double>(nullable: false),
                    biayaTambahan = table.Column<double>(nullable: false),
                    bruto = table.Column<double>(nullable: false),
                    cif = table.Column<double>(nullable: false),
                    freight = table.Column<double>(nullable: false),
                    hargaPenyerahan = table.Column<double>(nullable: false),
                    idPengguna = table.Column<string>(nullable: true),
                    isPosted = table.Column<bool>(nullable: false),
                    jabatanTtd = table.Column<string>(nullable: true),
                    jumlahKontainer = table.Column<int>(nullable: false),
                    kodeDokumen = table.Column<string>(nullable: true),
                    kodeJenisTpb = table.Column<string>(nullable: true),
                    kodeKantor = table.Column<string>(nullable: true),
                    kodeTujuanPengiriman = table.Column<string>(nullable: true),
                    kotaTtd = table.Column<string>(nullable: true),
                    namaTtd = table.Column<string>(nullable: true),
                    netto = table.Column<double>(nullable: false),
                    nik = table.Column<string>(nullable: true),
                    nilaiJasa = table.Column<double>(nullable: false),
                    nomorAju = table.Column<string>(nullable: true),
                    nomorDaftar = table.Column<string>(nullable: true),
                    postedBy = table.Column<string>(nullable: true),
                    seri = table.Column<int>(nullable: false),
                    tanggalAju = table.Column<DateTime>(nullable: false),
                    tanggalDaftar = table.Column<DateTime>(nullable: true),
                    tanggalTtd = table.Column<DateTime>(nullable: false),
                    uangMuka = table.Column<double>(nullable: false),
                    vd = table.Column<double>(nullable: false),
                    volume = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBHeader", x => x.Id);
                });


            migrationBuilder.CreateTable(
                name: "TPBBarang",
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
                    asuransi = table.Column<double>(nullable: false),
                    bruto = table.Column<double>(nullable: false),
                    cif = table.Column<double>(nullable: false),
                    cifRupiah = table.Column<double>(nullable: false),
                    diskon = table.Column<double>(nullable: false),
                    hargaEkspor = table.Column<double>(nullable: false),
                    hargaPenyerahan = table.Column<double>(nullable: false),
                    hargaPerolehan = table.Column<double>(nullable: false),
                    hargaSatuan = table.Column<double>(nullable: false),
                    isiPerKemasan = table.Column<int>(nullable: false),
                    jumlahKemasan = table.Column<double>(nullable: false),
                    jumlahRealisasi = table.Column<double>(nullable: false),
                    jumlahSatuan = table.Column<double>(nullable: false),
                    kodeAsalBahanBaku = table.Column<string>(nullable: true),
                    kodeBarang = table.Column<string>(nullable: true),
                    kodeDokumen = table.Column<string>(nullable: true),
                    kodeJenisKemasan = table.Column<string>(nullable: true),
                    kodeSatuanBarang = table.Column<string>(nullable: true),
                    merk = table.Column<string>(nullable: true),
                    ndpbm = table.Column<double>(nullable: false),
                    netto = table.Column<double>(nullable: false),
                    nilaiBarang = table.Column<double>(nullable: false),
                    nilaiJasa = table.Column<int>(nullable: false),
                    posTarif = table.Column<string>(nullable: true),
                    seriBarang = table.Column<int>(nullable: false),
                    spesifikasiLain = table.Column<string>(nullable: true),
                    tipe = table.Column<string>(nullable: true),
                    uangMuka = table.Column<double>(nullable: false),
                    ukuran = table.Column<string>(nullable: true),
                    uraian = table.Column<string>(nullable: true),
                    volume = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBBarang", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBBarang_TPBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "TPBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TPBDokumen",
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
                    kodeDokumen = table.Column<string>(nullable: true),
                    nomorDokumen = table.Column<string>(nullable: true),
                    seriDokumen = table.Column<int>(nullable: false),
                    tanggalDokumen = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBDokumen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBDokumen_TPBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "TPBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TPBEntitas",
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
                    alamatEntitas = table.Column<string>(nullable: true),
                    kodeEntitas = table.Column<string>(nullable: true),
                    kodeJenisApi = table.Column<string>(nullable: true),
                    kodeJenisIdentitas = table.Column<string>(nullable: true),
                    kodeStatus = table.Column<string>(nullable: true),
                    namaEntitas = table.Column<string>(nullable: true),
                    nibEntitas = table.Column<string>(nullable: true),
                    nomorIdentitas = table.Column<string>(nullable: true),
                    nomorIjinEntitas = table.Column<string>(nullable: true),
                    seriEntitas = table.Column<int>(nullable: false),
                    tanggalIjinEntitas = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBEntitas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBEntitas_TPBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "TPBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TPBKemasan",
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
                    jumlahKemasan = table.Column<int>(nullable: false),
                    kodeJenisKemasan = table.Column<string>(nullable: true),
                    merkKemasan = table.Column<string>(nullable: true),
                    seriKemasan = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBKemasan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBKemasan_TPBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "TPBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TPBKontainer",
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
                    kodeJenisKontainer = table.Column<string>(nullable: true),
                    kodeTipeKontainer = table.Column<string>(nullable: true),
                    kodeUkuranKontainer = table.Column<string>(nullable: true),
                    nomorKontainer = table.Column<string>(nullable: true),
                    seriKontainer = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBKontainer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBKontainer_TPBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "TPBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TPBPengangkut",
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
                    namaPengangkut = table.Column<string>(nullable: true),
                    nomorPengangkut = table.Column<string>(nullable: true),
                    seriPengangkut = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBPengangkut", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBPengangkut_TPBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "TPBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TPBPungutan",
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
                    kodeFasilitasTarif = table.Column<string>(nullable: true),
                    kodeJenisPungutan = table.Column<string>(nullable: true),
                    nilaiPungutan = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBPungutan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBPungutan_TPBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "TPBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TPBBarang_Tarif",
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
                    jumlahSatuan = table.Column<double>(nullable: false),
                    kodeFasilitasTarif = table.Column<string>(nullable: true),
                    kodeJenisPungutan = table.Column<string>(nullable: true),
                    kodeJenisTarif = table.Column<string>(nullable: true),
                    kodeSatuanBarang = table.Column<string>(nullable: true),
                    nilaiBayar = table.Column<double>(nullable: false),
                    nilaiFasilitas = table.Column<double>(nullable: false),
                    nilaiSudahDilunasi = table.Column<double>(nullable: false),
                    seriBarang = table.Column<int>(nullable: false),
                    tarif = table.Column<double>(nullable: false),
                    tarifFasilitas = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPBBarang_Tarif", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TPBBarang_Tarif_TPBBarang_IdBarang",
                        column: x => x.IdBarang,
                        principalTable: "TPBBarang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.CreateIndex(
                name: "IX_TPBBarang_IdHeader",
                table: "TPBBarang",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_TPBBarang_Tarif_IdBarang",
                table: "TPBBarang_Tarif",
                column: "IdBarang");

            migrationBuilder.CreateIndex(
                name: "IX_TPBDokumen_IdHeader",
                table: "TPBDokumen",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_TPBEntitas_IdHeader",
                table: "TPBEntitas",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_TPBKemasan_IdHeader",
                table: "TPBKemasan",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_TPBKontainer_IdHeader",
                table: "TPBKontainer",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_TPBPengangkut_IdHeader",
                table: "TPBPengangkut",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_TPBPungutan_IdHeader",
                table: "TPBPungutan",
                column: "IdHeader");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropTable(
                name: "TPBBarang_Tarif");

            migrationBuilder.DropTable(
                name: "TPBDokumen");

            migrationBuilder.DropTable(
                name: "TPBEntitas");

            migrationBuilder.DropTable(
                name: "TPBKemasan");

            migrationBuilder.DropTable(
                name: "TPBKontainer");

            migrationBuilder.DropTable(
                name: "TPBPengangkut");

            migrationBuilder.DropTable(
                name: "TPBPungutan");

            migrationBuilder.DropTable(
                name: "TPBBarang");

            migrationBuilder.DropTable(
                name: "TPBHeader");
        }
    }
}
