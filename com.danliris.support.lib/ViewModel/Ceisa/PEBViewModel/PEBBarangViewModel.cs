using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using com.danliris.support.lib.Utilities;



namespace com.danliris.support.lib.ViewModel.Ceisa.PEBViewModel
{
    public class PEBBarangViewModel 
    {
        //public long Id { get; set; }
        //public long IdHeader { get; set; }
        public double cif { get; set; }
        public double cifRupiah { get; set; }
        public double fob { get; set; }

        public decimal hargaEkspor { get; set; }

 
        public decimal hargaPatokan { get; set; }
        public double hargaPerolehan { get; set; }

     
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

        public decimal netto { get; set; } 
        public double nilaiBarang { get; set; } 
        public double nilaiDanaSawit { get; set; } 
        public string posTarif { get; set; }
        public int seriBarang { get; set; } 
        public string spesifikasiLain { get; set; }
        public string tipe { get; set; } 
        public string ukuran { get; set; } 
        public string uraian { get; set; }

        public decimal volume { get; set; }

        public List<PEBBarang_DokumenViewModel> barangDokumen { get; set; }
        public List<PEBBarang_PemilikViewModel> barangPemilik { get; set; }
        public List<PEBBarang_TarifViewModel> barangTarif { get; set; }

    }
}
