using Openbook.Data.Inventory;
using Openbook.Data.Setting;

namespace Openbook.Repository.Interface
{
	public interface IPurchaseReturn
	{
		Task<PurchaseReturnMasterView> PurchaseReturnInvoiceView(int id);
		Task<PurchaseReturnMaster> GetbyId(int id);
		Task<List<ProductView>> PurchaseReturnDetailsView(int id);
		Task<List<PurchaseReturnMaster>> PurchaseReturnBillView(int id);
		Task<List<PurchaseReturnMasterView>> PurchaseReturnInvoiceFilter(DateTime fromDate, DateTime toDate, int supplierid, string strVoucherNo, string strStatus, string strFilterType);
		Task<string> GetSerialNo();
		Task<int> Draft(PurchaseReturnMaster model);
		Task<bool> Update(PurchaseReturnMaster model);
		Task<bool> UpdateDraft(PurchaseReturnMaster model);
		Task<int> Approved(PurchaseReturnMaster model);
		Task<bool> ApprovedUpdate(PurchaseReturnMaster model);
		Task<bool> Delete(PurchaseReturnMaster model);
		bool BillnoCheckExistence(string VoucherNo);
	}
}
