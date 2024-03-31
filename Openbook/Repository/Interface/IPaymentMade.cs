using Openbook.Data.Inventory;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using System.Threading.Tasks;

namespace Openbook.Repository.Interface
{
	public interface IPaymentMade
	{
		Task<IList<PaymentMasterView>> GetAll(DateTime FromDate, DateTime ToDate, int intSupplierId, string VoucherNo , string strType);
		Task<IList<PaymentMaster>> PaymentMadePurchase(int id);
		Task<IList<PaymentDetailsView>> PaymentFor(int id);
		Task<IList<PaymentDetailsView>> PaymentDetailsView(int id);
		Task<int> Save(PaymentMaster model);
		Task<bool> Update(PaymentMaster model);
		Task<PaymentMaster> GetbyId(int id);
		Task<PaymentMasterView> GetbyIdView(int id);
		Task<string> GetSerialNo();
		Task<bool> Delete(PaymentMaster model);
		Task<decimal> Receiable();
	}
}
