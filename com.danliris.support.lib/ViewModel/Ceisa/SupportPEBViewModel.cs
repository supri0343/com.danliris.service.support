using System;
using System.Collections.Generic;
using System.Text;

namespace com.danliris.support.lib.ViewModel.Ceisa
{
    public class SupportPEBViewModel
    {
        public string BCId { get; set; }
        public string BCType { get; set; }
        public string BCNo { get; set; }
        public string CAR { get; set; }
        public DateTime BCDate { get; set; }
        public string ExpenditureNo { get; set; }
        public DateTime ExpenditureDate { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public decimal Netto { get; set; }
        public decimal Bruto { get; set; }
        public string Pack { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Vendor { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitQtyCode { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public string CurrencyCode { get; set; }
        //public string CAR { get; set; }
        public string UomUnit { get; set; }
        //public List<SupportPEBDetailViewModel> Items { get; set; }
    }
}
