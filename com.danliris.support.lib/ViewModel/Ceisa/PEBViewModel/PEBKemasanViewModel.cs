using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.ViewModel.Ceisa.PEBViewModel
{
    public class PEBKemasanViewModel
    {
        //public long IdHeader { get; set; }
        //public long Id { get; set; }
        public int jumlahKemasan { get; set; }
        public string kodeJenisKemasan { get; set; }
        public string merkKemasan { get; set; }
        public int seriKemasan { get; set; }
    }
}
