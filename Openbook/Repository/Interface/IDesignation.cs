using Openbook.Data.HrPayroll;
using Openbook.Data.HrPayrollModel;

namespace Openbook.Repository.Interface
{
    public interface IDesignation
	{
        Task<List<DesignationView>> GetAll();
       Task<bool> CheckName(string name);
       Task<int> CheckNameId(string name);
        Task<int> Save(Designation model);
        Task<bool> Update(Designation model);
        Task<Designation> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
