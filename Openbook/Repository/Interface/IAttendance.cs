using Openbook.Data.HrPayroll;
using Openbook.Data.HrPayrollModel;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface IAttendance
	{
		Task<IList<DailyAttendanceView>> GetAll();
		decimal HolliDayChecking(DateTime date);
		decimal HolidaySettings(DateTime date);
        Task<DailyAttendanceDetails> GetAttandanceDetails(string date, int employeeid);
        bool DailyAttendanceMasterMasterIdSearch(DateTime strDate);
		IList<DailyAttendanceView> DailyAttendanceDetailsSearchGridFill();
		Task<int> Save(DailyAttendanceMaster model);
		Task<bool> Update(DailyAttendanceMaster model);
		Task<bool> Delete(int id);
	}
}
