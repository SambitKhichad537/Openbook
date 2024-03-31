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
	public class ProductService : IProduct
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public ProductService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Product
                               where progm.ProductName == name
                               select progm.ProductId).Count();
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
            var checkResult = (from progm in _context.Product
                               where progm.ProductName == name
                               select progm.ProductId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Product
                                    where progm.ProductName == name
                                    select progm.ProductId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int ProductId)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            try
            {

                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand cmd = new SqlCommand("ProductDelete", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter para = new SqlParameter();
                para = cmd.Parameters.Add("@ProductId", SqlDbType.Int);
                para.Value = ProductId;
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

        public async Task<List<ProductView>> GetAll()
        {
            var result = await (from a in _context.Product
                                join b in _context.Categories on a.CategoriesId equals b.CategoriesId
                                join c in _context.Unit on a.UnitId equals c.UnitId
                                select new ProductView
                                {
									ProductId = a.ProductId,
									ProductName = a.ProductName,
                                    ProductCode = a.ProductCode,
                                    PurchaseRate = a.PurchaseRate,
                                    SalesRate = a.SalesRate,
                                    UnitId = a.UnitId,
                                    TaxId = a.TaxId,
                                    UnitName = c.UnitName,
									CategoryName = b.CategoryName,
                                    Image = a.Image,
                                    LedgerId = a.PurchaseAccount,
                                    SalesAccount = a.SalesAccount,
                                    AddedDate = a.AddedDate
                                }).ToListAsync();
            return result;
        }
		public async Task<List<ProductView>> GetAllSales()
		{
			var result = await (from a in _context.Product
								join b in _context.Categories on a.CategoriesId equals b.CategoriesId
								join c in _context.Unit on a.UnitId equals c.UnitId
								select new ProductView
								{
									ProductId = a.ProductId,
									ProductName = a.ProductName,
									ProductCode = a.ProductCode,
									PurchaseRate = a.PurchaseRate,
									SalesRate = a.SalesRate,
									UnitId = a.UnitId,
									TaxId = a.TaxId,
									UnitName = c.UnitName,
									CategoryName = b.CategoryName,
									Image = a.Image,
									LedgerId = a.SalesAccount,
									PurchaseAccount = a.PurchaseAccount,
									AddedDate = a.AddedDate
								}).ToListAsync();
			return result;
		}

		public async Task<Product> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@ProductId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Product>("SELECT *FROM Product where ProductId=@ProductId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(Product model)
        {
            await _context.Product.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.ProductId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(Product model)
        {
            _context.Product.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
			return true;
        }
		public string GetProductNo()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
                para.Add("@TenantId", tenantId);
				var val = sqlcon.Query<string>("SELECT ISNULL(MAX(Snno+1),1) as Snno FROM Product where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return val;
			}
		}
	}
}