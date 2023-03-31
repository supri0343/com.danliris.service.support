using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBDokumen : StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual TPBHeader TPBHeader { get; set; }
        public string kodeDokumen { get; set; }
        public string nomorDokumen { get; set; }
        public int seriDokumen { get; set; }
        public DateTime tanggalDokumen { get; set; }
        //261
        public string kodeFasilitas { get; set; }
    }
}
