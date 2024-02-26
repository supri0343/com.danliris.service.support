using com.danliris.support.lib.Models;
using com.danliris.support.lib.Models.Machine;
using com.danliris.support.lib.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace com.danliris.support.lib.Interfaces
{
    public interface IMachineService 
    {
        List<MachineBrand> GetMachineBrand(int size, string keyword);
        Task<int> CreateMachineBrand(MachineBrand machineBrand,string username);
        List<MachineCategory> GetMachineCategory(int size, string keyword);
        Task<int> CreateMachineCategory(MachineCategory machineBrand, string username);
        List<Machine> GetMachine(int Page = 1, int Size = 25, string Keyword = null, string Filter = "{}");
        List<MachineTypes> GetMachineByBrand(int Page = 1, int Size = 25, string Keyword = null, string Filter = "{}");
        List<Machine> GetMachines(string tipe, string ctg, string serial);
        Machine GetMachineById(string id);
        Task<int> UpdateMachine(string id, Machine machine, string username);
        Task<int> CreateMachineAndMutation(MachinesMutation machinesMutation, string username);
        List<MutationMachine> GetMachineMutation(string tipe, string ctg, string serial);
        MutationMachine GetMutationById(Guid id);
        Task<int> UpdateMutation(Guid id, MachineMutation mutation, string username);
        MemoryStream GetXlsMachineMutation(string tipe, string ctg, string serial);
        MemoryStream GetXlsMachine(string tipe, string ctg, string serial);
    }
}
