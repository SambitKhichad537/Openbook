using Openbook.Data.HrPayroll;
using Openbook.Data.HrPayrollModel;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface ISalaryVoucher
    {
        Task<List<SalaryVoucherMasterView>> GetAll();
        Task<IList<SalaryVoucherMasterView>> GetAllSalaryMonth();
        Task<IList<SalaryVoucherDetailsView>> MonthlySalaryReport(DateTime fromdate , DateTime todate , int employeeid, string month);
        Task<IList<SalaryVoucherDetailsView>> GetAllSalaryVoucher(string monthYear);
        Task<SalaryVoucherDetails> GetPaidUnpaid(string monthYear , int employeeid);
        Task<string> GetSerialNo();
	   Task<bool> CheckName(string name);
       Task<int> CheckNameId(string name);
        Task<int> Save(SalaryVoucherMaster model);
        Task<bool> Update(SalaryVoucherMaster model);
        Task<SalaryVoucherMaster> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
