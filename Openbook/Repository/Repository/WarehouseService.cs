using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
    public class WarehouseService : IWarehouse
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public WarehouseService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Warehouse
                               where progm.Name == name
                               select progm.WarehouseId).Count();
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
            var checkResult = (from progm in _context.Warehouse
                               where progm.Name == name
                               select progm.WarehouseId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Warehouse
                                    where progm.Name == name
                                    select progm.WarehouseId).FirstOrDefault();
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
                Categories user = await _context.Categories.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<WarehouseView>> GetAll()
        {
            var result = await (from a in _context.Warehouse
                                select new WarehouseView
                                {
                                    WarehouseId = a.WarehouseId,
                                    Name = a.Name,
                                    Country = a.Country,
                                    Mobile = a.Mobile,
                                    City = a.City,
                                    Email = a.Email
                                }).ToListAsync();
            return result;
        }

        public async Task<Warehouse> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@WarehouseId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Warehouse>("SELECT *FROM Warehouse where WarehouseId=@WarehouseId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(Warehouse model)
        {
            await _context.Warehouse.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.WarehouseId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(Warehouse model)
        {
            _context.Warehouse.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
            return true;
        }
    }
}