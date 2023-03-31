using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBJaminan : StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual TPBHeader TPBHeader { get; set; }
        public string idJaminan { get; set; }
        public string nomorBpj { get; set; }
        public string tanggalJatuhTempo { get; set; }
        public string kodeJenisJaminan { get; set; }
        public string nomorJaminan { get; set; }
        public string tanggalJaminan { get; set; }
        public double nilaiJaminan { get; set; }
        public string penjamin { get; set; }
        public string tanggalBpj { get; set; }
    }
}
