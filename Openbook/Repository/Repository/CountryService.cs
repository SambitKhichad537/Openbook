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

namespace Openbook.Repository.Repository
{
	public class CountryService : ICountry
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public CountryService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Country
                               where progm.Name == name
                               select progm.CountryId).Count();
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
            var checkResult = (from progm in _context.Country
                               where progm.Name == name
                               select progm.CountryId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Country
                                    where progm.Name == name
                                    select progm.CountryId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var checkResult = await (from progm in _context.CustomerSupplier
                                     where progm.CountryId == id
                                     select progm.CountryId).CountAsync();
            if (checkResult > 0)
            {
                return false;
            }
            else
            {
                Country user = await _context.Country.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<Country>> GetAll()
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var ListofPlan = sqlcon.Query<Country>("SELECT *FROM Country", null, null, true, 0, commandType: CommandType.Text).ToList();
                return ListofPlan;
            }
        }

        public async Task<Country> GetbyId(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@CountryId", id);
                var ListofPlan = sqlcon.Query<Country>("SELECT *FROM Country where CountryId=@CountryId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return ListofPlan;
            }
        }

        public async Task<int> Save(Country model)
        {
            await _context.Country.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.CountryId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(Country model)
        {
            _context.Country.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
            return true;
        }
    }
}