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
using OfficeOpenXml;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Data.SqlClient;

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

        public IQueryable<FactBeacukaiViewModel> GetReportINQuery(string type, DateTime? dateFrom, DateTime? dateTo, int offset, string no)
        {

            var array = new string[] { "BC 262", "BC 23", "BC 40", "BC 27" };
            if (type == "BC 2.6.2")
            { type = "BC 262"; }
            else if (type == "BC 2.3")
            { type = "BC 23"; }
            else if (type == "BC 4.0")
            { type = "BC 40"; }
            else if (type == "BC 2.7")
            { type = "BC 27"; }
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            string[] exceptSupplier = {"DAN LIRIS","DAN LIRIS ( DIVISI TEXTILE )","DAN LIRIS (DIVISI TEXTILE)","DAN LIRIS DIVISI TEXTILE","DAN LIRIS,GMT","PT DAN LIRIS","PT DAN LIRIS ( TEXTILE )","PT DAN LIRIS (TEXTILE)","PT DAN LIRIS DIVISI TEXTILE","PT DAN LIRIS TEXTILE ","PT DANLIRIS","PT DANLIRIS (DIV. TEXTILE)","PT. DAN LIRIS","PT. DAN LIRIS DIVISI TEXTILE","PT. DANLIRIS TEXTILE"

            };
            //if (type == "BC 27")
            //{
            var Query = type == "BC 27" ? (from a in context.ViewFactBeacukai
                                           where a.BCDate.AddHours(offset).Date >= DateFrom.Date
                                               && a.BCDate.AddHours(offset).Date <= DateTo.Date
                                               && a.Tipe == "in"
                                               && array.Contains(a.BCType)
                                               && a.BCType == (string.IsNullOrWhiteSpace(type) ? a.BCType : type)
                                               //&& a.SupplierName != "DAN LIRIS"
                                               && !exceptSupplier.Contains(a.SupplierName)
                                               && a.BCNo == (string.IsNullOrWhiteSpace(no) ? a.BCNo : no)
                                           select new FactBeacukaiViewModel
                                           {
                                               BCNo = a.BCNo,
                                               BCType = a.BCType,
                                               BCDate = a.BCDate,
                                               BonDate = a.BonDate.GetValueOrDefault(),
                                               BonNo = a.BonNo,
                                               ItemCode = a.ItemCode,
                                               ItemName = a.ItemName,
                                               SupplierName = a.SupplierName,
                                               Quantity = String.Format("{0:n}", a.Quantity),
                                               Nominal = String.Format("{0:n}", a.Nominal),
                                               CurrencyCode = a.CurrencyCode,
                                               UnitQtyName = a.UnitQtyName
                                           }) :
                         (from a in context.ViewFactBeacukai
                          where a.BCDate.AddHours(offset).Date >= DateFrom.Date
                              && a.BCDate.AddHours(offset).Date <= DateTo.Date
                              && a.Tipe == "in"
                              && array.Contains(a.BCType)
                              && a.BCType == (string.IsNullOrWhiteSpace(type) ? a.BCType : type)
                              //&& a.SupplierName != "DAN LIRIS"
                              && a.BCNo == (string.IsNullOrWhiteSpace(no) ? a.BCNo : no)
                          select new FactBeacukaiViewModel
                          {
                              BCNo = a.BCNo,
                              BCType = a.BCType,
                              BCDate = a.BCDate,
                              BonDate = a.BonDate.GetValueOrDefault(),
                              BonNo = a.BonNo,
                              ItemCode = a.ItemCode,
                              ItemName = a.ItemName,
                              SupplierName = a.SupplierName,
                              Quantity = String.Format("{0:n}", a.Quantity),
                              Nominal = String.Format("{0:n}", a.Nominal),
                              CurrencyCode = a.CurrencyCode,
                              UnitQtyName = a.UnitQtyName

                          });

            //} else
            //{
            //    var Query = (from a in context.ViewFactBeacukai
            //                 where a.BCDate.AddHours(offset).Date >= DateFrom.Date
            //                     && a.BCDate.AddHours(offset).Date <= DateTo.Date
            //                     && a.Tipe == "in"
            //                     && array.Contains(a.BCType)
            //                     && a.BCType == (string.IsNullOrWhiteSpace(type) ? a.BCType : type)
            //                 select new FactBeacukaiViewModel
            //                 {
            //                     BCNo = a.BCNo,
            //                     BCType = a.BCType,
            //                     BCDate = a.BCDate,
            //                     BonDate = a.BonDate.GetValueOrDefault(),
            //                     BonNo = a.BonNo,
            //                     ItemCode = a.ItemCode,
            //                     ItemName = a.ItemName,
            //                     SupplierName = a.SupplierName,
            //                     Quantity = String.Format("{0:n}", a.Quantity),
            //                     Nominal = String.Format("{0:n}", a.Nominal),
            //                     CurrencyCode = a.CurrencyCode,
            //                     UnitQtyName = a.UnitQtyName
            //                 });
            //}



            return Query;
        }

        public Tuple<List<FactBeacukaiViewModel>, int> GetReportIN(string type, DateTime? dateFrom, DateTime? dateTo,string no, int page, int size, string Order, int offset)
        {
            var Query = GetReportINQuery(type, dateFrom, dateTo, offset,no);

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
        
        public MemoryStream GenerateExcelIN(string type, DateTime? dateFrom, DateTime? dateTo, int offset,string no)
        {
            var Query = GetReportINQuery(type, dateFrom, dateTo, offset,no);
            Query = Query.OrderBy(b => b.BCType).ThenBy(b => b.BCNo);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Data Dok Pabean", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Jenis Dokumen", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Dokumen Pabean", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Bukti Penerimaan Barang / Good Receive Note", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pemasok / Pengirim Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Barang", DataType = typeof(Double) });

            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });

            result.Rows.Add("", "Jenis", "No. Daftar", "Tgl. Daftar", "No", "Tanggal", "", "", "","", 0,"","");
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "","",""); // to allow column name to be generated properly for empty data as template
            else
            {
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
                foreach (var item in Query)
                {
                    result.Rows.Add(item.count, item.BCType, item.BCNo, item.BCDate, item.BonNo, item.BonDate, item.SupplierName, item.ItemCode, item.ItemName, item.UnitQtyName, item.Quantity, item.Nominal, item.CurrencyCode);
                    
                }
            }

            ExcelPackage package = new ExcelPackage();
            bool styling = true;
            
            foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
            {
                var sheet = package.Workbook.Worksheets.Add(item.Value);
                sheet.Cells["A1"].LoadFromDataTable(item.Key, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);
                sheet.Cells["B1:D1"].Merge = true;
                sheet.Cells["B1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["E1:F1"].Merge = true;
                sheet.Cells["E1:F1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                char[] colToMerge = { 'A','G','H','I','J','K','L','M' };

                foreach(var col in colToMerge)
                {
                    sheet.Cells[$"{col}1:{col}2"].Merge = true;
                    sheet.Cells[$"{col}1:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"{col}1:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }
                

                Dictionary<string, int> counts = new Dictionary<string, int>();
                Dictionary<string, int> countsType = new Dictionary<string, int>();
                var docNo = Query.ToArray();
                int value;
                foreach (var a in Query)
                {
                    //FactBeacukaiViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo);
                    if (counts.TryGetValue(a.BCType + a.BCNo, out value))
                    {
                        counts[a.BCType + a.BCNo]++;
                    }
                    else
                    {
                        counts[a.BCType + a.BCNo]=1;
                    }

                    //FactBeacukaiViewModel dup1 = Array.Find(docNo, o => o.BCType == a.BCType);
                    if (countsType.TryGetValue(a.BCType, out value))
                    {
                        countsType[a.BCType]++;
                    }
                    else
                    {
                        countsType[a.BCType] = 1;
                    }
                }

                int index = 3;
                foreach(KeyValuePair<string,int> b in counts)
                {
                    sheet.Cells["A"+index+":A"+(index+b.Value-1)].Merge = true;
                    sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    if(type == "BC 2.3" || type == "BC 2.7")
                    {
                        sheet.Cells["L" + index + ":L" + (index + b.Value - 1)].Merge = true;
                        sheet.Cells["L" + index + ":L" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    }
                    index += b.Value;
                }

                index = 3;
                foreach (KeyValuePair<string, int> c in countsType)
                {
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public IQueryable<FactBeacukaiViewModel> GetReportOUTQuery(string type, DateTime? dateFrom, DateTime? dateTo, int offset,string no)
        {
            var array = new string[] { "BC 261", "BC 3.0",  "BC 41", "BC 27", "BC 25" };
			if (type == "BC 2.6.1")
			{ type = "BC 261"; }
			else if (type == "BC 3.0")
			{ type = "BC 3.0"; }
			else if (type == "BC 4.1")
			{ type = "BC 41"; }
			else if (type == "BC 2.5")
			{ type = "BC 25"; }
			else if (type == "BC 2.7")
			{ type = "BC 27"; }
			DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            string[] exceptSupplier = {"DAN LIRIS","DAN LIRIS ( DIVISI TEXTILE )","DAN LIRIS (DIVISI TEXTILE)","DAN LIRIS DIVISI TEXTILE","DAN LIRIS,GMT","PT DAN LIRIS","PT DAN LIRIS ( TEXTILE )","PT DAN LIRIS (TEXTILE)","PT DAN LIRIS DIVISI TEXTILE","PT DAN LIRIS TEXTILE ","PT DANLIRIS","PT DANLIRIS (DIV. TEXTILE)","PT. DAN LIRIS","PT. DAN LIRIS DIVISI TEXTILE","PT. DANLIRIS TEXTILE"};
            var Query = type == "BC 27" ? (from a in context.ViewFactBeacukai
                                           where a.BCDate.AddHours(offset).Date >= DateFrom.Date
                                               && a.BCDate.AddHours(offset).Date <= DateTo.Date
                                               && array.Contains(a.BCType)
                                               //&& a.Tipe == "in"
                                               && a.BCType == (string.IsNullOrWhiteSpace(type) ? a.BCType : type)
                                               //&& a.SupplierName == "DAN LIRIS"
                                               && exceptSupplier.Contains(a.SupplierName)
                                               && a.BCNo == (string.IsNullOrWhiteSpace(no) ? a.BCNo : no)
                                           select new FactBeacukaiViewModel
                                           {
                                               BCNo = a.BCNo,
                                               BCType = a.BCType,
                                               BCDate = a.BCDate,
                                               BonDate = a.BonDate.GetValueOrDefault(),
                                               BonNo = a.BonNo,
                                               ItemCode = a.ItemCode,
                                               ItemName = a.ItemName,
                                               SupplierName = a.Vendor,
                                               Quantity = String.Format("{0:n}", a.Quantity),
                                               Nominal = String.Format("{0:n}", a.Nominal),
                                               CurrencyCode = a.CurrencyCode,
                                               UnitQtyName = a.UnitQtyName
                                           }) :
                         (from a in context.ViewFactBeacukai
                          where a.BCDate.AddHours(offset).Date >= DateFrom.Date
                              && a.BCDate.AddHours(offset).Date <= DateTo.Date
                              && array.Contains(a.BCType)
                              && a.Tipe == "out"
                              && a.BCType == (string.IsNullOrWhiteSpace(type) ? a.BCType : type)
                              && a.BCNo == (string.IsNullOrWhiteSpace(no) ? a.BCNo : no)
                          select new FactBeacukaiViewModel
                          {
                              BCNo = a.BCNo,
                              BCType = a.BCType,
                              BCDate = a.BCDate,
                              BonDate = a.BonDate.GetValueOrDefault(),
                              BonNo = a.BonNo,
                              ItemCode = a.ItemCode,
                              ItemName = a.ItemName,
                              SupplierName = a.Vendor,
                              Quantity = String.Format("{0:n}", a.Quantity),
                              Nominal = String.Format("{0:n}", a.Nominal),
                              CurrencyCode = a.CurrencyCode,
                              UnitQtyName = a.UnitQtyName
                          });


            return Query;
        }

        public Tuple<List<FactBeacukaiViewModel>, int> GetReportOUT(string type, DateTime? dateFrom, DateTime? dateTo,string no, int page, int size, string Order, int offset)
        {
            var Query = GetReportOUTQuery(type, dateFrom, dateTo, offset,no);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.BCType).ThenBy(b=>b.BCDate).ThenBy(b => b.BCNo);
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

        public MemoryStream GenerateExcelOUT(string type, DateTime? dateFrom, DateTime? dateTo, int offset, string no)
        {
            var Query = GetReportOUTQuery(type, dateFrom, dateTo, offset,no);
            Query = Query.OrderBy(b => b.BCType).ThenBy(b => b.BCNo);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Data Dok Pabean", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Jenis Dokumen", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Dokumen Pabean", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Bukti Pengeluaran Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerima Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Barang", DataType = typeof(Double) });

            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });

            result.Rows.Add("", "Jenis", "No. Daftar", "Tgl. Daftar", "No", "Tanggal", "", "", "", "", 0,"","");
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "","",""); // to allow column name to be generated properly for empty data as template
            else
            {
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
                foreach (var item in Query)
                {
                    result.Rows.Add(item.count, item.BCType, item.BCNo, item.BCDate, item.BonNo, item.BonDate, item.SupplierName, item.ItemCode, item.ItemName, item.UnitQtyName, item.Quantity, item.Nominal, item.CurrencyCode);

                }
            }

            ExcelPackage package = new ExcelPackage();
            bool styling = true;

            foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
            {
                var sheet = package.Workbook.Worksheets.Add(item.Value);
                sheet.Cells["A1"].LoadFromDataTable(item.Key, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);
                sheet.Cells["B1:D1"].Merge = true;
                sheet.Cells["B1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["E1:F1"].Merge = true;
                sheet.Cells["E1:F1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                char[] colToMerge = { 'A', 'G', 'H', 'I', 'J', 'K','L','M' };

                foreach (var col in colToMerge)
                {
                    sheet.Cells[$"{col}1:{col}2"].Merge = true;
                    sheet.Cells[$"{col}1:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"{col}1:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                Dictionary<string, int> counts = new Dictionary<string, int>();
                Dictionary<string, int> countsType = new Dictionary<string, int>();
                var docNo = Query.ToArray();
                int value;
                foreach (var a in Query)
                {
                    //FactBeacukaiViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo);
                    if (counts.TryGetValue(a.BCType + a.BCNo, out value))
                    {
                        counts[a.BCType + a.BCNo]++;
                    }
                    else
                    {
                        counts[a.BCType + a.BCNo] = 1;
                    }

                    //FactBeacukaiViewModel dup1 = Array.Find(docNo, o => o.BCType == a.BCType);
                    if (countsType.TryGetValue(a.BCType, out value))
                    {
                        countsType[a.BCType]++;
                    }
                    else
                    {
                        countsType[a.BCType] = 1;
                    }
                }

                int index = 3;
                foreach (KeyValuePair<string, int> b in counts)
                {
                    sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += b.Value;
                }

                index = 3;
                foreach (KeyValuePair<string, int> c in countsType)
                {
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

		public IQueryable<BEACUKAI_TEMP> GetBeacukaiQuery(string no, int offset)
		{
			
			var Query = (from a in context.BeacukaiTemp
						 where  a.BCNo.Contains(no) && (a.JenisDokumen == "SURAT JALAN" || a.JenisDokumen == "INVOICE" || a.JenisDokumen == "PACKING LIST") 
						 select a);
			return Query;
		}
		public Tuple<List<BEACUKAI_TEMP>, int> GetBeacukai(string no, int page, int size, string Order, int offset)
		{
			var Query = GetBeacukaiQuery(no,  offset);

			Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
			var q = Query.ToList();
			if (OrderDictionary.Count.Equals(0))
			{
				Query = Query.OrderBy(b => b.TglBCNo).ThenBy(b => b.BCNo);
			}
			else
			{
				string Key = OrderDictionary.Keys.First();
				string OrderType = OrderDictionary[Key];

				//Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
			}
			 
			Query = q.AsQueryable();


			Pageable<BEACUKAI_TEMP> pageable = new Pageable<BEACUKAI_TEMP>(Query, page - 1, size);
			List<BEACUKAI_TEMP> Data = pageable.Data.ToList<BEACUKAI_TEMP>();

			int TotalData = pageable.TotalCount;

			return Tuple.Create(Data, TotalData);
		}
 

		public IQueryable<ViewFactBeacukai> GetBEACUKAI_TEMPs( string Keyword = null, string Filter = "{}")
		{
			string[] bcType = { "BC 262", "BC 23", "BC 40", "BC 27" };
			IQueryable<ViewFactBeacukai> Query = this.context.ViewFactBeacukai.Where(s=> bcType.Contains(s.BCType));
			Query = Query
				.Select(p => new ViewFactBeacukai
				{
					 
					BCNo=p.BCNo,
					BonNo = p.BonNo,
					BCDate=p.BCDate,
					BCType=p.BCType,
                    TglDatang=p.TglDatang
				}).Where(s=>s.BCNo.Contains(Keyword));

			return Query.Distinct();
		}


        public List<ViewFactBeacukai> GetBEACUKAI_ADDEDs(string invoice)
        {
            var invoices = invoice.Split(",").ToArray();
            string connectionString = APIEndpoint.ConnectionString;
            string cmdText = "Select a.BCNo, a.BCDate, a.ExpenditureDate, b.Quantity, b.ItemCode, b.ItemName, a.ExpenditureNo FROM BEACUKAI_ADDED a JOIN BEACUKAI_ADDED_DETAIL b on a.BCId = b.BCId WHERE a.ExpenditureNo IN ({0})";

            //string command = string.Format(cmdText, inClause);

            List<SqlParameter> parameters = new List<SqlParameter>();

            for (int i = 0; i < invoices.Length; i++)
            {
                parameters.Add(new SqlParameter("@inv" + i, invoices[i]));
            }

            string inClause = string.Join(", ", parameters.Select(s => s.ParameterName));




            List<ViewFactBeacukai> data = new List<ViewFactBeacukai>();

            if (parameters.Count > 0)
            {
                string command = string.Format(cmdText, inClause);

                using (SqlConnection connection = new SqlConnection(connectionString)) {

                    connection.Open();

                    SqlCommand cmd = new SqlCommand(command, connection);

                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    using (cmd)
                    {
                        cmd.CommandTimeout = (1000 * 60 * 20);
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataSet dSet = new DataSet();
                        dataAdapter.Fill(dSet);
                        foreach (DataRow dataRow in dSet.Tables[0].Rows)
                        {
                            ViewFactBeacukai trace = new ViewFactBeacukai
                            {
                                BCNo = dataRow["BCNo"].ToString(),
                                BonDate = Convert.ToDateTime(dataRow["ExpenditureDate"].ToString()),
                                BCDate = Convert.ToDateTime(dataRow["BCDate"].ToString()),
                                Quantity = (double)dataRow["Quantity"],
                                BonNo = dataRow["ExpenditureNo"].ToString(),
                                ItemCode = dataRow["ItemCode"].ToString()
                            };

                            data.Add(trace);
                        }
                    }
                }
            }
            return data;
        }

        public List<ViewFactBeacukai> GetBEACUKAI_ADDEDbyBCNo(string bcno)
        {
            string connectionString = APIEndpoint.ConnectionString;
            string cmdText = String.Format("Select a.BCNo, a.BCDate, b.Quantity, b.ItemCode, b.ItemName, a.ExpenditureNo, a.BCType FROM BEACUKAI_ADDED a JOIN BEACUKAI_ADDED_DETAIL b on a.BCId = b.BCId WHERE a.BCNo = '{0}'", bcno);

            //List<SqlParameter> parameters = new List<SqlParameter>();

            //parameters.Add(new SqlParameter("@bcno", bcno));

            List<ViewFactBeacukai> data = new List<ViewFactBeacukai>();

            //var reader 

            //if (parameters.Count > 0)
            //{
            //    string command = string.Format(cmdText, inClause);

            using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();

                    SqlCommand cmd = new SqlCommand(cmdText, connection);

                    //foreach (var parameter in parameters)
                    //{
                    //    cmd.Parameters.Add(parameter);
                    //}

                    using (cmd)
                    {
                        cmd.CommandTimeout = (1000 * 60 * 20);
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataSet dSet = new DataSet();
                        dataAdapter.Fill(dSet);
                        foreach (DataRow dataRow in dSet.Tables[0].Rows)
                        {
                            ViewFactBeacukai trace = new ViewFactBeacukai
                            {
                                BCNo = dataRow["BCNo"].ToString(),
                                BCDate = Convert.ToDateTime(dataRow["BCDate"].ToString()),
                                Quantity = (double)dataRow["Quantity"],
                                BonNo = dataRow["ExpenditureNo"].ToString(),
                                ItemCode = dataRow["ItemCode"].ToString(),
                                BCType = dataRow["BCType"].ToString()
                            };

                            data.Add(trace);
                        }
                    }
                }
            //}
            return data;
        }

        public List<ViewFactBeacukai> GetBEACUKAI_ADDEDbyDate(DateTime? dateFrom, DateTime? dateTo)
        {

            string connectionString = APIEndpoint.ConnectionString;
            string cmdText = String.Format("Select distinct a.BCNo, a.BCDate, b.Quantity, b.ItemCode, b.ItemName, a.ExpenditureNo, a.ExpenditureDate, a.BCType FROM BEACUKAI_ADDED a JOIN BEACUKAI_ADDED_DETAIL b on a.BCId = b.BCId WHERE a.ExpenditureDate BETWEEN '" + dateFrom + "' and '" + dateTo + "' ");

            //List<SqlParameter> parameters = new List<SqlParameter>();

            //parameters.Add(new SqlParameter("@bcno", bcno));

            List<ViewFactBeacukai> data = new List<ViewFactBeacukai>();

            //var reader 

            //if (parameters.Count > 0)
            //{
            //    string command = string.Format(cmdText, inClause);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();

                SqlCommand cmd = new SqlCommand(cmdText, connection);

                //foreach (var parameter in parameters)
                //{
                //    cmd.Parameters.Add(parameter);
                //}

                using (cmd)
                {
                    cmd.CommandTimeout = (1000 * 60 * 20);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    DataSet dSet = new DataSet();
                    dataAdapter.Fill(dSet);
                    foreach (DataRow dataRow in dSet.Tables[0].Rows)
                    {
                        ViewFactBeacukai trace = new ViewFactBeacukai
                        {
                            BCNo = dataRow["BCNo"].ToString(),
                            BCDate = Convert.ToDateTime(dataRow["BCDate"].ToString()),
                            BonDate = Convert.ToDateTime(dataRow["ExpenditureDate"].ToString()),
                            Quantity = (double)dataRow["Quantity"],
                            BonNo = dataRow["ExpenditureNo"].ToString(),
                            ItemCode = dataRow["ItemCode"].ToString(),
                            BCType = dataRow["BCType"].ToString()
                        };

                        data.Add(trace);
                    }
                }
            }
            //}
            return data;
        }

        public List<BEACUKAI_TEMPViewModel> GetBeacukaiTempQuery(string keyword, int offset)
        {

            var Query = (from a in context.BeacukaiTemp
                         where a.BCNo.Contains(keyword) && a.JenisBC == "BC 23" && a.TglBCNo.Year >= DateTime.Now.Year
                         select new BEACUKAI_TEMPViewModel
                         {
                             JenisBC = a.JenisBC,
                             BCNo = a.BCNo
                         }).Distinct();
                        
            return Query.ToList();
        }

    }
}
