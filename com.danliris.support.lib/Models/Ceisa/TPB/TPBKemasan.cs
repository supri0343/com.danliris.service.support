using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBKemasan : StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual TPBHeader TPBHeader { get; set; }
        public int jumlahKemasan { get; set; }
        public string kodeJenisKemasan { get; set; }
        public string merkKemasan { get; set; }
        public int seriKemasan { get; set; }
    }
}
