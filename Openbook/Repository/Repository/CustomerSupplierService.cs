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
    public class CustomerSupplierService : ICustomerSupplier
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public CustomerSupplierService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name , string type)
        {
            var checkResult = (from progm in _context.CustomerSupplier
                               where progm.Name == name && progm.Type == type
                               select progm.CustomerSupplierId).Count();
            if (checkResult > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<int> CheckNameId(string name , string type)
        {
            var checkResult = (from progm in _context.CustomerSupplier
                               where progm.Name == name && progm.Type == type
                               select progm.CustomerSupplierId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.CustomerSupplier
                                    where progm.Name == name && progm.Type == type
                                    select progm.CustomerSupplierId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int CustomerSupplierId)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            try
            {

                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand cmd = new SqlCommand("CustomerSupplierDelete", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter para = new SqlParameter();
                para = cmd.Parameters.Add("@CustomerSupplierId", SqlDbType.Int);
                para.Value = CustomerSupplierId;
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

        public async Task<List<CustomerSupplierView>> GetAll(string type)
        {
            var result = await (from a in _context.CustomerSupplier
                                where a.Type == type
                                select new CustomerSupplierView
								{
                                    CustomerSupplierId = a.CustomerSupplierId,
									Name = a.Name,
                                    WorkPhone = a.WorkPhone,
                                    Mobile = a.Mobile,
                                    City = a.City,
                                    Address = a.Address,
                                    CompanyName = a.CompanyName,
                                    Email = a.Email,
									AddedDate = a.AddedDate
                                }).ToListAsync();
            return result;
        }

        public async Task<CustomerSupplier> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@CustomerSupplierId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<CustomerSupplier>("SELECT *FROM CustomerSupplier where CustomerSupplierId=@CustomerSupplierId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(CustomerSupplier model)
        {
            await _context.CustomerSupplier.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.CustomerSupplierId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(CustomerSupplier model)
        {
            _context.CustomerSupplier.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
            return true;
        }
    }
}