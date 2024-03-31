using Openbook.Data.Inventory;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
	public interface IInventoryReport
	{
		IList<InventoryViewFinal> StockReport(int GroupId, int ProductId);
		Task<IList<PurchaseMasterView>> PurchasebyItem(DateTime fromDate, DateTime toDate, int ProductId);
		Task<IList<SalesMasterView>> SalesbyItem(DateTime fromDate, DateTime toDate, int ProductId);
		Task<IList<PurchaseMasterView>> ReceivablesSummary(DateTime FromDate, DateTime ToDate);
		Task<IList<SalesMasterView>> PayableSummary(DateTime FromDate, DateTime ToDate);
		Task<IList<ReceiptMasterView>> PayamentReceived(DateTime FromDate, DateTime ToDate);
		Task<IList<PaymentMasterView>> PayamentMade(DateTime FromDate, DateTime ToDate);
	}
}
