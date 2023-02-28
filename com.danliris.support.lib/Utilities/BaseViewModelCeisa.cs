using System;
using System.Collections.Generic;
using System.Text;

namespace com.danliris.support.lib.Utilities
{
    public abstract class BaseViewModelCeisa
    {
        //public long _id { get; set; }
        //public long Id { get; set; }
        public bool Active { get; set; }
        public DateTime _CreatedUtc { get; set; }
        public string _CreatedBy { get; set; }
        public string _CreatedAgent { get; set; }
        public DateTime _LastModifiedUtc { get; set; }
        public string _LastModifiedBy { get; set; }
        public string _LastModifiedAgent { get; set; }
        public bool _IsDeleted { get; set; }
        public DateTime _DeletedUtc { get; set; }
        public string _DeletedBy { get; set; }
        public string _DeletedAgent { get; set; }
    }
}
