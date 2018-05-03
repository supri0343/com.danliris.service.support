using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace com.danliris.support.lib.ViewModel
{
    public class WIPViewModel
    {
		[Key]
		public string Kode { get; set; }
		public string Comodity { get; set; }
		public string UnitQtyName { get; set; }
		public double WIP { get; set; }
	}
}
