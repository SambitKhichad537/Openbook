using Openbook.Data.SaasModels;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface ICoupons
    {
        Task<List<CouponsView>> GetAll();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(Coupons model);
        Task<bool> Update(Coupons model);
        Task<Coupons> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
