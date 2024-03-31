using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.AccountModel
{
    public class JournalDetails : IEntidadTenant
	{
        [Key]
        public int JournalDetailsId { get; set; }
        public int JournalMasterId { get; set; }
        public int LedgerId { get; set; }
        public int ProjectId { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string Narration { get; set; }
		public string TenantId { get; set; } = null!;
	}
}
