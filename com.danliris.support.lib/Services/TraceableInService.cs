using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.ViewModel;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace com.danliris.support.lib.Services
{
    public class TraceableInService
    {
        ILocalDbProductionDBContext dBContext;
        public TraceableInService(ILocalDbProductionDBContext _dBContext)
        {
            this.dBContext = _dBContext;
        }
        public IQueryable<TraceableINViewModel> getQueryTracable(string filter, string tipe)
        {
            List<TraceableINViewModel> reportData = new List<TraceableINViewModel>();

            try
            {
                string cmdtracable = "select * from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, isnull(round( sum (QtyReceipt),2),0) as QtyReceipt, ROJob,DetailExpendId, (select top(1) Invoice from ExpenditureGood e where e.RO= ROJob) as Invoice, (select top(1) PEB from ExpenditureGood e join Shipping.dbo.OmzetKonf o on o.Invoice_No=e.Invoice where e.RO= fcf.ROJob) PEB, (select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO, (select top 1 ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId) BUK, isnull((select top 1 SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK, (select top(1) TglPEB from ExpenditureGood e join Shipping.dbo.OmzetKonf o on o.Invoice_No=e.Invoice where e.RO= fcf.ROJob) TAnggalPEB, isnull((select Isnull(SUM(a.Qty),0) from ExpenditureGoodDetail a join ExpenditureGood b on a.ExpenditureGoodId = b.ExpenditureGoodId where b.RO = fcf.ROJob and ExpenditureType = 'E001'),0) EksporQty, isnull((select Isnull(SUM(a.Qty),0) from ExpenditureGoodDetail a join ExpenditureGood b on a.ExpenditureGoodId = b.ExpenditureGoodId where b.RO = fcf.ROJob and ExpenditureType = 'E003'),0) SampleQty, isnull((SELECT Sum(Qty) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI'),0) FinisihingOutQty, isnull((select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN'),0) subconOut, isnull((SELECT SUM(A.QtyCutting) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob),0) ProduksiQty  from FactControlFlow fcf group by  BCNo,BCType, BCDate,BonNo,ROJob ,DetailReceiptId,fcf.ItemCode,fcf.ItemName, DetailExpendId,POId) as data where " + tipe + " = @key order by BCNo";
                List<SqlParameter> parameters = new List<SqlParameter>();
                //parameters.Add(new SqlParameter("filter", tipe));
                parameters.Add(new SqlParameter("key", filter));

                var data = dBContext.ExecuteReader(cmdtracable, parameters);

                while (data.Read())
                {
                    reportData.Add(new TraceableINViewModel
                    {
                        BCType = data["BCType"].ToString(),
                        BCNo = data["BCNo"].ToString(),
                        BCDate = data["BCDate"].ToString(),
                        BonNo = data["BonNo"].ToString(),
                        PO = data["NoPO"].ToString(),
                        ItemCode = data["ItemCode"].ToString(),
                        ItemName = data["ItemName"].ToString(),
                        ReceiptQty = (double)data["QtyReceipt"],
                        SatuanReceipt = "MTR",
                        BUK = data["BUK"].ToString(),
                        QtyBUK = (double)data["QtyBUK"],
                        SatuanBUK = "MTR",
                        ROJob = data["ROJob"].ToString(),
                        ProduksiQty = (double)data["ProduksiQty"],
                        BJQty = (double)data["subconOut"] + (double)data["FinisihingOutQty"],
                        Invoice = data["Invoice"].ToString(),
                        PEB = data["PEB"].ToString(),
                        PEBDate = data["TAnggalPEB"].ToString() == null ? "-" : data["TAnggalPEB"].ToString(),
                        EksporQty = (double)data["EksporQty"],
                        SampleQty = (double)data["SampleQty"],
                        SubkonOutQty = (double)data["subconOut"]

                    });
                }

                //var b = reportData.ToArray();
                //var index = 0;
                //foreach (TraceableINViewModel a in reportData)
                //{
                //    TraceableINViewModel dup = Array.Find(b, o => o.BCType == a.BCType && o.BCNo == a.BCNo && o.BonNo == o.BonNo && o.PO == a.PO && o.ItemCode == a.ItemCode && o.ItemName == a.ItemName);
                //    if (dup != null)
                //    {
                //        if (dup.count == 0)
                //        {
                //            index++;
                //            dup.count = index;
                //        }
                //    }
                //    a.count = dup.count;
                //}

            }
            catch (Exception e)
            {

            }


            return reportData.AsQueryable();
        }
        public MemoryStream GetTraceableInExcel(string filter, string tipe)
        {
            var Query = getQueryTracable(filter, tipe);
            Query.OrderBy(x => x.BCNo);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis BC", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor BC", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal BC", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Terima", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No BUK", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Keluar", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Produksi Qty", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "BJ Qty", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BC Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl BC Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Ekspor Qty", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sample Qty", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", 0, "", "", 0, "", 0, 0, "", "", "", 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                //var docNo = Query.ToArray();
                //var q = Query.ToList();
                //var index = 0;
                //foreach (TraceableINViewModel a in q)
                //{
                //    TraceableINViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo && o.BonNo == o.BonNo && o.PO == a.PO && o.ItemCode == a.ItemCode && o.ItemName == a.ItemName);
                //    if (dup != null)
                //    {
                //        if (dup.count == 0)
                //        {
                //            index++;
                //            dup.count = index;
                //        }
                //    }
                //    a.count = dup.count;
                //}
                //Query = q.AsQueryable();
                var index = 0;
                foreach (var item in Query)
                {
                    index++;
                    result.Rows.Add(index, item.BCType, item.BCNo, item.BCDate, item.BonNo, item.PO, item.ItemCode, item.ItemName, item.ReceiptQty, item.SatuanReceipt, item.BUK, item.QtyBUK, item.SatuanBUK, item.ProduksiQty, item.BJQty, item.Invoice, item.PEB, item.PEBDate, item.EksporQty, item.SampleQty);

                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
            //ExcelPackage package = new ExcelPackage();
            //bool styling = true;

            //foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
            //{
            //    var sheet = package.Workbook.Worksheets.Add(item.Value);
            //    sheet.Cells["A1"].LoadFromDataTable(item.Key, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);
            //    //sheet.Cells["C1:D1"].Merge = true;
            //    //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //    //sheet.Cells["E1:F1"].Merge = true;
            //    //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            //    Dictionary<string, int> counts = new Dictionary<string, int>();
            //    Dictionary<string, int> countsType = new Dictionary<string, int>();
            //    var docNo = Query.ToArray();
            //    int value;
            //    foreach (var a in Query)
            //    {
            //        //FactBeacukaiViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo);
            //        if (counts.TryGetValue(a.BCType + a.BCNo + a.BonNo, out value))
            //        {
            //            counts[a.BCType + a.BCNo + a.BonNo]++;
            //        }
            //        else
            //        {
            //            counts[a.BCType + a.BCNo + a.BonNo] = 1;
            //        }

            //        //FactBeacukaiViewModel dup1 = Array.Find(docNo, o => o.BCType == a.BCType);
            //        if (countsType.TryGetValue(a.BCType, out value))
            //        {
            //            countsType[a.BCType]++;
            //        }
            //        else
            //        {
            //            countsType[a.BCType] = 1;
            //        }
            //    }

            //    int index = 2;
            //    foreach (KeyValuePair<string, int> b in counts)
            //    {
            //        sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Merge = true;
            //        sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            //        sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Merge = true;
            //        sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            //        sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Merge = true;
            //        sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            //        sheet.Cells["E" + index + ":E" + (index + b.Value - 1)].Merge = true;
            //        sheet.Cells["E" + index + ":E" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            //        index += b.Value;
            //    }

            //    index = 2;
            //    foreach (KeyValuePair<string, int> c in countsType)
            //    {
            //        sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Merge = true;
            //        sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            //        index += c.Value;
            //    }
            //    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            //}
            //MemoryStream stream = new MemoryStream();
            //package.SaveAs(stream);
            //return stream;
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            string Key = "";
            string cmd = "";

            List<TraceableINViewModel> data = new List<TraceableINViewModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            foreach (var f in FilterDictionary)
            {
                Key = f.Value;

            }
            if (Key == "ComodityName")
            {
                cmd = "Select Distinct a.ComodityName from Comoditi a join ExpenditureGoodDetail b on a.ComodityID = b.ComodityId where a.ComodityName Like @key";
            }
            else
            {
                cmd = "SELECT Distinct " + Key + "  FROM FactControlFlow where " + Key + " LIKE @key ";
            }
            parameters.Add(new SqlParameter("key", "%" + Keyword + "%"));
            var reader = dBContext.ExecuteReader(cmd, parameters);
            while (reader.Read())
            {
                switch (Key)
                {
                    case "BCNo":
                        data.Add(new TraceableINViewModel
                        {
                            BCNo = reader["BCNo"].ToString(),
                        });
                        break;
                    case "ROJob":
                        data.Add(new TraceableINViewModel
                        {
                            ROJob = reader["ROJob"].ToString(),
                        });
                        break;
                    case "ItemCode":
                        data.Add(new TraceableINViewModel
                        {
                            ItemCode = reader["ItemCode"].ToString(),
                        });
                        break;
                        //case "ComodityName":
                        //    data.Add(new TraceableINViewModel
                        //    {
                        //        ComodityName = reader["ComodityName"].ToString(),
                        //    });
                        //    break;
                }

            }

            Pageable<TraceableINViewModel> pageable = new Pageable<TraceableINViewModel>(data, Page - 1, Size);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            List<TraceableINViewModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            List<object> ListData = new List<object>();
            switch (Key)
            {
                case "BCNo":
                    ListData.AddRange(Data.Select(s => new
                    {
                        s.BCNo
                    }));
                    break;
                case "ROJob":
                    ListData.AddRange(Data.Select(s => new
                    {
                        s.ROJob

                    }));
                    break;
                case "ItemCode":
                    ListData.AddRange(Data.Select(s => new
                    {
                        s.ItemCode
                    }));
                    break;
                    //case "ComodityName":
                    //    ListData.AddRange(Data.Select(s => new
                    //    {
                    //        s.ComodityName
                    //    }));
                    //    break;
            }


            return new ReadResponse<object>(ListData, TotalData, OrderDictionary);
        }

        public class ReadResponse<TModel>
        {
            public List<TModel> Data { get; set; }
            public int TotalData { get; set; }
            public Dictionary<string, string> Order { get; set; }

            public ReadResponse(List<TModel> Data, int TotalData, Dictionary<string, string> Order)
            {
                this.Data = Data;
                this.TotalData = TotalData;
                this.Order = Order;
            }
        }
    }
}
