using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openbook.Data.Inventory
{
    public class PaymentMasterView
	{
		public int PaymentMasterId { get; set; }
        public string VoucherNo { get; set; }
        public DateTime Date { get; set; }
		public int LedgerId { get; set; }
		public int CustomerSupplierId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
		public decimal PaidAmount { get; set; }
        public decimal Amount { get; set; }
        public string SerialNo { get; set; }
        public string Narration { get; set; }
        public string Reference { get; set; }
		public int VoucherTypeId { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string PaymentType { get; set; }
        public int FinancialYearId { get; set; }
        public string LedgerName { get; set; }
		public string TenantId { get; set; } = null!;
		public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        
    }
}
