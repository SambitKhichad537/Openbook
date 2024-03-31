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
	public class CurrencyService : ICurrency
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public CurrencyService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Currency
                               where progm.CurrencyName == name
                               select progm.CurrencyId).Count();
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
            var checkResult = (from progm in _context.Currency
							   where progm.CurrencyName == name
                               select progm.CurrencyId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Currency
									where progm.CurrencyName == name
                                    select progm.CurrencyId).FirstOrDefault();
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
                                     where progm.CategoriesId == id
                                     select progm.CategoriesId).CountAsync();
            if (checkResult > 0)
            {
                return false;
            }
            else
            {
				Currency user = await _context.Currency.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<CurrencyView>> GetAll()
        {
            var result = await (from a in _context.Currency
								select new CurrencyView
                                {
CurrencyId = a.CurrencyId,
                                    CurrencyName = a.CurrencyName,
									CurrencySymbol = a.CurrencySymbol
                                }).ToListAsync();
            return result;
        }

        public async Task<Currency> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@CurrencyId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Currency>("SELECT *FROM Currency where CurrencyId=@CurrencyId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(Currency model)
        {
            await _context.Currency.AddAsync(model);
            await _context.SaveChangesAsync();
			_context.Entry(model).State = EntityState.Detached;
			int id = model.CurrencyId;
            return id;
        }

        public async Task<bool> Update(Currency model)
        {
            _context.Currency.Update(model);
            await _context.SaveChangesAsync();
			_context.Entry(model).State = EntityState.Detached;
			return true;
        }
    }
}