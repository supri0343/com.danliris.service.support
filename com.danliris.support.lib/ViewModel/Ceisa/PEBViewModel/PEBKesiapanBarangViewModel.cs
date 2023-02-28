using System;
using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.danliris.support.lib.ViewModel.Ceisa.PEBViewModel
{
    public class PEBKesiapanBarangViewModel
    {
        //public long Id { get; set; }
        //public long IdHeader { get; set; }
        public string kodeJenisBarang { get; set; }
        public string kodeJenisGudang { get; set; }
        public string namaPic { get; set; }
        public string alamat { get; set; }
        public string nomorTelpPic { get; set; }
        public string lokasiSiapPeriksa { get; set; }
        public string kodeCaraStuffing { get; set; }
        public string kodeJenisPartOf { get; set; }
        public DateTime tanggalPkb { get; set; }
        public DateTimeOffset waktuSiapPeriksa { get; set; }
        public int jumlahContainer20 { get; set; }
        public int jumlahContainer40 { get; set; }
    }
}
