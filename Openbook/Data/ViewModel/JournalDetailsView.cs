namespace Openbook.Data.ViewModel
{
    public class JournalDetailsView
    {
        public int JournalDetailsId { get; set; }
        public int JournalMasterId { get; set; }
        public int Id { get; set; }
        public int LedgerId { get; set; }
		public int ProjectId { get; set; }
		public string LedgerName { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string Narration { get; set; }
    }
}
