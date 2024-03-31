using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openbook.Data.Inventory
{
    public class ReceiptMaster : IEntidadTenant
	{
		[Key]
		public int ReceiptMasterId { get; set; }
        public string VoucherNo { get; set; }
        public DateTime Date { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a account.")]
        public int LedgerId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select.")]
        public int CustomerSupplierId { get; set; }
        public decimal PaidAmount { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string SerialNo { get; set; }
        public string Narration { get; set; }
        public string Reference { get; set; }
        public int VoucherTypeId { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string PaymentType { get; set; }
        public int FinancialYearId { get; set; }
        public string TenantId { get; set; } = null!;
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        [NotMapped]
        public List<ReceiptDetails> listOrder { get; set; } = new List<ReceiptDetails>();
        [NotMapped]
        public List<DeleteItem> listDelete { get; set; } = new List<DeleteItem>();
        [NotMapped]
        public decimal PreviousDue { get; set; }
    }
}
