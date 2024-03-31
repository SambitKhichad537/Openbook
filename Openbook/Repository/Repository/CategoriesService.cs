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
    public class CategoriesService : ICategories
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public CategoriesService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Categories
                               where progm.CategoryName == name
                               select progm.CategoriesId).Count();
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
            var checkResult = (from progm in _context.Categories
                               where progm.CategoryName == name
                               select progm.CategoriesId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Categories
                                    where progm.CategoryName == name
                                    select progm.CategoriesId).FirstOrDefault();
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

        public async Task<List<CategoriesView>> GetAll()
        {
            var result = await (from a in _context.Categories
                                select new CategoriesView
                                {
                                    CategoriesId = a.CategoriesId,
                                    CategoryName = a.CategoryName,
                                    Image = a.Image,
                                    AddedDate = a.AddedDate
                                }).ToListAsync();
            return result;
        }

        public async Task<Categories> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@CategoriesId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Categories>("SELECT *FROM Categories where CategoriesId=@CategoriesId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(Categories model)
        {
            await _context.Categories.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.CategoriesId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(Categories model)
        {
            _context.Categories.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
            return true;
        }
    }
}