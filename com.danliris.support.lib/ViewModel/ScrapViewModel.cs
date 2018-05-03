using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace com.danliris.support.lib.ViewModel
{
	public class ScrapViewModel
	{
		[Key]
		public string ClassificationId { get; set; }
		public string ClassificationCode { get; set; }
		public string ClassificationName { get; set; }
		public string StockId { get; set; }
		public string DestinationId { get; set; }
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
