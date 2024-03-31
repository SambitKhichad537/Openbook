using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class Brand : IEntidadTenant
	{
		[Key]
		public int BrandId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Image { get; set; }
		public string TenantId { get; set; } = null!;
		public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
