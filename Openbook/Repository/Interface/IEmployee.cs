using Openbook.Data.HrPayroll;
using Openbook.Data.HrPayrollModel;

namespace Openbook.Repository.Interface
{
    public interface IEmployee
	{
        Task<List<EmployeeView>> GetAll();
       Task<bool> CheckName(string name);
       Task<int> CheckNameId(string name);
        Task<int> Save(Employee model);
        Task<bool> Update(Employee model);
        Task<Employee> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
