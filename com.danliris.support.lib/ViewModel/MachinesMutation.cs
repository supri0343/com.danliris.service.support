using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace com.danliris.support.lib.ViewModel
{
    public class MachinesMutation : IValidatableObject
    {
        public string MachineID { get; set; }
        public string Classification { get; set; }
        public string MachineCategory { get; set; }
        public string MachineBrand { get; set; }
        public string MachineType { get; set; }
        public double? MachineIndex { get; set; }
        public int? MachineBeginningBalance { get; set; }
        public int? MachineQuantity { get; set; }
        public string UnitQuantity { get; set; }
        public string IDNumber { get; set; }
        public string SupplierType { get; set; }
        public DateTime? ActivateDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public double? PurchaseYear { get; set; }
        public string BCNumber { get; set; }
        public string BCOutNumber { get; set; }
        public double? MachineValue { get; set; }
        public string State { get; set; }


        public DateTime? TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public double? TransactionAmount { get; set; }
        public string Description { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TransactionType == "IN")
            {

                if (MachineCategory == null)
                {
                    yield return new ValidationResult("Kategori tidak boleh kosong", new List<string> { "MachineCategory" });
                }
                if (MachineBrand == null)
                {
                    yield return new ValidationResult("Brand tidak boleh kosong", new List<string> { "MachineBrand" });
                }
                if (MachineType == null)
                {
                    yield return new ValidationResult("Tipe tidak boleh kosong", new List<string> { "MachineType" });
                }
                if (MachineQuantity == null || MachineQuantity == 0)
                {
                    yield return new ValidationResult("Quantity tidak boleh kosong atau 0", new List<string> { "MachineQuantity" });
                }
                if (UnitQuantity == null)
                {
                    yield return new ValidationResult("Satuan tidak boleh kosong", new List<string> { "UnitQuantity" });
                }
                if (SupplierType == null || SupplierType == "-")
                {
                    yield return new ValidationResult("Supplier tidak boleh kosong", new List<string> { "SupplierType" });
                }
                if (PurchaseYear == null)
                {
                    yield return new ValidationResult("Tahun Pembelian tidak boleh kosong", new List<string> { "PurchaseYear" });
                }
                if (IDNumber == null || IDNumber == "-")
                {
                    yield return new ValidationResult("Serial Number tidak boleh kosong", new List<string> { "IDNumber" });
                }
                if (Classification == null || Classification == "-")
                {
                    yield return new ValidationResult("Klasifikasi Mesin tidak boleh kosong", new List<string> { "Classification" });
                }
                if (TransactionDate.Equals(DateTime.MinValue) || TransactionDate == null)
                {
                    yield return new ValidationResult("Tgl. Transaksi harus diisi", new List<string> { "TransactionDate" });
                }
                if (TransactionAmount == 0 || TransactionAmount == null)
                {
                    yield return new ValidationResult("Jumlah Transaksi harus diisi", new List<string> { "TransactionAmount" });
                }
            }
            else if (TransactionType == "OUT")
            {
                if (TransactionDate.Equals(DateTimeOffset.MinValue) || TransactionDate == null)
                {
                    yield return new ValidationResult("Tgl. Transaksi harus diisi", new List<string> { "TransactionDate" });
                }
                if (TransactionAmount == 0 || TransactionAmount == null)
                {
                    yield return new ValidationResult("Jumlah Transaksi harus diisi", new List<string> { "TransactionAmount" });
                }
                if (UnitQuantity == null)
                {
                    yield return new ValidationResult("Satuan tidak boleh kosong", new List<string> { "UnitQuantity" });
                }
            }

        }
    }

    public class MachineTypes
    {
        public string MachineType { get; set; }
    }
    public class MutationMachine
    {
        public string MachineCategory { get; set; }
        public string MachineBrand { get; set; }
        public string MachineType { get; set; }
        public string IDNumber { get; set; }
        public string UnitQuantity { get; set; }
        public int? MachineQuantity { get; set; }

        public Guid TransactionID { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public double? TransactionAmount { get; set; }
    }

}
