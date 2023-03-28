using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.Interfaces.Ceisa;
using com.danliris.support.lib.Models;
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
        GenerateBPNo GenerateBPNo = new GenerateBPNo();

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
        public async Task<int> UpdateAsync(int id, TPBHeader viewModel)
        {
            int Updated = 0;
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    TPBHeader data = await ReadById(id);

                    #region Delete
                    MoonlayEntityExtension.FlagForDelete(data, identityService.Username, USER_AGENT);
                    foreach (var barang in data.barang)
                    {
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
                    foreach (var pungutan in data.pungutan)
                    {
                        MoonlayEntityExtension.FlagForDelete(pungutan, identityService.Username, USER_AGENT);
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
                    foreach (var pungutan in viewModel.pungutan)
                    {
                        MoonlayEntityExtension.FlagForCreate(pungutan, identityService.Username, USER_AGENT);
                        MoonlayEntityExtension.FlagForUpdate(pungutan, identityService.Username, USER_AGENT);
                    }
                   

                    #endregion
                    dbSet.Add(viewModel);

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
        public async Task<int> Delete(int id)
        {
            int Deleted = 0;

            using (var transaction = context.Database.CurrentTransaction ?? context.Database.BeginTransaction())
            {
                try
                {
                    TPBHeader data = await ReadById(id);

                    MoonlayEntityExtension.FlagForDelete(data, identityService.Username, USER_AGENT);
                    foreach (var barang in data.barang)
                    {
                      
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
                    foreach (var pungutan in data.pungutan)
                    {
                        MoonlayEntityExtension.FlagForDelete(pungutan, identityService.Username, USER_AGENT);
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

        public async Task<int> PostToSupportTPB(int id, TPBViewModelList viewModel)
        {
            using (var transaction = context.Database.CurrentTransaction ?? context.Database.BeginTransaction())
            {
                try
                {
                    int Created = 0;

                    TPBHeader data = await ReadById(id);

                    data.tanggalDaftar = DateTime.Parse(viewModel.tanggalDaftar);
                    data.nomorDaftar = viewModel.nomorDaftar;
                    context.SaveChangesAsync();
                    var ID = await GenerateBPNo.GenerateNo();

                    var QueryBarang = from a in context.TPBHeader
                                join b in context.TPBBarang on a.Id equals b.IdHeader
                                join d in context.TPBEntitas on a.Id equals d.IdHeader          
                                where a.Id == id
                                select new BEACUKAI_TEMP
                                {
                                    ID = 0,
                                    BCId = ID,
                                    BCNo = viewModel.nomorDaftar,
                                    Barang = b.uraian,
                                    Bruto = Convert.ToInt32(a.bruto),
                                    CIF = Convert.ToInt32(a.cif),
                                    CIF_Rupiah = Convert.ToInt32(b.cifRupiah),
                                    JumlahSatBarang = Convert.ToInt32(b.jumlahSatuan),
                                    KodeBarang = b.kodeBarang,
                                    KodeKemasan = "",
                                    NamaKemasan = "",
                                    Netto = (decimal)a.netto,
                                    NoAju = a.nomorAju,
                                    NamaSupplier = d.kodeEntitas == "9" ? d.namaEntitas : "",
                                    TglDaftarAju = a.tanggalAju,
                                    TglBCNo = DateTime.Parse(viewModel.tanggalDaftar),
                                    Valuta = "IDR",
                                    JenisBC = "BC " + a.kodeDokumen,
                                    IDHeader = (int)a.Id,
                                    JenisDokumen = null,
                                    NomorDokumen = null,
                                    TanggalDokumen = null,
                                    JumlahBarang = context.TPBBarang.Where(x => x.IdHeader == id).Count(),
                                    Sat = b.kodeSatuanBarang,
                                    KodeSupplier = d.nomorIdentitas,
                                    TglDatang = viewModel.tanggalDatang.Value,
                                    CreatedBy = identityService.Username,
                                    Vendor = d.kodeEntitas == "7" ? d.namaEntitas : "",
                                    Hari = DateTime.Today
                                };

                    var QueryDokumen = from a in context.TPBHeader
                                      join c in context.TPBDokumen on a.Id equals c.IdHeader
                                      join d in context.TPBEntitas on a.Id equals d.IdHeader
                                      join e in context.BeacukaiDocuments on c.kodeDokumen equals e.Code.ToString()
                                      where a.Id == id
                                      select new BEACUKAI_TEMP
                                      {
                                          ID = 0,
                                          BCId = ID,
                                          BCNo = viewModel.nomorDaftar,
                                          Barang = null,
                                          Bruto = (decimal)a.bruto,
                                          CIF = (int)a.cif,
                                          CIF_Rupiah = null,
                                          JumlahSatBarang = null,
                                          KodeBarang = null,
                                          KodeKemasan = "",
                                          NamaKemasan = "",
                                          Netto = (decimal)a.netto,
                                          NoAju = a.nomorAju,
                                          NamaSupplier = d.kodeEntitas == "9" ? d.namaEntitas : "",
                                          TglDaftarAju = a.tanggalAju,
                                          TglBCNo = DateTime.Parse(viewModel.tanggalDaftar),
                                          Valuta = "IDR",
                                          JenisBC = "BC " + a.kodeDokumen,
                                          IDHeader = (int)a.Id,
                                          JenisDokumen = e.Name,
                                          NomorDokumen = c.nomorDokumen,
                                          TanggalDokumen = c.tanggalDokumen,
                                          JumlahBarang = context.TPBBarang.Where(x => x.IdHeader == id).Count(),
                                          Sat = null,
                                          KodeSupplier = d.nomorIdentitas,
                                          TglDatang = viewModel.tanggalDatang.Value,
                                          CreatedBy = identityService.Username,
                                          Vendor = d.kodeEntitas == "7" ? d.namaEntitas : "",
                                          Hari = DateTime.Today
                                      };


                    var lastNo = context.BeacukaiTemp.Select(x => x.ID).OrderByDescending(x => x).Take(1).ToArray();
                    var index = 1;
                    //foreach (var a in Query)
                    //{
                    //    a.ID = Convert.ToInt32(lastNo[0] + index);

                    //    index++;
                    //}

                    return Created;
                }
                
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }
    }
}
