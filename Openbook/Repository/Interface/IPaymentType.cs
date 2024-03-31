using Openbook.Data.SaasModels;

namespace Openbook.Repository.Interface
{
    public interface IPaymentType
    {
        Task<List<PaymentTypeView>> GetAll();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(PaymentType model);
        Task<bool> Update(PaymentType model);
        Task<PaymentType> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
