using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
    public class PaymentDetails : IEntidadTenant
	{
		[Key]
		public int PaymentDetailsId { get; set; }
        public int PaymentMasterId { get; set; }
        public int PurchaseMasterId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReceiveAmount { get; set; }
        public decimal DueAmount { get; set; }
		public string TenantId { get; set; } = null!;
	}
}
