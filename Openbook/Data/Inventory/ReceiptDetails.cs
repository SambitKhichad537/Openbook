using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
    public class ReceiptDetails : IEntidadTenant
	{
		[Key]
		public int ReceiptDetailsId { get; set; }
        public int ReceiptMasterId { get; set; }
        public int SalesMasterId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReceiveAmount { get; set; }
        public decimal DueAmount { get; set; }
		public string TenantId { get; set; } = null!;
	}
}
