using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.Models.Ceisa.PEB
{
    public class PEBBarang_Tarif : StandardEntity<long>
    {
        public long IdBarang { get; set; }
        [ForeignKey("IdBarang")]
        public virtual PEBBarang PEBBarang { get; set; }

        public string kodeJenisTarif { get; set; }

        [Column(TypeName = "decimal(20, 4)")]
        public double jumlahSatuan { get; set; }

        public string kodeFasilitasTarif { get; set; }
        public string kodeSatuanBarang { get; set; }
        public string kodeJenisPungutan { get; set; }
        public double nilaiBayar { get; set; }
        public int seriBarang { get; set; }
        public double tarif { get; set; }
        public double tarifFasilitas { get; set; }
    }
}
