using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.Inventory;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;
using System.Drawing;

namespace Openbook.Repository.Repository
{
	public class TimezoneService : ITimeZone
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public TimezoneService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.TimeZones
                               where progm.Name == name
                               select progm.TimeZoneId).Count();
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
            var checkResult = (from progm in _context.TimeZones
							   where progm.Name == name
                               select progm.TimeZoneId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.TimeZones
									where progm.Name == name
                                    select progm.TimeZoneId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var checkResult = await (from progm in _context.Company
                                     where progm.TimeZoneId == id
                                     select progm.TimeZoneId).CountAsync();
            if (checkResult > 0)
            {
                return false;
            }
            else
            {
				TimeZones user = await _context.TimeZones.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<TimeZonesView>> GetAll()
        {
            var result = await (from a in _context.TimeZones
								select new TimeZonesView
								{
                                    TimeZoneId = a.TimeZoneId,
									Name = a.Name
                                }).ToListAsync();
            return result;
		}

        public async Task<TimeZones> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TimeZoneId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<TimeZones>("SELECT *FROM TimeZones where TimeZoneId=@TimeZoneId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(TimeZones model)
        {
            await _context.TimeZones.AddAsync(model);
            await _context.SaveChangesAsync();
			_context.Entry(model).State = EntityState.Detached;
			int id = model.TimeZoneId;
            return id;
        }

        public async Task<bool> Update(TimeZones model)
        {
            _context.TimeZones.Update(model);
            await _context.SaveChangesAsync();
			_context.Entry(model).State = EntityState.Detached;
			return true;
        }
    }
}