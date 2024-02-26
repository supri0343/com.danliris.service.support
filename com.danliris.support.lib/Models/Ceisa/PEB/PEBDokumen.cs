using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace com.danliris.support.lib.Models.Ceisa.PEB
{
    public class PEBDokumen: StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual PEBHeader PEBHeader { get; set; }

        public string idDokumen { get; set; }
        public string kodeDokumen { get; set; }
        public string nomorDokumen { get; set; }
        public DateTime tanggalDokumen { get; set; }
        public int seriDokumen { get; set; }
    }
}
