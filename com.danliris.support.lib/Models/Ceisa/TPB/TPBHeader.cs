using com.danliris.support.lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.danliris.support.lib.Models.Ceisa.TPB
{
    public class TPBHeader : BaseModel
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
        public string nomorDaftar { get; set; }
        public DateTime? tanggalDaftar { get; set; }
        public bool isPosted { get; set; }
        public string postedBy { get; set; }
        //261
        public string disclaimer { get; set; }
        public string kodeValuta { get; set; }
        public double ndpbm { get; set; }
        public double nilaiBarang { get; set; }
        public string tempatStuffing { get; set; }
        public DateTime? tglAkhirBerlaku { get; set; }
        public DateTime? tglAwalBerlaku { get; set; }
        public double totalDanaSawit { get; set; }
        //23
        public double fob { get; set; }
        public string kodeTutupPu { get; set; }
        public DateTime? tanggalTiba { get; set; }
        public string kodePelBongkar { get; set; }
        public string kodeTujuanTpb { get; set; }
        public string kodeKantorBongkar { get; set; }
        public string nomorBc11 { get; set; }
        public DateTime? tanggalBc11 { get; set; }
        public string posBc11 { get; set; }
        public string subposBc11 { get; set; }
        public string kodePelMuat { get; set; }
        public string kodePelTransit { get; set; }
        public string kodeTps { get; set; }
        
        public string kodeIncoterm { get; set; }
        public string kodeAsuransi { get; set; }
        public string kodeKenaPajak { get; set; }
        //262
        public string kodeTujuanPemasukan { get; set; }
        //25
        public string kodeCaraBayar { get; set; }
        public string kodeLokasiBayar { get; set; }
        public double diskon { get; set; }
        public double ppnPajak { get; set; }
        public double tarifPpnPajak { get; set; }
        public double ppnbmPajak { get; set; }
        public double tarifPpnbmPajak { get; set; }
        public virtual IEnumerable<TPBBarang> barang { get; set; }
        public virtual IEnumerable<TPBEntitas> entitas { get; set; }
        public virtual IEnumerable<TPBDokumen> dokumen { get; set; }
        public virtual IEnumerable<TPBPengangkut> pengangkut { get; set; }
        public virtual IEnumerable<TPBKontainer> kontainer { get; set; }
        public virtual IEnumerable<TPBKemasan> kemasan { get; set; }
        public virtual IEnumerable<TPBPungutan> pungutan { get; set; }
        //261
        public virtual IEnumerable<TPBBahanBaku> bahanBaku { get; set; }
        public virtual IEnumerable<TPBBahanBakuTarif> bahanBakuTarif { get; set; }
        public virtual IEnumerable<TPBJaminan> jaminan { get; set; }


    }
}
