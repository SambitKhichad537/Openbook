using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.HrPayrollModel
{
    public class PayHeadView
	{
        public int PayHeadId { get; set; }
        public string PayHeadName { get; set; }
        public string Type { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }
        public int SalaryPackageDetailsId { get; set; }
        public int Id { get; set; }
    }
}
