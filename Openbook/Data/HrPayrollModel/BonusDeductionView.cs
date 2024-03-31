using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayroll
{
    public class BonusDeductionView
	{
        public int BonusDeductionId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime Date { get; set; }
        public DateTime Month { get; set; }
        public string YearMonth { get; set; }
		public decimal BonusAmount { get; set; }
        public decimal DeductionAmount { get; set; }
        public string Narration { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
