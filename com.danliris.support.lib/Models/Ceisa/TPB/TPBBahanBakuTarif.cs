using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBBahanBakuTarif : StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual TPBHeader TPBHeader { get; set; }
        public int seriBahanBaku { get; set; }
        public string kodeAsalBahanBaku { get; set; }
        public string kodeJenisTarif { get; set; }
        public double jumlahSatuan { get; set; }
        public string kodeFasilitasTarif { get; set; }
        public string kodeSatuanBarang { get; set; }
        public double tarif { get; set; }
        public double tarifFasilitas { get; set; }
        public string kodeJenisPungutan { get; set; }
        public double jumlahKemasan { get; set; }
        public double nilaiFasilitas { get; set; }
        public double nilaiBayar { get; set; }
        public double nilaiSudahDilunasi { get; set; }
    }
}
