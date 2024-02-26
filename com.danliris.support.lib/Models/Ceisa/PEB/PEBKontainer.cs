using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.Models.Ceisa.PEB
{
    public class PEBKontainer : StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual PEBHeader PEBHeader { get; set; }
        public string kodeTipeKontainer { get; set; }
        public string kodeUkuranKontainer { get; set; }
        public string nomorKontainer { get; set; }
        public string kodeJenisKontainer { get; set; }
        public int seriKontainer { get; set; }
    }
}
