using Openbook.Data.Inventory;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface IPaymentReceipt
    {
		Task<IList<ReceiptMasterView>> GetAll(DateTime FromDate, DateTime ToDate, int intSupplierId, string VoucherNo , string strType);
		Task<IList<ReceiptMaster>> ReceiptMadeSales(int id);
		Task<IList<ReceiptDetailsView>> ReceiptFor(int id);
		Task<IList<ReceiptDetailsView>> ReceiptDetailsView(int id);
		Task<int> Save(ReceiptMaster model);
		Task<bool> Update(ReceiptMaster model);
		Task<ReceiptMaster> GetbyId(int id);
		Task<ReceiptMasterView> GetbyIdView(int id);
		Task<string> GetSerialNo();
		Task<bool> Delete(ReceiptMaster model);
		Task<decimal> Payable();
	}
}
