using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.Inventory;
using Openbook.Data.SaasModels;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;
using System.Drawing;

namespace Openbook.Repository.Repository
{
	public class CompanyService : ICompany
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public CompanyService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
		public async Task<Company> GetById()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Company>("SELECT *FROM Company where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
		}
        public List<Company> GetCompany(string id)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@TenantId", id);
                var ListofPlan = sqlcon.Query<Company>("SELECT TOP 1 *FROM Company where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).ToList();
                return ListofPlan;
            }
        }
        public async Task<int> Save(Company company)
		{
			using (var transaction = this._context.Database.BeginTransaction())
			{
				try
				{
				 await _context.Company.AddAsync(company);
					await _context.SaveChangesAsync();
					int id = company.CompanyId;
					//AddPlanUpgrade
					PlanUpgrade model = new PlanUpgrade();
                    model.PlanId = 1;
                    model.IsActive = true;
					string elementId = Guid.NewGuid().ToString("N");
					model.OrderNo = elementId;
					model.TenantId = tenantId;
                    _context.PlanUpgrade.Add(model);
                    _context.SaveChanges();
                    _context.Entry(model).State = EntityState.Detached;
                    //AddFinancialYear
                    FinancialYear year = new FinancialYear();
					year.FromDate = company.StartDate;
					year.ToDate = DateTime.UtcNow.AddDays(+365);
					year.FiscalYear = string.Empty;
					year.TenantId = tenantId;
					year.AddedDate = DateTime.Now;
					_context.FinancialYear.Add(year);
					_context.SaveChanges();
					transaction.Commit();
					_context.Entry(company).State = EntityState.Detached;
					return id;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					return 0;
				}
			}
		}

		public async Task<bool> Update(Company company)
		{
			using (var transaction = this._context.Database.BeginTransaction())
			{
				try
				{
					_context.Company.Update(company);
					await _context.SaveChangesAsync();
					transaction.Commit();
					_context.Entry(company).State = EntityState.Detached;
					return true;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					return false;
				}
			}
		}
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
