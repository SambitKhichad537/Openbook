using Openbook.Data.HrPayroll;

namespace Openbook.Repository.Interface
{
    public interface IDailySalaryVoucher
    {
        Task<List<DailySalaryVoucherView>> GetAll();
        Task<IList<DailySalaryVoucherView>> GetAllSalaryMonth();
        Task<IList<DailySalaryDetailsView>> MonthlySalaryReport(DateTime fromdate, DateTime todate, int employeeid, string month);
        Task<IList<DailySalaryDetailsView>> GetAllSalaryVoucher(string monthYear);
        Task<IList<DailySalaryDetailsView>> GetAllEmployeeAttendance(string yearmonthday);
        Task<DailySalaryVoucherDetails> GetStatusPaidUnpaid(string monthYear, int employeeid);
        Task<string> GetSerialNo();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(DailySalaryVoucherMaster model);
        Task<bool> Update(DailySalaryVoucherMaster model);
        Task<DailySalaryVoucherMaster> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
