using System;
using System.Collections.Generic;
using System.Text;

namespace com.danliris.support.lib.ViewModel
{
    public class FactMutationItemViewModel
    {
        public string unitCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitQtyName { get; set; }
        public double BeginQty { get; set; }
        public double ReceiptQty { get; set; }
        public double ExpenditureQty { get; set; }
        public double AdjustmentQty { get; set; }
        public double OpnameQty { get; set; }
    }
}
