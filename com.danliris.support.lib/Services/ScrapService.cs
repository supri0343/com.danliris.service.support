using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.Models;
using com.danliris.support.lib.ViewModel;
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
    public class ScrapService
    {
		SupportDbContext context;
		public ScrapService(SupportDbContext _context)
		{
			this.context = _context;
		}


		public IQueryable<ScrapViewModel> GetScrapReport(DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime Datefrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime Dateto = dateTo == null ? new DateTime(1970, 1, 1) : (DateTime)dateTo;
			string DateFrom = Datefrom.ToString("yyyy-MM-dd");
			string DateTo = Dateto.ToString("yyyy-MM-dd");
			List<ScrapViewModel> scrapData = new List<ScrapViewModel>();
			try
			{
				string connectionString = APIEndpoint.ConnectionString;
				using (SqlConnection conn =
					new SqlConnection(connectionString))
				{
					conn.Open();
					using (SqlCommand cmd = new SqlCommand("select HASIL.*,convert(float, (HASIL.SaldoAwal + (HASIL.Pemasukan - HASIL.Pengeluaran))) as SaldoBuku from (select DATA.ClassificationID, DATA.ClassificationCode, DATA.ClassificationName, DATA.StockId, DATA.DestinationId, DATA.UnitQtyName,convert(float,((DATA.saldoIn + DATA.koreksisaldoIn) - (DATA.saldoOut + DATA.koreksisaldoOut))) as SaldoAwal,convert(float,(DATA.transaksiIn + DATA.koreksitransaksiIn)) as Pemasukan,convert(float,(DATA.transaksiOut + DATA.koreksitransaksiOut)) as Pengeluaran,convert(float,0) as Penyesuaian,convert(float,0) as StockOpname,convert(float,0) as Selisih from (select SC.ClassificationID, SC.ClassificationCode, SC.ClassificationName, SS.StockId, SS.DestinationId, UQ.UnitQtyName,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'IN' and a.TransactionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId) as saldoIn,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'OUT' and a.TransactionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID) as saldoOut,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'PLUS') - (select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'MINUS') as koreksisaldoIn,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and c.DetailType = 'PLUS')-(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and c.DetailType = 'MINUS') as koreksisaldoOut,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'IN' and a.TransactionDate >= '"+DateFrom+"' and a.TransactionDate < dateadd(day, 1, '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId) as transaksiIn,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'OUT' and a.TransactionDate >= '"+DateFrom+"' and a.TransactionDate < dateadd(day, 1,  '"+DateTo+"') and b.ClassificationID = SC.ClassificationID) as transaksiOut,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate >= '"+DateFrom+"' and d.CorrectionDate < dateadd(day, 1,'"+DateTo+"') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'PLUS')-(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate >= '"+DateFrom+"' and d.CorrectionDate < dateadd(day, 1,  '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'MINUS') as koreksitransaksiIn,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate >= '"+DateFrom+"' and d.CorrectionDate < dateadd(day, 1, '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and c.DetailType = 'PLUS')-(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate >= '"+DateFrom+"' and d.CorrectionDate < dateadd(day, 1,  '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and c.DetailType = 'MINUS') as koreksitransaksiOut from ScrapClassification SC inner join ScrapStock SS on SC.ClassificationID = SS.ClassificationId inner join UNITQUANTITY UQ on SS.UnitQty = UQ.UnitQtyCode where SC.ClassificationID = 'SC140001')DATA)HASIL", conn))
					{
						SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
						DataSet dSet = new DataSet();
						dataAdapter.Fill(dSet);
						foreach (DataRow data in dSet.Tables[0].Rows)
						{
							ScrapViewModel view = new ScrapViewModel {
								ClassificationId = data["ClassificationId"].ToString(),
								ClassificationCode = data["ClassificationCode"].ToString(),
								ClassificationName = data["ClassificationName"].ToString(),
								DestinationId = data["DestinationId"].ToString(),
								Pemasukan = String.Format("{0:n}", (double)data["Pemasukan"]),
								Pengeluaran= String.Format("{0:n}", (double)data["Pengeluaran"]),
								SaldoAwal= String.Format("{0:n}", (double)data["SaldoAwal"]),
								SaldoBuku= String.Format("{0:n}", (double)data["SaldoBuku"]),
								UnitQtyName=data["UnitQtyName"].ToString(),
								Penyesuaian= String.Format("{0:n}", (double)data["Penyesuaian"]),
								StockOpname= String.Format("{0:n}", (double)data["StockOpname"]),
								Selisih= String.Format("{0:n}", (double)data["Selisih"]),
								StockId= data["StockId"].ToString()

							};
							scrapData.Add(view);
						}
					}
					conn.Close();
				}
			}
			catch (SqlException ex)
			{
				//Log exception
				//Display Error message
			}

			return scrapData.AsQueryable();
		}

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetScrapReport(dateFrom, dateTo, offset);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sat", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pemasukan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penyesuaian", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Buku", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Stock Opname", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Selisih", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
                foreach (var item in Query)
                {
                    result.Rows.Add((item.ClassificationCode), item.ClassificationName, item.UnitQtyName, item.SaldoAwal, item.Pemasukan, item.Pengeluaran, item.Penyesuaian, item.SaldoBuku, item.StockOpname, item.Selisih);
                }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
    }
}
