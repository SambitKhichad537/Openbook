using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
	public interface ITimeZone
	{
        Task<List<TimeZonesView>> GetAll();
        Task<bool> CheckName(string name);
        Task<int> CheckNameId(string name);
        Task<int> Save(TimeZones model);
        Task<bool> Update(TimeZones model);
        Task<TimeZones> GetbyId(int id);
        Task<bool> Delete(int id);
    }
}
