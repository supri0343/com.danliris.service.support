using com.danliris.support.lib.ViewModel;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace com.danliris.support.lib.Services
{
    
    public class LogHistoriesService
    {
        SupportDbContext context;
        public LogHistoriesService(SupportDbContext _context) 
        {
            this.context = _context;
        }

        public List<LogHistoriesViewModel> GetQuery(DateTime? dateFrom, DateTime? dateTo) 
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var QueryTPB = context.BeacukaiTemp.Where(x => x.Hari.AddHours(7).Date >= dateFrom.Value.Date && x.Hari.AddHours(7).Date <= dateTo.Value.Date).Select(s => new LogHistoriesViewModel
            {
                Division = s.JenisBC == "BC 25" ? "SHIPPING" : "PEMBELIAN",
                Name = s.CreatedBy,
                //Date = s.Hari.ToLongDateString(),
                //Time = s.Hari.ToLongTimeString(),
                DateTime = s.Hari.ToString("dd/MM/yyyy HH:mm"),
                Activity = s.JenisBC == "BC 25" ? "Posting " + s.JenisBC + " Laporan Pengeluaran Nomor Daftar : " + s.BCNo : "Posting " + s.JenisBC + " Laporan Pemasukan Nomor Daftar : " + s.BCNo
            });

            var groupTPB = QueryTPB.GroupBy(x => new { x.Division, x.Name, x.DateTime, x.Activity }, (key, group) => new LogHistoriesViewModel
            {
                Division = key.Division,
                Name = key.Name,
                Date = "",
                Time = "",
                DateTime = key.DateTime,
                Activity = key.Activity
            });

            var QueryPEB = context.BEACUKAI_ADDED.Where(x => x.CreateDate.Value.AddHours(7).Date >= dateFrom.Value.Date && x.CreateDate.Value.AddHours(7).Date <= dateTo.Value.Date).Select(s => new LogHistoriesViewModel
            {
                Division =  "SHIPPING" ,
                Name = s.CreateUser,
                Date = "",
                Time = "",
                DateTime = s.CreateDate.Value.ToString("dd/MM/yyyy HH:mm"),
                Activity = "Posting " + s.BCType + " Laporan Pengeluaran Nomor Daftar : " + s.BCNo 
            }).Distinct();

            var result = groupTPB.Union(QueryPEB).ToList();

            foreach(var a in result)
            {
                a.Date = a.DateTime.Substring(0, 10);
                a.Time = a.DateTime.Substring(a.DateTime.Length - 5, 5);
            }

            return result.OrderBy(x => x.Division).ToList();
        }

        public Tuple<List<LogHistoriesViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order)
        {
            var Query = GetQuery(dateFrom, dateTo);

            //Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            //if (OrderDictionary.Count.Equals(0))
            //{
            //    Query = Query.OrderBy(b => b.Division);
            //}
            //else
            //{
            //    string Key = OrderDictionary.Keys.First();
            //    string OrderType = OrderDictionary[Key];
            //}

            Pageable<LogHistoriesViewModel> pageable = new Pageable<LogHistoriesViewModel>(Query, page - 1, size);
            List<LogHistoriesViewModel> Data = pageable.Data.ToList<LogHistoriesViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcelReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var Query = GetQuery(dateFrom, dateTo);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Bagian (Divisi)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "User", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Waktu", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jam", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Aktivitas", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "" ); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 1;
                foreach (var item in Query)
                {
                    result.Rows.Add(index++, item.Division, item.Name, item.Date, item.Time, item.Activity);

                }
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");

                worksheet.Cells["A1"].Value = "Laporan Aktivitas (Log) User";
                worksheet.Cells["A" + 1 + ":F" + 1 + ""].Merge = true;
                worksheet.Cells["A2"].Value = "Periode " + dateFrom.Value.ToString("dd-MM-yyyy") + " s/d " + dateTo.Value.ToString("dd-MM-yyyy");
                worksheet.Cells["A" + 2 + ":F" + 2 + ""].Merge = true;

                worksheet.Cells["A" + 1 + ":F" + 4 + ""].Style.Font.Bold = true;

                worksheet.Cells["A4"].LoadFromDataTable(result, true);

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                var stream = new MemoryStream();

                package.SaveAs(stream);

                return stream;
            }
        }
    
    }
}
