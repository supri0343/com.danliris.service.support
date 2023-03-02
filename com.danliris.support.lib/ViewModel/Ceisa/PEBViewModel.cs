using System;
using System.Collections.Generic;
using System.Text;

namespace com.danliris.support.lib.ViewModel.Ceisa
{
    public class PEBViewModelList
    {
        public long Id { get; set; }
        public string nomorAju { get; set; }
        public string kodeDokumen { get; set; }
        public string tanggalAju { get; set; }
        public string nomorDaftar { get; set; }
        public string tanggalDaftar { get; set; }
        public string namaPenerima { get; set; }
        public bool isPosted { get; set; }
        public string postedBy { get; set; }
        public string CreatedDate { get; set; }
    }
}
