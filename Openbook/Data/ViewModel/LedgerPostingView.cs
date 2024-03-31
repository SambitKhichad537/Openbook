namespace Openbook.Data.ViewModel
{
	public class LedgerPostingView
	{
		public int LedgerPostingId { get; set; }
		public DateTime Date { get; set; }
		public int VoucherTypeId { get; set; }
		public string VoucherNo { get; set; }
		public int LedgerId { get; set; }
		public string LedgerName { get; set; }
		public string Month { get; set; }
		public Decimal Debit { get; set; }
		public Decimal Credit { get; set; }
		public string InvoiceNo { get; set; }
		public string TenantId { get; set; } = null!;
		public string ReferenceN { get; set; }
		public string LongReference { get; set; }
		public decimal Receiable { get; set; }
		public decimal Payable { get; set; }
	}
}
