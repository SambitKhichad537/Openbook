using Openbook.Data.HrPayroll;

namespace Openbook.Repository.Interface
{
    public interface IBonusDeduction
	{
        Task<List<BonusDeductionView>> GetAll();
        Task<IList<BonusDeductionView>> GetAllSalaryMonth();
        Task<IList<BonusDeductionView>> BonusDeductionReport(DateTime fromdate, DateTime todate, int employeeid, string month);
        Task<bool> CheckName(string name , int employeeid);
       Task<int> CheckNameId(string name);
        Task<int> Save(BonusDeduction model);
        Task<bool> Update(BonusDeduction model);
        Task<BonusDeduction> GetbyId(int id);
        Task<BonusDeduction> GetBonusDeductionAmount(string name, int employeeid);
        Task<bool> Delete(int id);
    }
}
