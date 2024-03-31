using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayrollModel
{
    public class EmployeeView
	{
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
		public string DesignationName { get; set; }
		public DateTime Dob { get; set; }
        public string MaritalStatus { get; set; }
        public string Gender { get; set; }
        public string Qualification { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public DateTime JoiningDate { get; set; }
        public bool isActive { get; set; }
        public string Narration { get; set; }
        public string SalaryType { get; set; }
        public decimal DailyWage { get; set; }
        public int DefaultPackageId { get; set; }
    }
}
