using Com.Moonlay.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBEntitas : StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual TPBHeader TPBHeader { get; set; }
        public string alamatEntitas { get; set; }
        public string kodeEntitas { get; set; }
        public string kodeJenisIdentitas { get; set; }
        public string namaEntitas { get; set; }
        public string nibEntitas { get; set; }
        public string nomorIdentitas { get; set; }
        public string nomorIjinEntitas { get; set; }
        public int seriEntitas { get; set; }
        public DateTime tanggalIjinEntitas { get; set; }
        public string kodeJenisApi { get; set; }
        public string kodeStatus { get; set; }
        public string kodeNegara { get; set; }
        public string niperEntitas { get; set; }
    }
}
