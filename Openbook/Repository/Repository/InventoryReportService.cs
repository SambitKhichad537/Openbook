using Dapper;
using Microsoft.Data.SqlClient;
using Openbook.Data;
using Openbook.Data.Inventory;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
	public class InventoryReportService :IInventoryReport
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public InventoryReportService(ApplicationDbContext context, DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
		public IList<InventoryViewFinal> StockReport(int CategoriesId, int ProductId)
		{
			IList<InventoryViewFinal> _UsersModel = new List<InventoryViewFinal>();
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlCommand cmd = new SqlCommand("StockReport", sqlcon);//call Stored Procedure
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@CategoriesId", CategoriesId);
			cmd.Parameters.AddWithValue("@ProductId", ProductId);
			cmd.Parameters.AddWithValue("@TenantId", tenantId);
			SqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				InventoryViewFinal _UserModel = new InventoryViewFinal();
				_UserModel.ProductCode = reader["ProductCode"].ToString();
				_UserModel.ProductName = reader["ProductName"].ToString();
				//_UserModel.batchNo = reader["batchNo"].ToString();
				_UserModel.UnitName = reader["UnitName"].ToString();
				//_UserModel.purchaseRate = Convert.ToDecimal(reader["purchaseRate"].ToString());
				_UserModel.PurQty = Convert.ToDecimal(reader["PurQty"].ToString());
				_UserModel.SalesQty = Convert.ToDecimal(reader["SalesQty"].ToString());
				_UserModel.Rate = Convert.ToDecimal(reader["Rate"].ToString());
				_UserModel.PurchaseStockBal = Convert.ToDecimal(reader["PurchaseStockBal"].ToString());
				_UserModel.SalesStockBalance = Convert.ToDecimal(reader["SalesStockBalance"].ToString());
				_UserModel.Stock = Convert.ToDecimal(reader["Stock"].ToString());
				_UserModel.Stockvalue = Convert.ToDecimal(reader["Stockvalue"].ToString());
				_UserModel.SalesRate = Convert.ToDecimal(reader["SalesRate"].ToString());
				_UsersModel.Add(_UserModel);
			}
			reader.Close();
			sqlcon.Close();
			return _UsersModel;
		}
		public async Task<IList<PurchaseMasterView>> PurchasebyItem(DateTime FromDate, DateTime ToDate, int ProductId)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@FromDate", FromDate);
				para.Add("@ToDate", ToDate);
				para.Add("@ProductId", ProductId);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<PurchaseMasterView>("ItemWisePurchase", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}
		public async Task<IList<SalesMasterView>> SalesbyItem(DateTime FromDate, DateTime ToDate, int ProductId)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@FromDate", FromDate);
				para.Add("@ToDate", ToDate);
				para.Add("@ProductId", ProductId);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<SalesMasterView>("SalesbyItem", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}
		public async Task<IList<PurchaseMasterView>> ReceivablesSummary(DateTime FromDate, DateTime ToDate)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@FromDate", FromDate);
				para.Add("@ToDate", ToDate);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<PurchaseMasterView>("ReceivablesSummary", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}
		public async Task<IList<SalesMasterView>> PayableSummary(DateTime FromDate, DateTime ToDate)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@FromDate", FromDate);
				para.Add("@ToDate", ToDate);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<SalesMasterView>("PayableSummary", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}
		public async Task<IList<ReceiptMasterView>> PayamentReceived(DateTime FromDate, DateTime ToDate)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@FromDate", FromDate);
				para.Add("@ToDate", ToDate);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<ReceiptMasterView>("PayamentReceived", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}
		public async Task<IList<PaymentMasterView>> PayamentMade(DateTime FromDate, DateTime ToDate)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@FromDate", FromDate);
				para.Add("@ToDate", ToDate);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<PaymentMasterView>("PayamentMade", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}
	}
}
