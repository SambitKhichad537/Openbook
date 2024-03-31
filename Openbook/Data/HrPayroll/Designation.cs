using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class Designation : IEntidadTenant
    {
        [Key]
        public int DesignationId { get; set; }
        [Required]
        public string DesignationName { get; set; }
        public decimal LeaveDays { get; set; }
        public decimal AdvanceAmount { get; set; }
        public string Narration { get; set; }
        public string TenantId { get; set; } = null!;
    }
}
