using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface ICountry
	{
        Task<List<Country>> GetAll();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(Country model);
        Task<bool> Update(Country model);
        Task<Country> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
