namespace Openbook.Data.ViewModel
{
    public class ReceiptDetailsView
    {
        public int Id { get; set; }
        public int ReceiptDetailsId { get; set; }
        public int ReceiptMasterId { get; set; }
        public int SalesMasterId { get; set; }
        public int LedgerId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReceiveAmount { get; set; }
        public decimal DueAmount { get; set; }
        public string LedgerName { get; set; }
        public DateTime BillDate { get; set; }
        public string BillNo { get; set; }
        public decimal PurchaseAmount { get; set; }
    }
}
