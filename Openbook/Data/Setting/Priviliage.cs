using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class Priviliage : IEntidadComn
	{
        [Key]
      public int PriviliageId { get; set; }
        public string Menu { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string TenantId { get; set; } = null!;
    }
}
