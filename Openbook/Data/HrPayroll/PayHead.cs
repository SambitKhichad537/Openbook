using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class PayHead : IEntidadTenant
    {
        [Key]
        public int PayHeadId { get; set; }
        [Required]
        public string PayHeadName { get; set; }
        public string Type { get; set; }
        public string Narration { get; set; }
        public string TenantId { get; set; } = null!;
    }
}
