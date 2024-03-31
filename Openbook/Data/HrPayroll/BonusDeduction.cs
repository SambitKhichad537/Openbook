using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class BonusDeduction : IEntidadTenant
    {
        [Key]
        public int BonusDeductionId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a Employee.")]
        public int EmployeeId { get; set; }
		[Required]
        public DateTime Date { get; set; }
        public DateTime Month { get; set; }
        public string YearMonth { get; set; }
		public decimal BonusAmount { get; set; }
        public decimal DeductionAmount { get; set; }
        public string Narration { get; set; }
        public string TenantId { get; set; } = null!;
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
