using Openbook.Data.Inventory;
using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openbook.Data.AccountModel
{
    public class JournalMaster : IEntidadTenant
	{
		[Key]
		public int JournalMasterId { get; set; }
        public string SerialNo { get; set; }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceNo { get; set; }
        public string Narration { get; set; }
        public string UserId { get; set; }
        public int VoucherTypeId { get; set; }
        public int FinancialYearId { get; set; }
        public string Status { get; set; }
        public int CurrencyId { get; set; }
        public string TenantId { get; set; } = null!;
		public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        [NotMapped]
        public List<JournalDetails> listOrder { get; set; } = new List<JournalDetails>();
        [NotMapped]
        public List<DeleteItem> listDelete { get; set; } = new List<DeleteItem>();
    }
}
