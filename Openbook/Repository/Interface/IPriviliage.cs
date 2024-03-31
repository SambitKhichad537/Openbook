using Openbook.Data.Modules;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface IPriviliage
    {
        Task<List<Priviliage>> GetAll();
		Task<List<Priviliage>> GetAllWithTenant();
		Task<bool> CheckName();
        Task<int> Save(Priviliage model);
        Task<bool> Update(Priviliage model);
        Task<Priviliage> GetbyId(int id);
        Task<bool> Delete();
		Task<List<Priviliage>> GetUserRole(string role);
	}
}
