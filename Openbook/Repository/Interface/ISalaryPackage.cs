using Openbook.Data.HrPayroll;

namespace Openbook.Repository.Interface
{
    public interface ISalaryPackage
    {
        Task<List<SalaryPackage>> GetAll();
        Task<List<SalaryPackageDetailsView>> GetAllView(int id);
        Task<bool> CheckName(string name);
       Task<int> CheckNameId(string name);
        Task<int> Save(SalaryPackage model);
        Task<bool> Update(SalaryPackage model);
        Task<SalaryPackage> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
