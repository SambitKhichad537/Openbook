using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class AdvancePaymentView
    {
        public int AdvancePaymentId { get; set; }
        public string EmploymeeName { get; set; }
        public string EmploymeeCode { get; set; }
        public string LedgerName { get; set; }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime SalaryMonth { get; set; }
        public string YearMonth { get; set; }
        public string Narration { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
