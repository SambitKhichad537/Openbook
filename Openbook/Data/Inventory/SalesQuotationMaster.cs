using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openbook.Data.Inventory
{
    public class SalesQuotationMaster : IEntidadTenant
	{
		[Key]
		public int QuotationMasterId { get; set; }
        public string SerialNo { get; set; }
        public string VoucherNo { get; set; }
        public int VoucherTypeId { get; set; }
        public DateTime Date { get; set; }
        public int LedgerId { get; set; }
        public decimal TaxId { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TaxRate { get; set; }

        public decimal DisPer { get; set; }
        public decimal BillDiscount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public string Narration { get; set; }
        public int WarehouseId { get; set; }
        public int FinancialYearId { get; set; }
		public string TenantId { get; set; } = null!;
		public string UserId { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifyDate { get; set; }
        [NotMapped]
        public List<SalesQuotationDetails> listOrder { get; set; } = new List<SalesQuotationDetails>();
        [NotMapped]
        public List<DeleteItem> listDelete { get; set; } = new List<DeleteItem>();
    }
}
