using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.ViewModel.Ceisa.PEBViewModel
{
    public class PEBKontainerViewModel
    {
        //public long IdHeader { get; set; }
        //public long Id { get; set; }
        public string kodeTipeKontainer { get; set; }
        public string kodeUkuranKontainer { get; set; }
        public string nomorKontainer { get; set; }
        public string kodeJenisKontainer { get; set; }
        public int seriKontainer { get; set; }
    }
}
