using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
    public class SalesQuotationDetails : IEntidadTenant
	{
		[Key]
		public int QuotationDetailsId { get; set; }
        public int QuotationMasterId { get; set; }
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public int BatchId { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public int TaxId { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Amount { get; set; }
		public string TenantId { get; set; } = null!;
	}
}
