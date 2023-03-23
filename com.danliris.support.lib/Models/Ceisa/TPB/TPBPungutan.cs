using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBPungutan:StandardEntity<long>
    {
        public long IdHeader { get; set; }
        [ForeignKey("IdHeader")]
        public virtual TPBHeader TPBHeader { get; set; }
        public string kodeFasilitasTarif { get; set; }
        public string kodeJenisPungutan { get; set; }
        public double nilaiPungutan { get; set; }
    }
}
