using Microsoft.AspNetCore.Identity;
using Openbook.Data;
using Openbook.Data.Modules;
using Openbook.Data.ViewModel;
using Openbook.Models;

namespace Openbook.Repository.Interface
{
    public interface IUser
    {
        Task<int> Registro(RegistroViewModel model);
        Task<ResponseMessage> RegistroCompany(RegistroViewModel model);
        Task<bool> Login(LoginViewModel model);
		IEnumerable<ApplicationUser> ViewUser();
		IEnumerable<UserListDataModel> ViewUsers();
        List<UserCompany> ViewUserWithRole();
        Task<ApplicationUser> EditUser(string id);
        Task<bool> Update(ApplicationUser user);
		List<IdentityRole> ViewRole();
        bool Delete(string userId);
        Task<ResponseMessage> ChangePassword(ChangePasswordModel model);

	}
}
