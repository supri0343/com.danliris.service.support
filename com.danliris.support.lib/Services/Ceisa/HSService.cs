using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.Interfaces.Ceisa;
using com.danliris.support.lib.Models.Ceisa.Master.HS;
using com.danliris.support.lib.ViewModel.Ceisa.Master.HSViewModel;
using Com.DanLiris.Service.Purchasing.Lib.Helpers;
using Com.DanLiris.Service.support.lib.Services;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace com.danliris.support.lib.Services.Ceisa
{
    public class HSService : IHSService
    {
        private readonly string USER_AGENT = "Service";
        private readonly SupportDbContext context;
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;
        private readonly DbSet<HSModel> dbSet;

        public HSService(SupportDbContext context, IServiceProvider serviceProvider)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
            dbSet = context.Set<HSModel>();
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<HSModel> Query = dbSet.Where(x => x._IsDeleted == false);


            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<HSModel>.ConfigureFilter(Query, FilterDictionary);

            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "HSNo", "HSRemark"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<HSModel>.ConfigureOrder(Query, OrderDictionary);


            Pageable<HSModel> pageable = new Pageable<HSModel>(Query, Page - 1, Size);
            List<HSModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            List<object> ListData = new List<object>();
            ListData.AddRange(Data.Select(s => new
            {
                s.Id,
                s.HSNo,
                s.HSRemark,
            }));

            return new ReadResponse<object>(ListData, TotalData, null);
        }

        public async Task<HSModel> ReadById(long id)
        {
            //PEBHeaderViewModel data = new PEBHeaderViewModel();
            var model = await dbSet.Where(m => m.Id == id)
                .FirstOrDefaultAsync();

            return model;
        }

        public async Task<int> Create(HSModel model)
        {
            int Created = 0;
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    MoonlayEntityExtension.FlagForCreate(model, identityService.Username, USER_AGENT);

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

        public async Task<int> UpdateAsync(int id, HSViewModel viewModel)
        {
            using (var transaction = context.Database.CurrentTransaction ?? context.Database.BeginTransaction())
            {
                try
                {
                    int Updated = 0;

                    HSModel data = await ReadById(id);

                    if (data.HSNo != viewModel.hsNo)
                    {
                        data.HSNo = viewModel.hsNo;
                    }

                    if (data.HSRemark != viewModel.hsRemark)
                    {
                        data.HSRemark = viewModel.hsRemark;
                    }

                    MoonlayEntityExtension.FlagForUpdate(data, identityService.Username, USER_AGENT);

                    Updated = await context.SaveChangesAsync();
                    transaction.Commit();

                    return Updated;
                }
                catch (Exception e)
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

                    HSModel data = await ReadById(id);

                    MoonlayEntityExtension.FlagForDelete(data, identityService.Username, USER_AGENT);
                    Deleted = await context.SaveChangesAsync();
                    transaction.Commit();

                    return Deleted;
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
