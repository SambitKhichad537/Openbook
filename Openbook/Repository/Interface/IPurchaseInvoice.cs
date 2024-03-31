using Openbook.Data.Inventory;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface IPurchaseInvoice
    {
		Task<PurchaseMasterView> PurchaseInvoiceView(int id);
		Task<PurchaseMaster> GetbyId(int id);
		Task<List<ProductView>> PurchaseDetailsView(int id);
		Task<List<PurchaseMaster>> PurchaseBillView(int id);
		Task<List<PurchaseMasterView>> PurchaseInvoiceFilter( DateTime fromDate, DateTime toDate, int supplierid, string strVoucherNo, string strStatus , string strFilterType);
        Task<string> GetSerialNo();
		Task<int> Draft(PurchaseMaster model);
		Task<bool> Update(PurchaseMaster model);
		Task<bool> UpdateDraft(PurchaseMaster model);
		Task<int> Approved(PurchaseMaster model);
		Task<bool> ApprovedUpdate(PurchaseMaster model);
		Task<bool> Delete(PurchaseMaster model);
		bool BillnoCheckExistence(string VoucherNo);
	}
}
