using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class MonthlySalaryDetails : IEntidadTenant
    {
        [Key]
        public int MonthlySalaryDetailsId { get; set; }
        public int EmployeeId { get; set; }
        public int SalaryPackageId { get; set; }
        public int MonthlySalaryId { get; set; }
        public string TenantId { get; set; } = null!;
    }
}
