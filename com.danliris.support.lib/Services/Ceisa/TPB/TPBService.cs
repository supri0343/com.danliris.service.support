using com.danliris.support.lib.Interfaces.Ceisa;
using com.danliris.support.lib.Interfaces.Ceisa.TPB;
using com.danliris.support.lib.Models.Ceisa.TPB;
using com.danliris.support.lib.ViewModel.Ceisa.TPBViewModel;
using Com.DanLiris.Service.support.lib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Data;
using System.IO;

namespace com.danliris.support.lib.Services.Ceisa.TPB
{
    public class TPBService :ITPBService
    {
        private readonly string USER_AGENT = "Service";
        private readonly SupportDbContext context;
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;
        private ICeisaService CeisaService;
        private readonly DbSet<TPBHeader> dbSet;

        public TPBService(SupportDbContext context, IServiceProvider serviceProvider, ICeisaService ceisaService)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
            this.CeisaService = ceisaService;
            dbSet = context.Set<TPBHeader>();
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        public async Task<TPBViewModelList> ReadByIdToPushAsync(long id, string auth)
        {
            TPBViewModelList Query = dbSet.Where(s => s.Id == id && s._IsDeleted == false).Select(m => new TPBViewModelList
            {
                Id = m.Id,
                nomorAju = m.nomorAju,
                tanggalAju = m.tanggalAju.ToString("dd-MMM-yyyy"),
                nomorDaftar = string.IsNullOrWhiteSpace(m.nomorDaftar) ? "-" : m.nomorDaftar,
                tanggalDaftar = m.tanggalDaftar == null ? "-" : m.tanggalDaftar.Value.ToString("dd-MMM-yyyy"),
                namaPenerima = m.entitas.Where(x => x.kodeEntitas == "8").Select(i => i.namaEntitas).FirstOrDefault(),
                isPosted = m.isPosted,
                postedBy = string.IsNullOrWhiteSpace(m.postedBy) ? "-" : m.postedBy,
                CreatedDate = m._CreatedUtc.ToString("dd-MMM-yyyy")
            }).OrderByDescending(x => x.nomorAju).First();

            //var token = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJySDN6VVB6ZGFqT2E4WEdQeVlsNVloaWRFblpFZ1hSVlVJc3BaSEthY0xVIn0.eyJleHAiOjE2NzgzMjk1MjAsImlhdCI6MTY3ODMyODYyMCwianRpIjoiYWU5ZjI4Y2YtZTdiZC00MjlmLWI5ODktOGMxNDZhZWEyNmM3IiwiaXNzIjoiaHR0cDovL2FjY291bnQtZGV2LmJlYWN1a2FpLmdvLmlkL2F1dGgvcmVhbG1zL21hc3RlciIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiIyODY3M2E2Zi1jZTQ3LTQ4NDQtOTRiNy1mNTcyMzUyZjY2MTMiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJzbWFydF9jdXN0b20iLCJzZXNzaW9uX3N0YXRlIjoiNzk0ZDdmZTEtYzhjMy00OWFlLWE0NjEtNWQ5NGZmZTE1MTBkIiwiYWNyIjoiMSIsImFsbG93ZWQtb3JpZ2lucyI6WyIqIiwiaHR0cDovL2xvY2FsaG9zdDozMDAwIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCBvZmZsaW5lX2FjY2VzcyIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwiaWRlbnRpdGFzIjoiMDExMzk5MDc4NTMyMDAxIiwibmFtZSI6IlBULiBEQU4gTElSSVMiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJkbGRldjIwMjIiLCJnaXZlbl9uYW1lIjoiUFQuIERBTiBMSVJJUyIsImZhbWlseV9uYW1lIjoiIiwiZW1haWwiOiJyaWFuLmF0eWFzYWJ1ZGlAZGFubGlyaXMuY29tIn0.SLl9rtrdxBdB8BXx0UUyn4deBjxQbxiBU3Ir-fNxIO-Lh0noJVWWns8DwUWBzs7ydLDrdCUetIyycwP5oc2zJi2nHD37b6apyB_70cFF4q0FYezOElTQ0FgIcedH2Al0AdjsF4f4QR4eQ4NtaZ9ZuAGo1yv0FjDluN-N4S_i88LDQE99-VEd3UON_t7IhJCZ7YtFHyXAMenG8zcgtshwU4IpkRj1ja5Qrj7a_BlA8ebNCh4AXQWf1MJ-etcwc_AuZ2iS629oimdaFZruQKhAH_l-i99fB4StQ8CMK14bMJvXcLm1kffC0ZGUvZ_dTIqjmA46pfRy3WQCrO8nhNjtYA";
            //var aju = "3010178889G220220525000099";
            //PRD
            var Respon = await CeisaService.GetRespon(Query.nomorAju, auth);

            //var Respon =  CeisaService.GetRespon(aju, auth).Result;
          
            if (Respon != null)
            {
                var ResponDaftar = Respon.Where(x => x.nomorDaftar != null).FirstOrDefault();
                Query.nomorDaftar = ResponDaftar != null ? ResponDaftar.nomorDaftar : "-";
                Query.tanggalDaftar = ResponDaftar != null ? ResponDaftar.tanggalDaftar.ToString() : "-";
            }

            return Query;
        }

        public async Task<MemoryStream> GetExcel(string noAju)
        {
            var data = await dbSet.Where(x => x.nomorAju == noAju)
                .Include(x => x.barang)
                    .ThenInclude(x => x.barangTarif)
                .Include(x => x.barang)
                    .ThenInclude(x => x.barangDokumen)
                .Include(x => x.barang)
                    .ThenInclude(x => x.bahanBaku)
                        .ThenInclude(x => x.bahanBakuTarif)
                .Include(x => x.entitas)
                .Include(x => x.dokumen)
                .Include(x => x.pengangkut)
                .Include(x => x.kontainer)
                .Include(x => x.kemasan)
                .Include(x => x.pungutan)
                .Include(x => x.bahanBaku)
                .Include(x => x.bahanBakuTarif)
                .Include(x => x.jaminan)
                .FirstOrDefaultAsync();

            using (var package = new ExcelPackage())
            {

                #region resultHeader
                DataTable resultHeader = new DataTable();

                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE DOKUMEN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE KANTOR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE KANTOR BONGKAR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE KANTOR PERIKSA", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE KANTOR TUJUAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE KANTOR EKSPOR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS IMPOR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS EKSPOR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS TPB", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS PLB", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS PROSEDUR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE TUJUAN PEMASUKAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE TUJUAN PENGIRIMAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE TUJUAN TPB", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE CARA DAGANG", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE CARA BAYAR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE CARA BAYAR LAINNYA", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE GUDANG ASAL", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE GUDANG TUJUAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS KIRIM", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS PENGIRIMAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE KATEGORI EKSPOR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE KATEGORI MASUK FTZ", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE KATEGORI KELUAR FTZ", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE KATEGORI BARANG FTZ", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE LOKASI", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE LOKASI BAYAR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "LOKASI ASAL", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "LOKASI TUJUAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE DAERAH ASAL", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE GUDANG ASAL2", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE GUDANG TUJUAN2", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE NEGARA TUJUAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE TUTUP PU", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NOMOR BC11", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TANGGAL BC11", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NOMOR POS", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NOMOR SUB POS", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE PELABUHAN BONGKAR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE PELABUHAN MUAT", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE PELABUHAN MUAT AKHIR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE PELABUHAN TRANSIT", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE PELABUHAN TUJUAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE PELABUHAN EKSPOR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE TPS", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TANGGAL BERANGKAT", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TANGGAL EKSPOR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TANGGAL MASUK", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TANGGAL MUAT", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TANGGAL TIBA", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TANGGAL PERIKSA", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TEMPAT STUFFING", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TANGGAL STUFFING", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE TANDA PENGAMAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "JUMLAH TANDA PENGAMAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "FLAG CURAH", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "FLAG SDA", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "FLAG VD", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "FLAG AP BK", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "FLAG MIGAS", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE ASURANSI", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "ASURANSI", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NILAI BARANG", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NILAI INCOTERM", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NILAI MAKLON", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "ASURANSI2", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "FREIGHT", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "FOB", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "BIAYA TAMBAHAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "BIAYA PENGURANG", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "VD", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "CIF", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "HARGA_PENYERAHAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NDPBM", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TOTAL DANA SAWIT", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "DASAR PENGENAAN PAJAK", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NILAI JASA", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "UANG MUKA", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "BRUTO", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NETTO", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "VOLUME", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KOTA PERNYATAAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TANGGAL PERNYATAAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NAMA PERNYATAAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "JABATAN PERNYATAAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE VALUTA", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE INCOTERM", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE JASA KENA PAJAK", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NOMOR BUKTI BAYAR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TANGGAL BUKTI BAYAR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS NILAI", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE KANTOR MUAT", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "NOMOR DAFTAR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TANGGAL DAFTAR", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE ASAL BARANG FTZ", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE TUJUAN PENGELUARAN", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "PPN PAJAK", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "PPNBM PAJAK", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TARIF PPN PAJAK", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "TARIF PPNBM PAJAK", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "BARANG TIDAK BERWUJUD", DataType = typeof(string) });
                resultHeader.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS PENGELUARAN", DataType = typeof(string) });

                var tglbc11 = data.tanggalBc11 != null ? data.tanggalBc11.Value.ToString("yyyy-MM-dd") : "";
                var tglTiba = data.tanggalTiba != null ? data.tanggalTiba.Value.ToString("yyyy-MM-dd") : "";
                var tglTtd = data.tanggalTtd != null ? data.tanggalTtd.ToString("yyyy-MM-dd") : "";
                var tglDaftar = data.tanggalDaftar != null ? data.tanggalDaftar.Value.ToString("yyyy-MM-dd") : "";

                resultHeader.Rows.Add(data.nomorAju, data.kodeDokumen, data.kodeKantor, data.kodeKantorBongkar, "" /*Kode Kantor Periksa*/
                    , "" /*Kode Kantor Tujuan*/, "" /*Kode Kantor Export*/, "" /*Kode Jenis Import*/, "" /*Kode Jenis Export*/, data.kodeJenisTpb
                    , "" /*Kode Jenis PLB*/, "" /*Kode Jenis Prosedur*/, data.kodeTujuanPemasukan, data.kodeTujuanPengiriman, data.kodeTujuanTpb
                    , "" /*Kode Cara Dagang*/, data.kodeCaraBayar, "" /*Kode Cara Bayar Lainnye*/, "" /*Kode Gudang Asal*/ , "" /*Kode Gudang Tujuan*/
                    , "" /*Kode Jenis Kirim*/, "" /*Kode Jenis Pengiriman*/, "" /*Kode Kategori Export*/, "" /*Kode Kategori Masuk FTZ*/, "" /*Kode Kategori Keluar FTZ*/
                    , "" /*Kode Kategori Barang FTZ*/, "" /*Kode Lokasi*/, data.kodeLokasiBayar, "" /*Lokasi Asal*/, "" /*Lokasi Tujuan*/
                    , "" /*Kode Daerah Asal*/, "" /*Kode Gudang Asal*/, "" /*Kode Gudang Tujuan*/, "" /*Kode Negara Tujuan*/, data.kodeTutupPu
                    , data.nomorBc11, tglbc11, data.posBc11, data.subposBc11, data.kodePelBongkar
                    , data.kodePelMuat, "" /*Kode Pelabuhan Muat Akhir*/, data.kodePelTransit, "" /*Kode Pelabuhan Tujuan*/, "" /*Kode Pelabuhan Export*/
                    , data.kodeTps, "" /*Tanggal Berangkat*/, "" /*Tanggal Ekspor*/, "" /*Tanggal Masuk*/, "" /*Tanggal Muat*/
                    , tglTiba, "" /*Tanggal Periksa*/, data.tempatStuffing, "" /*Tanggal Stuffing*/, "" /*Kode Tanda Pengaman*/
                    , "" /*Jumlah Tanda Pengaman*/, "" /*Flag Curah*/, "" /*Flag SDA*/, "" /*Flag VD*/, "" /*Flag AP BK*/
                    , "" /*FLag Migas*/, data.kodeAsuransi, data.asuransi, data.nilaiBarang, "" /*Nilai Intercom*/
                    , "" /*Nilai Maklon*/, data.asuransi, data.freight, data.fob, data.biayaTambahan
                    , data.biayaPengurang, data.vd, data.cif, data.hargaPenyerahan, data.ndpbm
                    , data.totalDanaSawit, "" /*Dasar Pengenaan Pajak*/, data.nilaiJasa, data.uangMuka, data.bruto
                    , data.netto, data.volume, data.kotaTtd, tglTtd, data.namaTtd
                    , data.jabatanTtd, data.kodeValuta, data.kodeIncoterm, data.kodeKenaPajak, "" /*Nomor Bukti Bayar*/
                    , "" /*Tanggal Bukti Bayar*/, "" /*Kode Jenis Nilai*/, "" /*Kode Kantor Muat*/, data.nomorDaftar, tglDaftar
                    , "" /*Kode Asal Barang FTZ*/, "" /*Kode Tujuan Pengeluaran*/, data.ppnPajak, data.ppnbmPajak, data.tarifPpnPajak
                    , data.tarifPpnbmPajak, "" /*Barang Tidak Berwujud*/, "" /*Kode Jenis Pengeluaran*/);
                #endregion
                #region resultEntitas
                DataTable resultEntitas = new DataTable();

                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "SERI", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "KODE ENTITAS", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS IDENTITAS", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "NOMOR IDENTITAS", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "NAMA ENTITAS", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "ALAMAT ENTITAS", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "NIB ENTITAS", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS API", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "KODE STATUS", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "NOMOR IJIN ENTITAS", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "TANGGAL IJIN ENTITAS", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "KODE NEGARA", DataType = typeof(string) });
                resultEntitas.Columns.Add(new DataColumn() { ColumnName = "NIPER ENTITAS", DataType = typeof(string) });

                foreach (var a in data.entitas)
                {
                    var tgl = a.tanggalIjinEntitas != DateTime.MinValue ? a.tanggalIjinEntitas.Value.ToString("yyyy-MM-dd") : "";
                    resultEntitas.Rows.Add(data.nomorAju, a.seriEntitas, a.kodeEntitas, a.kodeJenisIdentitas, a.nomorIdentitas, a.namaEntitas, a.nibEntitas, a.kodeJenisApi
                        , a.kodeStatus, a.nomorIjinEntitas, tgl, a.kodeNegara, a.niperEntitas);
                }
                #endregion
                #region resultDokumen
                DataTable resultDokumen = new DataTable();

                resultDokumen.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultDokumen.Columns.Add(new DataColumn() { ColumnName = "SERI", DataType = typeof(string) });
                resultDokumen.Columns.Add(new DataColumn() { ColumnName = "KODE DOKUMEN", DataType = typeof(string) });
                resultDokumen.Columns.Add(new DataColumn() { ColumnName = "NOMOR DOKUMEN", DataType = typeof(string) });
                resultDokumen.Columns.Add(new DataColumn() { ColumnName = "TANGGAL DOKUMEN", DataType = typeof(string) });
                resultDokumen.Columns.Add(new DataColumn() { ColumnName = "KODE FASILITAS", DataType = typeof(string) });
                resultDokumen.Columns.Add(new DataColumn() { ColumnName = "KODE IJIN", DataType = typeof(string) });

                foreach (var a in data.dokumen)
                {
                    var tgl = a.tanggalDokumen != DateTime.MinValue ? a.tanggalDokumen.ToString("yyyy-MM-dd") : "";
                    resultDokumen.Rows.Add(data.nomorAju, a.seriDokumen, a.kodeDokumen, a.nomorDokumen, tgl, a.kodeFasilitas, "");
                }
                #endregion
                #region resultPengangkut
                DataTable resultPengangkut = new DataTable();

                resultPengangkut.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultPengangkut.Columns.Add(new DataColumn() { ColumnName = "SERI", DataType = typeof(string) });
                resultPengangkut.Columns.Add(new DataColumn() { ColumnName = "KODE CARA ANGKUT", DataType = typeof(string) });
                resultPengangkut.Columns.Add(new DataColumn() { ColumnName = "NAMA PENGANGKUT", DataType = typeof(string) });
                resultPengangkut.Columns.Add(new DataColumn() { ColumnName = "NOMOR PENGANGKUT", DataType = typeof(string) });
                resultPengangkut.Columns.Add(new DataColumn() { ColumnName = "KODE BENDERA", DataType = typeof(string) });
                resultPengangkut.Columns.Add(new DataColumn() { ColumnName = "CALL SIGN", DataType = typeof(string) });
                resultPengangkut.Columns.Add(new DataColumn() { ColumnName = "FLAG ANGKUT PLB", DataType = typeof(string) });

                foreach (var a in data.pengangkut)
                {
                    resultPengangkut.Rows.Add(data.nomorAju, a.seriPengangkut, a.kodeCaraAngkut, a.namaPengangkut, a.nomorPengangkut
                        , a.kodeBendera, "" /*Call Sign*/, "" /*FLag Angkut PLB*/);
                }
                #endregion
                #region resultKemasan
                DataTable resultKemasan = new DataTable();

                resultKemasan.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultKemasan.Columns.Add(new DataColumn() { ColumnName = "SERI", DataType = typeof(string) });
                resultKemasan.Columns.Add(new DataColumn() { ColumnName = "KODE KEMASAN", DataType = typeof(string) });
                resultKemasan.Columns.Add(new DataColumn() { ColumnName = "JUMLAH KEMASAN", DataType = typeof(string) });
                resultKemasan.Columns.Add(new DataColumn() { ColumnName = "MEREK", DataType = typeof(string) });

                foreach (var a in data.kemasan)
                {
                    resultKemasan.Rows.Add(data.nomorAju, a.seriKemasan, a.kodeJenisKemasan, a.jumlahKemasan, a.merkKemasan);
                }

                #endregion
                #region resultKontainer
                DataTable resultKontainer = new DataTable();

                resultKontainer.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultKontainer.Columns.Add(new DataColumn() { ColumnName = "SERI", DataType = typeof(string) });
                resultKontainer.Columns.Add(new DataColumn() { ColumnName = "NOMOR KONTINER", DataType = typeof(string) });
                resultKontainer.Columns.Add(new DataColumn() { ColumnName = "KODE UKURAN KONTAINER", DataType = typeof(string) });
                resultKontainer.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS KONTAINER", DataType = typeof(string) });
                resultKontainer.Columns.Add(new DataColumn() { ColumnName = "KODE TIPE KONTAINER", DataType = typeof(string) });

                foreach (var a in data.kontainer)
                {
                    resultKontainer.Rows.Add(data.nomorAju, a.seriKontainer, a.nomorKontainer, a.kodeUkuranKontainer, a.kodeJenisKontainer, a.kodeTipeKontainer);
                }

                #endregion
                #region resultBarang

                #region Barang
                DataTable resultBarang = new DataTable();

                resultBarang.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "SERI BARANG", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "HS", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE BARANG", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "URAIAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "MEREK", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "TIPE", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "UKURAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "SPESIFIKASI LAIN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE SATUAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "JUMLAH SATUAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE KEMASAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "JUMLAH KEMASAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE DOKUMEN ASAL", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE KANTOR ASAL", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "NOMOR DAFTAR ASAL", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "TANGGAL DAFTAR ASAL", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU ASAL", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "SERI BARANG ASAL", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "NETTO", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "BRUTO", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "VOLUME", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "SALDO AWAL", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "SALDO AKHIR", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "JUMLAH REALISASI", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "CIF", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "CIF RUPIAH", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "NDPBM", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "FOB", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "ASURANSI", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "FREIGHT", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "NILAI TAMBAH", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "DISKON", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "HARGA PENYERAHAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "HARGA PEROLEHAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "HARGA SATUAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "HARGA EKSPOR", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "HARGA PATOKAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "NILAI BARANG", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "NILAI JASA", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "NILAI DANA SAWIT", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "NILAI DEVISA", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "PERSENTASE IMPOR", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE ASAL BARANG", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE DAERAH ASAL", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE GUNA BARANG", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS NILAI", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "JATUH TEMPO ROYALTI", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE KATEGORI BARANG", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE KONDISI BARANG", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE NEGARA ASAL", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE PERHITUNGAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "PERNYATAAN LARTAS", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "FLAG 4 TAHUN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "SERI IZIN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "TAHUN PEMBUATAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KAPASITAS SILINDER", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE BKC", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE KOMODITI BKC", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "KODE SUB KOMODITI BKC", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "FLAG TIS", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "ISI PER KEMASAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "JUMLAH DILEKATKAN", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "JUMLAH PITA CUKAI", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "HJE CUKAI", DataType = typeof(string) });
                resultBarang.Columns.Add(new DataColumn() { ColumnName = "TARIF CUKAI", DataType = typeof(string) });
                #endregion

                #region BarangTarif
                DataTable resultBarangTarif = new DataTable();

                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "SERI BARANG", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "KODE PUNGUTAN", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "KODE TARIF", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "TARIF", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "KODE FASILITAS", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "TARIF FASILITAS", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "NILAI BAYAR", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "NILAI FASILITAS", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "NILAI SUDAH DILUNASI", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "KODE SATUAN", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "JUMLAH SATUAN", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "FLAG BMT SEMENTARA", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "KODE KOMODITI CUKAI", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "KODE SUB KOMODITI CUKAI", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "FLAG TIS", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "FLAG PELEKATAN", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "KODE KEMASAN", DataType = typeof(string) });
                resultBarangTarif.Columns.Add(new DataColumn() { ColumnName = "JUMLAH KEMASAN", DataType = typeof(string) });

                #endregion
                #region BarangDokumen
                DataTable resultBarangDokumen = new DataTable();

                resultBarangDokumen.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultBarangDokumen.Columns.Add(new DataColumn() { ColumnName = "SERI BARANG", DataType = typeof(string) });
                resultBarangDokumen.Columns.Add(new DataColumn() { ColumnName = "SERI DOKUMEN", DataType = typeof(string) });
                resultBarangDokumen.Columns.Add(new DataColumn() { ColumnName = "SERI IZIN", DataType = typeof(string) });

                #endregion
                #region BarangEntitas
                DataTable resultBarangEntitas = new DataTable();

                resultBarangEntitas.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultBarangEntitas.Columns.Add(new DataColumn() { ColumnName = "SERI BARANG", DataType = typeof(string) });
                resultBarangEntitas.Columns.Add(new DataColumn() { ColumnName = "SERI DOKUMEN", DataType = typeof(string) });
                #endregion
                #region BarangSpekKhusus
                DataTable resultBarangSpekKhusus = new DataTable();

                resultBarangSpekKhusus.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultBarangSpekKhusus.Columns.Add(new DataColumn() { ColumnName = "SERI BARANG", DataType = typeof(string) });
                resultBarangSpekKhusus.Columns.Add(new DataColumn() { ColumnName = "KODE", DataType = typeof(string) });
                resultBarangSpekKhusus.Columns.Add(new DataColumn() { ColumnName = "URAIAN", DataType = typeof(string) });
                #endregion
                #region BarangVD
                DataTable resultBarangVD = new DataTable();

                resultBarangVD.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultBarangVD.Columns.Add(new DataColumn() { ColumnName = "SERI BARANG", DataType = typeof(string) });
                resultBarangVD.Columns.Add(new DataColumn() { ColumnName = "KODE VD", DataType = typeof(string) });
                resultBarangVD.Columns.Add(new DataColumn() { ColumnName = "NILAI BARANG", DataType = typeof(string) });
                resultBarangVD.Columns.Add(new DataColumn() { ColumnName = "BIAYA TAMBAHAN", DataType = typeof(string) });
                resultBarangVD.Columns.Add(new DataColumn() { ColumnName = "BIAYA PENGURANG", DataType = typeof(string) });
                resultBarangVD.Columns.Add(new DataColumn() { ColumnName = "JATUH TEMPO", DataType = typeof(string) });
                #endregion
                foreach (var a in data.barang)
                {
                    resultBarang.Rows.Add(data.nomorAju, a.seriBarang, a.posTarif, a.kodeBarang, a.uraian
                        , a.merk, a.tipe, a.ukuran, a.spesifikasiLain, a.kodeSatuanBarang
                        , a.jumlahSatuan, a.kodeJenisKemasan, a.jumlahKemasan, a.kodeDokAsal, "" /*Kode Kantor Asal*/
                        , "" /*Nomor Daftar Asal*/, "" /*Tanggal Daftar Asal*/, "" /*Nomor Aju Asal*/, "" /*Seri Barang Asal*/, a.netto
                        , a.bruto, a.volume, "" /*Saldo Awal*/, "" /*Saldo Akhir*/, a.jumlahRealisasi
                        , a.cif, a.cifRupiah, a.ndpbm, a.fob, a.asuransi
                        , a.freight, a.nilaiTambah, a.diskon, a.hargaPenyerahan, a.hargaPerolehan
                        , a.hargaSatuan, a.hargaEkspor, "" /*Harga Patokan*/, a.nilaiBarang, a.nilaiJasa
                        , "" /*Nilai Dana Sawit*/, "" /*Nilai Devisa*/, "" /*Persentasi Impor*/, a.kodeAsalBarang, "" /*Kode Daerah Asal*/
                        , a.kodeGunaBarang, "" /*Kode Jenia Nilai*/, "" /*Jatuh Tempo Royalti*/, a.kodeKategoriBarang, a.kodeKondisiBarang
                        , a.kodeNegaraAsal, a.kodePerhitungan, "" /*Pernyataan Lartas*/, "" /*FLag 4 Tahun*/, "" /*Seri Izin*/
                        , "" /*Tahun Pembuatan*/, "" /*Kapasitas Silinder*/, "" /*Kode BKC*/, "" /*Kode Komoditi BKC*/, "" /*Kode Sub Komoditi BKC*/
                        , "" /*Flag TIS*/, a.isiPerKemasan, "" /*Jumlah Dilekatkan*/, "" /*Jumlah Pita Cukai*/, "" /*HJE Cukai*/
                        , "" /*Tarif Cukai*/
                        );

                    if (a.barangTarif.Count > 0)
                    {
                        foreach (var b in a.barangTarif)
                        {
                            resultBarangTarif.Rows.Add(data.nomorAju, a.seriBarang, b.kodeJenisPungutan, b.kodeJenisTarif, b.tarif
                                , b.kodeFasilitasTarif, b.tarifFasilitas, b.nilaiBayar, b.nilaiFasilitas, b.nilaiSudahDilunasi
                                , b.kodeSatuanBarang, b.jumlahSatuan, "" /*FLag BMT Sementara*/, "" /*Kode Komoditi Cukai*/, "" /*Kode Sub Komoditi Cukai*/
                                , "" /*Flag TIS*/, "" /*FLag Pelekatan*/, "" /*Kode Kemasan*/, "" /*Jumlah Kemasan*/);
                        }
                    }
                    else
                    {
                        resultBarangTarif.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                    }


                    if (a.barangDokumen.Count > 0)
                    {
                        foreach (var c in a.barangDokumen)
                        {
                            resultBarangDokumen.Rows.Add(data.nomorAju, a.seriBarang, c.seriDokumen, "" /*Seri Izin*/);
                        }
                    }
                    else
                    {
                        resultBarangDokumen.Rows.Add("", "", "", "" /*Seri Izin*/);
                    }


                    resultBarangEntitas.Rows.Add("", "", "");
                    resultBarangSpekKhusus.Rows.Add("", "", "", "");
                    resultBarangVD.Rows.Add("", "", "", "", "", "", "");
                }

                #endregion
                #region resultBahanBaku
                DataTable resultBahanBaku = new DataTable();

                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "SERI BARANG", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "SERI BAHAN BAKU", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "KODE ASAL BAHAN BAKU", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "HS", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "KODE BARANG", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "URAIAN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "MEREK", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "TIPE", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "UKURAN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "SPESIFIKASI LAIN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "KODE SATUAN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "JUMLAH SATUAN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "KODE KEMASAN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "JUMLAH KEMASAN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "KODE DOKUMEN ASAL", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "KODE KANTOR ASAL", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "NOMOR DAFTAR ASAL", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "TANGGAL DAFTAR ASAL", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU ASAL", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "SERI BARANG ASAL", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "NETTO", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "BRUTO", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "VOLUME", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "CIF", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "CIF RUPIAH", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "NDPBM", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "HARGA PENYERAHAN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "HARGA PEROLEHAN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "NILAI JASA", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "SERI IZIN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "VALUTA", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "KODE BKC", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "KODE KOMODITI BKC", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "KODE SUB KOMODITI BKC", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "FLAG TIS", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "ISI PER KEMASAN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "JUMLAH DILEKATKAN", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "JUMLAH PITA CUKAI", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "HJE CUKAI", DataType = typeof(string) });
                resultBahanBaku.Columns.Add(new DataColumn() { ColumnName = "TARIF CUKAI", DataType = typeof(string) });

                if (data.bahanBaku.Count > 0)
                {
                    foreach (var a in data.bahanBaku)
                    {
                        resultBahanBaku.Rows.Add(data.nomorAju, a.seriBarang, a.seriBahanBaku, a.kodeAsalBahanBaku, a.posTarif
                            , a.kodeBarang, a.uraianBarang, a.merkBarang, a.tipeBarang, a.ukuranBarang
                            , a.spesifikasiLainBarang, a.kodeSatuanBarang, a.jumlahSatuan, "" /*Kode Kemasan*/, "" /*Jumlah Kemasan*/
                            , a.kodeDokAsal, a.kodeKantor, a.nomorDaftarDokAsal, a.tanggalDaftarDokAsal, "" /*Nomor Aju Asal*/
                            , a.seriBarangDokAsal, a.netto, "" /*Brutto*/, "" /*Volume*/, a.cif
                            , a.cifRupiah, a.ndpbm, a.hargaPenyerahan, a.hargaPerolehan, a.nilaiJasa
                            , a.seriIjin, "" /*Valuta*/, "" /*Kode BKC*/, "" /*Kode Komoditi BKC*/, "" /*Kode Sub Komoditi BKC*/
                            , a.flagTis, a.isiPerKemasan, "" /*Jumlah Dilekatkan*/, "" /*Jumlah Pita Cukai*/, "" /*HJE Cukai*/
                            , "" /*Tarif Cukai*/);
                    }
                }
                else
                {
                    resultBahanBaku.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
                #endregion
                #region resultBahanBakuTarif
                DataTable resultBahanBakuTarif = new DataTable();

                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "SERI BARANG", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "SERI BAHAN BAKU", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "KODE ASAL BAHAN BAKU", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "KODE PUNGUTAN", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "KODE TARIF", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "TARIF", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "KODE FASILITAS", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "TARIF FASILITAS", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "NILAI BAYAR", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "NILAI FASILITAS", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "NILAI SUDAH DILUNASI", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "KODE SATUAN", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "JUMLAH SATUAN", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "FLAG BMT SEMENTARA", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "KODE KOMODITI CUKAI", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "KODE SUB KOMODITI CUKAI", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "FLAG TIS", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "FLAG PELEKATAN", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "KODE KEMASAN", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "KODE SUB KOMODITI CUKAI2", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "FLAG TIS2", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "FLAG PELEKATAN2", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "KODE KEMASAN2", DataType = typeof(string) });
                resultBahanBakuTarif.Columns.Add(new DataColumn() { ColumnName = "JUMLAH KEMASAN", DataType = typeof(string) });

                if (data.bahanBakuTarif.Count > 0)
                {
                    foreach (var a in data.bahanBakuTarif)
                    {
                        resultBahanBakuTarif.Rows.Add(data.nomorAju, "" /*Seri Barang*/, a.seriBahanBaku, a.kodeAsalBahanBaku, a.kodeJenisPungutan
                            , a.kodeJenisTarif, a.tarif, a.kodeFasilitasTarif, a.tarifFasilitas, a.nilaiBayar
                            , a.nilaiFasilitas, a.nilaiSudahDilunasi, a.kodeSatuanBarang, a.jumlahSatuan, "" /*Flag BMT Sementara*/
                            , "" /*Kode Komoditi Cukai*/, "" /*Kode Sub Komoditi Cukai*/, "" /*Flag Tis*/, "" /*Flag Pelekatan*/, "" /*Kode Kemasan*/
                            , "" /*Kode Sub Komoditi Cukai*/, "" /*Flag Tis*/, "" /*Flag Pelekatan*/, "" /*Kode Kemasan*/, a.jumlahSatuan);
                    }

                }
                else
                {
                    resultBahanBakuTarif.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }

                #endregion  
                #region resultBahanBakuDokumen
                DataTable resultBahanBakuDokumen = new DataTable();

                resultBahanBakuDokumen.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultBahanBakuDokumen.Columns.Add(new DataColumn() { ColumnName = "SERI BARANG", DataType = typeof(string) });
                resultBahanBakuDokumen.Columns.Add(new DataColumn() { ColumnName = "SERI BAHAN BAKU", DataType = typeof(string) });
                resultBahanBakuDokumen.Columns.Add(new DataColumn() { ColumnName = "KODE_ASAL_BAHAN_BAKU", DataType = typeof(string) });
                resultBahanBakuDokumen.Columns.Add(new DataColumn() { ColumnName = "SERI DOKUMEN", DataType = typeof(string) });
                resultBahanBakuDokumen.Columns.Add(new DataColumn() { ColumnName = "SERI IZIN", DataType = typeof(string) });

                resultBahanBakuDokumen.Rows.Add("", "", "", "", "", "");

                #endregion
                #region resultPungutan
                DataTable resultPungutan = new DataTable();

                resultPungutan.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultPungutan.Columns.Add(new DataColumn() { ColumnName = "KODE FASILITAS TARIF", DataType = typeof(string) });
                resultPungutan.Columns.Add(new DataColumn() { ColumnName = "KODE JENIS PUNGUTAN", DataType = typeof(string) });
                resultPungutan.Columns.Add(new DataColumn() { ColumnName = "NILAI PUNGUTAN", DataType = typeof(string) });
                resultPungutan.Columns.Add(new DataColumn() { ColumnName = "NPWP BILLING", DataType = typeof(string) });

                if (data.pungutan.Count() > 0)
                {
                    foreach (var a in data.pungutan)
                    {
                        resultPungutan.Rows.Add(data.nomorAju, a.kodeFasilitasTarif, a.kodeJenisPungutan, a.nilaiPungutan, "");
                    }
                }
                else
                {
                    resultPungutan.Rows.Add("", "", "", "", "");
                }
                #endregion
                #region resultJaminan
                DataTable resultJaminan = new DataTable();

                resultJaminan.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultJaminan.Columns.Add(new DataColumn() { ColumnName = "KODE KANTOR", DataType = typeof(string) });
                resultJaminan.Columns.Add(new DataColumn() { ColumnName = "KODE JAMINAN", DataType = typeof(string) });
                resultJaminan.Columns.Add(new DataColumn() { ColumnName = "NOMOR JAMINAN", DataType = typeof(string) });
                resultJaminan.Columns.Add(new DataColumn() { ColumnName = "TANGGAL JAMINAN", DataType = typeof(string) });
                resultJaminan.Columns.Add(new DataColumn() { ColumnName = "NILAI JAMINAN", DataType = typeof(string) });
                resultJaminan.Columns.Add(new DataColumn() { ColumnName = "PENJAMIN", DataType = typeof(string) });
                resultJaminan.Columns.Add(new DataColumn() { ColumnName = "TANGGAL JATUH TEMPO", DataType = typeof(string) });
                resultJaminan.Columns.Add(new DataColumn() { ColumnName = "NOMOR BPJ", DataType = typeof(string) });
                resultJaminan.Columns.Add(new DataColumn() { ColumnName = "TANGGAL BPJ", DataType = typeof(string) });

                if (data.jaminan.Count() > 0)
                {
                    foreach (var a in data.jaminan)
                    {
                        resultJaminan.Rows.Add(data.nomorAju, "", a.kodeJenisJaminan, a.nomorJaminan, a.tanggalJaminan
                            , a.nilaiJaminan, a.penjamin, a.tanggalJatuhTempo, a.nomorBpj, a.tanggalBpj);
                    }
                }
                else
                {
                    resultJaminan.Rows.Add("", "", "", "", "", "", "", "", "", "");
                }
                #endregion
                #region resultBankDevisa
                DataTable resultBankDevisa = new DataTable();

                resultBankDevisa.Columns.Add(new DataColumn() { ColumnName = "NOMOR AJU", DataType = typeof(string) });
                resultBankDevisa.Columns.Add(new DataColumn() { ColumnName = "SERI", DataType = typeof(string) });
                resultBankDevisa.Columns.Add(new DataColumn() { ColumnName = "KODE", DataType = typeof(string) });
                resultBankDevisa.Columns.Add(new DataColumn() { ColumnName = "NAMA", DataType = typeof(string) });


                resultBankDevisa.Rows.Add("", "", "", "");
                #endregion
                var sheetHeader = package.Workbook.Worksheets.Add("HEADER");

                var sheetEntitas = package.Workbook.Worksheets.Add("ENTITAS");

                var sheetDokumen = package.Workbook.Worksheets.Add("DOKUMEN");

                var sheetPengangkut = package.Workbook.Worksheets.Add("PENGANGKUT");

                var sheetKemasan = package.Workbook.Worksheets.Add("KEMASAN");

                var sheetKontainer = package.Workbook.Worksheets.Add("KONTAINER");

                var sheetBarang = package.Workbook.Worksheets.Add("BARANG");

                var sheetBarangTarif = package.Workbook.Worksheets.Add("BARANGTARIF");

                var sheetBarangDokumen = package.Workbook.Worksheets.Add("BARANGDOKUMEN");

                var sheetBarangEntitas = package.Workbook.Worksheets.Add("BARANGENTITAS");

                var sheetBarangSpekKhusus = package.Workbook.Worksheets.Add("BARANGSPEKKHUSUS");

                var sheetBarangVD = package.Workbook.Worksheets.Add("BARANGVD");

                var sheetBahanBaku = package.Workbook.Worksheets.Add("BAHANBAKU");

                var sheetBahanBakuTarif = package.Workbook.Worksheets.Add("BAHANBAKUTARIF");

                var sheetBahanBakuDokumen = package.Workbook.Worksheets.Add("BAHANBAKUDOKUMEN");

                var sheetPungutan = package.Workbook.Worksheets.Add("PUNGUTAN");

                var sheetJaminan = package.Workbook.Worksheets.Add("JAMINAN");

                var sheetBankDevisa = package.Workbook.Worksheets.Add("BANKDEVISA");

                // Do something with the package

                sheetHeader.Cells["A1"].LoadFromDataTable(resultHeader, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetEntitas.Cells["A1"].LoadFromDataTable(resultEntitas, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetDokumen.Cells["A1"].LoadFromDataTable(resultDokumen, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetPengangkut.Cells["A1"].LoadFromDataTable(resultPengangkut, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetBarang.Cells["A1"].LoadFromDataTable(resultBarang, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetKemasan.Cells["A1"].LoadFromDataTable(resultKemasan, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetKontainer.Cells["A1"].LoadFromDataTable(resultKontainer, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetBarangTarif.Cells["A1"].LoadFromDataTable(resultBarangTarif, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetBarangDokumen.Cells["A1"].LoadFromDataTable(resultBarangDokumen, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetBarangEntitas.Cells["A1"].LoadFromDataTable(resultBarangEntitas, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetBarangSpekKhusus.Cells["A1"].LoadFromDataTable(resultBarangSpekKhusus, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetBarangVD.Cells["A1"].LoadFromDataTable(resultBarangVD, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetBahanBaku.Cells["A1"].LoadFromDataTable(resultBahanBaku, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetBahanBakuTarif.Cells["A1"].LoadFromDataTable(resultBahanBakuTarif, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetBahanBakuDokumen.Cells["A1"].LoadFromDataTable(resultBahanBakuDokumen, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetPungutan.Cells["A1"].LoadFromDataTable(resultPungutan, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetJaminan.Cells["A1"].LoadFromDataTable(resultJaminan, true, OfficeOpenXml.Table.TableStyles.Light16);
                sheetBankDevisa.Cells["A1"].LoadFromDataTable(resultBankDevisa, true, OfficeOpenXml.Table.TableStyles.Light16);

                sheetHeader.Cells.AutoFitColumns();
                sheetEntitas.Cells.AutoFitColumns();
                sheetDokumen.Cells.AutoFitColumns();
                sheetPengangkut.Cells.AutoFitColumns();
                sheetKemasan.Cells.AutoFitColumns();
                sheetKontainer.Cells.AutoFitColumns();
                sheetBarang.Cells.AutoFitColumns();
                sheetBarangTarif.Cells.AutoFitColumns();
                sheetBarangDokumen.Cells.AutoFitColumns();
                sheetBankDevisa.Cells.AutoFitColumns();
                sheetBarangEntitas.Cells.AutoFitColumns();
                sheetBarangSpekKhusus.Cells.AutoFitColumns();
                sheetBarangVD.Cells.AutoFitColumns();
                sheetBahanBaku.Cells.AutoFitColumns();
                sheetBahanBakuTarif.Cells.AutoFitColumns();
                sheetBahanBakuDokumen.Cells.AutoFitColumns();
                sheetPungutan.Cells.AutoFitColumns();
                sheetJaminan.Cells.AutoFitColumns();
                MemoryStream stream = new MemoryStream();
                package.SaveAs(stream);
                return stream;
            }



        }


    }
}
