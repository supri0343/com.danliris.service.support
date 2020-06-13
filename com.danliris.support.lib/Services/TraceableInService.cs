using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.ViewModel;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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

        public IQueryable<TraceableINViewModel> getQueryTracable2(string filter, string tipe, DateTime? Datefrom, DateTime? DateTo)
        {
            DateTime dateFrom = Datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)Datefrom;
            DateTime dateTo = DateTo == null ? DateTime.Now : (DateTime)DateTo;
            string datefrom = dateFrom.ToString("yyyy-MM-dd");
            string dateto = dateTo.ToString("yyyy-MM-dd");
            List<TraceableINViewModel> reportData = new List<TraceableINViewModel>();
                string connectionString = APIEndpoint.LocalConnectionString;
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (tipe == "BCDate")
                    {
                        
                        using (SqlCommand cmd = new SqlCommand("select * from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, b.Invoice,Sum(c.Qty) As EksporQty, b.ExpenditureType, isnull(a.PEB,'-') as PEB, isnull(a.TglPEB,'1970-01-01') as TAnggalPEB, " +
                            "isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = fcf.DetailShippingOrderId),0) As QtyReceipt," +
                            "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk," +
                            "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar," +
                            "ROJob,DetailExpendId," +
                            "(select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO," +
                            "isnull((select  ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),'-') BUK," +
                            "isnull((select  SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK," +
                            "(SELECT isnull(Sum(Qty),0) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI') FinisihingOutQty," +
                            "(select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN') subconOut," +
                            "(SELECT isnull(SUM(A.QtyCutting),0) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob) ProduksiQty " +
                            "from FactControlFlow fcf " +
                            "join ExpenditureGood b on fcf.ROJob = b.RO " +
                            "join ExpenditureGoodDetail c on c.ExpenditureGoodId = b.ExpenditureGoodId " +
                            "left join Shipping.dbo.OmzetKonf a on b.Invoice = a.Invoice_No " +
                            "group by BCNo,BCType, BCDate,BonNo," +
                            "ROJob ,DetailReceiptId, DetailShippingOrderId," +
                            "fcf.ItemCode," +
                            "fcf.ItemName, DetailExpendId,POId, b.Invoice, b.ExpenditureType, a.PEB, a.TglPEB) as data where  bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' and ExpenditureType != 'E002' order by PEB, Invoice, BCNo, BonNo, BCType, NoPO", conn))
                        {
                            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                            DataSet dSet = new DataSet();
                            dataAdapter.Fill(dSet);
                            foreach (DataRow data in dSet.Tables[0].Rows)
                            {
                                TraceableINViewModel trace = new TraceableINViewModel
                                {
                                    BCType = data["BCType"].ToString(),
                                    BCNo = data["BCNo"].ToString(),
                                    BCDate = data["BCDate"].ToString(),
                                    BonNo = data["BonNo"].ToString(),
                                    PO = data["NoPO"].ToString(),
                                    ItemCode = data["ItemCode"].ToString(),
                                    ItemName = data["ItemName"].ToString(),
                                    ReceiptQty = (double)data["QtyReceipt"],
                                    SatuanReceipt = data["SatMasuk"].ToString(),
                                    BUK = data["BUK"].ToString(),
                                    QtyBUK = (double)data["QtyBUK"],
                                    SatuanBUK = data["SatKeluar"].ToString(),
                                    ROJob = data["ROJob"].ToString(),
                                    ProduksiQty = (double)data["ProduksiQty"],
                                    BJQty = (double)data["subconOut"] + (double)data["FinisihingOutQty"],
                                    Invoice = data["Invoice"].ToString(),
                                    PEB = data["PEB"].ToString(),
                                    PEBDate = data["TAnggalPEB"].ToString() == null ? "-" : data["TAnggalPEB"].ToString(),
                                    EksporQty = data["ExpenditureType"].ToString() == "E001" ? (double)data["EksporQty"] : 0,
                                    SampleQty = data["ExpenditureType"].ToString() == "E003" ? (double)data["SampleQty"] : 0,
                                    SubkonOutQty = (double)data["subconOut"],
                                    Sisa = (double)data["QtyReceipt"] - (double)data["QtyBUK"],
                                    ExType = data["ExpenditureType"].ToString()
                                };
                                reportData.Add(trace);
                            }
                        }


                    }
                    else
                    {
                        string cmdtraceable = "select ROW_NUMBER() Over(Order By NoPO, BUK,  QtyBUK, QtyReceipt) row_num, '' as BCNo, '' as BonNo, '' as BCType, null as BCDate,ItemCode,  ItemName,'' as Invoice, 0 as EksporQty, '' as ExpenditureType, '' as PEB, null as TAnggalPEB, QtyReceipt, '' as SatMasuk, '' as SatKeluar, '' as ROJob, BUK, QtyBUK, 0 as ProduksiQty, 0 as subconOut, NoPO, 0 as FinisihingOutQty into #PO " +
                            "from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, b.Invoice,Sum(c.Qty) As EksporQty, b.ExpenditureType, isnull(a.PEB,'-') as PEB, isnull(a.TglPEB,'1970-01-01') as TAnggalPEB, " +
                            "isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = fcf.DetailShippingOrderId),0) As QtyReceipt," +
                            "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk," +
                            "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar," +
                            "ROJob,DetailExpendId," +
                            "(select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO," +
                            "isnull((select  ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),'-') BUK," +
                            "isnull((select  SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK," +
                            "(SELECT isnull(Sum(Qty),0) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI') FinisihingOutQty," +
                            "(select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN') subconOut," +
                            "(SELECT isnull(SUM(A.QtyCutting),0) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob) ProduksiQty " +
                            "from FactControlFlow fcf " +
                            "join ExpenditureGood b on fcf.ROJob = b.RO " +
                            "join ExpenditureGoodDetail c on c.ExpenditureGoodId = b.ExpenditureGoodId " +
                            "left join Shipping.dbo.OmzetKonf a on b.Invoice = a.Invoice_No " +
                            "group by BCNo,BCType, BCDate,BonNo," +
                            "ROJob ,DetailReceiptId, DetailShippingOrderId," +
                            "fcf.ItemCode," +
                            "fcf.ItemName, DetailExpendId,POId, b.Invoice, b.ExpenditureType, a.PEB, a.TglPEB) as data where " + tipe + " = '" + filter + "' AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' and ExpenditureType != 'E002' order by NoPO, BUK,  QtyBUK,data.QtyReceipt " +
                            "select ROW_NUMBER() Over(order by Invoice, EksporQty, PEB, BCNo, BonNo, BCType, ROJob, ProduksiQty, FinisihingOutQty) row_num, BCNo,  BonNo, BCType, BCDate, '' as ItemCode, '' as ItemName, Invoice, EksporQty, ExpenditureType, PEB, TAnggalPEB, 0 as QtyReceipt, SatMasuk, SatKeluar, ROJob, '' as BUK, 0 as QtyBUK, ProduksiQty, subconOut, '' as NoPO, FinisihingOutQty into #BCNo " +
                            "from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, b.Invoice,Sum(c.Qty) As EksporQty, b.ExpenditureType, isnull(a.PEB,'-') as PEB, isnull(a.TglPEB,'1970-01-01') as TAnggalPEB, " +
                            "isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = fcf.DetailShippingOrderId),0) As QtyReceipt," +
                            "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk," +
                            "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar," +
                            "ROJob,DetailExpendId," +
                            "(select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO," +
                            "isnull((select  ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),'-') BUK," +
                            "isnull((select  SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK," +
                            "(SELECT isnull(Sum(Qty),0) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI') FinisihingOutQty," +
                            "(select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN') subconOut," +
                            "(SELECT isnull(SUM(A.QtyCutting),0) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob) ProduksiQty " +
                            "from FactControlFlow fcf " +
                            "join ExpenditureGood b on fcf.ROJob = b.RO " +
                            "join ExpenditureGoodDetail c on c.ExpenditureGoodId = b.ExpenditureGoodId " +
                            "left join Shipping.dbo.OmzetKonf a on b.Invoice = a.Invoice_No " +
                            "group by BCNo,BCType, BCDate,BonNo," +
                            "ROJob ,DetailReceiptId, DetailShippingOrderId," +
                            "fcf.ItemCode," +
                            "fcf.ItemName, DetailExpendId,POId, b.Invoice, b.ExpenditureType, a.PEB, a.TglPEB) as data where " + tipe + " = '" + filter + "' AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' and ExpenditureType != 'E002' order by Invoice, EksporQty, PEB, BCNo, BonNo, BCType, ROJob, ProduksiQty, FinisihingOutQty " +
                            "select * from (select * from #PO union all select * from #BCNo)as data " +
                            "drop table #PO " +
                            "drop table #BCNo ";
                        using (SqlCommand cmd = new SqlCommand(cmdtraceable, conn)) { 
                            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                            DataSet dSet = new DataSet();
                            dataAdapter.Fill(dSet);
                            foreach (DataRow data in dSet.Tables[0].Rows)
                            {
                                TraceableINViewModel trace = new TraceableINViewModel
                                {
                                    No = (long)data["row_num"],
                                    BCType = data["BCType"].ToString(),
                                    BCNo = data["BCNo"].ToString(),
                                    BCDate = data["BCDate"].ToString() == null ? "" : data["BCDate"].ToString(),
                                    BonNo = data["BonNo"].ToString(),
                                    PO = data["NoPO"].ToString(),
                                    ItemCode = data["ItemCode"].ToString(),
                                    ItemName = data["ItemName"].ToString(),
                                    ReceiptQty = (double)data["QtyReceipt"],
                                    SatuanReceipt = data["SatMasuk"].ToString(),
                                    BUK = data["BUK"].ToString(),
                                    QtyBUK = (double)data["QtyBUK"],
                                    SatuanBUK = data["SatKeluar"].ToString(),
                                    ROJob = data["ROJob"].ToString(),
                                    ProduksiQty = (double)data["ProduksiQty"],
                                    BJQty = (double)data["subconOut"] + (double)data["FinisihingOutQty"],
                                    Invoice = data["Invoice"].ToString(),
                                    PEB = data["PEB"].ToString(),
                                    PEBDate = data["TAnggalPEB"].ToString() == null ? "" : data["TAnggalPEB"].ToString(),
                                    EksporQty = data["ExpenditureType"].ToString() == "E001" ? (double)data["EksporQty"] : 0,
                                    SampleQty = data["ExpenditureType"].ToString() == "E003" ? (double)data["SampleQty"] : 0,
                                    SubkonOutQty = (double)data["subconOut"],
                                    Sisa = (double)data["QtyReceipt"] - (double)data["QtyBUK"],
                                    ExType = data["ExpenditureType"].ToString()
                                };
                                reportData.Add(trace);
                            }
                        }
                    }
                    conn.Close();
                }
                var report = (from a in reportData
                             group a by a.No into datagroup
                             select new TraceableINViewModel
                             {
                                 BCType = String.Join("", datagroup.Select(i=>i.BCType)),
                                 BCNo = String.Join("", datagroup.Select(i => i.BCNo)),
                                 BCDate = String.Join("", datagroup.Select(i => i.BCDate)).Trim(),
                                 BonNo = String.Join("", datagroup.Select(i => i.BonNo)),
                                 PO = String.Join("", datagroup.Select(i => i.PO)),
                                 ItemCode = String.Join("", datagroup.Select(i => i.ItemCode)),
                                 ItemName = String.Join("", datagroup.Select(i => i.ItemName)),
                                 ReceiptQty = datagroup.Sum(x => x.ReceiptQty),
                                 SatuanReceipt = String.Join("", datagroup.Select(i => i.SatuanReceipt)),
                                 BUK = String.Join("", datagroup.Select(i => i.BUK)),
                                 QtyBUK = datagroup.Sum(x => x.QtyBUK),
                                 SatuanBUK = String.Join("", datagroup.Select(i => i.SatuanBUK)),
                                 ROJob = String.Join("", datagroup.Select(i => i.ROJob)),
                                 ProduksiQty = datagroup.Sum(x => x.ProduksiQty),
                                 BJQty = datagroup.Sum(x => x.BJQty),
                                 Invoice = String.Join("", datagroup.Select(i => i.Invoice)),
                                 PEB = String.Join("", datagroup.Select(i => i.PEB)),
                                 PEBDate = String.Join("", datagroup.Select(i => i.PEBDate)).Trim(),
                                 EksporQty = datagroup.Sum(x => x.EksporQty),
                                 SampleQty = datagroup.Sum(x => x.SampleQty),
                                 SubkonOutQty = datagroup.Sum(x => x.SubkonOutQty),
                                 Sisa = datagroup.Sum(x => x.Sisa),
                                 ExType = String.Join("", datagroup.Select(i => i.ExType))
                             }).ToList();
                var b = report.ToArray();
                var index = 0;
                foreach (TraceableINViewModel a in report)
                {
                    TraceableINViewModel dup = Array.Find(b, o => o.BCType == a.BCType && o.BCNo == a.BCNo && o.BonNo == a.BonNo);
                    //TraceableINViewModel dup = Array.Find(b, o => o.Invoice == a.Invoice && o.PO == a.PO && o.PEB == a.PEB && o.BCType == a.BCType);
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
            
            return report.AsQueryable();
        }
        
        public MemoryStream GetTraceableInExcel(string filter, string tipe,DateTime? Datefrom, DateTime? DateTo)
        {
            var Query = getQueryTracable2(filter, tipe, Datefrom, DateTo);
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
            result.Columns.Add(new DataColumn() { ColumnName = "ROJob", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No BUK", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Keluar", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sisa", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Produksi Qty", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "BJ Qty", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BC Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl BC Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Ekspor Qty", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sample Qty", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", 0, "","", "", 0, "",0, 0, 0, "", "", "", 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                var docNo = Query.ToArray();
                var q = Query.ToList();
                var index = 0;
                foreach (TraceableINViewModel a in q)
                {
                    TraceableINViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo && o.BonNo == o.BonNo);
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
                //var index = 0;
                foreach (var item in Query)
                {
                    string bcdate = String.IsNullOrWhiteSpace(item.BCDate) ? "-" : Convert.ToDateTime(item.BCDate).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string pebdate = String.IsNullOrWhiteSpace(item.PEBDate) ? "-" : Convert.ToDateTime(item.PEBDate).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(item.count, item.BCType, item.BCNo, bcdate, item.BonNo, item.PO, item.ItemCode, item.ItemName, item.ReceiptQty, item.SatuanReceipt, item.ROJob, item.BUK, item.QtyBUK, item.SatuanBUK,item.Sisa, item.ProduksiQty, item.BJQty, item.Invoice, item.PEB, pebdate, item.EksporQty, item.SampleQty);

                }
            }
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
            ExcelPackage package = new ExcelPackage();
            bool styling = true;

            foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
            {
                var sheet = package.Workbook.Worksheets.Add(item.Value);
                sheet.Cells["A1"].LoadFromDataTable(item.Key, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);
                //sheet.Cells["C1:D1"].Merge = true;
                //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //sheet.Cells["E1:F1"].Merge = true;
                //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                Dictionary<string, int> counts = new Dictionary<string, int>();
                Dictionary<string, int> countsType = new Dictionary<string, int>();
                Dictionary<string, int> countsPO = new Dictionary<string, int>();
                Dictionary<string, int> countsPEB = new Dictionary<string, int>();
                Dictionary<string, int> countsekspor = new Dictionary<string, int>();
                Dictionary<string, int> countsRO = new Dictionary<string, int>();
                Dictionary<string, int> countsBUK = new Dictionary<string, int>();
                var docNo = Query.ToArray();
                int value;
                foreach (var a in Query)
                {
                    //FactBeacukaiViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo);
                    if (counts.TryGetValue(a.BCType + a.BCNo + a.BonNo, out value))
                    {
                        counts[a.BCType + a.BCNo + a.BonNo]++;
                    }
                    else
                    {
                        counts[a.BCType + a.BCNo + a.BonNo] = 1;
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

                    if (countsPO.TryGetValue(a.BonNo + a.PO + a.ItemCode + a.ItemName + a.SatuanReceipt, out value))
                    {
                        countsPO[a.BonNo + a.PO + a.ItemCode + a.ItemName + a.SatuanReceipt]++;
                    }
                    else
                    {
                        countsPO[a.BonNo + a.PO + a.ItemCode + a.ItemName + a.SatuanReceipt] = 1;
                    }
                    if (countsPEB.TryGetValue(a.PEB + a.Invoice, out value))
                    {
                        countsPEB[a.PEB + a.Invoice]++;
                    }
                    else
                    {
                        countsPEB[a.PEB + a.Invoice] = 1;
                    }
                    if (countsekspor.TryGetValue(a.Invoice + a.EksporQty + a.BJQty + a.ProduksiQty + a.PO, out value))
                    {
                        countsekspor[a.Invoice + a.EksporQty + a.BJQty + a.ProduksiQty + a.PO]++;
                    }
                    else
                    {
                        countsekspor[a.Invoice + a.EksporQty + a.BJQty + a.ProduksiQty + a.PO] = 1;
                    }
                    if (countsRO.TryGetValue(a.ROJob + a.Invoice + a.PO, out value))
                    {
                        countsRO[a.ROJob + a.Invoice + a.PO]++;
                    }
                    else
                    {
                        countsRO[a.ROJob + a.Invoice + a.PO] = 1;
                    }
                    if(countsBUK.TryGetValue(a.PO + a.ItemCode + a.ItemName + a.SatuanReceipt + a.QtyBUK + a.BUK + a.BonNo + a.Sisa, out value))
                    {
                        countsBUK[a.PO + a.ItemCode + a.ItemName + a.SatuanReceipt + a.QtyBUK + a.BUK + a.BonNo + a.Sisa]++;
                    }
                    else
                    {
                        countsBUK[a.PO + a.ItemCode + a.ItemName + a.SatuanReceipt + a.QtyBUK + a.BUK + a.BonNo + a.Sisa] = 1;
                    }
                }

                int index = 2;
                foreach (KeyValuePair<string, int> b in counts)
                {
                    sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["E" + index + ":E" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["E" + index + ":E" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["V" + index + ":V" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["V" + index + ":V" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += b.Value;
                }

                index = 2;
                foreach (KeyValuePair<string, int> c in countsType)
                {
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in countsPO)
                {
                    sheet.Cells["F" + index + ":F" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["F" + index + ":F" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["G" + index + ":G" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["G" + index + ":G" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["H" + index + ":H" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["H" + index + ":H" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["I" + index + ":I" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["I" + index + ":I" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["J" + index + ":J" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["J" + index + ":J" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in countsRO)
                {
                    sheet.Cells["K" + index + ":K" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["K" + index + ":K" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in countsPEB)
                {
                    sheet.Cells["R" + index + ":R" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["R" + index + ":R" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["S" + index + ":S" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["S" + index + ":S" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["T" + index + ":T" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["T" + index + ":T" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in countsekspor)
                {
                    sheet.Cells["P" + index + ":P" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["P" + index + ":P" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["Q" + index + ":Q" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["Q" + index + ":Q" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["U" + index + ":U" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["U" + index + ":U" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in countsBUK)
                {
                    sheet.Cells["L" + index + ":L" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["L" + index + ":L" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["N" + index + ":N" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["N" + index + ":N" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
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
