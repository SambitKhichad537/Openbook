using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;
using System.Drawing;

namespace Openbook.Repository.Repository
{
	public class UnitService : IUnits
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public UnitService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Unit
                               where progm.UnitName == name
                               select progm.UnitId).Count();
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
            var checkResult = (from progm in _context.Unit
							   where progm.UnitName == name
                               select progm.UnitId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Unit
									where progm.UnitName == name
                                    select progm.UnitId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var checkResult = await (from progm in _context.Product
                                     where progm.UnitId == id
                                     select progm.UnitId).CountAsync();
            if (checkResult > 0)
            {
                return false;
            }
            else
            {
                Unit user = await _context.Unit.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<UnitView>> GetAll()
        {
            var result = await (from a in _context.Unit
								select new UnitView
                                {
                                    UnitId = a.UnitId,
                                    UnitName = a.UnitName
                                }).ToListAsync();
			return result;
		}

        public async Task<Unit> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@UnitId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Unit>("SELECT *FROM Unit where UnitId=@UnitId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(Unit model)
        {
            try
            {
                await _context.Unit.AddAsync(model);
                await _context.SaveChangesAsync();
                int id = model.UnitId;
                _context.Entry(model).State = EntityState.Detached;
                return id;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> Update(Unit model)
        {
            try
            {
                _context.Unit.Update(model);
                await _context.SaveChangesAsync();
                _context.Entry(model).State = EntityState.Detached;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}