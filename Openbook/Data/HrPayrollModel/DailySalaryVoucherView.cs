using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class DailySalaryVoucherView
    {
        public int DailySalaryVoucherMasterId { get; set; }
        public string LedgerName { get; set; }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public DateTime SalaryDate { get; set; }
        public string YearMonthDay { get; set; }
        public decimal TotalAmount { get; set; }
        public string Narration { get; set; }
        public string VoucherTypeName { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int VoucherTypeId { get; set; }
        public int FinancialYearId { get; set; }
    }
}
