using Openbook.Entidades;

namespace Openbook.Data.Inventory
{
    public class ExpensesDetailsView : IEntidadTenant
	{
        public int ExpensesDetailsId { get; set; }
        public int ExpensiveMasterId { get; set; }
        public int LedgerId { get; set; }
        public string LedgerName { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public int Id { get; set; }
        public string LedgerCode { get; set; }
		public string TenantId { get; set; } = null!;
	}
}
