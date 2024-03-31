using System;
using System.ComponentModel.DataAnnotations;
namespace Openbook.Data.HrPayroll
{
	public class SalaryPackageDetailsView
    {
        public int Id { get; set; }
        public int SalaryPackageDetailsId { get; set; }
        public int SalaryPackageId { get; set; }
        public int PayHeadId { get; set; }
        public decimal Amount { get; set; }
        public string PayHeadName { get; set; }
        public string Type { get; set; }
        public string Narration { get; set; }
    }
}
