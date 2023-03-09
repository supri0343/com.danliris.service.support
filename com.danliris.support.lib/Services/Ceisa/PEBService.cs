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

namespace com.danliris.support.lib.Services.Ceisa
{
    public class PEBService : IPEBService
    {
        private readonly string USER_AGENT = "Service";
        private readonly SupportDbContext context;
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;
        private readonly DbSet<PEBHeader> dbSet;

        public PEBService(SupportDbContext context, IServiceProvider serviceProvider)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
            dbSet = context.Set<PEBHeader>();
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<PEBViewModelList> Query = dbSet.Where(s => s.kodeDokumen == "30" && s._IsDeleted == false).Select(m => new PEBViewModelList
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
            }).OrderByDescending(x => x.nomorAju) ;
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

        
    }
}
