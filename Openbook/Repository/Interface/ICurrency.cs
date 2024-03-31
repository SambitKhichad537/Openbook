using Openbook.Data.Inventory;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
	public interface ICurrency
	{
        Task<List<CurrencyView>> GetAll();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(Currency model);
        Task<bool> Update(Currency model);
        Task<Currency> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
