using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class AccountReportView
    {
        [Key]
        public int LedgerPostingId { get; set; }
        public int LedgerId { get; set; }
        public DateTime Date { get; set; }
        public string LedgerName { get; set; }
        public string Name { get; set; } //Customer & SupplierName
        public string VoucherNo { get; set; }
        public string VoucherTypeName { get; set; }
        public Decimal Debit { get; set; }
        public Decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
