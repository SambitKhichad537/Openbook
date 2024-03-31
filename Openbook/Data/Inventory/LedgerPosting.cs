using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
    public class LedgerPosting : IEntidadTenant
	{
		[Key]
		public int LedgerPostingId { get; set; }
        public DateTime Date { get; set; }
        public int VoucherTypeId { get; set; }
        public string VoucherNo { get; set; }
        public int LedgerId { get; set; }
        public Decimal Debit { get; set; }
        public Decimal Credit { get; set; }
        public int DetailsId { get; set; }
        public int YearId { get; set; }
        public string InvoiceNo { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeDate { get; set; }
		public string TenantId { get; set; } = null!;
		public string ReferenceN { get; set; }
        public string LongReference { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
