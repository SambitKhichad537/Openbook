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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Openbook.Repository.Repository
{
    public class TaxService : ITax
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public TaxService(ApplicationDbContext context, DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
		public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Tax
                               where progm.TaxName == name
                               select progm.TaxId).Count();
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
            var checkResult = (from progm in _context.Tax
                               where progm.TaxName == name
                               select progm.TaxId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Tax
                                    where progm.TaxName == name
                                    select progm.TaxId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int TaxId)
        {
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			try
			{

				if (sqlcon.State == ConnectionState.Closed)
				{
					sqlcon.Open();
				}
				SqlCommand cmd = new SqlCommand("TaxDelete", sqlcon);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlParameter para = new SqlParameter();
				para = cmd.Parameters.Add("@TaxId", SqlDbType.Int);
				para.Value = TaxId;
				para = cmd.Parameters.Add("@TenantId", SqlDbType.NVarChar);
				para.Value = tenantId;
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

        public async Task<List<TaxView>> GetAll()
        {
            var result = await (from a in _context.Tax
                                select new TaxView
                                {
                                    TaxId = a.TaxId,
                                    TaxName = a.TaxName,
                                    Rate = a.Rate,
                                    IsActive = a.IsActive
                                }).ToListAsync();
            return result;
        }

        public async Task<Tax> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TaxId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Tax>("SELECT *FROM Tax where TaxId=@TaxId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }
		public async Task<Tax> ViewTaxRate(int id)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TaxId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Tax>("SELECT *FROM Tax  where TaxId=@TaxId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
		}

		public async Task<int> Save(Tax model)
        {
            try
            {
                await _context.Tax.AddAsync(model);
                await _context.SaveChangesAsync();
                int id = model.TaxId;
                _context.Entry(model).State = EntityState.Detached;
                return id;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> Update(Tax model)
        {
            try
            {
                _context.Tax.Update(model);
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