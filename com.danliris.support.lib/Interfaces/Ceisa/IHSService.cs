using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.Models.Ceisa.Master.HS;
using com.danliris.support.lib.ViewModel.Ceisa.Master.HSViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace com.danliris.support.lib.Interfaces.Ceisa
{
    public interface IHSService
    {
        Task<int> Create(HSModel pEBHeader);
        ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        Task<HSModel> ReadById(long id);
        Task<int> UpdateAsync(int id, HSViewModel viewModel);
        Task<int> Delete(int id);
    }
}
