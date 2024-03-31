using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface ICategories
    {
        Task<List<CategoriesView>> GetAll();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(Categories model);
        Task<bool> Update(Categories model);
        Task<Categories> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
