using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBBahanBaku : StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual TPBHeader TPBHeader { get; set; }
        public double cif { get; set; }
        public double hargaPenyerahan { get; set; }
        public double hargaPerolehan { get; set; }
        public double jumlahSatuan { get; set; }
        public string kodeAsalBahanBaku { get; set; }
        public string kodeBarang { get; set; }
        public string kodeDokAsal { get; set; }
        public string kodeKantor { get; set; }
        public string kodeSatuanBarang { get; set; }
        public string merkBarang { get; set; }
        public double nilaiJasa { get; set; }
        public string nomorDaftarDokAsal { get; set; }
        public string nomorDokumen { get; set; }
        public string posTarif { get; set; }
        public int seriBahanBaku { get; set; }
        public int seriBarang { get; set; }
        public int seriBarangDokAsal { get; set; }
        public string spesifikasiLainBarang { get; set; }
        public string tanggalDaftarDokAsal { get; set; }
        public string tipeBarang { get; set; }
        public string ukuranBarang { get; set; }
        public string uraianBarang { get; set; }
        public double isiPerKemasan { get; set; }
        public double cifRupiah { get; set; }
        public string flagTis { get; set; }
        public string kodeDokumen { get; set; }
        public double ndpbm { get; set; }
        public double netto { get; set; }
        public int seriIjin { get; set; }
    }
}
