using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
    public class AccountLedger : IEntidadTenant
	{
        [Key]
        public int LedgerId { get; set; }
		[Range(1, int.MaxValue, ErrorMessage = "Please select a type.")]
		public int GroupUnder { get; set; }
        [Required]
        public string LedgerName { get; set; }
        public string LedgerCode { get; set; }
        public decimal OpeningBalance { get; set; }
        public bool IsDefault { get; set; }
        public string CrOrDr { get; set; }
        public string Type { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
		public string TenantId { get; set; } = null!;
		public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
