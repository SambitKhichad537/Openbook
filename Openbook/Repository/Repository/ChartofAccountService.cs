using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.Inventory;
using Openbook.Data.Setting;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
	public class ChartofAccountService : IChartofAccount
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public ChartofAccountService(ApplicationDbContext context , DatabaseConnection conn , IServicioTenant servicioTenant)
		{
			_context = context;
            _conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.AccountLedger
                               where progm.LedgerName == name
                               select progm.LedgerId).Count();
            if (checkResult > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<int> CheckNameId(string name)
        {
            var checkResult = (from progm in _context.AccountLedger
                               where progm.LedgerName == name
                               select progm.LedgerId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.AccountLedger
                                    where progm.LedgerName == name
                                    select progm.LedgerId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int LedgerId)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            try
            {

                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand cmd = new SqlCommand("AccountLedgerDelete", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter para = new SqlParameter();
                para = cmd.Parameters.Add("@LedgerId", SqlDbType.Int);
                para.Value = LedgerId;
                para = cmd.Parameters.Add("@TenantId", SqlDbType.NVarChar);
                para.Value = tenantId;
                int rowAffacted = cmd.ExecuteNonQuery();
                if (rowAffacted > 0)
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                sqlcon.Close();
            }
        }

        public async Task<List<AccountLedgerView>> GetAll()
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var ListofPlan = sqlcon.Query<AccountLedgerView>("SELECT *FROM AccountLedger where GroupUnder=0", null, null, true, 0, commandType: CommandType.Text).ToList();
                return ListofPlan;
            }
        }
		public async Task<List<AccountLedgerView>> GetParentAccount(int id)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@LedgerId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<AccountLedgerView>("SELECT *FROM AccountLedger where LedgerId=@LedgerId AND TenantId='0' OR TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}
		public async Task<List<AccountLedgerView>> GetAllChartofAccount()
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<AccountLedgerView>("SELECT *FROM AccountLedger where TenantId='0' OR TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).ToList();
                return ListofPlan;
            }
        }
		public async Task<List<AccountLedgerView>> GetAllChartofAccountsearch(string name)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
                para.Add("@LedgerName", name);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<AccountLedgerView>("SELECT *FROM AccountLedger where LedgerName=@LedgerName AND TenantId='0' OR TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}
		public async Task<AccountLedger> GetGroup(int id)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@LedgerId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<AccountLedger>("SELECT Type FROM AccountLedger where LedgerId=@LedgerId AND TenantId='0' OR TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
		}
		public async Task<AccountLedger> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@LedgerId", id);
				var ListofPlan = sqlcon.Query<AccountLedger>("SELECT *FROM AccountLedger where LedgerId=@LedgerId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(AccountLedger model)
        {
            await _context.AccountLedger.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.LedgerId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(AccountLedger model)
        {
            _context.AccountLedger.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
			return true;
        }
		public string GetAccountLedgerNo()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TenantId", tenantId);
				var val = sqlcon.Query<string>("SELECT ISNULL(MAX(LedgerCode+1),1) as Snno FROM AccountLedger where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return val;
			}
		}
	}
}