using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.HrPayroll;
using Openbook.Data.HrPayrollModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;
using System.Drawing;

namespace Openbook.Repository.Repository
{
    public class PayheadService : IPayHead
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseConnection _conn;
        private string tenantId;
        public PayheadService(ApplicationDbContext context , IServicioTenant servicioTenant , DatabaseConnection conn)
        {
            _context = context;
            _conn = conn;
            tenantId = servicioTenant.ObtenerTenant();
        }
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.PayHead
                               where progm.PayHeadName == name
                               select progm.PayHeadId).Count();
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
            var checkResult = (from progm in _context.PayHead
							   where progm.PayHeadName == name
                               select progm.PayHeadId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.PayHead
									where progm.PayHeadName == name
                                    select progm.PayHeadId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var checkResult = await(from progm in _context.SalaryPackageDetails
                                    where progm.PayHeadId == id
                                    select progm.PayHeadId).CountAsync();
            if (checkResult > 0)
            {
                return false;
            }
            else
            {
                PayHead user = await _context.PayHead.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<PayHeadView>> GetAll()
        {
            var result = await(from a in _context.PayHead
							   select new PayHeadView
							   {
                                   PayHeadId = a.PayHeadId,
                                   PayHeadName = a.PayHeadName,
                                   Type = a.Type
							   }).ToListAsync();
            return result;
        }

        public async Task<PayHead> GetbyId(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
				para.Add("@PayHeadId", id);
				para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<PayHead>("SELECT *FROM PayHead where PayHeadId=@PayHeadId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return ListofPlan;
            }
        }

        public async Task<int> Save(PayHead model)
        {
            await _context.PayHead.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.PayHeadId;
            return id;
        }

        public async Task<bool> Update(PayHead model)
        {
            _context.PayHead.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
