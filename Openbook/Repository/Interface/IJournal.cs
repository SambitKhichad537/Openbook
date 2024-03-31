using Openbook.Data.AccountModel;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
	public interface IJournal
	{
		Task<JournalMasterView> JournalView(int id);
		Task<JournalMaster> GetbyId(int id);
		Task<List<JournalDetailsView>> JournalDetailsView(int id);
		Task<List<JournalMasterView>> GetAll(string strStatus , string strDate);
        Task<List<JournalMasterView>> GetAllByDate(string strStatus, DateTime startDate , DateTime endDate);
        Task<string> GetSerialNo();
		decimal CheckLedgerBalance(int LedgerId);
		Task<int> Draft(JournalMaster model);
		Task<int> Update(JournalMaster model);
		Task<int> Publish(JournalMaster model);
        Task<bool> Delete(JournalMaster model);
		bool VouchernoCheckExistence(string VoucherNo);
		Task<List<LedgerPostingView>> Income();
		Task<List<LedgerPostingView>> Expenses();
	}
}
