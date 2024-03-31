using Openbook.Data.HrPayroll;
using Openbook.Data.HrPayrollModel;
using Openbook.Data.Setting;

namespace Openbook.Repository.Interface
{
    public interface ISalaryMonthSetting
    {
        Task<List<MonthlySalarySettingView>> GetAll(string YearMonth);
        Task<List<MonthlySalary>> GetAllMonth();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(MonthlySalary model);
        Task<MonthlySalary> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
