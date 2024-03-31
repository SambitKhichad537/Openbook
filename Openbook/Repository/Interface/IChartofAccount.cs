using Openbook.Data.Inventory;

namespace Openbook.Repository.Interface
{
	public interface IChartofAccount
	{
        Task<List<AccountLedgerView>> GetAll();
        Task<List<AccountLedgerView>> GetAllChartofAccount();
		Task<List<AccountLedgerView>> GetAllChartofAccountsearch(string name);
		Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(AccountLedger model);
        Task<bool> Update(AccountLedger model);
        Task<AccountLedger> GetbyId(int id);
        Task<bool> Delete(int id);
		string GetAccountLedgerNo();
		Task<List<AccountLedgerView>> GetParentAccount(int id);
		Task<AccountLedger> GetGroup(int id);
	}
}
