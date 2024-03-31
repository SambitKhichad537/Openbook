using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.SaasModels;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;
using System.Drawing;

namespace Openbook.Repository.Repository
{
    public class PlanUpgradeRepository : IPlanUpgrade
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public PlanUpgradeRepository(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@TenantId", name);
                return sqlcon.Query<bool>("SELECT PlanId FROM PlanUpgrade where TenantId=@TenantId", para, null, true, 0, CommandType.Text).SingleOrDefault();
            }
        }
        public bool CheckNameId(int planid,string name)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@PlanId", planid);
                para.Add("@TenantId", name);
                return sqlcon.Query<bool>("SELECT PlanId FROM PlanUpgrade where PlanId=@PlanId AND TenantId=@TenantId", para, null, true, 0, CommandType.Text).SingleOrDefault();
            }
        }

        public async Task<bool> Delete(int id)
        {
            PlanUpgrade user = await _context.PlanUpgrade.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
        }

        public async Task<List<PlanUpgrade>> GetAll()
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var ListofPlan = sqlcon.Query<PlanUpgrade>("SELECT *FROM PlanUpgrade", null, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}

        public async Task<PlanUpgrade> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TenantId", id);
				var ListofPlan = sqlcon.Query<PlanUpgrade>("SELECT *FROM PlanUpgrade where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<bool> Save(PlanUpgrade model)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            sqlcon.Open();
            var paraCash = new DynamicParameters();
            paraCash.Add("@PlanId", model.PlanId);
			paraCash.Add("@OrderNo", model.OrderNo);
			paraCash.Add("@IsActive", model.IsActive);
			paraCash.Add("@TenantId", model.TenantId);
            var valuePayable = sqlcon.Query<bool>("INSERT INTO PlanUpgrade (PlanId,TenantId,OrderNo,IsActive)VALUES(@PlanId,@TenantId,@OrderNo,@IsActive)", paraCash, null, true, 0, commandType: CommandType.Text);
            sqlcon.Close();
            return true;

        }

        public async Task<bool> Update(PlanUpgrade model)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            sqlcon.Open();
            var paraCash = new DynamicParameters();
            paraCash.Add("@PlanId", model.PlanId);
			paraCash.Add("@OrderNo", model.OrderNo);
			paraCash.Add("@IsActive", model.IsActive);
			paraCash.Add("@TenantId", model.TenantId);
            var valuePayable = sqlcon.Query<bool>("UPDATE PlanUpgrade SET PlanId=@PlanId,OrderNo=@OrderNo,IsActive=@IsActive where TenantId=@TenantId", paraCash, null, true, 0, commandType: CommandType.Text);
            sqlcon.Close();
            return true;
        }
        public async Task<bool> UpdateStatus(PlanUpgrade model)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            sqlcon.Open();
            var paraCash = new DynamicParameters();
            paraCash.Add("@TenantId", model.TenantId);
            paraCash.Add("@IsActive", model.IsActive);
            var valuePayable = sqlcon.Query<bool>("UPDATE PlanUpgrade SET IsActive=@IsActive where TenantId=@TenantId", paraCash, null, true, 0, commandType: CommandType.Text);
            sqlcon.Close();
            return true;
        }
		public async Task<PlanMasterView> TotalUsers()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var ListofPlan = sqlcon.Query<PlanMasterView>("SELECT COUNT(TenantId) AS TotalUser FROM PlanUpgrade", null, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
		}
		public async Task<PlanMasterView> PaidUsers()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var ListofPlan = sqlcon.Query<PlanMasterView>("SELECT COUNT(TenantId) AS TotalUser FROM PlanUpgrade where IsActive='True'", null, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
		}
		public async Task<PlanMasterView> TotalOrderAmounts()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var ListofPlan = sqlcon.Query<PlanMasterView>("SELECT        SUM(Price) as Price FROM dbo.PlanMaster INNER JOIN dbo.PlanUpgrade ON dbo.PlanMaster.PlanId = dbo.PlanUpgrade.PlanId", null, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
		}
		public async Task<PlanMasterView> TotalPlans()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
                var ListofPlan = sqlcon.Query<PlanMasterView>("SELECT COUNT(PlanId) AS PlanId FROM PlanMaster", null, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
 			}
		}
        public async Task<PlanMasterView> CheckPlanActiveorNot()
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<PlanMasterView>("PlanCheckValidorNot", para, null, true, 0, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return ListofPlan;
            }
        }
		public async Task<int> CountProduct()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TenantId", tenantId);
				var val = sqlcon.Query<int>("SELECT COUNT(ProductId) AS ProductId FROM Product where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return val;
			}
		}
		public async Task<int> CountInvoice()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TenantId", tenantId);
				var val = sqlcon.Query<int>("SELECT COUNT(SalesMasterId) AS SalesMasterId FROM SalesMaster where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return val;
			}
		}
		public async Task<int> CountSupplierCustomer(string Type)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@Type", Type);
                para.Add("@TenantId", tenantId);
                var val = sqlcon.Query<int>("SELECT COUNT(CustomerSupplierId) AS CustomerSupplierId FROM CustomerSupplier where Type=@Type AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return val;
            }
        }
    }
}