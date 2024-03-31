using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class SalaryVoucherDetailsView
    {
        public int SalaryVoucherDetailsId { get; set; }
        public int SalaryVoucherMasterId { get; set; }
        public int MonthlySalaryId { get; set; }
        public int SalaryPackageId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        public decimal Advance { get; set; }
        public decimal Lop { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; }
        public string YearMonth { get; set; }
        public int MasterId { get; set; }
        public int DetailsId { get; set; }
    }
}
