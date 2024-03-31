using Openbook.Data.HrPayroll;
using Openbook.Data.HrPayrollModel;

namespace Openbook.Repository.Interface
{
    public interface IPayHead
	{
        Task<List<PayHeadView>> GetAll();
       Task<bool> CheckName(string name);
       Task<int> CheckNameId(string name);
        Task<int> Save(PayHead model);
        Task<bool> Update(PayHead model);
        Task<PayHead> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
