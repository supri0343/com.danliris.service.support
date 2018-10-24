using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace com.danliris.support.lib.Services
{
    public class FinishedGoodService
    {
		SupportDbContext context;
		public FinishedGoodService(SupportDbContext _context)
		{
			this.context = _context;
		}
		public IQueryable<FinishedGoodViewModel> GetFinishedGoodReport(DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime Datefrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime Dateto = dateTo == null ? new DateTime(1970, 1, 1) : (DateTime)dateTo;
			string DateFrom = Datefrom.ToString("yyyy-MM-dd");
			string DateTo = Dateto.ToString("yyyy-MM-dd");
			List<FinishedGoodViewModel> wipData = new List<FinishedGoodViewModel>();
			try
			{
				string connectionString = APIEndpoint.ConnectionString;
				using (SqlConnection conn =
					new SqlConnection(connectionString))
				{
					conn.Open();
					using (SqlCommand cmd = new SqlCommand("declare @tglBalance datetime set @tglBalance=(select top 1 BalanceDate from BalanceStockProduction where BalanceDate<'" + DateFrom+"' order by BalanceDate desc) "+
							"if @tglBalance is null	set @tglBalance = CAST(GETDATE() as datetime) "+
							"select UnitCode, RO, ComodityID, SizeId, Quantity, UnitQuantity, TotalPrice into #tmp from BalanceStockProduction where BalanceDate=@tglBalance select HASIL.KodeBarang, HASIL.NamaBarang, HASIL.Satuan,convert(float, SUM(HASIL.SaldoAwal)) as SaldoAwal, convert(float,SUM(HASIL.Pemasukan)) as Pemasukan, convert(float,SUM(HASIL.Pengeluaran)) as Pengeluaran,convert(float,SUM(HASIL.Penyesuaian)) as Penyesuaian, convert(float,SUM(HASIL.SaldoBuku)) as SaldoBuku, convert(float,SUM(HASIL.StockOpname)) as StockOpname, convert(float,SUM(HASIL.Selisih) ) as Selisih from (select DATA.KodeBarang, DATA.NamaBarang, DATA.Satuan, SUM(DATA.SaldoAwal) as SaldoAwal, SUM(DATA.Pemasukan) as Pemasukan, SUM(DATA.Pengeluaran) as Pengeluaran, SUM(DATA.Penyesuaian) as Penyesuaian, SUM(DATA.SaldoBuku) as SaldoBuku, SUM(DATA.StockOpname) as StockOpname, SUM(DATA.Selisih) as Selisih from (select TERIMA.KodeBarang, TERIMA.NamaBarang, TERIMA.Satuan, TERIMA.SaldoAwal, TERIMA.Pemasukan, TERIMA.Pengeluaran, TERIMA.Penyesuaian,(TERIMA.SaldoAwal + (TERIMA.Pemasukan - TERIMA.Pengeluaran) + TERIMA.Penyesuaian) as SaldoBuku, 0 as StockOpname, 0 as Selisih from (select KodeBarang,   NamaBarang, 'PCS' as Satuan, 0 as SaldoAwal,QuantityFinOut, QuantityStockHistory, QuantityStockHistory as Pemasukan, 0 as Pengeluaran, 0 as Penyesuaian " +
							"from FactFinishingOutDetail where /* ProcessDate >= '" + DateFrom+ "' and*/  ProcessDate < '"+DateTo+"' " +
							"union all " +
							"select  KodeBarang,  NamaBarang, 'PCS' as Satuan, (QuantityStockHistory) as SaldoAwal, 0, 0, 0 as Pemasukan, 0 as Pengeluaran, 0 as Penyesuaian " +
							"from FactFinishingOutDetail " +
                            "where /*ProcessDate >= DATEADD(day, 1, @tglBalance) and*/   ProcessDate <= DATEADD(day, -1, '" + DateFrom+"') " +
							"union all " +
							"select b.ComodityCode as KodeBarang, b.ComodityName as NamaBarang, 'PCS' as Satuan, a.Quantity as SaldoAwal, 0, 0, 0 as Pemasukan, 0 as Pengeluaran, 0 as Penyesuaian " +
							"from #tmp a inner join Comodity b on a.ComodityID=b.ComodityID " +
							")TERIMA)DATA group by DATA.KodeBarang, DATA.NamaBarang, DATA.Satuan " +
							"union all " +
							"select DATA2.KodeBarang, DATA2.NamaBarang, DATA2.Satuan, SUM(DATA2.SaldoAwal) as SaldoAwal, SUM(DATA2.Pemasukan) as Pemasukan, SUM(DATA2.Pengeluaran) as Pengeluaran, " +
							"SUM(DATA2.Penyesuaian) as Penyesuaian, SUM(DATA2.SaldoBuku) as SaldoBuku, SUM(DATA2.StockOpname) as StockOpname, SUM(DATA2.Selisih) as Selisih " +
							"from(select KELUAR.KodeBarang, KELUAR.NamaBarang, KELUAR.Satuan, KELUAR.SaldoAwal, KELUAR.Pemasukan, KELUAR.Pengeluaran, KELUAR.Penyesuaian,(KELUAR.SaldoAwal + (KELUAR.Pemasukan - KELUAR.Pengeluaran) + KELUAR.Penyesuaian) as SaldoBuku, 0 as StockOpname, 0 as Selisih " +
							"from(select  KodeBarang,   NamaBarang, 'PCS' as Satuan, 0 as SaldoAwal,  Qty, 0 as Pemasukan, (Qty) as Pengeluaran, 0 as Penyesuaian " +
							"from FactExpenditureGood " +
							"where  ProcessDate >= '"+DateFrom+ "' and  ProcessDate <= '"+DateTo+"' " +
							"union all " +
							"select   KodeBarang,   NamaBarang, 'PCS' as Satuan, -(Qty) as SaldoAwal, 0, 0 as Pemasukan, 0 as Pengeluaran, 0 as Penyesuaian " +
                            "from FactExpenditureGood  where /*ProcessDate >= DATEADD(day, 1, @tglBalance) and*/   ProcessDate <= DATEADD(day, -1, '" + DateFrom + "'))KELUAR )DATA2 group by DATA2.KodeBarang, DATA2.NamaBarang, DATA2.Satuan )HASIL group by HASIL.KodeBarang, HASIL.NamaBarang, HASIL.Satuan " +
							"order by HASIL.KodeBarang drop table #tmp    ", conn))
					{
						SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
						DataSet dSet = new DataSet();
						dataAdapter.Fill(dSet);
						foreach (DataRow data in dSet.Tables[0].Rows)
						{
							FinishedGoodViewModel view = new FinishedGoodViewModel
							{
								KodeBarang = data["KodeBarang"].ToString(),
								NamaBarang=data["NamaBarang"].ToString(),
								Pemasukan = String.Format("{0:n}", (double)data["Pemasukan"]),
								Pengeluaran = String.Format("{0:n}", (double)data["Pengeluaran"]),
								SaldoAwal = String.Format("{0:n}", (double)data["SaldoAwal"]),
								SaldoBuku = String.Format("{0:n}", (double)data["SaldoBuku"]),
								UnitQtyName = data["Satuan"].ToString(),
								Penyesuaian = String.Format("{0:n}", (double)data["Penyesuaian"]),
								StockOpname = String.Format("{0:n}", (double)data["StockOpname"]),
								Selisih = String.Format("{0:n}", (double)data["Selisih"])
							};
							wipData.Add(view);
						}
					}
					conn.Close();
				}
			}
			catch (SqlException ex)
			{

			}
			return wipData.AsQueryable();
		}

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetFinishedGoodReport(dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.KodeBarang);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sat", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pemasukan", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penyesuaian", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Buku", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Stock Opname", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Selisih", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
                foreach (var item in Query)
                {
                    result.Rows.Add((item.KodeBarang), item.NamaBarang, item.UnitQtyName, item.SaldoAwal, item.Pemasukan, item.Pengeluaran, item.Penyesuaian, item.SaldoBuku, item.StockOpname, item.Selisih);
                }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
    }
}
