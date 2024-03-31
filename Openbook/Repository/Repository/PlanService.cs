using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.SaasModels;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openbook.Repository.Repository
{
    public class PlanService : IPlans
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public PlanService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.PlanMaster
							   where progm.Name == name
                               select progm.PlanId).Count();
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
            var checkResult = (from progm in _context.PlanMaster
							   where progm.Name == name
                               select progm.PlanId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.PlanMaster
									where progm.Name == name
                                    select progm.PlanId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
				PlanMaster user = await _context.PlanMaster.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
        }

        public async Task<List<PlanMaster>> GetAll()
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var ListofPlan = sqlcon.Query<PlanMaster>("SELECT * FROM PlanMaster", null, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}
		public async Task<PlanMaster> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@PlanId", id);
				var ListofPlan = sqlcon.Query<PlanMaster>("SELECT * FROM PlanMaster where PlanId=@PlanId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }
        public PlanMasterView GetPlansCompany(string id)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@TenantId", id);
                var ListofPlan = sqlcon.Query<PlanMasterView>("SELECT dbo.PlanMaster.Name,PlanUpgrade.IsActive,PlanUpgrade.OrderNo,PlanUpgrade.PlanId FROM dbo.PlanMaster INNER JOIN dbo.PlanUpgrade ON dbo.PlanMaster.PlanId = dbo.PlanUpgrade.PlanId where PlanUpgrade.TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return ListofPlan;
            }
        }

        public async Task<int> Save(PlanMaster model)
        {
            await _context.PlanMaster.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.PlanId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(PlanMaster model)
        {
            _context.PlanMaster.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
            return true;
        }
    }
}