using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace com.danliris.support.lib.Helpers
{
    public class GenerateBPNo
    {
        public async Task<string> GenerateNo()
        {
            //var Index = 0;
            var date = DateTime.UtcNow;
            //var date2 = DateTime.Today();

            var bp = "BP";
            var year = (date.Year.ToString()).Substring(2, 2);
            var mounth = date.ToString("MM");
            var day = date.ToString("dd");
            var hour = date.ToString("HH");
            var minute = date.ToString("mm");
            var sec = date.ToString("ss");
            var msec = date.ToString("ffffff");

            string no = string.Concat(bp, year, mounth, day, hour, minute, sec, msec);

            return no;

        }
    }
}
