using System.Threading.Tasks;
using System.Collections.Generic;
using com.danliris.support.lib.ViewModel.Ceisa;
using static com.danliris.support.lib.Services.Ceisa.CeisaService;
using System;

namespace com.danliris.support.lib.Interfaces.Ceisa
{
    public interface ICeisaService
    {
        Task<List<RateValutaViewModel>> GetValutaRate(string kode, string token);
        Task<List<LartasViewModel>> GetLartas(string kode, string token);
        Task<ManifesBC11ViewModel> GetManifestBC11(string kodeKantor, string noHostBl, DateTime tglHostBl, string token);
        Task<List<ResponViewModel>> GetRespon(string kode, string token);
        Task<List<LartasViewModel>> GetTarifHS(string kode, string token);
    }
}
