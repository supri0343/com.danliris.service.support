using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBBarang_Tarif : StandardEntity<long>
    {
        public long IdBarang { get; set; }
        [ForeignKey("IdBarang")]
        public virtual TPBBarang TPBBarang { get; set; }
        public string kodeJenisTarif { get; set; }
        public double jumlahSatuan { get; set; }
        public string kodeFasilitasTarif { get; set; }
        public string kodeSatuanBarang { get; set; }
        public double nilaiBayar { get; set; }
        public double nilaiFasilitas { get; set; }
        public double nilaiSudahDilunasi { get; set; }
        public int seriBarang { get; set; }
        public double tarif { get; set; }
        public double tarifFasilitas { get; set; }
        public string kodeJenisPungutan { get; set; }
    }
}
