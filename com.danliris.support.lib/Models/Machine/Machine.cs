using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models.Machine
{
    public class Machine : IValidatableObject
    {
        [StringLength(50)]
        public string MachineID { get; set; }

        [StringLength(50)]
        public string Classification { get; set; }

        [StringLength(50)]
        public string MachineCategory { get; set; }

        [StringLength(50)]
        public string MachineBrand { get; set; }

        [StringLength(50)]
        public string MachineType { get; set; }

        public double? MachineIndex { get; set; }

        public int? MachineBeginningBalance { get; set; }

        public int? MachineQuantity { get; set; }

        [StringLength(50)]
        public string UnitQuantity { get; set; }

        [StringLength(256)]
        public string IDNumber { get; set; }

        [StringLength(50)]
        public string SupplierType { get; set; }

        public DateTime? ActivateDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public double? PurchaseYear { get; set; }

        [StringLength(100)]
        public string BCNumber { get; set; }

        [StringLength(100)]
        public string BCOutNumber { get; set; }

        public double? MachineValue { get; set; }

        [StringLength(10)]
        public string State { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MachineBrand == null || string.IsNullOrWhiteSpace(MachineBrand))
            {
                yield return new ValidationResult("Brand tidak boleh kosong", new List<string> { "MachineBrand" });
            }
            if (MachineQuantity == null || MachineQuantity == 0)
            {
                yield return new ValidationResult("Quantity tidak boleh kosong atau 0", new List<string> { "MachineQuantity" });
            }
            if (UnitQuantity == null || string.IsNullOrWhiteSpace(UnitQuantity))
            {
                yield return new ValidationResult("Satuan tidak boleh kosong", new List<string> { "UnitQuantity" });
            }
            if (SupplierType == null || SupplierType == "-" || string.IsNullOrWhiteSpace(SupplierType))
            {
                yield return new ValidationResult("Supplier tidak boleh kosong", new List<string> { "SupplierType" });
            }
            if (MachineType == null || string.IsNullOrWhiteSpace(MachineType))
            {
                yield return new ValidationResult("Tipe tidak boleh kosong", new List<string> { "MachineType" });
            }
            if (MachineCategory == null || string.IsNullOrWhiteSpace(MachineCategory))
            {
                yield return new ValidationResult("Kategori tidak boleh kosong", new List<string> { "MachineCategory" });
            }
            if (PurchaseYear == null || PurchaseYear == 0)
            {
                yield return new ValidationResult("Tahun Pembelian tidak boleh kosong", new List<string> { "PurchaseYear" });
            }
            if (IDNumber == null || IDNumber == "-" || string.IsNullOrWhiteSpace(IDNumber))
            {
                yield return new ValidationResult("Serial Number tidak boleh kosong", new List<string> { "IDNumber" });
            }
            if (Classification == null || Classification == "-" || string.IsNullOrWhiteSpace(Classification))
            {
                yield return new ValidationResult("Klasifikasi Mesin tidak boleh kosong", new List<string> { "Classification" });
            }
        }
    }
}
