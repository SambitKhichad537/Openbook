using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface IPreference
	{
        Task<GeneralSetting> GetAll();
        Task<int> Save(GeneralSetting model);
        Task<bool> Update(GeneralSetting model);
    }
}
