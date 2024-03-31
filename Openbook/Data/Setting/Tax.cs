using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class Tax : IEntidadTenant
	{
		[Key]
		public int TaxId { get; set; }
        [Required]
        public string TaxName { get; set; }
        public Decimal Rate { get; set; }
        public bool IsActive { get; set; }
		public string TenantId { get; set; } = null!;
		public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
