using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace com.danliris.support.lib.Models.Ceisa.PEB
{
    public class PEBBarang : StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual PEBHeader PEBHeader { get; set; }

        public double cif { get; set; }
        public double cifRupiah { get; set; }
        public double fob { get; set; }

        [Column(TypeName = "decimal(24, 4)")]
        public decimal hargaEkspor { get; set; }

        [Column(TypeName = "decimal(24, 4)")]
        public decimal hargaPatokan { get; set; }
        public double hargaPerolehan { get; set; }

        [Column(TypeName = "decimal(24, 4)")]
        public decimal hargaSatuan { get; set; }
        public double jumlahKemasan { get; set; }
        public double jumlahSatuan { get; set; }
        public string kodeAsalBahanBaku { get; set; }
        public string kodeBarang { get; set; }
        public string kodeDaerahAsal { get; set; }
        public string kodeDokumen { get; set; }
        public string kodeJenisKemasan { get; set; }
        public string kodeNegaraAsal { get; set; }
        public string kodeSatuanBarang { get; set; } 
        public string merk { get; set; }
        public double ndpbm { get; set; }

        [Column(TypeName = "decimal(24, 4)")]
        public decimal netto { get; set; } 
        public double nilaiBarang { get; set; } 
        public double nilaiDanaSawit { get; set; } 
        public string posTarif { get; set; }
        public int seriBarang { get; set; } 
        public string spesifikasiLain { get; set; }
        public string tipe { get; set; } 
        public string ukuran { get; set; } 
        public string uraian { get; set; }

        [Column(TypeName = "decimal(24, 4)")]
        public decimal volume { get; set; }

        public virtual IEnumerable<PEBBarang_Dokumen> barangDokumen { get; set; }
        public virtual IEnumerable<PEBBarang_Pemilik> barangPemilik { get; set; }
        public virtual IEnumerable<PEBBarang_Tarif> barangTarif { get; set; }

    }
}
