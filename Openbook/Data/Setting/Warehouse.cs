using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openbook.Data.Setting
{
    public class Warehouse : IEntidadTenant
    {
        [Key]
        public int WarehouseId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string TenantId { get; set; } = null!;
    }
}
