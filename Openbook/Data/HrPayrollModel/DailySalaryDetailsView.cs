using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class DailySalaryDetailsView
    {
        public int DailySalaryVoucherDetailsId { get; set; }
        public int DailySalaryVoucherMasterId { get; set; }
        public int EmployeeId { get; set; }
          public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public decimal Wage { get; set; }
        public string Status { get; set; }
        public string AttendanceStatus { get; set; }
    }
}
