using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class SalaryVoucherMasterView
    {
        public int SalaryVoucherMasterId { get; set; }
        public string LedgerName { get; set; }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public DateTime Month { get; set; }
        public decimal TotalAmount { get; set; }
        public string Narration { get; set; }
        public string VoucherTypeName { get; set; }
    }
}
