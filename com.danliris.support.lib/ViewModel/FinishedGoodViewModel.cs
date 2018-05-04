using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace com.danliris.support.lib.ViewModel
{
    public class FinishedGoodViewModel
    {
		[Key]
		public string KodeBarang { get; set; }
		public string NamaBarang { get; set; }
		public string UnitQtyName { get; set; }
		public double SaldoAwal { get; set; }
		public double Pemasukan { get; set; }
		public double Pengeluaran { get; set; }
		public double Penyesuaian { get; set; }
		public double StockOpname { get; set; }
		public double Selisih { get; set; }
		public double SaldoBuku { get; set; }
	}
}
