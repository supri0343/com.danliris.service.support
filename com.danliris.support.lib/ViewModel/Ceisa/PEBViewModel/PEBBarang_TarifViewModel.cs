using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.ViewModel.Ceisa.PEBViewModel
{
    public class PEBBarang_TarifViewModel
    {
        //public long IdBarang { get; set; }
        //public long Id { get; set; }
        public string kodeJenisTarif { get; set; }
        public decimal jumlahSatuan { get; set; }
        public string kodeFasilitasTarif { get; set; }
        public string kodeSatuanBarang { get; set; }
        public string kodeJenisPungutan { get; set; }
        public double nilaiBayar { get; set; }
        public int seriBarang { get; set; }
        public double tarif { get; set; }
        public double tarifFasilitas { get; set; }
    }
}
