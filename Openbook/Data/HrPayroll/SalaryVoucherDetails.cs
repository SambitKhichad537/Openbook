using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class SalaryVoucherDetails : IEntidadTenant
    {
        [Key]
        public int SalaryVoucherDetailsId { get; set; }
        public int SalaryVoucherMasterId { get; set; }
        public int EmployeeId { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        public decimal Advance { get; set; }
        public decimal Lop { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; }
        public string TenantId { get; set; } = null!;
    }
}
