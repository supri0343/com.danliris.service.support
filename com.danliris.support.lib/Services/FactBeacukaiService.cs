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

        public IQueryable<FactBeacukaiViewModel> GetReportINQuery(string type, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var array = new string[] { "BC 262", "BC 23", "BC 40", "BC 27"};
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Query = (from a in context.ViewFactBeacukai
                         where a.BCDate.AddHours(offset).Date >= DateFrom.Date
                             && a.BCDate.AddHours(offset).Date <= DateTo.Date
                             && array.Contains(a.BCType)
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
                             Quantity= String.Format("{0:n}", a.Quantity),
                             Nominal= String.Format("{0:n}", a.Nominal),
                             CurrencyCode=a.CurrencyCode,
                             UnitQtyName=a.UnitQtyName
                         });
            return Query;
        }

        public Tuple<List<FactBeacukaiViewModel>, int> GetReportIN(string type, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportINQuery(type, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.BCType).ThenBy(b => b.BCNo); 
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            var docNo = Query.ToArray();
            var q = Query.ToList();
            var index = 0;
            foreach (FactBeacukaiViewModel a in q)
            {
                FactBeacukaiViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo);
                if (dup != null)
                {
                    if (dup.count == 0)
                    {
                        index++;
                        dup.count = index;
                    }
                }
                a.count = dup.count;
            }
            Query = q.AsQueryable();

            Pageable<FactBeacukaiViewModel> pageable = new Pageable<FactBeacukaiViewModel>(Query, page - 1, size);
            List<FactBeacukaiViewModel> Data = pageable.Data.ToList<FactBeacukaiViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public IQueryable<FactBeacukaiViewModel> GetReportOUTQuery(string type, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var array = new string[] { "BC 2.6.1", "BC 3.0", "BC 4.0", "BC 4.1", "BC 2.7", "BC 2.5" };
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Query = (from a in context.ViewFactBeacukai
                         where a.BCDate.AddHours(offset).Date >= DateFrom.Date
                             && a.BCDate.AddHours(offset).Date <= DateTo.Date
                             && array.Contains(a.BCType)
                             && a.BCType == (string.IsNullOrWhiteSpace(type) ? a.BCType : type)

                         select new FactBeacukaiViewModel
                         {
                             BCNo = a.BCNo,
                             BCType = a.BCType,
                             BCDate = a.BCDate,
                             BonDate = a.BonDate,
                             BonNo = a.BonNo,
                             ItemCode = a.ItemCode,
                             ItemName = a.ItemName,
                             SupplierName = a.SupplierName,
                             Quantity = String.Format("{0:n}", a.Quantity),
                             Nominal = String.Format("{0:n}", a.Nominal),
                             CurrencyCode = a.CurrencyCode,
                             UnitQtyName = a.UnitQtyName
                         });
            
            return Query;
        }

        public Tuple<List<FactBeacukaiViewModel>, int> GetReportOUT(string type, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportOUTQuery(type, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.BCType).ThenBy(b => b.BCNo);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }
            var docNo = Query.ToArray();
            var q = Query.ToList();
            var index = 0;
            foreach (FactBeacukaiViewModel a in q)
            {
                FactBeacukaiViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo);
                if (dup != null)
                {
                    if (dup.count == 0)
                    {
                        index++;
                        dup.count = index;
                    }
                }
                a.count = dup.count;
            }
            Query = q.AsQueryable();


            Pageable<FactBeacukaiViewModel> pageable = new Pageable<FactBeacukaiViewModel>(Query, page - 1, size);
            List<FactBeacukaiViewModel> Data = pageable.Data.ToList<FactBeacukaiViewModel>();
            
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }
    }
}
