using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
    public class StockPosting : IEntidadTenant
	{
		[Key]
		public int StockPostingId { get; set; }
        public DateTime Date { get; set; }
        public int VoucherTypeId { get; set; }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public int ProductId { get; set; }
        public int BatchId { get; set; }
        public int UnitId { get; set; }
        public int WarehouseId { get; set; }
        public int AgainstVoucherTypeId { get; set; }
        public string AgainstInvoiceNo { get; set; }
        public string AgainstVoucherNo { get; set; }
        public Decimal InwardQty { get; set; }
        public Decimal OutwardQty { get; set; }
        public Decimal Rate { get; set; }
        public int FinancialYearId { get; set; }
		public string TenantId { get; set; } = null!;
		public int DetailsId { get; set; }
        public string StockCalculate { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
