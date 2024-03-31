namespace Openbook.Data.HrPayrollModel
{
    public class MonthlySalarySettingView
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public int DefaultPackageId { get; set; }
        public int SalaryPackageId { get; set; }
        public int MonthlySalaryId { get; set; }
        public int MonthlySalaryDetailsId { get; set; }
        public string Narration { get; set; }
    }
}
