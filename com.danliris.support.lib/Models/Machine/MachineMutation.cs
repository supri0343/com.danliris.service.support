using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.danliris.support.lib.Models.Machine
{
    public class MachineMutation : IValidatableObject
    {
        [Key]
        public Guid TransactionID { get; set; }

        [StringLength(50)]
        public string MachineID { get; set; }

        public DateTime? TransactionDate { get; set; }

        [StringLength(50)]
        public string TransactionType { get; set; }

        public double? TransactionAmount { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TransactionType == null || string.IsNullOrWhiteSpace(TransactionType))
            {
                yield return new ValidationResult("Tipe tidak boleh kosong", new List<string> { "TransactionType" });
            }
            if (TransactionAmount == null || TransactionAmount == 0)
            {
                yield return new ValidationResult("Jumlah Transaksi tidak boleh kosong", new List<string> { "TransactionAmount" });
            }
            if (TransactionDate == null || (TransactionDate.Equals(DateTime.MinValue)))
            {
                yield return new ValidationResult("Tanggal tidak boleh kosong", new List<string> { "TransactionDate" });
            }
        }
    }
}
