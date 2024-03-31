using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.Modules;
using Openbook.Data.Setting;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;
using System.Security.Claims;

namespace Openbook.Repository.Repository
{
    public class PriviliageService : IPriviliage
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private readonly IHttpContextAccessor _httpContext;
		private string tenantId;
		public PriviliageService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant, IHttpContextAccessor httpContext)
		{
			_context = context;
			_httpContext = httpContext;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName()
        {
            var checkResult = (from progm in _context.Priviliage
                               where progm.TenantId == tenantId
                               select progm.TenantId).Count();
            if (checkResult > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> Delete()
        {
			//DeleteLedgerPosting
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var paraLedgerDelete = new DynamicParameters();
				paraLedgerDelete.Add("@TenantId", tenantId);
				var valueScDelete = sqlcon.Query<long>("DELETE FROM Priviliage where TenantId=@TenantId", paraLedgerDelete, null, true, 0, commandType: CommandType.Text);
				return true;
			}
		}

		public async Task<List<Priviliage>> GetAll()
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var ListofPlan = sqlcon.Query<Priviliage>("SELECT *FROM Priviliage where TenantId='0'", null, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}
        public async Task<List<Priviliage>> GetAllWithTenant()
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Priviliage>("SELECT *FROM Priviliage where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}

        public async Task<Priviliage> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@PriviliageId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Priviliage>("SELECT *FROM Priviliage where PriviliageId=@PriviliageId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }
		public async Task<List<Priviliage>> GetUserRole(string role)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@RoleName", role);
				para.Add("TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Priviliage>("SELECT *FROM Priviliage where RoleName=@RoleName AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}

		public async Task<int> Save(Priviliage model)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var paraPayable = new DynamicParameters();
				paraPayable.Add("@RoleName", model.RoleName);
				paraPayable.Add("@Menu", model.Menu);
				paraPayable.Add("@IsActive", model.IsActive);
				paraPayable.Add("@TenantId", tenantId);
				var valuePayable = sqlcon.Query<int>("INSERT INTO Priviliage (RoleName,Menu,IsActive,TenantId)VALUES(@RoleName,@Menu,@IsActive,@TenantId)", paraPayable, null, true, 0, commandType: CommandType.Text);
			}
            return 1;
        }

        public async Task<bool> Update(Priviliage model)
        {
            _context.Priviliage.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
            return true;
        }
    }
}