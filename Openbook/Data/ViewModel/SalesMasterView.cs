using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class SalesMasterView
    {
        public int SalesMasterId { get; set; }
        public string SerialNo { get; set; }
        public string VoucherNo { get; set; }
        public string OrderNo { get; set; }
        public int WarehouseId { get; set; }
        public int VoucherTypeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public int CustomerSupplierId { get; set; }
        public string Reference { get; set; }
        public string Narration { get; set; }
        public int PurchaseOrderMasterId { get; set; }
        public decimal TotalTax { get; set; }
        public string DiscountType { get; set; }
        public decimal DisPer { get; set; }
        public decimal BillDiscount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmounts { get; set; }
        public decimal PayAmount { get; set; }
        public decimal BalanceDue { get; set; }
        public string Status { get; set; }
        public decimal PreviousDue { get; set; }
        public string PaymentStatus { get; set; }
        public string UserId { get; set; }
        public int OrderMasterId { get; set; }
        public int FinancialYearId { get; set; }
        public string TenantId { get; set; } = null!;
        public string Email { get; set; }
		public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Name { get; set; }
    }
}
