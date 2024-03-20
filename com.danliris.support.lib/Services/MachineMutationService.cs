﻿using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.ViewModel;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace com.danliris.support.lib.Services
{
    public class MachineMutationService
    {
		SupportDbContext context;
		public MachineMutationService(SupportDbContext _context)
		{
			this.context = _context;
		}


		public IQueryable<FinishedGoodViewModel> GetMachineMutationReport(DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime Datefrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime Dateto = dateTo == null ? new DateTime(1970, 1, 1) : (DateTime)dateTo;
			string DateFrom = Datefrom.ToString("yyyy-MM-dd");
			string DateTo = Dateto.ToString("yyyy-MM-dd");
			List<FinishedGoodViewModel> machine = new List<FinishedGoodViewModel>();
			try
			{
				string connectionString = APIEndpoint.ConnectionString;
				using (SqlConnection conn =
					new SqlConnection(connectionString))
				{
					conn.Open();
					using (SqlCommand cmd = new SqlCommand("declare @TglLaporan datetime='"+DateFrom+"'"+
														   "declare @Tgl2 datetime = '" + DateTo + "'" +
															"select B.KodeBarang, B.NamaBarang, B.Sat,convert(float, B.SaldoAwal) as SaldoAwal,convert(float, B.Pemasukan) as Pemasukan,convert(float, B.Pengeluaran) as Pengeluaran,convert(float,B.Penyesuaian) as Penyesuaian,convert(float, (B.SaldoAwal + (B.Pemasukan - B.Pengeluaran))) as SaldoBuku,convert(float, B.StockOpname) as StockOpname, convert(float,B.Selisih) as Selisih, B.ActivateDate, B.CloseDate from " +
															"(select A.KodeBarang, A.NamaBarang, A.Sat,(A.SaldoAwal + (select isnull(SUM(TransactionAmount), 0) from dbo.MachineMutation where MachineID = A.KodeBarang and TransactionType = 'IN' and TransactionDate < @TglLaporan) - (select isnull(SUM(TransactionAmount), 1) from dbo.MachineMutation where MachineID = A.KodeBarang and TransactionType = 'OUT' and TransactionDate < @TglLaporan)) as SaldoAwal,	(select isnull(SUM(TransactionAmount), 0) from dbo.MachineMutation where MachineID = A.KodeBarang and TransactionType = 'IN' and TransactionDate>= @TglLaporan and TransactionDate<DATEADD(DAY,1,@Tgl2)) as Pemasukan, "+
															"(select isnull(SUM(TransactionAmount), 0) from dbo.MachineMutation where MachineID = A.KodeBarang and TransactionType = 'OUT' and TransactionDate>= @TglLaporan and TransactionDate<DATEADD(DAY,1,@Tgl2)) as Pengeluaran, "+
															"A.Penyesuaian, A.StockOpname, A.Selisih, A.ActivateDate, A.CloseDate from view_machinereport A where A.ActivateDate <= @Tgl2)B "+
															"where B.SaldoAwal > 0 or(B.SaldoAwal + (B.Pemasukan - B.Pengeluaran)) > 0 order by B.KodeBarang  ", conn))
					{
						SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
						DataSet dSet = new DataSet();
						dataAdapter.Fill(dSet);
						double Pemasukan = 0;
						double Pengeluaran = 0;
						double Penyesuaian = 0;
						double SaldoAwal = 0;
						double SaldoBuku = 0;
						double Selisih = 0;
						double StockOpname = 0;
						foreach (DataRow data in dSet.Tables[0].Rows)
						{
							Pemasukan += (double)data["Pemasukan"];
							Pengeluaran += (double)data["Pengeluaran"];
							SaldoAwal += (double)data["SaldoAwal"];
							SaldoBuku += (double)data["SaldoBuku"];
							Penyesuaian += (double)data["Penyesuaian"];
							StockOpname += (double)data["StockOpname"];
							Selisih += (double)data["Selisih"];

							FinishedGoodViewModel view = new FinishedGoodViewModel
							{
								KodeBarang = data["KodeBarang"].ToString(),
								NamaBarang = data["NamaBarang"].ToString(),
								Pemasukan = String.Format("{0:n}", (double)data["Pemasukan"]),
								Pengeluaran = String.Format("{0:n}", (double)data["Pengeluaran"]),
								SaldoAwal = String.Format("{0:n}", (double)data["SaldoAwal"]),
								SaldoBuku = String.Format("{0:n}", (double)data["SaldoBuku"]),
								UnitQtyName = data["Sat"].ToString(),
								Penyesuaian = String.Format("{0:n}", (double)data["Penyesuaian"]),
								StockOpname = String.Format("{0:n}", (double)data["StockOpname"]),
								Selisih = String.Format("{0:n}", (double)data["Selisih"])
							};
							machine.Add(view);
						}
						machine.OrderBy(x => x.KodeBarang);

						machine.Add(new FinishedGoodViewModel
						{
							KodeBarang = "TOTAL",
							NamaBarang = "",
							Pemasukan = String.Format("{0:n}", Pemasukan),
							Pengeluaran = String.Format("{0:n}", Pengeluaran),
							SaldoAwal = String.Format("{0:n}", SaldoAwal),
							SaldoBuku = String.Format("{0:n}", SaldoBuku),
							UnitQtyName = "",
							Penyesuaian = String.Format("{0:n}", Penyesuaian),
							StockOpname = String.Format("{0:n}", StockOpname),
							Selisih = String.Format("{0:n}", Selisih)
						});
					}
					conn.Close();
				}
			}
			catch (SqlException ex)
			{
				//Log exception
				//Display Error message
			}

			return machine.AsQueryable();
		}
        public Tuple<List<FinishedGoodViewModel>, int> GetMachineMutationReportData(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetMachineMutationReport(dateFrom, dateTo, offset);

            //Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            //if (OrderDictionary.Count.Equals(0))
            //{
            //    Query = Query.OrderBy(b => b.KodeBarang);
            //}
            //else
            //{
            //    string Key = OrderDictionary.Keys.First();
            //    string OrderType = OrderDictionary[Key];

            //    //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            //}


            Pageable<FinishedGoodViewModel> pageable = new Pageable<FinishedGoodViewModel>(Query, page - 1, size);
            List<FinishedGoodViewModel> Data = pageable.Data.ToList<FinishedGoodViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetMachineMutationReport(dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.KodeBarang);
            DataTable result = new DataTable();
			result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(string) });
			result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(string) });
			result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(string) });
			result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang", DataType = typeof(string) });
			result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Barang", DataType = typeof(double) });
			result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(double) });
			result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Pemasukan Barang", DataType = typeof(double) });
			result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Pengeluaran Barang", DataType = typeof(double) });
			result.Columns.Add(new DataColumn() { ColumnName = "Penyesuaian (Adjustment)", DataType = typeof(double) });
			result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(double) });
			result.Columns.Add(new DataColumn() { ColumnName = "Hasil Pencacahan", DataType = typeof(double) });
			result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Selisih", DataType = typeof(double) });
			result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
			if (Query.ToArray().Count() == 0)
				result.Rows.Add("", "", "", "", "", 0, 0, 0, 0, 0, 0, 0,0, ""); // to allow column name to be generated properly for empty data as template
			else
			{

				int idx = 1;
				foreach (var item in Query)
				{
					result.Rows.Add(idx.ToString(),(item.KodeBarang), item.NamaBarang, item.UnitQtyName,0, item.SaldoAwal, item.Pemasukan, item.Pengeluaran, item.Penyesuaian, item.SaldoBuku, item.StockOpname, item.Selisih,"-");
					idx++;
				}
			}
			//return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
			ExcelPackage package = new ExcelPackage();
			var sheet = package.Workbook.Worksheets.Add("Data");

			sheet.Cells["A1"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

			var a = Query.Count();
			sheet.Cells[$"A{a + 1}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
			sheet.Cells[$"A{a + 1}:C{a + 1}"].Merge = true;
			sheet.Cells[$"A{a + 1}:C{a + 1}"].Style.Font.Bold = true;
			sheet.Cells[$"A{a + 1}:C{a + 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			sheet.Cells[$"A{a + 1}:C{a + 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

			MemoryStream stream = new MemoryStream();
			package.SaveAs(stream);
			return stream;
		}
    }
}
