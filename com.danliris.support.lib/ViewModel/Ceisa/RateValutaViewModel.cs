using System;
using System.Collections.Generic;
using System.Text;

namespace com.danliris.support.lib.ViewModel.Ceisa
{
    public class RateValutaViewModel
    {
        public string kodeValuta { get; set; }
        public int nilaiKurs { get; set; }
    }

    //public class ResultLoginCeisa
    //{
    //    public string access_token { get; set; }
    //    public string refresh_token { get; set; }
    //}

    //public class TokenCeisa
    //{
    //    public string access_token { get; set; }
    //    public string refresh_token { get; set; }
    //}

    public class ResultLoginCeisa
    {
        public string status { get; set; }
        public string message { get; set; }
        public Item item { get; set; }
    }

    public class Item
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public int refresh_expires_in { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
}
