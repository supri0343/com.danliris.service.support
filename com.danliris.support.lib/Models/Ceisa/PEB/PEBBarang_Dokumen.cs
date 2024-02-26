using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.Models.Ceisa.PEB
{
    public class PEBBarang_Dokumen : StandardEntity<long>
    {
        public long IdBarang { get; set; }
        [ForeignKey("IdBarang")]
        public virtual PEBBarang PEBBarang { get; set; }
        public int seriDokumen { get; set; }

    }
}
