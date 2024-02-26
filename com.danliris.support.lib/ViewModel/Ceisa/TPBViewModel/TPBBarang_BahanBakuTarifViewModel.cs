using System;
using System.Collections.Generic;
using System.Text;

namespace com.danliris.support.lib.ViewModel.Ceisa.TPBViewModel
{
    public class TPBBarang_BahanBakuTarifViewModel
    {
        public int seriBahanBaku { get; set; }
        public string kodeJenisTarif { get; set; }
        public double jumlahSatuan { get; set; }
        public string kodeFasilitasTarif { get; set; }
        public double tarif { get; set; }
        public double tarifFasilitas { get; set; }
        public string kodeJenisPungutan { get; set; }
        public double nilaiFasilitas { get; set; }
        public double nilaiBayar { get; set; }
        public double nilaiSudahDilunasi { get; set; }
    }
}
