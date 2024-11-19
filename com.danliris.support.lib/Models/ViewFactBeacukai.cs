﻿using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models
{
    public class ViewFactBeacukai /*: StandardEntity*/
    {
        public Guid Id { get; set; }
        //public string DetailshippingOrderId { get; set; }
        //public string BCId { get; set; }
        public string BCNo { get; set; }
		public string Tipe { get; set; }
		public string BCType { get; set; }
        public DateTime BCDate { get; set; }
        public string BonNo { get; set; }
        public DateTime? BonDate { get; set; }
        public string SupllierCode { get; set; }
        public string SupplierName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitQtyName { get; set; }
        public string Quantity { get; set; }
        public decimal Nominal { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime TglDatang { get; set; }
        public string Vendor { get; set; }


    }
}
