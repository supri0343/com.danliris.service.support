using com.danliris.support.lib.Interfaces.Ceisa;
using com.danliris.support.lib.Interfaces.Ceisa.TPB;
using com.danliris.support.lib.Models.Ceisa.TPB;
using com.danliris.support.lib.ViewModel.Ceisa.TPBViewModel;
using Com.DanLiris.Service.support.lib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.danliris.support.lib.Services.Ceisa.TPB
{
    public class TPBService :ITPBService
    {
        private readonly string USER_AGENT = "Service";
        private readonly SupportDbContext context;
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;
        private ICeisaService CeisaService;
        private readonly DbSet<TPBHeader> dbSet;

        public TPBService(SupportDbContext context, IServiceProvider serviceProvider, ICeisaService ceisaService)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
            this.CeisaService = ceisaService;
            dbSet = context.Set<TPBHeader>();
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        public async Task<TPBViewModelList> ReadByIdToPushAsync(long id, string auth)
        {
            TPBViewModelList Query = dbSet.Where(s => s.Id == id && s._IsDeleted == false).Select(m => new TPBViewModelList
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
            var ResponDaftar = Respon.Where(x => x.nomorDaftar != null).FirstOrDefault();
            if (ResponDaftar != null)
            {
                Query.nomorDaftar = ResponDaftar.nomorDaftar;
                Query.tanggalDaftar = ResponDaftar.tanggalDaftar.ToString();
            }

            return Query;
        }

    }
}
