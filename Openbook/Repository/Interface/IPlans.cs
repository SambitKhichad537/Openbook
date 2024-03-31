using Openbook.Data.SaasModels;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using System.Threading.Tasks;

namespace Openbook.Repository.Interface
{
    public interface IPlans
	{
        Task<List<PlanMaster>> GetAll();
		Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(PlanMaster model);
        Task<bool> Update(PlanMaster model);
        Task<PlanMaster> GetbyId(int id);
        //GetPlansCompany
        PlanMasterView GetPlansCompany(string id);
        Task<bool> Delete(int id);
    }
}
