using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class SalesQuotationView
    {
        [Key]
        public int QuotationMasterId { get; set; }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public string VoucherTypeName { get; set; }
        public int LedgerId { get; set; }
        public string LedgerName { get; set; }
        public string LedgerCode { get; set; }
        public string Narration { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal BillDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public string UserId { get; set; }
        public string Address { get; set; }
        public string Pan { get; set; }
        public decimal TaxRate { get; set; }
        public string Status { get; set; }
        public string WarehouseName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
