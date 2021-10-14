using com.danliris.support.lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.danliris.support.lib.Services
{
    public class BeacukaiTempService : IBeacukaiTempService
    {
        private readonly SupportDbContext context;

        public BeacukaiTempService(SupportDbContext context)
        {
            this.context = context;
        }

        public List<BEACUKAI_TEMP> Get(int size = 25, string keyword = null)
        {
            string[] bcType = { "BC 262", "BC 23", "BC 40", "BC 27" };

            IQueryable<BEACUKAI_TEMP> Query = context.BeacukaiTemp.Where(s => bcType.Contains(s.JenisBC) && s.TglBCNo.Year >= DateTime.Now.Year - 1 && s.Barang != null);

            Query = Query
                .Select(p => new BEACUKAI_TEMP
                {

                    BCNo = p.BCNo,
                    BCId = p.BCId, //BonNo = p.BonNo,
                    TglBCNo = p.TglBCNo, //BCDate = p.BCDate,
                    JenisBC = p.JenisBC, //BCType = p.BCType,
                    TglDatang = p.TglDatang,
                    Hari = p.Hari,
                    Netto = Convert.ToDouble(p.Netto),
                    Bruto = Convert.ToDouble(p.Bruto)
                });

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                Query = Query.Where(s => s.BCNo.StartsWith(keyword));
            }

            Query = Query
                .OrderBy(o => o.BCNo)
                .Distinct();

            Query = Query
                .Take(size);

            return Query.ToList();
        }


    }

    public interface IBeacukaiTempService
    {
        List<BEACUKAI_TEMP> Get(int size = 25, string keyword = null);
    }
}
