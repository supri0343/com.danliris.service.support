using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.Interfaces.Ceisa;
using com.danliris.support.lib.Models;
using com.danliris.support.lib.Models.Ceisa.TPB;
using com.danliris.support.lib.ViewModel.Ceisa;
using com.danliris.support.lib.ViewModel.Ceisa.TPBViewModel;
using Com.DanLiris.Service.Purchasing.Lib.Helpers;
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
        private readonly DbSet<BEACUKAI_TEMP> dbSetBeacukaiTemp;
        GenerateBPNo GenerateBPNo = new GenerateBPNo();

        public BC40Service(SupportDbContext context, IServiceProvider serviceProvider)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
            dbSet = context.Set<TPBHeader>();
            dbSetBeacukaiTemp = context.Set<BEACUKAI_TEMP>();
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
                namaPenerima = m.entitas.Where(x => x.kodeEntitas == "9").Select(i => i.namaEntitas).FirstOrDefault(),
                isPosted = m.isPosted,
                postedBy = string.IsNullOrWhiteSpace(m.postedBy) ? "-" : m.postedBy,
                CreatedDate = m._CreatedUtc.ToString("dd-MMM-yyyy")
            }).OrderByDescending(x => x.nomorAju);

            //add filter search keyword
            List<string> SearchAtt = new List<string>() { "namaPenerima", "nomorAju", "nomorDaftar" };

            Query = QueryHelper<TPBViewModelList>.ConfigureSearch(Query, SearchAtt, Keyword);
            //--------


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
                        MoonlayEntityExtension.FlagForDelete(barang, identityService.Username, USER_AGENT);
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
                        MoonlayEntityExtension.FlagForDelete(barang, identityService.Username, USER_AGENT);
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
                bool itsOK = false;
                try
                {
                    int Created = 0;
                    var existAju = context.BeacukaiTemp.Select(x => x.NoAju).Distinct().ToList();
                  
                    if (existAju.Contains(viewModel.nomorAju)) 
                    {
                        itsOK = true;
                        TPBHeader data = await ReadById(id);
                        data.tanggalDaftar = DateTime.Parse(viewModel.tanggalDaftar);
                        data.nomorDaftar = viewModel.nomorDaftar;
                        await context.SaveChangesAsync();
                        transaction.Commit();
                        throw new Exception($"Data dengan No Aju - {viewModel.nomorAju} - tidak disimpan karena sudah ada di database.");
                    } else
                    {
                        TPBHeader data = await ReadById(id);

                        data.tanggalDaftar = DateTime.Parse(viewModel.tanggalDaftar);
                        data.nomorDaftar = viewModel.nomorDaftar;
                        await context.SaveChangesAsync();
                        var ID = await GenerateBPNo.GenerateNo();

                        var QueryBarang = from a in context.TPBHeader
                                          join b in context.TPBBarang on a.Id equals b.IdHeader
                                          //join d in context.TPBEntitas on a.Id equals d.IdHeader          
                                          where a.Id == id && a._IsDeleted == false && b._IsDeleted == false

                                          select new BEACUKAI_TEMP
                                          {
                                              ID = 0,
                                              BCId = ID,
                                              BCNo = viewModel.nomorDaftar,
                                              Barang = b.uraian,
                                              Bruto = Convert.ToDecimal(a.bruto),
                                              CIF = Convert.ToDecimal(a.cif),
                                              CIF_Rupiah = Convert.ToDecimal(b.hargaPenyerahan),
                                              JumlahSatBarang = Convert.ToDecimal(b.jumlahSatuan),
                                              KodeBarang = b.kodeBarang,
                                              KodeKemasan = null,
                                              NamaKemasan = null,
                                              Netto = Convert.ToDecimal(a.netto),
                                              NoAju = a.nomorAju,
                                              //NamaSupplier = d.kodeEntitas == "9" ? d.namaEntitas : "",
                                              TglDaftarAju = a.tanggalAju,
                                              TglBCNo = DateTime.Parse(viewModel.tanggalDaftar),
                                              Valuta = "IDR",
                                              JenisBC = "BC " + a.kodeDokumen,
                                              IDHeader = a.Id,
                                              JenisDokumen = null,
                                              NomorDokumen = null,
                                              TanggalDokumen = null,
                                              JumlahBarang = context.TPBBarang.Where(x => x.IdHeader == id).Count(),
                                              Sat = b.kodeSatuanBarang,
                                              //KodeSupplier = d.nomorIdentitas,
                                              TglDatang = viewModel.tanggalDatang.Value,
                                              CreatedBy = identityService.Username,
                                              //Vendor = d.kodeEntitas == "7" ? d.namaEntitas : "",
                                              Hari = DateTime.Today
                                          };

                        var QueryDokumen = from a in context.TPBHeader
                                           join c in context.TPBDokumen on a.Id equals c.IdHeader
                                           //join d in context.TPBEntitas on a.Id equals d.IdHeader
                                           join e in context.BeacukaiDocuments on c.kodeDokumen equals e.Code.ToString()
                                           where a.Id == id && a._IsDeleted == false && c._IsDeleted == false
                                           select new BEACUKAI_TEMP
                                           {
                                               ID = 0,
                                               BCId = ID,
                                               BCNo = viewModel.nomorDaftar,
                                               Barang = null,
                                               Bruto = Convert.ToDecimal(a.bruto),
                                               CIF = null,
                                               CIF_Rupiah = null,
                                               JumlahSatBarang = null,
                                               KodeBarang = null,
                                               KodeKemasan = null,
                                               NamaKemasan = null,
                                               Netto = Convert.ToDecimal(a.netto),
                                               NoAju = a.nomorAju,
                                               //NamaSupplier = d.kodeEntitas == "9" ? d.namaEntitas : "",
                                               TglDaftarAju = a.tanggalAju,
                                               TglBCNo = DateTime.Parse(viewModel.tanggalDaftar),
                                               Valuta = "IDR",
                                               JenisBC = "BC " + a.kodeDokumen,
                                               IDHeader = a.Id,
                                               JenisDokumen = e.Name,
                                               NomorDokumen = c.nomorDokumen,
                                               TanggalDokumen = c.tanggalDokumen,
                                               JumlahBarang = context.TPBBarang.Where(x => x.IdHeader == id).Count(),
                                               Sat = null,
                                               //KodeSupplier = d.nomorIdentitas,
                                               TglDatang = viewModel.tanggalDatang.Value,
                                               CreatedBy = identityService.Username,
                                               //Vendor = d.kodeEntitas == "7" ? d.namaEntitas : "",
                                               Hari = DateTime.Today
                                           };

                        var DataToPost = QueryBarang.Concat(QueryDokumen);

                        TPBEntitas Supplier = new TPBEntitas();
                        TPBEntitas Vendor = new TPBEntitas();

                        switch (data.kodeDokumen)
                        {
                            case "23":
                                Supplier = context.TPBEntitas.Where(x => x.IdHeader == id && x.kodeEntitas == "5").First();
                                Vendor = context.TPBEntitas.Where(x => x.IdHeader == id && x.kodeEntitas == "3").First();
                                break;
                            case "40":
                                Supplier = context.TPBEntitas.Where(x => x.IdHeader == id && x.kodeEntitas == "9").First();
                                Vendor = context.TPBEntitas.Where(x => x.IdHeader == id && x.kodeEntitas == "7").First();
                                break;
                            case "261":
                                Supplier = context.TPBEntitas.Where(x => x.IdHeader == id && x.kodeEntitas == "3").First();
                                Vendor = context.TPBEntitas.Where(x => x.IdHeader == id && x.kodeEntitas == "8").First();
                                break;
                            case "262":
                                Supplier = context.TPBEntitas.Where(x => x.IdHeader == id && x.kodeEntitas == "9").First();
                                //Vendor = context.TPBEntitas.Where(x => x.IdHeader == id && x.kodeEntitas == "8").First();
                                break;
                            case "25":
                                Supplier = context.TPBEntitas.Where(x => x.IdHeader == id && x.kodeEntitas == "3").First();
                                Vendor = context.TPBEntitas.Where(x => x.IdHeader == id && x.kodeEntitas == "8").First();
                                break;

                        }
                        
                        var lastNo = context.BeacukaiTemp.Select(x => x.ID).OrderByDescending(x => x).Take(1).ToList().First();
                        var index = 1;
                        foreach (var a in DataToPost)
                        {
                            a.ID = Convert.ToInt64(lastNo + index);
                            a.NamaSupplier = Supplier.namaEntitas;
                            a.KodeSupplier = Supplier.nomorIdentitas;
                            a.Vendor = Vendor.namaEntitas;

                            context.BeacukaiTemp.Add(a);
                            //await context.SaveChangesAsync();
                            index++;
                        }

                        Created = await context.SaveChangesAsync();
                        transaction.Commit();
                    }
                   
                    return Created;
                }
                
                catch (Exception e)
                {
                    if (itsOK != true)
                    {
                        transaction.Rollback();
                    }
                    
                    throw e;
                }
            }
        }
    }
}
