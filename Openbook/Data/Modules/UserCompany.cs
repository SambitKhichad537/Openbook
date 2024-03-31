using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Modules
{
    public class UserCompany : IEntidadTenant
    {
        [Key]
        public int UserCompId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string UserId { get; set; }
        public string TenantId { get; set; } = null!;
    }
}
