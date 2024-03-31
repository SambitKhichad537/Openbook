using Openbook.Data.Setting;
using Openbook.Data.ViewModel;

namespace Openbook.Repository.Interface
{
    public interface IEmails
    {
        Task<EmailSetting> GetAll();
        Task<int> Save(EmailSetting model);
        Task<bool> Update(EmailSetting model);
    }
}
