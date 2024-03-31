namespace Openbook.Data.ViewModel
{
    public class PaymentDetailsView
    {
        public int Id { get; set; }
        public int PaymentDetailsId { get; set; }
        public int PaymentMasterId { get; set; }
        public int PurchaseMasterId { get; set; }
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
