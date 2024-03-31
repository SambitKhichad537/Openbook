using Openbook.Data.Inventory;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface ISalesInvoice
    {
		Task<SalesMasterView> SalesInvoiceView(int id);
		Task<SalesMaster> GetbyId(int id);
		SalesMaster GetbyIdView(int id);
		Task<List<ProductView>> SalesDetailsView(int id);
		Task<List<SalesMaster>> SalesBillView(int id);
		Task<List<SalesMasterView>> SalesInvoiceFilter( DateTime fromDate, DateTime toDate, int supplierid, string strVoucherNo, string strStatus , string strFilterType);
        Task<string> GetSerialNo();
		Task<int> Draft(SalesMaster model);
		Task<bool> Update(SalesMaster model);
		Task<bool> UpdateDraft(SalesMaster model);
		Task<int> Approved(SalesMaster model);
		Task<bool> ApprovedUpdate(SalesMaster model);
		Task<bool> Delete(SalesMaster model);
		bool BillnoCheckExistence(string VoucherNo);
	}
}
