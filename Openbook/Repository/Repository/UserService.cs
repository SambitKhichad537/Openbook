using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.Modules;
using Openbook.Data.SaasModels;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using Openbook.Models;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;
using System.Security.Claims;

namespace Openbook.Repository.Repository
{
    public class UserService : IUser
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ICompany _company;
        private readonly DatabaseConnection _conn;
        private readonly IHttpContextAccessor _httpContext;
        private string tenantId;

        public UserService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, IServicioTenant servicioTenant, ICompany company, DatabaseConnection conn, IHttpContextAccessor httpcontext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
            _httpContext = httpcontext;
            _company = company;
            _conn = conn;
            tenantId = servicioTenant.ObtenerTenant();
        }

        public async Task<ApplicationUser> EditUser(string userId)
        {
			var objFromDb = _context.ApplicationUser.FirstOrDefault(u => u.Id == userId);
			if (objFromDb == null)
			{
				//return NotFound();
			}
			var userRole = _context.UserRoles.ToList();
			var roles = _context.Roles.ToList();
			var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);
			if (role != null)
			{
				objFromDb.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;
			}
			objFromDb.RoleList = _context.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
			{
				Text = u.Name,
				Value = u.Id
			});
			return objFromDb;
			
		}

        public async Task<bool> Login(LoginViewModel modelo)
        {
            var resultado = await signInManager.PasswordSignInAsync(modelo.Email,
                   modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
            _context.Entry(modelo).State = EntityState.Detached;
        }

        public async Task<int> Registro(RegistroViewModel modelo)
        {

			var usuario = new ApplicationUser { UserName = modelo.Email, Email = modelo.Email, Name = modelo.Name };


			var resultado = await userManager.CreateAsync(usuario, modelo.Password);
			//var usuario = new IdentityUser() { Email = modelo.Email, UserName = modelo.Email };

			//var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

			if (resultado.Succeeded)
            {
                var claimsPersonalizados = new List<Claim>()
            {
                new Claim(Constantes.ClaimTenantId, usuario.Id)
            };

                await userManager.AddClaimsAsync(usuario, claimsPersonalizados);
                await userManager.AddToRoleAsync(usuario, modelo.RoleName);
                
                using (var transaction = this._context.Database.BeginTransaction())
                {
                    try
                    {
                        Company model = new Company();
                        model.CompanyName = modelo.Name;
                        model.Address = string.Empty;
                        model.City = string.Empty;
                        model.CountryName = string.Empty;
                        model.TaxId = string.Empty;
                        model.TimeZoneId = 1;
                        model.DateFormat = "MM-DD-YYYY";
                        model.PhoneNo = string.Empty;
                        model.Email = modelo.Email;
                        model.CurrencyId = 1;
                        model.FinancialYearId = 0;
                        model.Website = string.Empty;
                        model.Logo = string.Empty;
                        model.TenantId = usuario.Id;
                        model.StartDate = DateTime.UtcNow;
                        model.AddedDate = DateTime.UtcNow;
                        _context.Company.Add(model);
                        _context.SaveChanges();
                        //int id = model.CompanyId;
                        //AddPlanUpgrade
                        PlanUpgrade models = new PlanUpgrade();
                        models.PlanId = 1;
						string elementId = Guid.NewGuid().ToString("N");
						models.OrderNo = elementId;
						models.IsActive = true;
						models.TenantId = usuario.Id;
                        _context.PlanUpgrade.Add(models);
                        _context.SaveChanges();
                        _context.Entry(models).State = EntityState.Detached;
                        //AddFinancialYear
                        FinancialYear year = new FinancialYear();
                        year.FromDate = model.StartDate;
                        year.ToDate = DateTime.UtcNow.AddDays(+365);
                        year.FiscalYear = string.Empty;
						year.TenantId = usuario.Id;
                        year.AddedDate = DateTime.Now;
                        _context.FinancialYear.Add(year);
                        _context.SaveChanges();
						//UserCompany
						UserCompany usercomp = new UserCompany();
                        usercomp.Name = modelo.Name;
						usercomp.Email = modelo.Email;
                        usercomp.RoleName = modelo.RoleName;
                        usercomp.UserId = usuario.Id;
						usercomp.TenantId = usuario.Id;
                        _context.UserCompany.Add(usercomp);
                        _context.SaveChanges();
                        transaction.Commit();
                        _context.Entry(model).State = EntityState.Detached;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
                return 1;
			}
			else
            {
                foreach (var error in resultado.Errors)
                {
                    //ModelState.AddModelError(string.Empty, error.Description);
                }
				return 0;
			}

		}
		public async Task<ResponseMessage> RegistroCompany(RegistroViewModel modelo)
		{
            ResponseMessage message = new ResponseMessage();
            var usuario = new ApplicationUser { UserName = modelo.Email, Email = modelo.Email, Name = modelo.Name };
			var resultado = await userManager.CreateAsync(usuario, modelo.Password);
			//var usuario = new IdentityUser() { Email = modelo.Email, UserName = modelo.Email };

			//var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

			if (resultado.Succeeded)
			{
				var claimsPersonalizados = new List<Claim>()
			{
				new Claim(Constantes.ClaimTenantId, tenantId)
			};

				await userManager.AddClaimsAsync(usuario, claimsPersonalizados);
				await userManager.AddToRoleAsync(usuario, modelo.RoleName);

				using (var transaction = this._context.Database.BeginTransaction())
				{
					try
					{
                        //UserCompany
                        UserCompany usercomp = new UserCompany();
                        usercomp.Name = modelo.Name;
                        usercomp.Email = modelo.Email;
                        usercomp.RoleName = modelo.RoleName;
                        usercomp.UserId = usuario.Id;
                        usercomp.TenantId = tenantId;
                        _context.UserCompany.Add(usercomp);
                        _context.SaveChanges();
                        transaction.Commit();
						_context.Entry(usercomp).State = EntityState.Detached;
					}
					catch (Exception ex)
					{
						transaction.Rollback();
                        message.Description = "Transaction failed";
                        return message;
                    }
				}
                message.Description = "User created successfully";
                return message;
            }
			else
			{
				foreach (var error in resultado.Errors)
				{
                    message.Description = error.Description;

                }
				return message;
			}

		}
		public IEnumerable<ApplicationUser> ViewUser()
		{
			var userList = _context.ApplicationUser.ToList();
            
			var userRole = _context.UserRoles.ToList();
			var roles = _context.Roles.ToList();
			foreach (var user in userList)
			{
				var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
				if (role == null)
				{
					user.Role = "None";
				}
				else
				{
					user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
				}
			}
			return userList;
		}


        public IEnumerable<UserListDataModel> ViewUsers()
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var ListofPlan = sqlcon.Query<UserListDataModel>(@"
            SELECT uc.Name as Name, uc.RoleName, uc.Email, p.PlanId, pm.Name as PlanName, uc.TenantId as TenantId, p.IsActive as IsActive
            FROM UserCompany uc
            JOIN PlanUpgrade p ON uc.TenantId = p.TenantId
            JOIN PlanMaster pm ON pm.PlanId = p.PlanId", null, null, true, 0, commandType: CommandType.Text);
                return ListofPlan;
            }
        }

        public List<UserCompany> ViewUserWithRole()
        {
            var result = (from a in _context.UserCompany
                          select new UserCompany
                               {
                                   UserCompId = a.UserCompId,
                                   Name = a.Name,
                                   Email = a.Email,
                                   RoleName = a.RoleName,
                                   TenantId = a.TenantId,
                                   UserId = a.UserId
                               }).ToList();
            return result;
        }
        public List<IdentityRole> ViewRole()
        {
            var roles = _context.Roles.ToList();
            return roles;
        }
		public async Task<bool> Update(ApplicationUser model)
		{
            var user = await userManager.FindByIdAsync(model.Id);
            user.Name = model.Name;
            user.Email = model.Email;
            var result = await userManager.UpdateAsync(user);

            //UpdateUserCompany
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            sqlcon.Open();
            var para = new DynamicParameters();
            para.Add("@Name", model.Name);
            para.Add("@Email", model.Email);
            para.Add("@UserId", model.Id);
            para.Add("@TenantId", tenantId);
            sqlcon.Execute("UPDATE UserCompany SET Name=@Name,Email=@Email where UserId=@UserId AND TenantId=@TenantId ", para, null, 0, CommandType.Text);
            sqlcon.Close();
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
		}
		public bool Delete(string userId)
		{
			var objFromDb = _context.ApplicationUser.FirstOrDefault(u => u.Id == userId);
			if (objFromDb == null)
			{
				return false;
				//return NotFound();
			}
			_context.ApplicationUser.Remove(objFromDb);
			_context.SaveChanges();

            //DeleteUserCompany
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            sqlcon.Open();
            var paraLedgerDelete = new DynamicParameters();
            paraLedgerDelete.Add("@UserId", userId);
            paraLedgerDelete.Add("@TenantId", tenantId);
            var valueScDelete = sqlcon.Query<long>("DELETE FROM UserCompany where UserId=@UserId AND TenantId=@TenantId", paraLedgerDelete, null, true, 0, commandType: CommandType.Text);
            sqlcon.Close();
            return true;
		}
        public async Task<ResponseMessage> ChangePassword(ChangePasswordModel model)
        {
            ResponseMessage message = new ResponseMessage();
            var userId = _httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
           var result1 = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if(result1.Succeeded)
            {
				message.Description = "Changed";
			}
            else
            {
				foreach (var error in result1.Errors)
				{
					message.Description = error.Description;
				}
			}
			return message;
        }
	}
}
