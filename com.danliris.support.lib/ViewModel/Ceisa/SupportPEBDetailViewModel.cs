using System;
using System.Collections.Generic;
using System.Text;

namespace com.danliris.support.lib.ViewModel.Ceisa
{
    public class SupportPEBDetailViewModel
    {
        public string DetailBCId { get; set; }
        public string BCId { get; set; }

        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitQtyCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public string CAR { get; set; }
        public string UomUnit { get; set; }
    }
}
