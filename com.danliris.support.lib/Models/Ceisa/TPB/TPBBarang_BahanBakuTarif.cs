using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBBarang_BahanBakuTarif : StandardEntity<long>
    {
        public long IdBarang_BahanBaku { get; set; }
        [ForeignKey("IdBarang_BahanBaku")]
        public virtual TPBBarang_BahanBaku TPBBarang_BahanBaku { get; set; }
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
