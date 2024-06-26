using com.danliris.support.lib.ViewModel.Ceisa.TPBViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace com.danliris.support.lib.Interfaces.Ceisa.TPB
{
    public interface ITPBService 
    {
        Task<TPBViewModelList> ReadByIdToPushAsync(long id, string auth);
        Task<MemoryStream> GetExcel(string noAju);
    }
}
