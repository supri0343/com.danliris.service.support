using Com.Moonlay.NetCore.Lib;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Data;
using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.Models;
using com.danliris.support.lib.ViewModel;

namespace com.danliris.support.lib.Services
{
    public class FactBeacukaiService 
    {
        SupportDbContext context;
        public FactBeacukaiService(SupportDbContext _context)
        {
            this.context = _context;
        }

        public List<FactBeacukai> ReadModel(int size)
        {
            return this.context.FactBeacukai.Take(size).ToList();
        }

        public IQueryable<FactBeacukaiViewModel> GetReportQuery(string type, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Query = (from a in context.ViewFactBeacukai
                         where a.BCDate.AddHours(offset).Date >= DateFrom.Date
                             && a.BCDate.AddHours(offset).Date <= DateTo.Date
                             && a.BCType== (string.IsNullOrWhiteSpace(type) ? a.BCType : type)
                         select new FactBeacukaiViewModel
                         {
                             BCNo = a.BCNo,
                             BCType= a.BCType,
                             BCDate=a.BCDate,
                             BonDate=a.BonDate,
                             BonNo=a.BonNo,
                             ItemCode=a.ItemCode,
                             ItemName=a.ItemName,
                             SupplierName=a.SupplierName,
                             Quantity=a.Quantity,
                             Nominal=a.Nominal,
                             CurrencyCode=a.CurrencyCode,
                             UnitQtyName=a.UnitQtyName
                         });
            return Query;
        }

        public Tuple<List<FactBeacukaiViewModel>, int> GetReport(string type, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(type, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.BonDate);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<FactBeacukaiViewModel> pageable = new Pageable<FactBeacukaiViewModel>(Query, page - 1, size);
            List<FactBeacukaiViewModel> Data = pageable.Data.ToList<FactBeacukaiViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        //public override Tuple<List<FactBeacukai>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        //{
        //    throw new NotImplementedException();
        //}
    }
}
