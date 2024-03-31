using Openbook.Data.SaasModels;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface IPlanUpgrade
    {
        Task<List<PlanUpgrade>> GetAll();
        Task<bool> CheckName(string name);
        bool CheckNameId(int planid,string name);
        Task<bool> Save(PlanUpgrade model);
        Task<bool> Update(PlanUpgrade model);
        Task<bool> UpdateStatus(PlanUpgrade model);
        Task<PlanUpgrade> GetbyId(int id);
        Task<bool> Delete(int id);
		//GetSuperAdminData
		Task<PlanMasterView> TotalUsers();
		Task<PlanMasterView> PaidUsers();
		Task<PlanMasterView> TotalOrderAmounts();
		Task<PlanMasterView> TotalPlans();
        //CheckPlanActiveorNot
        Task<PlanMasterView> CheckPlanActiveorNot();
      Task<int> CountProduct();
        Task<int> CountSupplierCustomer(string type);
		Task<int> CountInvoice();

	}
}
