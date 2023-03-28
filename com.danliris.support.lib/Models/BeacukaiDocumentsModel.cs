using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.Models
{
    [Table("BEACUKAI_DOCUMENTS")]
    public class BeacukaiDocumentsModel
    {
        
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
    }
}
