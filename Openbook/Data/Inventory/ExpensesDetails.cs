using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
    public class ExpensesDetails : IEntidadTenant
	{
		[Key]
		public int ExpensesDetailsId { get; set; }
        public int ExpensiveMasterId { get; set; }
        public int LedgerId { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
		public string TenantId { get; set; } = null!;
	}
}
