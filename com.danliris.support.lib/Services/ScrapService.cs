using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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


		public IQueryable<ViewScrap> GetScrapReport(DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime DateTo = dateTo == null ? new DateTime(1970, 1, 1) : (DateTime)dateTo;
			List<ViewScrap> scrapData = new List<ViewScrap>();
			try
			{
				string connectionString = APIEndpoint.ConnectionString;
				using (SqlConnection conn =
					new SqlConnection(connectionString))
				{
					conn.Open();
					using (SqlCommand cmd = new SqlCommand("select HASIL.*,convert(float, (HASIL.SaldoAwal + (HASIL.Pemasukan - HASIL.Pengeluaran))) as SaldoBuku from (select DATA.ClassificationID, DATA.ClassificationCode, DATA.ClassificationName, DATA.StockId, DATA.DestinationId, DATA.UnitQtyName,convert(float,((DATA.saldoIn + DATA.koreksisaldoIn) - (DATA.saldoOut + DATA.koreksisaldoOut))) as SaldoAwal,convert(float,(DATA.transaksiIn + DATA.koreksitransaksiIn)) as Pemasukan,convert(float,(DATA.transaksiOut + DATA.koreksitransaksiOut)) as Pengeluaran,convert(float,0) as Penyesuaian,convert(float,0) as StockOpname,convert(float,0) as Selisih from (select SC.ClassificationID, SC.ClassificationCode, SC.ClassificationName, SS.StockId, SS.DestinationId, UQ.UnitQtyName,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'IN' and a.TransactionDate < '" + DateFrom+"' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId) as saldoIn,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'OUT' and a.TransactionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID) as saldoOut,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'PLUS') - (select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'MINUS') as koreksisaldoIn,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and c.DetailType = 'PLUS')-(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and c.DetailType = 'MINUS') as koreksisaldoOut,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'IN' and a.TransactionDate >= '"+DateFrom+"' and a.TransactionDate < dateadd(day, 1, '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId) as transaksiIn,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'OUT' and a.TransactionDate >= '"+DateFrom+"' and a.TransactionDate < dateadd(day, 1,  '"+DateTo+"') and b.ClassificationID = SC.ClassificationID) as transaksiOut,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate >= '"+ DateFrom+"' and d.CorrectionDate < dateadd(day, 1,'"+DateTo+"') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'PLUS')-(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate >= '"+DateFrom+"' and d.CorrectionDate < dateadd(day, 1,  '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'MINUS') as koreksitransaksiIn,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate >= '"+DateFrom+"' and d.CorrectionDate < dateadd(day, 1, '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and c.DetailType = 'PLUS')-(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate >= '"+DateFrom+"' and d.CorrectionDate < dateadd(day, 1,  '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and c.DetailType = 'MINUS') as koreksitransaksiOut from ScrapClassification SC inner join ScrapStock SS on SC.ClassificationID = SS.ClassificationId inner join UNITQUANTITY UQ on SS.UnitQty = UQ.UnitQtyCode where SC.ClassificationID = 'SC140001')DATA)HASIL", conn))
					{
						SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
						DataSet dSet = new DataSet();
						dataAdapter.Fill(dSet);
						foreach (DataRow data in dSet.Tables[0].Rows)
						{
							ViewScrap view = new ViewScrap {
								ClassificationId = data["ClassificationId"].ToString(),
								ClassificationCode = data["ClassificationCode"].ToString(),
								ClassificationName = data["ClassificationName"].ToString(),
								DestinationId = data["DestinationId"].ToString(),
								Pemasukan = (double)data["Pemasukan"],
								Pengeluaran=(double)data["Pengeluaran"],
								SaldoAwal=(double)data["SaldoAwal"],
								SaldoBuku=(double)data["SaldoBuku"],
								UnitQtyName=data["UnitQtyName"].ToString(),
								Penyesuaian=(double)data["Penyesuaian"],
								StockOpname=(double)data["StockOpname"],
								Selisih=(double)data["Selisih"],
								StockId=data["StockId"].ToString()

							};
							scrapData.Add(view);
						}
					}
				}
			
			}
			catch (SqlException ex)
			{
				//Log exception
				//Display Error message
			}
			 

			return scrapData.AsQueryable();
		}
	}
}
