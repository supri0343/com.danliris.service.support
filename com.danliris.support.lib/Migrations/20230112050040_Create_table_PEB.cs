using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace com.danliris.support.lib.Migrations
{
    public partial class Create_table_PEB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "PEBHeader",
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
                    bruto = table.Column<double>(type: "decimal(24, 4)", nullable: false),
                    cif = table.Column<double>(nullable: false),
                    disclaimer = table.Column<string>(nullable: true),
                    flagCurah = table.Column<string>(nullable: true),
                    flagMigas = table.Column<string>(nullable: true),
                    fob = table.Column<double>(nullable: false),
                    freight = table.Column<double>(nullable: false),
                    idPengguna = table.Column<string>(nullable: true),
                    jabatanTtd = table.Column<string>(nullable: true),
                    jumlahKontainer = table.Column<double>(nullable: false),
                    kodeAsuransi = table.Column<string>(nullable: true),
                    kodeCaraBayar = table.Column<string>(nullable: true),
                    kodeCaraDagang = table.Column<string>(nullable: true),
                    kodeDokumen = table.Column<string>(nullable: true),
                    kodeIncoterm = table.Column<string>(nullable: true),
                    kodeJenisEkspor = table.Column<string>(nullable: true),
                    kodeJenisNilai = table.Column<string>(nullable: true),
                    kodeJenisProsedur = table.Column<string>(nullable: true),
                    kodeKantor = table.Column<string>(nullable: true),
                    kodeKantorEkspor = table.Column<string>(nullable: true),
                    kodeKantorMuat = table.Column<string>(nullable: true),
                    kodeKantorPeriksa = table.Column<string>(nullable: true),
                    kodeKategoriEkspor = table.Column<string>(nullable: true),
                    kodeLokasi = table.Column<string>(nullable: true),
                    kodeNegaraTujuan = table.Column<string>(nullable: true),
                    kodePelBongkar = table.Column<string>(nullable: true),
                    kodePelEkspor = table.Column<string>(nullable: true),
                    kodePelMuat = table.Column<string>(nullable: true),
                    kodePelTujuan = table.Column<string>(nullable: true),
                    kodePembayar = table.Column<string>(nullable: true),
                    kodeTps = table.Column<string>(nullable: true),
                    kodeValuta = table.Column<string>(nullable: true),
                    kotaTtd = table.Column<string>(nullable: true),
                    namaTtd = table.Column<string>(nullable: true),
                    ndpbm = table.Column<double>(type: "decimal(24, 4)", nullable: false),
                    netto = table.Column<double>(type: "decimal(24, 4)", nullable: false),
                    nilaiMaklon = table.Column<double>(nullable: false),
                    nomorAju = table.Column<string>(nullable: true),
                    nomorBc11 = table.Column<string>(nullable: true),
                    nomorDaftar = table.Column<string>(nullable: true),
                    posBc11 = table.Column<string>(nullable: true),
                    seri = table.Column<int>(nullable: false),
                    subposBc11 = table.Column<string>(nullable: true),
                    tanggalAju = table.Column<DateTime>(nullable: false),
                    tanggalBc11 = table.Column<DateTime>(nullable: false),
                    tanggalDaftar = table.Column<DateTime>(nullable: true),
                    tanggalEkspor = table.Column<DateTime>(nullable: false),
                    tanggalPeriksa = table.Column<DateTime>(nullable: false),
                    tanggalTtd = table.Column<DateTime>(nullable: false),
                    totalDanaSawit = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEBHeader", x => x.Id);
                });

          

            migrationBuilder.CreateTable(
                name: "PEBBankDevisa",
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
                    kodeBank = table.Column<string>(nullable: true),
                    seriBank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEBBankDevisa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEBBankDevisa_PEBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "PEBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEBBarang",
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
                    fob = table.Column<double>(nullable: false),
                    hargaEkspor = table.Column<double>(type: "decimal(24, 4)", nullable: false),
                    hargaPatokan = table.Column<double>(type: "decimal(24, 4)", nullable: false),
                    hargaPerolehan = table.Column<double>(nullable: false),
                    hargaSatuan = table.Column<double>(type: "decimal(24, 4)", nullable: false),
                    jumlahKemasan = table.Column<double>(nullable: false),
                    jumlahSatuan = table.Column<double>(nullable: false),
                    kodeAsalBahanBaku = table.Column<string>(nullable: true),
                    kodeBarang = table.Column<string>(nullable: true),
                    kodeDaerahAsal = table.Column<string>(nullable: true),
                    kodeDokumen = table.Column<string>(nullable: true),
                    kodeJenisKemasan = table.Column<string>(nullable: true),
                    kodeNegaraAsal = table.Column<string>(nullable: true),
                    kodeSatuanBarang = table.Column<string>(nullable: true),
                    merk = table.Column<string>(nullable: true),
                    ndpbm = table.Column<double>(nullable: false),
                    netto = table.Column<double>(type: "decimal(24, 4)", nullable: false),
                    nilaiBarang = table.Column<double>(nullable: false),
                    nilaiDanaSawit = table.Column<double>(nullable: false),
                    posTarif = table.Column<string>(nullable: true),
                    seriBarang = table.Column<int>(nullable: false),
                    spesifikasiLain = table.Column<string>(nullable: true),
                    tipe = table.Column<string>(nullable: true),
                    ukuran = table.Column<string>(nullable: true),
                    uraian = table.Column<string>(nullable: true),
                    volume = table.Column<double>(type: "decimal(24, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEBBarang", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEBBarang_PEBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "PEBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEBDokumen",
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
                    idDokumen = table.Column<string>(nullable: true),
                    kodeDokumen = table.Column<string>(nullable: true),
                    nomorDokumen = table.Column<string>(nullable: true),
                    seriDokumen = table.Column<int>(nullable: false),
                    tanggalDokumen = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEBDokumen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEBDokumen_PEBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "PEBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEBEntitas",
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
                    kodeJenisIdentitas = table.Column<string>(nullable: true),
                    namaEntitas = table.Column<string>(nullable: true),
                    nibEntitas = table.Column<string>(nullable: true),
                    nomorIdentitas = table.Column<string>(nullable: true),
                    seriEntitas = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEBEntitas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEBEntitas_PEBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "PEBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEBKemasan",
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
                    table.PrimaryKey("PK_PEBKemasan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEBKemasan_PEBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "PEBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEBKesiapanBarang",
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
                    alamat = table.Column<string>(nullable: true),
                    jumlahContainer20 = table.Column<int>(nullable: false),
                    jumlahContainer40 = table.Column<int>(nullable: false),
                    kodeCaraStuffing = table.Column<string>(nullable: true),
                    kodeJenisBarang = table.Column<string>(nullable: true),
                    kodeJenisGudang = table.Column<string>(nullable: true),
                    kodeJenisPartOf = table.Column<string>(nullable: true),
                    lokasiSiapPeriksa = table.Column<string>(nullable: true),
                    namaPic = table.Column<string>(nullable: true),
                    nomorTelpPic = table.Column<string>(nullable: true),
                    tanggalPkb = table.Column<DateTime>(nullable: false),
                    waktuSiapPeriksa = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEBKesiapanBarang", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEBKesiapanBarang_PEBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "PEBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEBKontainer",
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
                    table.PrimaryKey("PK_PEBKontainer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEBKontainer_PEBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "PEBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEBPengangkut",
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
                    kodeBendera = table.Column<string>(nullable: true),
                    kodeCaraAngkut = table.Column<string>(nullable: true),
                    namaPengangkut = table.Column<string>(nullable: true),
                    nomorPengangkut = table.Column<string>(nullable: true),
                    seriPengangkut = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEBPengangkut", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEBPengangkut_PEBHeader_IdHeader",
                        column: x => x.IdHeader,
                        principalTable: "PEBHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEBBarang_Dokumen",
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
                    seriDokumen = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEBBarang_Dokumen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEBBarang_Dokumen_PEBBarang_IdBarang",
                        column: x => x.IdBarang,
                        principalTable: "PEBBarang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEBBarang_Pemilik",
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
                    seriEntitas = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEBBarang_Pemilik", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEBBarang_Pemilik_PEBBarang_IdBarang",
                        column: x => x.IdBarang,
                        principalTable: "PEBBarang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEBBarang_Tarif",
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
                    jumlahSatuan = table.Column<double>(type: "decimal(20, 4)", nullable: false),
                    kodeFasilitasTarif = table.Column<string>(nullable: true),
                    kodeJenisPungutan = table.Column<string>(nullable: true),
                    kodeJenisTarif = table.Column<string>(nullable: true),
                    kodeSatuanBarang = table.Column<string>(nullable: true),
                    nilaiBayar = table.Column<double>(nullable: false),
                    seriBarang = table.Column<int>(nullable: false),
                    tarif = table.Column<double>(nullable: false),
                    tarifFasilitas = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEBBarang_Tarif", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEBBarang_Tarif_PEBBarang_IdBarang",
                        column: x => x.IdBarang,
                        principalTable: "PEBBarang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PEBBankDevisa_IdHeader",
                table: "PEBBankDevisa",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_PEBBarang_IdHeader",
                table: "PEBBarang",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_PEBBarang_Dokumen_IdBarang",
                table: "PEBBarang_Dokumen",
                column: "IdBarang");

            migrationBuilder.CreateIndex(
                name: "IX_PEBBarang_Pemilik_IdBarang",
                table: "PEBBarang_Pemilik",
                column: "IdBarang");

            migrationBuilder.CreateIndex(
                name: "IX_PEBBarang_Tarif_IdBarang",
                table: "PEBBarang_Tarif",
                column: "IdBarang");

            migrationBuilder.CreateIndex(
                name: "IX_PEBDokumen_IdHeader",
                table: "PEBDokumen",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_PEBEntitas_IdHeader",
                table: "PEBEntitas",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_PEBKemasan_IdHeader",
                table: "PEBKemasan",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_PEBKesiapanBarang_IdHeader",
                table: "PEBKesiapanBarang",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_PEBKontainer_IdHeader",
                table: "PEBKontainer",
                column: "IdHeader");

            migrationBuilder.CreateIndex(
                name: "IX_PEBPengangkut_IdHeader",
                table: "PEBPengangkut",
                column: "IdHeader");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "PEBBankDevisa");

            migrationBuilder.DropTable(
                name: "PEBBarang_Dokumen");

            migrationBuilder.DropTable(
                name: "PEBBarang_Pemilik");

            migrationBuilder.DropTable(
                name: "PEBBarang_Tarif");

            migrationBuilder.DropTable(
                name: "PEBDokumen");

            migrationBuilder.DropTable(
                name: "PEBEntitas");

            migrationBuilder.DropTable(
                name: "PEBKemasan");

            migrationBuilder.DropTable(
                name: "PEBKesiapanBarang");

            migrationBuilder.DropTable(
                name: "PEBKontainer");

            migrationBuilder.DropTable(
                name: "PEBPengangkut");


            migrationBuilder.DropTable(
                name: "PEBBarang");

            migrationBuilder.DropTable(
                name: "PEBHeader");
        }
    }
}
