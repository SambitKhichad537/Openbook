using Openbook.Data.Inventory;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface ICustomerSupplier
	{
        Task<List<CustomerSupplierView>> GetAll( string type);
        Task<bool> CheckName(string name , string type);
        Task<int> CheckNameId(string name , string type);
        Task<int> Save(CustomerSupplier model);
        Task<bool> Update(CustomerSupplier model);
        Task<CustomerSupplier> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
