using com.danliris.support.lib.ViewModel.NewIntegrationVM;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.danliris.support.lib.ViewModel
{
    public class MachineMutationByBCNo
    {
        public string Classification { get; set; }
        public string MachineCategory { get; set; }
        public string MachineBrand { get; set; }
        public string MachineType { get; set; }
        public int? MachineQuantity { get; set; }
        public string UnitQuantity { get; set; }
        public double? PurchaseYear { get; set; }
        public string BCNumber { get; set; }
        public string BCOutNumber { get; set; }
        public double? QtyOut { get; set; }
        public SupplierViewModel Buyer { get; set; }
        public string IDNumber { get; set; }
        public string SupplierType { get; set; }
        public string MachineID { get; set; }
        public string TransactionType { get; set; }
    }
}
