using Openbook.Data.Setting;

namespace Openbook.Repository.Interface
{
	public interface IAccountReports
	{
		AccountReportView TrailBalance(int LedgerId, DateTime fromDate, DateTime toDate);
        AccountReportView Income(int LedgerId, DateTime fromDate, DateTime toDate);
        AccountReportView Expenses(int LedgerId, DateTime fromDate, DateTime toDate);
        AccountReportView TotalBalance(string strGroup, DateTime fromDate, DateTime toDate);
        AccountReportView TotalBalanceTrailBalance(DateTime fromDate, DateTime toDate);
        AccountReportView GrossProit(DateTime fromDate, DateTime toDate , string strType);
        AccountReportView NetLoss(DateTime fromDate, DateTime toDate , string strType);
        IList<AccountReportView> ProfitNLoss(DateTime fromDate, DateTime toDate);
		IList<AccountReportView> BalanceSheet(DateTime fromDate, DateTime toDate);
		IList<AccountReportView> AccountTransaction(DateTime fromDate, DateTime toDate);
		IList<AccountReportView> GeneralLedger(DateTime fromDate, DateTime toDate , int ledgerId);
	}
}
