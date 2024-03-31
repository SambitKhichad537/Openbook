using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
	public class AccountLedgerView
	{
        public int LedgerId { get; set; }
		public int GroupUnder { get; set; }
        public string GroupName { get; set; }
        public string? LedgerName { get; set; }
        public string LedgerCode { get; set; }
        public decimal OpeningBalance { get; set; }
        public bool IsDefault { get; set; }
        public string CrOrDr { get; set; }
        public string? Type { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public bool IsGroup { get { return Type != null; } }
        public string TenantId { get; set; } = null!;
		public int JournalDetailsId { get; set; }
		public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
