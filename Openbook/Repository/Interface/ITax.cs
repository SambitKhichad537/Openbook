using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface ITax
    {
        Task<List<TaxView>> GetAll();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(Tax model);
        Task<bool> Update(Tax model);
        Task<Tax> GetbyId(int id);
        Task<Tax> ViewTaxRate(int id);
		Task<bool> Delete(int id);
    }
}
