using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class Categories : IEntidadTenant
	{
		public int CategoriesId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public string Image { get; set; }
		public string TenantId { get; set; } = null!;
		public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
