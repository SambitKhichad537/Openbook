using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.HrPayroll;
using Openbook.Data.HrPayrollModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
    public class DesignationService : IDesignation
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseConnection _conn;
        private string tenantId;
        public DesignationService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
        {
            _context = context;
            _conn = conn;
            tenantId = servicioTenant.ObtenerTenant();
        }
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Designation
                               where progm.DesignationName == name
                               select progm.DesignationId).Count();
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
            var checkResult = (from progm in _context.Designation
							   where progm.DesignationName == name
                               select progm.DesignationId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Designation
									where progm.DesignationName == name
                                    select progm.DesignationId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int DesignationId)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand cmd = new SqlCommand("IF NOT EXISTS(SELECT DesignationId from Employee where DesignationId=@DesignationId and TenantId=@TenantId) DELETE FROM Designation where DesignationId=@DesignationId and TenantId=@TenantId", sqlcon);
                cmd.CommandType = CommandType.Text;
                SqlParameter para = new SqlParameter();
                para = cmd.Parameters.Add("@DesignationId", SqlDbType.Int);
                para.Value = DesignationId;
                para = cmd.Parameters.Add("@TenantId", SqlDbType.NVarChar);
                para.Value = tenantId;
                long rowAffacted = cmd.ExecuteNonQuery();
                if (rowAffacted > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                sqlcon.Close();
            }
        }

        public async Task<List<DesignationView>> GetAll()
        {
            var result = await(from a in _context.Designation
                               select new DesignationView
							   {
                                   DesignationId = a.DesignationId,
                                   DesignationName = a.DesignationName,
                                   LeaveDays = a.LeaveDays,
								   AdvanceAmount = a.AdvanceAmount,
                                   Narration = a.Narration
							   }).ToListAsync();
            return result;
        }

        public async Task<Designation> GetbyId(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@DesignationId", id);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<Designation>("SELECT *FROM Designation where DesignationId=@DesignationId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return ListofPlan;
            }
        }

        public async Task<int> Save(Designation model)
        {
            await _context.Designation.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.DesignationId;
            return id;
        }

        public async Task<bool> Update(Designation model)
        {
            _context.Designation.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
