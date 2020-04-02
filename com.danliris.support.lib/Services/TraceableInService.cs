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
            try
            {
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
                            "fcf.ItemName, DetailExpendId,POId, b.Invoice, b.ExpenditureType, a.PEB, a.TglPEB) as data where " + tipe + " = '" + filter + "' AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' and ExpenditureType != 'E002' order by PEB, Invoice, BCNo, BonNo, BCType, NoPO", conn)) { 
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
                    conn.Close();
                }
                
                var b = reportData.ToArray();
                var index = 0;
                foreach (TraceableINViewModel a in reportData)
                {
                    //TraceableINViewModel dup = Array.Find(b, o => o.BCType == a.BCType && o.BCNo == a.BCNo && o.BonNo == a.BonNo && o.PO == a.PO && o.ItemCode == a.ItemCode && a.ItemName == a.ItemName && o.ReceiptQty == a.ReceiptQty && o.SatuanReceipt == a.SatuanReceipt && o.ROJob == a.ROJob && o.ProduksiQty == a.ProduksiQty && o.BJQty == a.BJQty && o.Invoice == a.Invoice && o.PEB == a.PEB && o.PEBDate == a.PEBDate && o.EksporQty == a.EksporQty && o.SampleQty == a.SampleQty);
                    TraceableINViewModel dup = Array.Find(b, o => o.Invoice == a.Invoice && o.PO == a.PO && o.PEB == a.PEB && o.BCType == a.BCType);
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

                
            }
            catch (Exception e)
            {

            }
            return reportData.AsQueryable();
        }
        //public IQueryable<TraceableINViewModel> getQueryTracable(string filter, string tipe, DateTime? Datefrom, DateTime? DateTo)
        //{
        //    DateTime dateFrom = Datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)Datefrom;
        //    DateTime dateTo = DateTo == null ? DateTime.Now : (DateTime)DateTo;
        //    string datefrom = dateFrom.ToString("yyyy-MM-dd");
        //    string dateto = dateTo.ToString("yyyy-MM-dd");
        //    List<TraceableINViewModel> reportData = new List<TraceableINViewModel>();

        //    try
        //    {
        //        if (tipe == "BCDate") {

        //            //string cmdtracable = "select * from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = fcf.DetailShippingOrderId),0) As QtyReceipt,(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk,(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar, ROJob,DetailExpendId, (select top(1) Invoice from ExpenditureGood e where e.RO= ROJob) as Invoice, (select top(1) PEB from ExpenditureGood e join Shipping.dbo.OmzetKonf o on o.Invoice_No=e.Invoice where e.RO= fcf.ROJob) PEB, (select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO, (select top 1 ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId) BUK, isnull((select top 1 SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK, (select top(1) TglPEB from ExpenditureGood e join Shipping.dbo.OmzetKonf o on o.Invoice_No=e.Invoice where e.RO= fcf.ROJob) TAnggalPEB, isnull((select Isnull(SUM(a.Qty),0) from ExpenditureGoodDetail a join ExpenditureGood b on a.ExpenditureGoodId = b.ExpenditureGoodId where b.RO = fcf.ROJob and ExpenditureType = 'E001'),0) EksporQty, isnull((select Isnull(SUM(a.Qty),0) from ExpenditureGoodDetail a join ExpenditureGood b on a.ExpenditureGoodId = b.ExpenditureGoodId where b.RO = fcf.ROJob and ExpenditureType = 'E003'),0) SampleQty, isnull((SELECT Sum(Qty) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI'),0) FinisihingOutQty, isnull((select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN'),0) subconOut, isnull((SELECT SUM(A.QtyCutting) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob),0) ProduksiQty  from FactControlFlow fcf group by  BCNo,BCType, BCDate,BonNo,ROJob ,DetailReceiptId,DetailShippingOrderId,fcf.ItemCode,fcf.ItemName, DetailExpendId,POId) as data where  bcdate > @datefrom And bcdate <= @dateto order by BCType,BCNo,BonNo,ItemCode,ItemName,QtyReceipt,SatMasuk,ROJob,ProduksiQty,FinisihingOutQty,Invoice,PEB,TAnggalPEB,EksporQty,SampleQty,NoPO";
        //            //string cmdtracable = "select * from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = fcf.DetailShippingOrderId),0) As QtyReceipt,(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk,(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar, ROJob,DetailExpendId, (select top(1) Invoice from ExpenditureGood e where e.RO= ROJob) as Invoice, (select top(1) PEB from ExpenditureGood e join Shipping.dbo.OmzetKonf o on o.Invoice_No=e.Invoice where e.RO= fcf.ROJob) PEB, (select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO, (select top 1 ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId) BUK, isnull((select top 1 SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK, (select top(1) TglPEB from ExpenditureGood e join Shipping.dbo.OmzetKonf o on o.Invoice_No=e.Invoice where e.RO= fcf.ROJob) TAnggalPEB, isnull((select Isnull(SUM(a.Qty),0) from ExpenditureGoodDetail a join ExpenditureGood b on a.ExpenditureGoodId = b.ExpenditureGoodId where b.RO = fcf.ROJob and ExpenditureType = 'E001'),0) EksporQty, isnull((select Isnull(SUM(a.Qty),0) from ExpenditureGoodDetail a join ExpenditureGood b on a.ExpenditureGoodId = b.ExpenditureGoodId where b.RO = fcf.ROJob and ExpenditureType = 'E003'),0) SampleQty, isnull((SELECT Sum(Qty) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI'),0) FinisihingOutQty, isnull((select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN'),0) subconOut, isnull((SELECT SUM(A.QtyCutting) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob),0) ProduksiQty  from FactControlFlow fcf group by  BCNo,BCType, BCDate,BonNo,ROJob ,DetailReceiptId,DetailShippingOrderId,fcf.ItemCode,fcf.ItemName, DetailExpendId,POId) as data where  bcdate > @datefrom And bcdate <= @dateto order by BCType,BCNo,BonNo,NoPO";
        //            string cmdtracable = "select * from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName,b.Invoice, Sum(c.Qty) As EksporQty, b.ExpenditureType, isnull(a.PEB,'-') as PEB, a.TglPEB as TAnggalPEB, " +
        //                                  "isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = fcf.DetailShippingOrderId),0) As QtyReceipt," +
        //                                  "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk," +
        //                                  "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar, " +
        //                                  "ROJob,DetailExpendId, " +
        //                                  "(select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO, " +
        //                                  "(select top 1 ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId) BUK, " +
        //                                  "isnull((select top 1 SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK, " +
        //                                  "isnull((SELECT Sum(Qty) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI'),0) FinisihingOutQty, " +
        //                                  "isnull((select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN'),0) subconOut, " +
        //                                  "isnull((SELECT SUM(A.QtyCutting) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob),0) ProduksiQty  " +
        //                                  "from FactControlFlow fcf join ExpenditureGood b on fcf.ROJob = b.RO join ExpenditureGoodDetail c on c.ExpenditureGoodId = b.ExpenditureGoodId left join Shipping.dbo.OmzetKonf a on b.Invoice = a.Invoice_No" +
        //                                  "group by  BCNo,BCType, BCDate,BonNo,ROJob ,DetailReceiptId,DetailShippingOrderId,fcf.ItemCode,fcf.ItemName, DetailExpendId,POId,b.Invoice, b.ExpenditureType, a.PEB, a.TglPEB) as data " +
        //                                  "where  bcdate > @datefrom And bcdate <= @dateto order by PEB, Invoice, BCNo, BonNo, BCType, NoPO";
        //            List<SqlParameter> parameters = new List<SqlParameter>();
        //            //parameters.Add(new SqlParameter("filter", tipe));
        //            parameters.Add(new SqlParameter("dateto", dateto));
        //            parameters.Add(new SqlParameter("datefrom", datefrom));

        //            var data = dBContext.ExecuteReader(cmdtracable,parameters);

        //            while (data.Read())
        //            {
        //                reportData.Add(new TraceableINViewModel
        //                {
        //                    BCType = data["BCType"].ToString(),
        //                    BCNo = data["BCNo"].ToString(),
        //                    BCDate = data["BCDate"].ToString(),
        //                    BonNo = data["BonNo"].ToString(),
        //                    PO = data["NoPO"].ToString(),
        //                    ItemCode = data["ItemCode"].ToString(),
        //                    ItemName = data["ItemName"].ToString(),
        //                    ReceiptQty = (double)data["QtyReceipt"],
        //                    SatuanReceipt = data["SatMasuk"].ToString(),
        //                    BUK = data["BUK"].ToString(),
        //                    QtyBUK = (double)data["QtyBUK"],
        //                    SatuanBUK = data["SatKeluar"].ToString(),
        //                    ROJob = data["ROJob"].ToString(),
        //                    ProduksiQty = (double)data["ProduksiQty"],
        //                    BJQty = (double)data["subconOut"] + (double)data["FinisihingOutQty"],
        //                    Invoice = data["Invoice"].ToString(),
        //                    PEB = data["PEB"].ToString(),
        //                    PEBDate = data["TAnggalPEB"].ToString() == null ? "-" : data["TAnggalPEB"].ToString(),
        //                    EksporQty = data["ExpenditureType"].ToString() == "E001" ? (double)data["EksporQty"] : 0,
        //                    SampleQty = data["ExpenditureType"].ToString() == "E003" ? (double)data["SampleQty"] : 0,
        //                    SubkonOutQty = (double)data["subconOut"],
        //                    Sisa = (double)data["QtyReceipt"] - (double)data["QtyBUK"],
        //                    ExType = data["ExpenditureType"].ToString()

        //                });
        //            }

                    
        //        }
        //        else
        //        {
        //            //string cmdtracable = "select * from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, isnull((select Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = fcf.DetailShippingOrderId),0) As QtyReceipt,(select b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk,(select b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar, ROJob,DetailExpendId, (select top(1) Invoice from ExpenditureGood e where e.RO= ROJob) as Invoice, (select top(1) PEB from ExpenditureGood e join Shipping.dbo.OmzetKonf o on o.Invoice_No=e.Invoice where e.RO= fcf.ROJob) PEB, (select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO, (select top 1 ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId) BUK, isnull((select top 1 SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK, (select top(1) TglPEB from ExpenditureGood e join Shipping.dbo.OmzetKonf o on o.Invoice_No=e.Invoice where e.RO= fcf.ROJob) TAnggalPEB, isnull((select Isnull(SUM(a.Qty),0) from ExpenditureGoodDetail a join ExpenditureGood b on a.ExpenditureGoodId = b.ExpenditureGoodId where b.RO = fcf.ROJob and ExpenditureType = 'E001'),0) EksporQty, isnull((select Isnull(SUM(a.Qty),0) from ExpenditureGoodDetail a join ExpenditureGood b on a.ExpenditureGoodId = b.ExpenditureGoodId where b.RO = fcf.ROJob and ExpenditureType = 'E003'),0) SampleQty, isnull((SELECT Sum(Qty) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI'),0) FinisihingOutQty, isnull((select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN'),0) subconOut, isnull((SELECT SUM(A.QtyCutting) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob),0) ProduksiQty  from FactControlFlow fcf group by  BCNo,BCType, BCDate,BonNo,ROJob ,DetailReceiptId,DetailShippingOrderId,fcf.ItemCode,fcf.ItemName, DetailExpendId,POId) as data where " + tipe + " = @key AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' order by BCType,BCNo,BonNo,ItemCode,ItemName,QtyReceipt,SatMasuk,ROJob,ProduksiQty,FinisihingOutQty,Invoice,PEB,TAnggalPEB,EksporQty,SampleQty,NoPO";
        //            //string cmdtracable = "select * from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, isnull((select Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = fcf.DetailShippingOrderId),0) As QtyReceipt,(select b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk,(select b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar, ROJob,DetailExpendId, (select top(1) Invoice from ExpenditureGood e where e.RO= ROJob) as Invoice, (select top(1) PEB from ExpenditureGood e join Shipping.dbo.OmzetKonf o on o.Invoice_No=e.Invoice where e.RO= fcf.ROJob) PEB, (select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO, (select top 1 ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId) BUK, isnull((select top 1 SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK, (select top(1) TglPEB from ExpenditureGood e join Shipping.dbo.OmzetKonf o on o.Invoice_No=e.Invoice where e.RO= fcf.ROJob) TAnggalPEB, isnull((select Isnull(SUM(a.Qty),0) from ExpenditureGoodDetail a join ExpenditureGood b on a.ExpenditureGoodId = b.ExpenditureGoodId where b.RO = fcf.ROJob and ExpenditureType = 'E001'),0) EksporQty, isnull((select Isnull(SUM(a.Qty),0) from ExpenditureGoodDetail a join ExpenditureGood b on a.ExpenditureGoodId = b.ExpenditureGoodId where b.RO = fcf.ROJob and ExpenditureType = 'E003'),0) SampleQty, isnull((SELECT Sum(Qty) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI'),0) FinisihingOutQty, isnull((select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN'),0) subconOut, isnull((SELECT SUM(A.QtyCutting) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob),0) ProduksiQty  from FactControlFlow fcf group by  BCNo,BCType, BCDate,BonNo,ROJob ,DetailReceiptId,DetailShippingOrderId,fcf.ItemCode,fcf.ItemName, DetailExpendId,POId) as data where " + tipe + " = @key AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' order by BCType,BCNo,BonNo,NoPO,ROJob";
        //            //string cmdtracable = "select * from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName,b.Invoice, Sum(c.Qty) As EksporQty, b.ExpenditureType, isnull(a.PEB,'-') as PEB, a.TglPEB as TAnggalPEB, " +
        //            //                      "isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = fcf.DetailShippingOrderId),0) As QtyReceipt," +
        //            //                      "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk," +
        //            //                      "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar, " +
        //            //                      "ROJob,DetailExpendId, " +
        //            //                      "(select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO, " +
        //            //                      "(select top 1 ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId) BUK, " +
        //            //                      "isnull((select top 1 SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK, " +
        //            //                      "isnull((SELECT Sum(Qty) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI'),0) FinisihingOutQty, " +
        //            //                      "isnull((select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN'),0) subconOut, " +
        //            //                      "isnull((SELECT SUM(A.QtyCutting) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob),0) ProduksiQty  " +
        //            //                      "from FactControlFlow fcf join ExpenditureGood b on fcf.ROJob = b.RO join ExpenditureGoodDetail c on c.ExpenditureGoodId = b.ExpenditureGoodId left join Shipping.dbo.OmzetKonf a on (b.Invoice = a.Invoice_No and a.RO = ROJob ) " +
        //            //                      "group by  BCNo,BCType, BCDate,BonNo,ROJob ,DetailReceiptId,DetailShippingOrderId,fcf.ItemCode,fcf.ItemName, DetailExpendId,POId,b.Invoice,  b.ExpenditureType, a.PEB, a.TglPEB) as data where " + tipe + " = @key AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' and ExpenditureType != 'E002'" +
        //            //                      "order by PEB, Invoice, BCNo, BonNo, BCType, NoPO";
        //            string cmdtracable = "select ROW_NUMBER() Over(Order By NoPO, BUK, QtyBUK, QtyReceipt) row_num, NoPO, QtyReceipt, BUK,ItemCode,ItemName, QtyBUK into #PO from(select distinct " +
        //                                 "BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, '' as Invoice,'' As EksporQty, b.ExpenditureType, d.ExpenditureNo as BUK, isnull(d.SmallestQuantity, 0) as QtyBUK,isnull(e.Quantity * e.ConvertValue, 0) as QtyReceipt, "+
					   //                  "isnull(a.PEB, '-') as PEB, "+ 
					   //                  "isnull(a.TglPEB, '1970-01-01') as TAnggalPEB, "+
				    //                     "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk, "+
				    //                     "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar, "+
				    //                     "ROJob,DetailExpendId, "+
				    //                     "f.PlanPO AS NoPO, "+
				    //                     "(SELECT isnull(Sum(Qty), 0) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI') FinisihingOutQty, "+
				    //                     "(select isnull(sum(a.Qty), 0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN') subconOut, "+
				    //                     "(SELECT isnull(SUM(A.QtyCutting), 0) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob) ProduksiQty "+
        //                                 "from FactControlFlow fcf "+
        //                                 "join ExpenditureGood b on fcf.ROJob = b.RO "+
        //                                 "join ExpenditureGoodDetail c on c.ExpenditureGoodId = b.ExpenditureGoodId "+
        //                                 "left join Shipping.dbo.OmzetKonf a on(b.Invoice = a.Invoice_No) "+
        //                                 "join DL_Supports.dbo.PURCHASE_ORDER f on fcf.POId = f.POId "+
        //                                 "left join DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS d on fcf.DetailExpendId = d.DetailExpenditureGoodsId "+
        //                                 "left join DL_Inventories.dbo.DETAIL_RECEIPT_GOODS e on fcf.DetailReceiptId = e.DetailReceiptGoodsId "+
        //                                 "group by BCNo,BCType, BCDate,BonNo, "+
				    //                     "ROJob ,DetailReceiptId, fcf.DetailShippingOrderId, "+
				    //                     "fcf.ItemCode, "+
        //                                 "fcf.ItemName, DetailExpendId,fcf.POId, b.Invoice, b.ExpenditureType, a.PEB, a.TglPEB, f.PlanPO, d.ExpenditureNo, e.Quantity, e.ConvertValue, d.SmallestQuantity) as data where " + tipe + " = @key AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' order by NoPO, BUK, QtyBUK,data.QtyReceipt, ItemCode, ItemName " +
        //                                 "select ROW_NUMBER() Over(Order by Invoice, PEB, EksporQty, BCNo, BonNo, BCType, ProduksiQty, FinisihingOutQty) row_num, * into #dataPEBInvoice from(select distinct " +
        //                                 "BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, b.Invoice,Sum(c.Qty) As EksporQty, b.ExpenditureType, "+
					   //                  "isnull(a.PEB, '-') as PEB, "+
					   //                  "isnull(a.TglPEB, '1970-01-01') as TAnggalPEB, "+				 
				    //                      "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailReceiptGoodsId = fcf.DetailReceiptId) as SatMasuk, "+
				    //                      "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar, "+
				    //                      "ROJob,DetailExpendId, "+				  
				    //                      "(SELECT isnull(Sum(Qty), 0) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI') FinisihingOutQty, "+
				    //                      "(select isnull(sum(a.Qty), 0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN') subconOut, "+
				    //                      "(SELECT isnull(SUM(A.QtyCutting), 0) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob) ProduksiQty "+
        //                                   "from FactControlFlow fcf "+
        //                                  "join ExpenditureGood b on fcf.ROJob = b.RO "+
        //                                  "join ExpenditureGoodDetail c on c.ExpenditureGoodId = b.ExpenditureGoodId "+
        //                                 "left join Shipping.dbo.OmzetKonf a on(b.Invoice = a.Invoice_No and a.RO = ROJob) "+
        //                                  "join DL_Supports.dbo.PURCHASE_ORDER f on fcf.POId = f.POId "+
        //                                 "left join DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS d on fcf.DetailExpendId = d.DetailExpenditureGoodsId "+
        //                                  "left join DL_Inventories.dbo.DETAIL_RECEIPT_GOODS e on fcf.DetailReceiptId = e.DetailReceiptGoodsId "+
        //                                  "group by BCNo,BCType, BCDate,BonNo, "+				  
				    //                      "ROJob ,DetailReceiptId, "+				  
				    //                      "fcf.ItemCode, "+
        //                                  "fcf.ItemName, DetailExpendId,fcf.POId, b.Invoice, b.ExpenditureType, a.PEB, a.TglPEB, f.PlanPO) as data where " + tipe + " = @key AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' order by Invoice, EksporQty, PEB, BCNo, BonNo, BCType, ProduksiQty, FinisihingOutQty "+
        //                                  "select * from #dataPEBInvoice a "+
        //                                  "join #PO b on a.row_num = b.row_num "+
        //                                  "where ExpenditureType != 'E002' "+
        //                                  "Drop table #dataPEBInvoice "+
        //                                  "Drop table #PO ";
        //            List<SqlParameter> parameters = new List<SqlParameter>();
        //            //parameters.Add(new SqlParameter("filter", tipe));
        //            parameters.Add(new SqlParameter("key", filter));

        //            var data = dBContext.ExecuteReader(cmdtracable, parameters);

        //            while (data.Read())
        //            {
        //                reportData.Add(new TraceableINViewModel
        //                {
        //                    BCType = data["BCType"].ToString(),
        //                    BCNo = data["BCNo"].ToString(),
        //                    BCDate = data["BCDate"].ToString(),
        //                    BonNo = data["BonNo"].ToString(),
        //                    PO = data["NoPO"].ToString(),
        //                    ItemCode = data["ItemCode"].ToString(),
        //                    ItemName = data["ItemName"].ToString(),
        //                    ReceiptQty = (double)data["QtyReceipt"],
        //                    SatuanReceipt = data["SatMasuk"].ToString(),
        //                    BUK = data["BUK"].ToString(),
        //                    QtyBUK = (double)data["QtyBUK"],
        //                    SatuanBUK = data["SatKeluar"].ToString(),
        //                    ROJob = data["ROJob"].ToString(),
        //                    ProduksiQty = (double)data["ProduksiQty"],
        //                    BJQty = (double)data["subconOut"] + (double)data["FinisihingOutQty"],
        //                    Invoice = data["Invoice"].ToString(),
        //                    PEB = data["PEB"].ToString(),
        //                    PEBDate = data["TAnggalPEB"].ToString() == null ? "-" : data["TAnggalPEB"].ToString(),
        //                    EksporQty = data["ExpenditureType"].ToString() == "E001" ? (double)data["EksporQty"] : 0,
        //                    SampleQty = data["ExpenditureType"].ToString() == "E003" ? (double)data["SampleQty"] : 0,
        //                    SubkonOutQty = (double)data["subconOut"],
        //                    Sisa = (double)data["QtyReceipt"] - (double)data["QtyBUK"],
        //                    ExType = data["row_num"].ToString(),
                           
        //                });
        //            }

        //        }
        //        var report = from a in reportData
        //                     group a by new { a.Invoice, a.PO, a.BUK } into data
        //                     select new TraceableINViewModel
        //                     {
        //                         BCType = data.FirstOrDefault().BCNo,
        //                         BCNo = data.FirstOrDefault().BCNo,
        //                         BCDate = data.FirstOrDefault().BCDate,
        //                         BonNo = data.FirstOrDefault().BonNo,
        //                         PO = data.FirstOrDefault().PO,
        //                         ItemCode = data.FirstOrDefault().ItemCode,
        //                         ItemName = data.FirstOrDefault().ItemName,
        //                         ReceiptQty = data.FirstOrDefault().ReceiptQty,
        //                         SatuanReceipt = data.FirstOrDefault().SatuanReceipt,
        //                         BUK = data.FirstOrDefault().BUK,
        //                         QtyBUK = data.FirstOrDefault().QtyBUK,
        //                         SatuanBUK = data.FirstOrDefault().SatuanBUK,
        //                         ROJob = data.FirstOrDefault().ROJob,
        //                         ProduksiQty = data.FirstOrDefault().ProduksiQty,
        //                         BJQty = data.FirstOrDefault().BJQty,
        //                         Invoice = data.FirstOrDefault().Invoice,
        //                         PEB = data.FirstOrDefault().PEB,
        //                         PEBDate = data.FirstOrDefault().PEBDate,
        //                         EksporQty = data.FirstOrDefault().EksporQty,
        //                         SampleQty = data.FirstOrDefault().SampleQty,
        //                         SubkonOutQty = data.FirstOrDefault().SampleQty,
        //                         Sisa = data.FirstOrDefault().Sisa,
        //                         ExType = data.FirstOrDefault().ExType
        //                     };
        //        var b = reportData.ToArray();
        //        var index = 0;
        //        foreach (TraceableINViewModel a in reportData)
        //        {
        //            //TraceableINViewModel dup = Array.Find(b, o => o.BCType == a.BCType && o.BCNo == a.BCNo && o.BonNo == a.BonNo && o.PO == a.PO && o.ItemCode == a.ItemCode && a.ItemName == a.ItemName && o.ReceiptQty == a.ReceiptQty && o.SatuanReceipt == a.SatuanReceipt && o.ROJob == a.ROJob && o.ProduksiQty == a.ProduksiQty && o.BJQty == a.BJQty && o.Invoice == a.Invoice && o.PEB == a.PEB && o.PEBDate == a.PEBDate && o.EksporQty == a.EksporQty && o.SampleQty == a.SampleQty);
        //            TraceableINViewModel dup = Array.Find(b, o=>o.Invoice == a.Invoice && o.PO == a.PO && o.PEB == a.PEB && o.BCType == a.BCType);
        //            if (dup != null)
        //            {
        //                if (dup.count == 0)
        //                {
        //                    index++;
        //                    dup.count = index;
        //                }
        //            }
        //            a.count = dup.count;
        //        }


        //    }
        //        catch (Exception e)
        //    {

        //    }

            
        //    return reportData.AsQueryable();
        //}
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

                    if(countsPO.TryGetValue(a.BonNo + a.PO + a.ItemCode + a.ItemName + a.SatuanReceipt, out value))
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
                    if (countsekspor.TryGetValue(a.Invoice + a.EksporQty + a.BJQty + a.ProduksiQty, out value))
                    {
                        countsekspor[a.Invoice + a.EksporQty + a.BJQty + a.ProduksiQty]++;
                    }
                    else
                    {
                        countsekspor[a.Invoice + a.EksporQty + a.BJQty + a.ProduksiQty] = 1;
                    }
                    if (countsRO.TryGetValue(a.ROJob, out value))
                    {
                        countsRO[a.ROJob]++;
                    }
                    else
                    {
                        countsRO[a.ROJob] = 1;
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
