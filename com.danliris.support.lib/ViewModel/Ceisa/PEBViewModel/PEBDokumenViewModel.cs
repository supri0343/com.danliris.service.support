using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace com.danliris.support.lib.ViewModel.Ceisa.PEBViewModel
{
    public class PEBDokumenViewModel
    {
        //public long IdHeader { get; set; }
        //public long Id { get; set; }

        public string idDokumen { get; set; }
        public string kodeDokumen { get; set; }
        public string nomorDokumen { get; set; }
        public DateTime tanggalDokumen { get; set; }
        public int seriDokumen { get; set; }
    }
}
