using System;
using System.Collections.Generic;
using System.Text;
using com.danliris.support.lib.Utilities;
namespace com.danliris.support.lib.ViewModel.Ceisa.PEBViewModel
{
    public class PEBHeaderViewModel : BaseViewModelCeisa
    {
        //public long Id { get; set; }
        public string asalData { get; set; }
        public double asuransi { get; set; }
        public decimal bruto { get; set; }
        public double cif { get; set; }
        //public string disclaimer { get; set; }
        public string flagCurah { get; set; }
        public string flagMigas { get; set; }
        public double fob { get; set; }
        public double freight { get; set; }
        public string idPengguna { get; set; }
        public string jabatanTtd { get; set; }
        public double jumlahKontainer { get; set; }
        public string kodeAsuransi { get; set; }
        public string kodeCaraBayar { get; set; }
        public string kodeCaraDagang { get; set; }
        public string kodeDokumen { get; set; }
        public string kodeIncoterm { get; set; }
        public string kodeJenisEkspor { get; set; }
        //public string kodeJenisNilai { get; set; }
        //public string kodeJenisProsedur { get; set; }
        public string kodeKantor { get; set; }
        public string kodeKantorEkspor { get; set; }
        public string kodeKantorMuat { get; set; }
        public string kodeKantorPeriksa { get; set; }
        public string kodeKategoriEkspor { get; set; }
        public string kodeLokasi { get; set; }
        public string kodeNegaraTujuan { get; set; }
        public string kodePelBongkar { get; set; }
        public string kodePelEkspor { get; set; }
        public string kodePelMuat { get; set; }
        public string kodePelTujuan { get; set; }
        public string kodePembayar { get; set; }
        public string kodeTps { get; set; }
        public string kodeValuta { get; set; }
        public string kotaTtd { get; set; }
        public string namaTtd { get; set; }

        public decimal ndpbm { get; set; }

        public decimal netto { get; set; }

        public double nilaiMaklon { get; set; }
        public string nomorAju { get; set; }
        //public string nomorBc11 { get; set; }
        //public string posBc11 { get; set; }
        public int seri { get; set; }
        //public string subposBc11 { get; set; }
        public DateTime tanggalAju { get; set; }
        //public DateTime tanggalBc11 { get; set; }
        public DateTime tanggalEkspor { get; set; }
        public DateTime tanggalPeriksa { get; set; }
        public DateTime tanggalTtd { get; set; }
        public double totalDanaSawit { get; set; }
        public string nomorDaftar { get; set; }
        public DateTime? tanggalDaftar { get; set; }
        public virtual List<PEBBarangViewModel> barang { get; set; }
        public virtual List<PEBEntitasViewModel> entitas { get; set; }
        public virtual List<PEBKemasanViewModel> kemasan { get; set; }
        public virtual List<PEBKontainerViewModel> kontainer { get; set; }
        public ICollection<PEBDokumenViewModel> dokumen { get; set; }
        public virtual List<PEBPengangkutViewModel> pengangkut { get; set; }
        public virtual List<PEBBankDevisaViewModel> bankDevisa { get; set; }
        public virtual List<PEBKesiapanBarangViewModel> kesiapanBarang { get; set; }
    }
}
