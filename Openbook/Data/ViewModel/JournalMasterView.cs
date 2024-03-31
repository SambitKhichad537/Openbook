namespace Openbook.Data.ViewModel
{
    public class JournalMasterView
    {
        public int JournalMasterId { get; set; }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public string SerialNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public string ReferenceNo { get; set; }
        public string UserId { get; set; }
        public string VoucherTypeName { get; set; }
        public string LedgerName { get; set; }
        public int CompanyId { get; set; }
        public string Status { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
