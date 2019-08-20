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
					using (SqlCommand cmd = new SqlCommand(" select kode,komoditi,Satuan,sum(WIP) WIP from (SELECT  ro,Kode,ComodityName as komoditi, Satuan,  case when SUM(JumlahCutting-jumlahFinish)<0 then 0 else SUM(JumlahCutting-jumlahFinish)end AS WIP FROM  (SELECT HO.No RO, HO.Qty, HO.Kode, KOM.ComodityName, LI.SizeNumber,LI.JumlahCutting, ISNULL(FOUT.JumlahFinish, 0) AS JumlahFinish, 'PCS' AS Satuan  FROM  HOrder AS HO INNER JOIN  Comodity AS KOM ON HO.Kode = KOM.ComodityCode INNER JOIN  (SELECT  ro,ComodityCode,SizeId,SizeNumber,sum(JumlahCutting)JumlahCutting from Factcutting where  ProcessDate <='" + Dates + "' group by ro,ComodityCode,SizeId,SizeNumber) AS LI ON HO.No = LI.RO and ho.kode = li.ComodityCode  LEFT OUTER JOIN  (SELECT ro,ComodityCode,SizeId,SizeNumber,sum(JumlahFinish) JumlahFinish from FactFinishing where  ProcessDate <='" + Dates + "' group by ro,ComodityCode,SizeId,SizeNumber) AS FOUT ON LI.RO = FOUT.RO AND LI.SizeId = FOUT.SizeId  and LI.ComodityCode=FOUT.ComodityCode  ) AS HASIL GROUP BY Kode, ComodityName, Satuan,ro ) as data group BY Kode, komoditi,Satuan  ", conn))
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
            result.Columns.Add(new DataColumn() { ColumnName = "WIP", DataType = typeof(Double) });
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
