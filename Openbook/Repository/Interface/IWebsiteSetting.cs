using Openbook.Data.SaasModels;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface IWebsiteSetting
	{
        Task<WebsiteSetting> GetAll();
        Task<bool> Update(WebsiteSetting model);
    }
}
