using System;
using System.Collections.Generic;
using System.Text;

namespace com.danliris.support.lib.ViewModel.Ceisa.TPBViewModel
{
    public class TPBHeaderViewModel
    {
        public string asalData { get; set; }
        public double asuransi { get; set; }
        public double bruto { get; set; }
        public double cif { get; set; }
        public string kodeJenisTpb { get; set; }
        public double freight { get; set; }
        public double hargaPenyerahan { get; set; }
        public string idPengguna { get; set; }
        public string jabatanTtd { get; set; }
        public int jumlahKontainer { get; set; }
        public string kodeDokumen { get; set; }
        public string kodeKantor { get; set; }
        public string kodeTujuanPengiriman { get; set; }
        public string kotaTtd { get; set; }
        public string namaTtd { get; set; }
        public double netto { get; set; }
        public string nik { get; set; }
        public string nomorAju { get; set; }
        public int seri { get; set; }
        public DateTime tanggalAju { get; set; }
        public DateTime tanggalTtd { get; set; }
        public double volume { get; set; }
        public double biayaTambahan { get; set; }
        public double biayaPengurang { get; set; }
        public double vd { get; set; }
        public double uangMuka { get; set; }
        public double nilaiJasa { get; set; }
        //public string nomorDaftar { get; set; }
        //public DateTime? tanggalDaftar { get; set; }
        //public bool isPosted { get; set; }
        //public string postedBy { get; set; }
        public virtual List<TPBBarangViewModel> barang { get; set; }
        public virtual List<TPBEntitasViewModel> entitas { get; set; }
        public virtual List<TPBDokumenViewModel> dokumen { get; set; }
        public virtual List<TPBPengangkutViewModel> pengangkut { get; set; }
        public virtual List<TPBKontainerViewModel> kontainer { get; set; }
        public virtual List<TPBKemasanViewModel> kemasan { get; set; }
        public virtual List<TPBPungutanViewModel> pungutan { get; set; }
    }
}
