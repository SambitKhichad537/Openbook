using Dapper;
using Microsoft.Data.SqlClient;
using Openbook.Data;
using Openbook.Data.Setting;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
	public class AccountReportService : IAccountReports
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public AccountReportService(ApplicationDbContext context, DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}

		public IList<AccountReportView> AccountTransaction(DateTime FromDate, DateTime ToDate)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@FromDate", FromDate);
				para.Add("@ToDate", ToDate);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<AccountReportView>("AccountTransaction", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}

		public IList<AccountReportView> BalanceSheet(DateTime fromDate, DateTime toDate)
		{
			throw new NotImplementedException();
		}

		public IList<AccountReportView> GeneralLedger(DateTime fromDate, DateTime toDate, int ledgerId)
		{
			throw new NotImplementedException();
		}

		public IList<AccountReportView> ProfitNLoss(DateTime fromDate, DateTime toDate)
		{
			throw new NotImplementedException();
		}

		public AccountReportView TrailBalance(int LedgerId, DateTime FromDate, DateTime ToDate)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
                para.Add("@LedgerId", LedgerId);
                para.Add("@FromDate", FromDate);
				para.Add("@ToDate", ToDate);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<AccountReportView>("SELECT ISNULL(SUM(Debit), 0) as Debit, ISNULL(SUM(Credit), 0) as Credit, ISNULL(SUM(Debit), 0) - ISNULL(SUM(Credit), 0) as Balance FROM LedgerPosting where LedgerId=@LedgerId AND Date BETWEEN @FromDate AND @ToDate AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
		}
        public AccountReportView Income(int LedgerId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@LedgerId", LedgerId);
                para.Add("@FromDate", FromDate);
                para.Add("@ToDate", ToDate);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<AccountReportView>("SELECT ISNULL(SUM(Credit), 0) - ISNULL(SUM(Debit), 0) as Credit FROM LedgerPosting where LedgerId=@LedgerId AND Date BETWEEN @FromDate AND @ToDate AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return ListofPlan;
            }
        }
        public AccountReportView Expenses(int LedgerId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@LedgerId", LedgerId);
                para.Add("@FromDate", FromDate);
                para.Add("@ToDate", ToDate);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<AccountReportView>("SELECT ISNULL(SUM(Debit), 0) - ISNULL(SUM(Credit), 0) as Debit FROM LedgerPosting where LedgerId=@LedgerId AND Date BETWEEN @FromDate AND @ToDate AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return ListofPlan;
            }
        }
        public AccountReportView GrossProit(DateTime FromDate, DateTime ToDate , string Type)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@FromDate", FromDate);
                para.Add("@ToDate", ToDate);
                para.Add("@Type", Type);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<AccountReportView>("SELECT ISNULL(SUM(dbo.LedgerPosting.Debit), 0) - ISNULL(SUM(dbo.LedgerPosting.Credit), 0) AS Debit FROM dbo.LedgerPosting INNER JOIN dbo.AccountLedger ON dbo.LedgerPosting.LedgerId = dbo.AccountLedger.LedgerId where LedgerPosting.Date BETWEEN @FromDate AND @ToDate AND AccountLedger.Type=@Type AND LedgerPosting.TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return ListofPlan;
            }
        }
        public AccountReportView NetLoss(DateTime FromDate, DateTime ToDate, string Type)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@FromDate", FromDate);
                para.Add("@ToDate", ToDate);
                para.Add("@Type", Type);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<AccountReportView>("SELECT ISNULL(SUM(dbo.LedgerPosting.Credit), 0) - ISNULL(SUM(dbo.LedgerPosting.Debit), 0) AS Credit FROM dbo.LedgerPosting INNER JOIN dbo.AccountLedger ON dbo.LedgerPosting.LedgerId = dbo.AccountLedger.LedgerId where LedgerPosting.Date BETWEEN @FromDate AND @ToDate AND AccountLedger.Type=@Type AND LedgerPosting.TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return ListofPlan;
            }
        }
        public AccountReportView TotalBalance(string Type, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
				para.Add("@Type", Type);
				para.Add("@FromDate", FromDate);
                para.Add("@ToDate", ToDate);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<AccountReportView>("TotalBalanceGroup", para, null, true, 0, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return ListofPlan;
            }
        }
        public AccountReportView TotalBalanceTrailBalance(DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@FromDate", FromDate);
                para.Add("@ToDate", ToDate);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<AccountReportView>("SELECT ISNULL(SUM(Debit), 0) as Debit, ISNULL(SUM(Credit), 0) as Credit, ISNULL(SUM(Debit), 0) - ISNULL(SUM(Credit), 0) as Balance FROM LedgerPosting where LedgerPosting.Date BETWEEN @FromDate AND @ToDate AND LedgerPosting.TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return ListofPlan;
            }
        }
    }
}
