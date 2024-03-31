using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class Employee : IEntidadTenant
    {
        [Key]
        public int EmployeeId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a Designation.")]
        public int DesignationId { get; set; }
        [Required]
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
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
        public string TenantId { get; set; } = null!;
        [Range(1, int.MaxValue, ErrorMessage = "Please select Package.")]
		public int DefaultPackageId { get; set; }
    }
}
