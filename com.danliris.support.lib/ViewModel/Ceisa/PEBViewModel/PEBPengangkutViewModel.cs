using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.ViewModel.Ceisa.PEBViewModel
{
    public class PEBPengangkutViewModel
    {
        //public long IdHeader { get; set; }
        //public long Id { get; set; }
        public string kodeBendera { get; set; }
        public string namaPengangkut { get; set; }
        public string nomorPengangkut { get; set; }
        public string kodeCaraAngkut { get; set; }
        public int seriPengangkut { get; set; }
    }
}
