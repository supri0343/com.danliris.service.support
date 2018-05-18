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
    public class WIPService
    {
		SupportDbContext context;
		public WIPService(SupportDbContext _context)
		{
			this.context = _context;
		}
		public IQueryable<WIPViewModel> GetWIPReport(DateTime? date, int offset)
		{
			DateTime Date = date == null ? new DateTime(1970, 1, 1) : (DateTime)date;
			string Dates = Date.ToString("yyyy-MM-dd");
			List<WIPViewModel> wipData = new List<WIPViewModel>();
			try
			{
				string connectionString = APIEndpoint.ConnectionString;
				using (SqlConnection conn =
					new SqlConnection(connectionString))
				{
					conn.Open();
					using (SqlCommand cmd = new SqlCommand("SELECT     Kode, komoditi, Satuan, SUM(JumlahPotong - JumlahFinish) AS WIP FROM "+
											"(SELECT HO.No, HO.Qty, HO.Kode, KOM.komoditi, CU.SizeNumber, CU.JumlahPotong, ISNULL(FOUT.JumlahFinish, 0) AS JumlahFinish, 'PCS' AS Satuan FROM  HOrder AS HO INNER JOIN  Komoditi AS KOM ON HO.Kode = KOM.kode INNER JOIN "+
											"(SELECT  a.RO, c.SizeId, c.SizeNumber, SUM(b.Qty) AS JumlahPotong FROM  CuttingOut AS a INNER JOIN CuttingOutDetail AS b ON a.CuttingNo = b.CuttingNo INNER JOIN Sizes AS c ON b.SizeId = c.SizeId "+
											"WHERE(a.CuttingOutTo = 'SEWING') AND(a.ProcessDate <= '"+ Dates + "') GROUP BY a.RO, c.SizeId, c.SizeNumber) AS CU ON HO.No = CU.RO LEFT OUTER JOIN "+
											"(SELECT a.RO, c.SizeId, c.SizeNumber, SUM(b.Qty) AS JumlahFinish FROM   FinishingOut AS a INNER JOIN  FinishingOutDetail AS b ON a.FinishingOutId = b.FinishingOutId INNER JOIN Sizes AS c ON b.SizeId = c.SizeId "+
											"WHERE(a.FinishingOutTo = 'GUDANG JADI') AND(a.ProcessDate <='" + Dates + "') GROUP BY a.RO, c.SizeId, c.SizeNumber) AS FOUT ON CU.RO = FOUT.RO AND CU.SizeId = FOUT.SizeId) AS HASIL GROUP BY Kode, komoditi, Satuan ORDER BY Kode, komoditi", conn))
					{
						SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
						DataSet dSet = new DataSet();
						dataAdapter.Fill(dSet);
						foreach (DataRow data in dSet.Tables[0].Rows)
						{
							WIPViewModel view = new WIPViewModel
							{
								Kode = data["Kode"].ToString(),
								Comodity = data["komoditi"].ToString(),
								UnitQtyName=data["Satuan"].ToString(),
								WIP = String.Format("{0:n}", (double)data["WIP"])
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

        public MemoryStream GenerateExcel(DateTime? date, int offset)
        {
            var Query = GetWIPReport(date, offset);
            Query = Query.OrderBy(b => b.Kode).ThenBy(b=>b.Comodity);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(int) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "WIP", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else {
                var index = 1;
                foreach (var item in Query)
                {
                    result.Rows.Add((index), item.Kode, item.Comodity, item.UnitQtyName, item.WIP);
                    index++;
                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

    }
}
