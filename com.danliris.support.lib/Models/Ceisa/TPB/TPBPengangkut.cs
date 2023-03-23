using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBPengangkut : StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual TPBHeader TPBHeader { get; set; }
        public string namaPengangkut { get; set; }
        public string nomorPengangkut { get; set; }
        public int seriPengangkut { get; set; }
    }
}
