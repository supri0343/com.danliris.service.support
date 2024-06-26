using System;
using System.Collections.Generic;
using System.Text;
using com.danliris.support.lib.Models.Ceisa.PEB;
using System.Threading.Tasks;
using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.ViewModel.Ceisa;
using System.IO;

namespace com.danliris.support.lib.Interfaces
{
    public interface IPEBService
    {
        Task<int> Create(PEBHeader pEBHeader);
        ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        string Urut(string tipe);
        Task<PEBHeader> ReadById(long id);
        Task<int> UpdateAsync(int id, PEBHeader viewModel);
        Task<PEBViewModelList> ReadByIdToPushAsync(long id, string auth);
        Task<int> PostToSupportPEB(int id, PEBViewModelList viewModel);
        Task<int> Delete(int id);
        Task<MemoryStream> GetExcel(string noAju);


    }
}
