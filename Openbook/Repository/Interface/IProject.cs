using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface IProject
    {
        Task<List<ProjectView>> GetAll();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(Project model);
        Task<bool> Update(Project model);
        Task<Project> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
