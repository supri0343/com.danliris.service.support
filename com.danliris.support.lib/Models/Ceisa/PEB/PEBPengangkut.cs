using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.Models.Ceisa.PEB
{
    public class PEBPengangkut : StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual PEBHeader PEBHeader { get; set; }
        public string kodeBendera { get; set; }
        public string namaPengangkut { get; set; }
        public string nomorPengangkut { get; set; }
        public string kodeCaraAngkut { get; set; }
        public int seriPengangkut { get; set; }
    }
}
