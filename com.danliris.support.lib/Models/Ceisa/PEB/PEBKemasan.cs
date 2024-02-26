using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.Models.Ceisa.PEB
{
    public class PEBKemasan : StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual PEBHeader PEBHeader { get; set; }

        public int jumlahKemasan { get; set; }
        public string kodeJenisKemasan { get; set; }
        public string merkKemasan { get; set; }
        public int seriKemasan { get; set; }
    }
}
