using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBBarang_Dokumen : StandardEntity<long>
    {
        public long IdBarang { get; set; }
        [ForeignKey("IdBarang")]
        public virtual TPBBarang TPBBarang { get; set; }
        public string seriDokumen { get; set; }
        
    }
}
