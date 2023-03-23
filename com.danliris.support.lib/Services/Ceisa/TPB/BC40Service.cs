using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.Interfaces.Ceisa;
using com.danliris.support.lib.Models.Ceisa.TPB;
using com.danliris.support.lib.ViewModel.Ceisa.TPBViewModel;
using Com.DanLiris.Service.support.lib.Services;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.danliris.support.lib.Services.Ceisa.TPB
{
    public class BC40Service : IBC40
    {
        private readonly string USER_AGENT = "Service";
        private readonly SupportDbContext context;
        private readonly IServiceProvider serviceProvider                                                                                                       ;
        private readonly IdentityService identityService;
        private readonly DbSet<TPBHeader> dbSet;

        public BC40Service(SupportDbContext context, IServiceProvider serviceProvider)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
            dbSet = context.Set<TPBHeader>();
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<TPBViewModelList> Query = dbSet.Where(s => s.kodeDokumen == "40" && s._IsDeleted == false).Select(m => new TPBViewModelList
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
            }).OrderByDescending(x => x.nomorAju);


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
        public string Urut()
        {
            var Query = dbSet.Where(s => s.kodeDokumen == "40" && s._IsDeleted == false).OrderByDescending(x => x.nomorAju).Select(x => new { x.nomorAju }).FirstOrDefault();
            var NoUrut = "";
            if (Query != null)
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
        public async Task<int> Create(TPBHeader model)
        {
            int Created = 0;
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    MoonlayEntityExtension.FlagForCreate(model, identityService.Username, USER_AGENT);
                    foreach (var barang in model.barang)
                    {
                        MoonlayEntityExtension.FlagForCreate(barang, identityService.Username, USER_AGENT);
                        foreach (var barangTarif in barang.barangTarif)
                        {
                            MoonlayEntityExtension.FlagForCreate(barangTarif, identityService.Username, USER_AGENT);
                        }
                    }

                    foreach (var entitas in model.entitas)
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
                    foreach (var pungutan in model.pungutan)
                    {
                        MoonlayEntityExtension.FlagForCreate(pungutan, identityService.Username, USER_AGENT);
                    }
                   

                    dbSet.Add(model);
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
        public async Task<TPBHeader> ReadById(long id)
        {
            var model = await dbSet.Where(m => m.Id == id)
                .Include(m => m.barang)
                .ThenInclude(x => x.barangTarif)
                .Include(m => m.entitas)
                .Include(m => m.kemasan)
                .Include(m => m.kontainer)
                .Include(m => m.dokumen)
                .Include(m => m.pengangkut)
                .Include(m => m.pungutan)
                .FirstOrDefaultAsync();

            return model;
        }
    }
}
