using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using com.danliris.support.lib.Interfaces;
using com.danliris.support.lib.Models;
using Com.DanLiris.Service.support.lib.Services;
using System.Threading.Tasks;
using com.danliris.support.lib.Models.Ceisa.PEB;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using com.danliris.support.lib.Helpers;
//using Newtonsoft.Json;
using com.danliris.support.lib.ViewModel.Ceisa;
using com.danliris.support.lib.ViewModel.Ceisa.PEBViewModel;
using Com.Moonlay.NetCore.Lib;
using AutoMapper;
using Newtonsoft.Json;
using com.danliris.support.lib.Interfaces.Ceisa;
using System.IO;
using Com.DanLiris.Service.Purchasing.Lib.Helpers;

namespace com.danliris.support.lib.Services.Ceisa
{
    public class PEBService : IPEBService
    {
        private readonly string USER_AGENT = "Service";
        private readonly SupportDbContext context;
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;
        private readonly DbSet<PEBHeader> dbSet;
        private readonly DbSet<BEACUKAI_ADDED> bEACUKAI_ADDEDs;
        private readonly DbSet<BEACUKAI_ADDED_DETAIL> bEACUKAI_ADDED_DETAILs;
        private ICeisaService CeisaService;

        public PEBService(SupportDbContext context, IServiceProvider serviceProvider, ICeisaService ceisaService)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
            this.CeisaService = ceisaService;
            dbSet = context.Set<PEBHeader>();
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<PEBViewModelList> Query = dbSet.Where(s => s.kodeDokumen == "30"
            && s._IsDeleted == false)
                .Select(m => new PEBViewModelList
            {
                Id = m.Id,
                nomorAju = m.nomorAju,
                tanggalAju = m.tanggalAju.ToString("dd-MMM-yyyy"),
                nomorDaftar = string.IsNullOrWhiteSpace(m.nomorDaftar) ? "-" : m.nomorDaftar,
                tanggalDaftar = m.tanggalDaftar == null ? "-" : m.tanggalDaftar.Value.ToString("dd-MMM-yyyy"),
                namaPenerima = m.entitas.Where(x => x.kodeEntitas == "8")
                .Select(i => i.namaEntitas)
                .FirstOrDefault(),
                isPosted = m.isPosted,
                postedBy = string.IsNullOrWhiteSpace(m.postedBy) ? "-" : m.postedBy,
                CreatedDate = m._CreatedUtc.ToString("dd-MMM-yyyy")
            }).OrderByDescending(x => x.nomorAju) ;

            List<string> SearchAtt = new List<string>() { "namaPenerima", "nomorAju", "nomorDaftar" };

            Query = QueryHelper<PEBViewModelList>.ConfigureSearch(Query, SearchAtt,Keyword);


            //Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            //Query = QueryHelper<PEBViewModel>.ConfigureFilter(Query, FilterDictionary);

            //Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            //Query = QueryHelper<PEBViewModel>.ConfigureOrder(Query, OrderDictionary);

            //Pageable<PEBViewModel> pageable = new Pageable<PEBViewModel>(Query, Page - 1, Size);
            //List<PEBViewModel> Data = pageable.Data.ToList();
            //int TotalData = pageable.TotalCount;

            List<object> ListData = new List<object>();
            ListData.AddRange(Query.Select(s => new
            {
                s.Id,
                s.nomorAju,
                s.tanggalAju,
                s.nomorDaftar,
                s.tanggalDaftar,
                s.namaPenerima,
                s.isPosted,
                s.postedBy,
                s.CreatedDate
            }));

            return new ReadResponse<object>(ListData, 0, null);
        }

        
        public async Task<PEBHeader> ReadById(long id)
        {
            //PEBHeaderViewModel data = new PEBHeaderViewModel();
            var model = await dbSet.Where(m => m.Id == id)
                .Include(m => m.barang)
                .ThenInclude(x => x.barangDokumen)
                .Include(m => m.barang)
                .ThenInclude(x => x.barangTarif)
                .Include(m => m.barang)
                .ThenInclude(x => x.barangPemilik)
                .Include(m => m.entitas)
                .Include(m => m.kemasan)
                .Include(m => m.kontainer)
                .Include(m => m.dokumen)
                .Include(m => m.pengangkut)
                .Include(m => m.bankDevisa)
                .Include(m => m.kesiapanBarang)
                .FirstOrDefaultAsync();
            
            return model;
        }

        public string Urut(string tipe)
        {
            var Query = dbSet.Where(s => s.kodeDokumen == tipe && s._IsDeleted==false).OrderByDescending(x => x.nomorAju).Select(x => new { x.nomorAju }).FirstOrDefault();
            var NoUrut = "";
            if (Query!= null)
            {
                var recentNo = Query.nomorAju.Substring(20, 6);
                var No = int.Parse(recentNo) + 1;

                NoUrut = No.ToString().PadLeft(6, '0');
            }
            else
            {
                NoUrut = "1".PadLeft(6, '0');
            }
            return NoUrut;

        }

        public async Task<int> Create(PEBHeader model)
        {
            int Created = 0;
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    MoonlayEntityExtension.FlagForCreate(model, identityService.Username, USER_AGENT);
                    foreach(var barang in model.barang)
                    {
                        MoonlayEntityExtension.FlagForCreate(barang, identityService.Username, USER_AGENT);
                        foreach(var barangDokumen in barang.barangDokumen )
                        {
                            MoonlayEntityExtension.FlagForCreate(barangDokumen, identityService.Username, USER_AGENT);
                        }
                        foreach (var barangPemilik in barang.barangPemilik)
                        {
                            MoonlayEntityExtension.FlagForCreate(barangPemilik, identityService.Username, USER_AGENT);
                        }
                        foreach (var barangTarif in barang.barangTarif)
                        {
                            MoonlayEntityExtension.FlagForCreate(barangTarif, identityService.Username, USER_AGENT);
                        }
                    }

                    foreach(var entitas in model.entitas)
                    {
                        MoonlayEntityExtension.FlagForCreate(entitas, identityService.Username, USER_AGENT);
                    }
                    foreach (var kemasan in model.kemasan)
                    {
                        MoonlayEntityExtension.FlagForCreate(kemasan, identityService.Username, USER_AGENT);
                    }
                    foreach (var kontainer in model.kontainer)
                    {
                        MoonlayEntityExtension.FlagForCreate(kontainer, identityService.Username, USER_AGENT);
                    }
                    foreach (var entitas in model.entitas)
                    {
                        MoonlayEntityExtension.FlagForCreate(entitas, identityService.Username, USER_AGENT);
                    }
                    foreach (var dokumen in model.dokumen)
                    {
                        MoonlayEntityExtension.FlagForCreate(dokumen, identityService.Username, USER_AGENT);
                    }
                    foreach (var pengangkut in model.pengangkut)
                    {
                        MoonlayEntityExtension.FlagForCreate(pengangkut, identityService.Username, USER_AGENT);
                    }
                    foreach (var bankDevisa in model.bankDevisa)
                    {
                        MoonlayEntityExtension.FlagForCreate(bankDevisa, identityService.Username, USER_AGENT);
                    }
                    foreach (var kesiapanBarang in model.kesiapanBarang)
                    {
                        MoonlayEntityExtension.FlagForCreate(kesiapanBarang, identityService.Username, USER_AGENT);
                    }

                    dbSet.Add(model);
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

        public async Task<int> UpdateAsync(int id, PEBHeader viewModel)
        {
            using (var transaction = context.Database.CurrentTransaction ?? context.Database.BeginTransaction())
            {
                try
                {
                    int Updated = 0;

                    PEBHeader data = await ReadById(id);


                    #region Delete
                    MoonlayEntityExtension.FlagForDelete(data, identityService.Username, USER_AGENT);
                    foreach (var barang in data.barang)
                    {
                        MoonlayEntityExtension.FlagForDelete(barang, identityService.Username, USER_AGENT);
                        foreach (var barangDokumen in barang.barangDokumen)
                        {
                            MoonlayEntityExtension.FlagForDelete(barangDokumen, identityService.Username, USER_AGENT);
                        }
                        foreach (var barangPemilik in barang.barangPemilik)
                        {
                            MoonlayEntityExtension.FlagForDelete(barangPemilik, identityService.Username, USER_AGENT);
                        }
                        foreach (var barangTarif in barang.barangTarif)
                        {
                            MoonlayEntityExtension.FlagForDelete(barangTarif, identityService.Username, USER_AGENT);
                        }
                    }

                    foreach (var entitas in data.entitas)
                    {
                        MoonlayEntityExtension.FlagForDelete(entitas, identityService.Username, USER_AGENT);
                    }
                    foreach (var kemasan in data.kemasan)
                    {
                        MoonlayEntityExtension.FlagForDelete(kemasan, identityService.Username, USER_AGENT);
                    }
                    foreach (var kontainer in data.kontainer)
                    {
                        MoonlayEntityExtension.FlagForDelete(kontainer, identityService.Username, USER_AGENT);
                    }
                    foreach (var entitas in data.entitas)
                    {
                        MoonlayEntityExtension.FlagForDelete(entitas, identityService.Username, USER_AGENT);
                    }
                    foreach (var dokumen in data.dokumen)
                    {
                        MoonlayEntityExtension.FlagForDelete(dokumen, identityService.Username, USER_AGENT);
                    }
                    foreach (var pengangkut in data.pengangkut)
                    {
                        MoonlayEntityExtension.FlagForDelete(pengangkut, identityService.Username, USER_AGENT);
                    }
                    foreach (var bankDevisa in data.bankDevisa)
                    {
                        MoonlayEntityExtension.FlagForDelete(bankDevisa, identityService.Username, USER_AGENT);
                    }
                    foreach (var kesiapanBarang in data.kesiapanBarang)
                    {
                        MoonlayEntityExtension.FlagForDelete(kesiapanBarang, identityService.Username, USER_AGENT);
                    }

                    #endregion

                    #region Insert
                    if (viewModel.isPosted)
                    {
                        viewModel.postedBy = identityService.Username;
                    }
                    MoonlayEntityExtension.FlagForCreate(viewModel, identityService.Username, USER_AGENT);
                    MoonlayEntityExtension.FlagForUpdate(viewModel, identityService.Username, USER_AGENT);
                    foreach (var barang in viewModel.barang)
                    {
                        MoonlayEntityExtension.FlagForCreate(barang, identityService.Username, USER_AGENT);
                        MoonlayEntityExtension.FlagForUpdate(barang, identityService.Username, USER_AGENT);
                        foreach (var barangDokumen in barang.barangDokumen)
                        {
                            MoonlayEntityExtension.FlagForCreate(barangDokumen, identityService.Username, USER_AGENT);
                            MoonlayEntityExtension.FlagForUpdate(barangDokumen, identityService.Username, USER_AGENT);
                        }
                        foreach (var barangPemilik in barang.barangPemilik)
                        {
                            MoonlayEntityExtension.FlagForCreate(barangPemilik, identityService.Username, USER_AGENT);
                            MoonlayEntityExtension.FlagForUpdate(barangPemilik, identityService.Username, USER_AGENT);
                        }
                        foreach (var barangTarif in barang.barangTarif)
                        {
                            MoonlayEntityExtension.FlagForCreate(barangTarif, identityService.Username, USER_AGENT);
                            MoonlayEntityExtension.FlagForUpdate(barangTarif, identityService.Username, USER_AGENT);
                        }
                        
                    }

                    foreach (var entitas in viewModel.entitas)
                    {
                        MoonlayEntityExtension.FlagForCreate(entitas, identityService.Username, USER_AGENT);
                        MoonlayEntityExtension.FlagForUpdate(entitas, identityService.Username, USER_AGENT);
                    }
                    foreach (var kemasan in viewModel.kemasan)
                    {
                        MoonlayEntityExtension.FlagForCreate(kemasan, identityService.Username, USER_AGENT);
                        MoonlayEntityExtension.FlagForUpdate(kemasan, identityService.Username, USER_AGENT);
                    }
                    foreach (var kontainer in viewModel.kontainer)
                    {
                        MoonlayEntityExtension.FlagForCreate(kontainer, identityService.Username, USER_AGENT);
                        MoonlayEntityExtension.FlagForUpdate(kontainer, identityService.Username, USER_AGENT);
                    }
                    foreach (var entitas in viewModel.entitas)
                    {
                        MoonlayEntityExtension.FlagForCreate(entitas, identityService.Username, USER_AGENT);
                        MoonlayEntityExtension.FlagForUpdate(entitas, identityService.Username, USER_AGENT);
                    }
                    foreach (var dokumen in viewModel.dokumen)
                    {
                        MoonlayEntityExtension.FlagForCreate(dokumen, identityService.Username, USER_AGENT);
                        MoonlayEntityExtension.FlagForUpdate(dokumen, identityService.Username, USER_AGENT);
                    }
                    foreach (var pengangkut in viewModel.pengangkut)
                    {
                        MoonlayEntityExtension.FlagForCreate(pengangkut, identityService.Username, USER_AGENT);
                        MoonlayEntityExtension.FlagForUpdate(pengangkut, identityService.Username, USER_AGENT);
                    }
                    foreach (var bankDevisa in viewModel.bankDevisa)
                    {
                        MoonlayEntityExtension.FlagForCreate(bankDevisa, identityService.Username, USER_AGENT);
                        MoonlayEntityExtension.FlagForUpdate(bankDevisa, identityService.Username, USER_AGENT);
                    }
                    foreach (var kesiapanBarang in viewModel.kesiapanBarang)
                    {
                        MoonlayEntityExtension.FlagForCreate(kesiapanBarang, identityService.Username, USER_AGENT);
                        MoonlayEntityExtension.FlagForUpdate(kesiapanBarang, identityService.Username, USER_AGENT);

                    }
                    dbSet.Add(viewModel);
                    #endregion

                    Updated = await context.SaveChangesAsync();
                    transaction.Commit();

                    return Updated;
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public string GenerateBCID()
        {
            var dateNow = new DateTime();
            var year = dateNow.ToString("yy");
            var month = dateNow.ToString("MM");
            var day = dateNow.ToString("dd");
            var hours = dateNow.ToString("hh");
            var minute = dateNow.ToString("mm");
            var second = dateNow.ToString("ss");
            var milisec = dateNow.ToString("ffffff");

            var BCId = "BC" + year + month + day + hours + minute + second + milisec;
            return BCId;

        }

        public async Task<int> PostToSupportPEB(int id, PEBViewModelList viewModel)
        {
            using (var transaction = context.Database.CurrentTransaction ?? context.Database.BeginTransaction())
            {
                try
                {
                    int Created = 0;

                    PEBHeader data = await ReadById(id);

                    data.tanggalDaftar = DateTime.Parse(viewModel.tanggalDaftar);
                    data.nomorDaftar = viewModel.nomorDaftar;

                    var Query = from a in context.PEBHeader
                                join b in context.PEBBarang on a.Id equals b.IdHeader
                                join c in context.PEBDokumen on a.Id equals c.IdHeader
                                join d in context.PEBEntitas on a.Id equals d.IdHeader
                                join e in context.PEBKemasan on a.Id equals e.IdHeader
                                where d.kodeEntitas == "6" && a.Id == id
                                select new SupportPEBViewModel
                                {
                                    BCType = a.kodeDokumen == "30" ? "BC 3.0" : a.kodeDokumen,
                                    BCNo = viewModel.nomorDaftar,
                                    CAR = a.nomorAju,
                                    BCDate = DateTime.Parse(viewModel.tanggalDaftar),
                                    ExpenditureNo = c.nomorDokumen,
                                    ExpenditureDate = c.tanggalDokumen,
                                    BuyerCode = "-",
                                    BuyerName = d.namaEntitas,
                                    Netto = a.netto,
                                    Bruto = a.bruto,
                                    Pack = e.kodeJenisKemasan,
                                    CreateUser = identityService.Username,
                                    CreateDate = DateTime.Now,
                                    UpdateUser = identityService.Username,
                                    UpdateDate = DateTime.Now,
                                    Vendor = d.namaEntitas,
                                    ItemCode = "-",
                                    ItemName = b.kodeBarang + " " + b.uraian + " " + b.merk,
                                    UnitQtyCode = b.kodeSatuanBarang,
                                    Quantity = b.jumlahSatuan,
                                    Price = b.fob,
                                    CurrencyCode = a.kodeValuta,
                                    UomUnit = b.kodeSatuanBarang,
                                };


                    //Input Header
                    var genBCid = GenerateBCID();
                    var InputBC_ADDED = Query.Select(x => new BEACUKAI_ADDED
                    {
                        BCId = genBCid,
                        BCType = x.BCType,
                        BCNo = x.BCNo,
                        CAR = x.CAR,
                        BCDate = x.BCDate,
                        ExpenditureNo = x.ExpenditureNo,
                        ExpenditureDate = x.ExpenditureDate,
                        BuyerCode = x.BuyerCode,
                        BuyerName = x.BuyerName,
                        Netto = Convert.ToDouble(x.Netto),
                        Bruto = Convert.ToDouble(x.Bruto),
                        Pack = x.Pack,
                        CreateUser = x.CreateUser,
                        CreateDate = x.CreateDate,
                        UpdateUser = x.UpdateUser,
                        UpdateDate = x.UpdateDate,
                    }).First();
                    //bEACUKAI_ADDEDs.Add(InputBC_ADDED);

                    //Input Item
                    var idDetail = 1;
                    foreach(var a in Query)
                    {
                        var item = new BEACUKAI_ADDED_DETAIL
                        {
                            DetailBCId = genBCid + idDetail.ToString().PadLeft(3,'0'),
                            BCId = genBCid,
                            ItemCode = a.ItemCode,
                            ItemName = a.ItemName,
                            UnitQtyCode = a.UnitQtyCode,
                            Quantity =a.Quantity,
                            Price = Convert.ToDecimal(a.Price),
                            CurrencyCode = a.CurrencyCode,
                            UomUnit = a.UomUnit,
                        };
                        //bEACUKAI_ADDED_DETAILs.Add(item);
                        InputBC_ADDED.Items.Add(item);
                        idDetail++;
                    }

                    bEACUKAI_ADDEDs.Add(InputBC_ADDED);
                    Created = await context.SaveChangesAsync();
                    transaction.Commit();

                    return Created;
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public async Task<int> Delete(int id)
        {
            int Deleted = 0;

            using (var transaction = context.Database.CurrentTransaction ?? context.Database.BeginTransaction())
            {
                try
                {
                    PEBHeader data = await ReadById(id);

                    MoonlayEntityExtension.FlagForDelete(data, identityService.Username, USER_AGENT);
                    foreach (var barang in data.barang)
                    {
                        MoonlayEntityExtension.FlagForDelete(barang, identityService.Username, USER_AGENT);
                        foreach (var barangDokumen in barang.barangDokumen)
                        {
                            MoonlayEntityExtension.FlagForDelete(barangDokumen, identityService.Username, USER_AGENT);
                        }
                        foreach (var barangPemilik in barang.barangPemilik)
                        {
                            MoonlayEntityExtension.FlagForDelete(barangPemilik, identityService.Username, USER_AGENT);
                        }
                        foreach (var barangTarif in barang.barangTarif)
                        {
                            MoonlayEntityExtension.FlagForDelete(barangTarif, identityService.Username, USER_AGENT);
                        }
                    }

                    foreach (var entitas in data.entitas)
                    {
                        MoonlayEntityExtension.FlagForDelete(entitas, identityService.Username, USER_AGENT);
                    }
                    foreach (var kemasan in data.kemasan)
                    {
                        MoonlayEntityExtension.FlagForDelete(kemasan, identityService.Username, USER_AGENT);
                    }
                    foreach (var kontainer in data.kontainer)
                    {
                        MoonlayEntityExtension.FlagForDelete(kontainer, identityService.Username, USER_AGENT);
                    }
                    foreach (var entitas in data.entitas)
                    {
                        MoonlayEntityExtension.FlagForDelete(entitas, identityService.Username, USER_AGENT);
                    }
                    foreach (var dokumen in data.dokumen)
                    {
                        MoonlayEntityExtension.FlagForDelete(dokumen, identityService.Username, USER_AGENT);
                    }
                    foreach (var pengangkut in data.pengangkut)
                    {
                        MoonlayEntityExtension.FlagForDelete(pengangkut, identityService.Username, USER_AGENT);
                    }
                    foreach (var bankDevisa in data.bankDevisa)
                    {
                        MoonlayEntityExtension.FlagForDelete(bankDevisa, identityService.Username, USER_AGENT);
                    }
                    foreach (var kesiapanBarang in data.kesiapanBarang)
                    {
                        MoonlayEntityExtension.FlagForDelete(kesiapanBarang, identityService.Username, USER_AGENT);
                    }
                    Deleted = await context.SaveChangesAsync();
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }

            }
            return Deleted;
        }
        public async Task<PEBViewModelList> ReadByIdToPushAsync(long id, string auth)
        {
            PEBViewModelList Query = dbSet.Where(s => s.Id == id && s._IsDeleted == false).Select(m => new PEBViewModelList
            {
                Id = m.Id,
                nomorAju = m.nomorAju,
                tanggalAju = m.tanggalAju.ToString("dd-MMM-yyyy"),
                nomorDaftar = string.IsNullOrWhiteSpace(m.nomorDaftar) ? "-" : m.nomorDaftar,
                tanggalDaftar = m.tanggalDaftar == null ? "-" : m.tanggalDaftar.Value.ToString("dd-MMM-yyyy"),
                namaPenerima = m.entitas.Where(x => x.kodeEntitas == "8").Select(i => i.namaEntitas).FirstOrDefault(),
                isPosted = m.isPosted,
                postedBy = string.IsNullOrWhiteSpace(m.postedBy) ? "-" : m.postedBy,
                CreatedDate = m._CreatedUtc.ToString("dd-MMM-yyyy")
            }).OrderByDescending(x => x.nomorAju).First();

            //var token = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJySDN6VVB6ZGFqT2E4WEdQeVlsNVloaWRFblpFZ1hSVlVJc3BaSEthY0xVIn0.eyJleHAiOjE2NzgzMjk1MjAsImlhdCI6MTY3ODMyODYyMCwianRpIjoiYWU5ZjI4Y2YtZTdiZC00MjlmLWI5ODktOGMxNDZhZWEyNmM3IiwiaXNzIjoiaHR0cDovL2FjY291bnQtZGV2LmJlYWN1a2FpLmdvLmlkL2F1dGgvcmVhbG1zL21hc3RlciIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiIyODY3M2E2Zi1jZTQ3LTQ4NDQtOTRiNy1mNTcyMzUyZjY2MTMiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJzbWFydF9jdXN0b20iLCJzZXNzaW9uX3N0YXRlIjoiNzk0ZDdmZTEtYzhjMy00OWFlLWE0NjEtNWQ5NGZmZTE1MTBkIiwiYWNyIjoiMSIsImFsbG93ZWQtb3JpZ2lucyI6WyIqIiwiaHR0cDovL2xvY2FsaG9zdDozMDAwIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCBvZmZsaW5lX2FjY2VzcyIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwiaWRlbnRpdGFzIjoiMDExMzk5MDc4NTMyMDAxIiwibmFtZSI6IlBULiBEQU4gTElSSVMiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJkbGRldjIwMjIiLCJnaXZlbl9uYW1lIjoiUFQuIERBTiBMSVJJUyIsImZhbWlseV9uYW1lIjoiIiwiZW1haWwiOiJyaWFuLmF0eWFzYWJ1ZGlAZGFubGlyaXMuY29tIn0.SLl9rtrdxBdB8BXx0UUyn4deBjxQbxiBU3Ir-fNxIO-Lh0noJVWWns8DwUWBzs7ydLDrdCUetIyycwP5oc2zJi2nHD37b6apyB_70cFF4q0FYezOElTQ0FgIcedH2Al0AdjsF4f4QR4eQ4NtaZ9ZuAGo1yv0FjDluN-N4S_i88LDQE99-VEd3UON_t7IhJCZ7YtFHyXAMenG8zcgtshwU4IpkRj1ja5Qrj7a_BlA8ebNCh4AXQWf1MJ-etcwc_AuZ2iS629oimdaFZruQKhAH_l-i99fB4StQ8CMK14bMJvXcLm1kffC0ZGUvZ_dTIqjmA46pfRy3WQCrO8nhNjtYA";
            //var aju = "3010178889G220220525000099";
            //PRD
            var Respon = await CeisaService.GetRespon(Query.nomorAju, auth);

            //var Respon =  CeisaService.GetRespon(aju, auth).Result;
          
            if (Respon != null)
            {
                var ResponDaftar = Respon.Where(x => x.nomorDaftar != null).FirstOrDefault();
                Query.nomorDaftar = ResponDaftar.nomorDaftar;
                Query.tanggalDaftar = ResponDaftar.tanggalDaftar.ToString();
            }

            return Query;
        }

        public Task<MemoryStream> GetExcel(string noAju)
        {
            throw new NotImplementedException();
        }
    }
}
