using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface IWarehouse
    {
        Task<List<WarehouseView>> GetAll();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(Warehouse model);
        Task<bool> Update(Warehouse model);
        Task<Warehouse> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
