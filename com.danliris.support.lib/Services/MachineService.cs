using com.danliris.support.lib.Interfaces;
using com.danliris.support.lib.Models;
using com.danliris.support.lib.ViewModel;
using System;
using System.Collections.Generic;
using Com.Moonlay.NetCore.Lib;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using com.danliris.support.lib.Models.Machine;
using Newtonsoft.Json;
using Com.DanLiris.Service.Purchasing.Lib.Helpers;
using System.IO;
using System.Data;
using OfficeOpenXml;

namespace com.danliris.support.lib.Services
{
    public class MachineService : IMachineService
    {
        private readonly SupportDbContext context;


        public MachineService(SupportDbContext context) 
        {
            this.context = context;
        }

        public List<MachineBrand> GetMachineBrand(int size, string keyword)
        {
            IQueryable<MachineBrand> Query = context.MachineBrand;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                Query = Query.Where(s => s.BrandName.Contains(keyword));
            }

            Query = Query
               .OrderByDescending(o => o.BrandID)
               .Distinct();

            Query = Query.Take(size);

            return Query.ToList();
        }

        public async Task<int>CreateMachineBrand(MachineBrand machineBrand,string username)
        {
            int Created = 0;
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var db = context.MachineBrand;

                    var count = context.MachineBrand.Select(x => x.BrandID).Count();
                    var ID = "MB" + (count+1).ToString().PadLeft(5, '0');
                    MachineBrand brand = new MachineBrand
                    {
                        BrandID = ID,
                        BrandName = machineBrand.BrandName,
                        Description = machineBrand.Description,
                        CreatedBy = username,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = username,
                        ModifiedDate = DateTime.Now,
                    };

                    db.Add(brand);
                    Created = await context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }

        public List<MachineCategory> GetMachineCategory(int size, string keyword)
        {
            IQueryable<MachineCategory> Query = context.MachineCategory;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                Query = Query.Where(s => s.CategoryName.Contains(keyword));
            }

            Query = Query
               .OrderByDescending(o => o.CategoryID)
               .Distinct();

            Query = Query.Take(size);

            return Query.ToList();
        }

        public async Task<int> CreateMachineCategory(MachineCategory machineBrand,string username)
        {
            int Created = 0;
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var db = context.MachineCategory;

                    var count = context.MachineCategory.Select(x => x.CategoryID).Count();
                    var ID = "MC" + (count + 1).ToString().PadLeft(5, '0');
                    MachineCategory category = new MachineCategory
                    {
                        CategoryID = ID,
                        CategoryName = machineBrand.CategoryName,
                        Description = machineBrand.Description,
                        CreatedBy = username,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = username,
                        ModifiedDate = DateTime.Now,
                    };

                    db.Add(category);
                    Created = await context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }

        public List<Machine> GetMachine(int Page = 1, int Size = 25, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Machine> Query = context.Machine;

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<Machine>.ConfigureFilter(Query, FilterDictionary);

            List<string> searchAttributes = new List<string>()
            {
                "MachineBrand","MachineCategory","MachineType","IDNumber"
            };

            Query = QueryHelper<Machine>.ConfigureSearch(Query, searchAttributes, Keyword);
            
            Query = Query.Distinct().Take(25);
            

            return Query.ToList();
        }

        public List<Machine> GetMachines(string tipe, string ctg, string serial)
        {
            //IQueryable<Machine> Query = context.Machine;

            var Query = (from a in context.Machine
                         where
                         a.MachineType == (string.IsNullOrWhiteSpace(tipe) ? a.MachineType : tipe)
                         && a.MachineCategory == (string.IsNullOrWhiteSpace(ctg) ? a.MachineCategory : ctg)
                         && a.IDNumber == (string.IsNullOrWhiteSpace(serial) ? a.IDNumber : serial)
                         select a);

            //Query = Query.Distinct().Take(25);


            return Query.ToList();
        }

        public List<MachineTypes> GetMachineByBrand(int Page = 1, int Size = 25, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Machine> Query = context.Machine;

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<Machine>.ConfigureFilter(Query, FilterDictionary);

            List<string> searchAttributes = new List<string>()
            {
             "MachineType"
            };

            Query = QueryHelper<Machine>.ConfigureSearch(Query, searchAttributes, Keyword);

            var query = Query.Select(x => new MachineTypes { MachineType = x.MachineType }).Distinct().Take(25);


            return query.ToList();
        }

        public Machine GetMachineById(string id)
        {
            var model = context.Machine.Where(x => x.MachineID == id).FirstOrDefault();
            return model;
        }

        public async Task<int> UpdateMachine(string id, Machine machine,string username)
        {
            int Updated = 0;
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var oldMachine = context.Machine.FirstOrDefault(x => x.MachineID == id);

                    oldMachine.MachineCategory = machine.MachineCategory;
                    oldMachine.MachineBrand = machine.MachineBrand;
                    oldMachine.MachineType = machine.MachineType;
                    oldMachine.MachineQuantity = machine.MachineQuantity;
                    oldMachine.UnitQuantity = machine.UnitQuantity;
                    oldMachine.SupplierType = machine.SupplierType;
                    oldMachine.PurchaseYear = machine.PurchaseYear;
                    oldMachine.BCNumber = machine.BCNumber;
                    oldMachine.IDNumber = machine.IDNumber;
                    oldMachine.CloseDate = machine.CloseDate;
                    oldMachine.Classification = machine.Classification;
                    oldMachine.BCOutNumber = machine.BCOutNumber;
                    oldMachine.ModifiedDate = DateTime.Now;
                    oldMachine.ModifiedBy = username;

                    Updated = await context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }
            return Updated;
        }


        public async Task<int> CreateMachineAndMutation(MachinesMutation machinesMutation, string username)
        {
            int Created = 0;
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    //Transaksi Masuk
                    if (machinesMutation.TransactionType == "IN")
                    {
                        var count = context.Machine.Select(x => x.MachineID).Count();
                        var machineID = "M" + (count + 1).ToString().PadLeft(10, '0');

                        var cek_database = (from v in context.Machine where v.MachineType == machinesMutation.MachineType select v).Count();

                        Machine machine = new Machine
                        {
                            MachineID = machineID,
                            MachineIndex = cek_database + 1,
                            MachineCategory = machinesMutation.MachineCategory,
                            MachineBrand = machinesMutation.MachineBrand,
                            MachineType = machinesMutation.MachineType,
                            MachineQuantity = machinesMutation.MachineQuantity,
                            UnitQuantity = machinesMutation.UnitQuantity,
                            SupplierType = machinesMutation.SupplierType,
                            PurchaseYear = machinesMutation.PurchaseYear,
                            IDNumber = machinesMutation.IDNumber,
                            Classification = machinesMutation.Classification,
                            BCNumber = machinesMutation.BCNumber,
                            MachineBeginningBalance = 1,
                            MachineValue = 0,
                            ActivateDate = DateTime.Now,
                            CloseDate = null,
                            CreatedBy = username,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = username,
                            ModifiedDate = DateTime.Now,
                            State = "Active"

                        };

                        MachineMutation mutation = new MachineMutation
                        {
                            TransactionID = Guid.NewGuid(),
                            MachineID = machineID,
                            TransactionDate = machinesMutation.TransactionDate,
                            TransactionType = machinesMutation.TransactionType,
                            TransactionAmount = machinesMutation.TransactionAmount,
                            CreatedBy = username,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = username,
                            ModifiedDate = DateTime.Now,
                            Description = null,
                        };

                        context.Machine.Add(machine);
                        context.MachineMutation.Add(mutation);
                        Created = await context.SaveChangesAsync();
                        transaction.Commit();
                    }

                    //Transaksi Keluar
                    else if (machinesMutation.TransactionType == "OUT")
                    {
                        MachineMutation mutation = new MachineMutation
                        {
                            TransactionID = Guid.NewGuid(),
                            MachineID = machinesMutation.MachineID,
                            TransactionDate = machinesMutation.TransactionDate,
                            TransactionType = machinesMutation.TransactionType,
                            TransactionAmount = machinesMutation.TransactionAmount,
                            CreatedBy = username,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = username,
                            ModifiedDate = DateTime.Now,
                            Description = null,
                        };

                        var oldMachine = context.Machine.FirstOrDefault(x => x.MachineID == machinesMutation.MachineID);
                        oldMachine.BCOutNumber = machinesMutation.BCOutNumber;

                        context.MachineMutation.Add(mutation);
                        Created = await context.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }

        public List<MutationMachine> GetMachineMutation(string tipe, string ctg, string serial)
        {
            var Query = (from a in context.Machine
                         join b in context.MachineMutation on a.MachineID equals b.MachineID
                         where
                          a.MachineType == (string.IsNullOrWhiteSpace(tipe) ? a.MachineType : tipe)
                         && a.MachineCategory == (string.IsNullOrWhiteSpace(ctg) ? a.MachineCategory : ctg)
                         && a.IDNumber == (string.IsNullOrWhiteSpace(serial) ? a.IDNumber : serial)
                         select new MutationMachine
                         {
                             MachineBrand = a.MachineBrand,
                             MachineCategory = a.MachineCategory,
                             MachineQuantity = a.MachineQuantity,
                             UnitQuantity = a.UnitQuantity,
                             MachineType = a.MachineType,
                             IDNumber = a.IDNumber,
                             TransactionAmount = b.TransactionAmount,
                             TransactionID = b.TransactionID,
                             TransactionType = b.TransactionType,
                             TransactionDate = b.TransactionDate,
                         });

            Query = Query
               .OrderByDescending(o => o.TransactionDate)
               .Distinct();   

            return Query.ToList();
        }


        public MutationMachine GetMutationById(Guid id)
        {
            var model = (from a in context.Machine
                         join b in context.MachineMutation on a.MachineID equals b.MachineID
                         where b.TransactionID == id
                         select new MutationMachine
                         {
                             MachineBrand = a.MachineBrand,
                             MachineCategory = a.MachineCategory,
                             MachineQuantity = a.MachineQuantity,
                             MachineType = a.MachineType,
                             TransactionAmount = b.TransactionAmount,
                             TransactionID = b.TransactionID,
                             TransactionType = b.TransactionType,
                             TransactionDate = b.TransactionDate,
                         }).FirstOrDefault();

            return model;
        }

        public async Task<int> UpdateMutation(Guid id, MachineMutation mutation, string username)
        {
            int Updated = 0;
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var oldMutation = context.MachineMutation.FirstOrDefault(x => x.TransactionID == id);

                    oldMutation.TransactionType = mutation.TransactionType;
                    oldMutation.TransactionAmount = mutation.TransactionAmount;
                    oldMutation.TransactionDate = mutation.TransactionDate;
                    oldMutation.ModifiedBy = username;
                    oldMutation.ModifiedDate = DateTime.Now;


                    Updated = await context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }
            return Updated;
        }

        public MemoryStream GetXlsMachineMutation(string tipe, string ctg, string serial)
        {
            var Query = GetMachineMutation(tipe, ctg, serial);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Brand", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Serial", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah", DataType = typeof(decimal) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Transaksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Transaksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Transaksi", DataType = typeof(String) });
            

            if(Query.Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", 0, "", "", "", "");
            }
            else
            {
                int index = 1;
                foreach(var a in Query)
                {
                    var transDate = a.TransactionDate.Value.AddHours(7).ToString("dd-MMM-yyyy");
                    result.Rows.Add(index++, a.MachineBrand, a.MachineCategory, a.MachineType, a.IDNumber, a.MachineQuantity, a.UnitQuantity, a.TransactionType, a.TransactionAmount, transDate);
                }
            }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Report");
            sheet.Cells["A1"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }

        public MemoryStream GetXlsMachine(string tipe, string ctg, string serial)
        {
            var Query = GetMachines(tipe, ctg, serial);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Klasifikasi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Brand", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Serial", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah", DataType = typeof(decimal) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tahun Beli", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
         


            if (Query.Count() == 0)
            {
                result.Rows.Add("", "", "", "", "","", 0, "", "", "");
            }
            else
            {
                int index = 1;
                foreach (var a in Query)
                {
                   
                    result.Rows.Add(index++,a.Classification, a.MachineBrand, a.MachineCategory, a.MachineType, a.IDNumber, a.MachineQuantity, a.UnitQuantity,a.PurchaseYear,a.SupplierType);
                }
            }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Report");
            sheet.Cells["A1"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }

    }
}


