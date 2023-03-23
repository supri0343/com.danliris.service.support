using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBBarang:StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual TPBHeader TPBHeader { get; set; }

        public double asuransi { get; set; }
        public double bruto { get; set; }
        public double cif { get; set; }
        public double diskon { get; set; }
        public double hargaEkspor { get; set; }
        public double hargaPenyerahan { get; set; }
        public double hargaSatuan { get; set; }
        public int isiPerKemasan { get; set; }
        public double jumlahKemasan { get; set; }
        public double jumlahRealisasi { get; set; }
        public double jumlahSatuan { get; set; }
        public string kodeBarang { get; set; }
        public string kodeDokumen { get; set; }
        public string kodeJenisKemasan { get; set; }
        public string kodeSatuanBarang { get; set; }
        public string merk { get; set; }
        public double netto { get; set; }
        public double nilaiBarang { get; set; }
        public string posTarif { get; set; }
        public int seriBarang { get; set; }
        public string spesifikasiLain { get; set; }
        public string tipe { get; set; }
        public string ukuran { get; set; }
        public string uraian { get; set; }
        public double volume { get; set; }
        public double cifRupiah { get; set; }
        public double hargaPerolehan { get; set; }
        public string kodeAsalBahanBaku { get; set; }
        public double ndpbm { get; set; }
        public double uangMuka { get; set; }
        public int nilaiJasa { get; set; }
        public virtual IEnumerable<TPBBarang_Tarif> barangTarif { get; set; }
    }
}
