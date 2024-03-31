using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
    public class IncomeMaster : IEntidadTenant
	{
		[Key]
		public int IncomeMasterId { get; set; }
        public int LedgerId { get; set; }
        public DateTime? Date { get; set; }
        public int VoucherTypeId { get; set; }
        public string SerialNo { get; set; }
        public string VoucherNo { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public int FinancialYearId { get; set; }
		public string TenantId { get; set; } = null!;
		public string UserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
