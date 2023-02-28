using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.ViewModel.Ceisa.PEBViewModel
{
    public class PEBEntitasViewModel
    {
        //public long Id { get; set; }
        //public long IdHeader { get; set; }
        //public virtual PEBHeaderViewModel PEBHeader { get; set; }

        public string alamatEntitas { get; set; }
        public string kodeEntitas { get; set; }
        public string kodeNegara { get; set; }
        public string kodeJenisIdentitas { get; set; }
        public string namaEntitas { get; set; }
        //public string nibEntitas { get; set; }
        public string nomorIdentitas { get; set; }
        public int seriEntitas { get; set; }
    }
}
