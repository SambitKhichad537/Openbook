namespace Openbook.Data.HrPayrollModel
{
	public class DailyAttendanceView
	{
		public int EmployeeId { get; set; }
		public string EmployeeName { get; set; }
		public string EmployeeCode { get; set; }
		public int DailyAttendanceDetailsId { get; set; }
		public int DailyAttendanceMasterId { get; set; }
		public DateTime Date { get; set; }
        public string status { get; set; }
		public string narration { get; set; }
		public string MasterNarration { get; set; }
	}
}
