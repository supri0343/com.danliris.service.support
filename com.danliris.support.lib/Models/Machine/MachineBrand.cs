using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace com.danliris.support.lib.Models
{
    public class MachineBrand
    {
        [Key]
        [StringLength(50)]
        public string BrandID { get; set; }

        [StringLength(100)]
        public string BrandName { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
