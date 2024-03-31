using Openbook.Data.Inventory;
using Openbook.Data.Setting;

namespace Openbook.Repository.Interface
{
	public interface ISalesReturn
	{
		Task<SalesReturnMasterView> SalesReturnInvoiceView(int id);
		Task<SalesReturnMaster> GetbyId(int id);
		Task<List<ProductView>> SalesReturnDetailsView(int id);
		Task<List<SalesReturnMaster>> SalesReturnBillView(int id);
		Task<List<SalesReturnMasterView>> SalesReturnInvoiceFilter(DateTime fromDate, DateTime toDate, int supplierid, string strVoucherNo, string strStatus, string strFilterType);
		Task<string> GetSerialNo();
		Task<int> Draft(SalesReturnMaster model);
		Task<bool> Update(SalesReturnMaster model);
		Task<bool> UpdateDraft(SalesReturnMaster model);
		Task<int> Approved(SalesReturnMaster model);
		Task<bool> ApprovedUpdate(SalesReturnMaster model);
		Task<bool> Delete(SalesReturnMaster model);
		bool BillnoCheckExistence(string VoucherNo);
	}
}
