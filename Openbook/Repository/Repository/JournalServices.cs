using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.AccountModel;
using Openbook.Data.Inventory;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
    public class JournalServices : IJournal
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
        
        public JournalServices(ApplicationDbContext context, DatabaseConnection conn , IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<List<JournalMasterView>> GetAll(string strStatus , string strDate)
        {
                using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
                {
                    var para = new DynamicParameters();
                    para.Add("@Status", strStatus);
                    para.Add("@TenantId", tenantId);
                    var ListofPlan = sqlcon.Query<JournalMasterView>("SELECT JournalMasterId,Date,VoucherNo,Narration,ReferenceNo,Amount,Status,UserId   FROM JournalMaster  where JournalMaster.Status=(CASE WHEN @Status='All' THEN JournalMaster.Status ELSE @Status END) AND TenantId=@TenantId order by Date DESC", para, null, true, 0, commandType: CommandType.Text).ToList();
                    return ListofPlan;
                }
        }
        public async Task<List<JournalMasterView>> GetAllByDate(string strStatus, DateTime startDate , DateTime endDate)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@Status", strStatus);
                para.Add("@startDate", startDate);
                para.Add("@endDate", endDate);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<JournalMasterView>("SELECT JournalMasterId,Date,VoucherNo,Narration,ReferenceNo,Amount,Status,UserId   FROM JournalMaster  where JournalMaster.Status=(CASE WHEN @Status='All' THEN JournalMaster.Status ELSE @Status END) AND date between @startDate and @endDate AND TenantId=@TenantId order by Date DESC", para, null, true, 0, commandType: CommandType.Text).ToList();
                return ListofPlan;
            }
        }
        public async Task<string> GetSerialNo()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TenantId", tenantId);
				var val = sqlcon.Query<string>("SELECT ISNULL(MAX(SerialNo+1),1) as VoucherNo FROM JournalMaster where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return val;
			}
		}
		public decimal CheckLedgerBalance(int LedgerId)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@LedgerId", LedgerId);
				para.Add("@TenantId", tenantId);
				var val = sqlcon.Query<decimal>("SELECT isnull(SUM(Debit),0)- isnull(sum(Credit),0)  FROM LedgerPosting WHERE LedgerId=@LedgerId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return val;
			}
		}
		public async Task<int> Draft(JournalMaster model)
		{
			//using (var transaction = this._context.Database.BeginTransaction())
			//{
				try
				{
					if (model.JournalMasterId > 0)
					{
						_context.JournalMaster.Update(model);
						await _context.SaveChangesAsync();
						_context.Entry(model).State = EntityState.Detached;
						int id = model.JournalMasterId;
						//JournalDetails table
						foreach (var item in model.listOrder)
						{
							//AddJournalDetails
							JournalDetails details = new JournalDetails();
							if (item.JournalDetailsId == 0)
							{
								if (item.LedgerId > 0)
								{
									details.JournalMasterId = id;
									details.LedgerId = item.LedgerId;
									details.ProjectId = item.ProjectId;
									details.Debit = item.Debit;
									details.Credit = item.Credit;
									details.Narration = item.Narration;
									details.TenantId = tenantId;
									_context.JournalDetails.Add(details);
									await _context.SaveChangesAsync();
								}
							}
							else
							{
								if (item.LedgerId > 0)
								{
									details.JournalDetailsId = item.JournalDetailsId;
									details.JournalMasterId = id;
									details.LedgerId = item.LedgerId;
									details.ProjectId = item.ProjectId;
									details.Debit = item.Debit;
									details.Credit = item.Credit;
									details.Narration = item.Narration;
									details.TenantId = tenantId;
									_context.JournalDetails.Update(details);
									await _context.SaveChangesAsync();
									_context.Entry(details).State = EntityState.Detached;
								}
							}

						}
						foreach (var deleteitem in model.listDelete)
						{
							JournalDetails x = _context.JournalDetails.Find(deleteitem.DeleteJournalDetailsId);
							_context.JournalDetails.Remove(x);
							await _context.SaveChangesAsync();
							_context.Entry(x).State = EntityState.Detached;
						}
						return id;
					}
					else
					{

						_context.JournalMaster.Add(model);
						await _context.SaveChangesAsync();
						int id = model.JournalMasterId;
						_context.Entry(model).State = EntityState.Detached;
						//JournalDetails table
						foreach (var item in model.listOrder)
						{
							//AddJournalDetails
							JournalDetails details = new JournalDetails();
							if (item.LedgerId > 0)
							{
								details.JournalMasterId = id;
								details.LedgerId = item.LedgerId;
								details.ProjectId = item.ProjectId;
								details.Debit = item.Debit;
								details.Credit = item.Credit;
								details.Narration = item.Narration;
								details.TenantId = tenantId;
								_context.JournalDetails.Add(details);
								await _context.SaveChangesAsync();
								int intPurchaseDId = details.JournalDetailsId;
								_context.Entry(details).State = EntityState.Detached;

							}
						}
						return id;
					}
					//transaction.Commit();
				}
				catch (Exception ex)
				{
					//transaction.Rollback();
					return 0;
				}
			//}
			}
		public async Task<int> Publish(JournalMaster model)
		{
			//using (var transaction = this._context.Database.BeginTransaction())
			//{
				try
				{
					_context.JournalMaster.Update(model);
					await _context.SaveChangesAsync();
					int id = model.JournalMasterId;
					_context.Entry(model).State = EntityState.Detached;
					//JournalDetails table
					foreach (var item in model.listOrder)
					{
						///AddJournalDetails
						JournalDetails details = new JournalDetails();
						if (item.JournalDetailsId == 0)
						{
							if (item.LedgerId > 0)
							{
								details.JournalMasterId = id;
								details.LedgerId = item.LedgerId;
								details.ProjectId = item.ProjectId;
								details.Debit = item.Debit;
								details.Credit = item.Credit;
								details.Narration = item.Narration;
								details.TenantId = tenantId;
								_context.JournalDetails.Add(details);
								await _context.SaveChangesAsync();
								_context.Entry(details).State = EntityState.Detached;
							}
						}
						else
						{
							if (item.LedgerId > 0)
							{
								details.JournalDetailsId = item.JournalDetailsId;
								details.JournalMasterId = id;
								details.LedgerId = item.LedgerId;
								details.ProjectId = item.ProjectId;
								details.Debit = item.Debit;
								details.Credit = item.Credit;
								details.Narration = item.Narration;
								details.TenantId = tenantId;
								_context.JournalDetails.Update(details);
								await _context.SaveChangesAsync();
								_context.Entry(details).State = EntityState.Detached;
							}
						}
					}
					foreach (var item in model.listOrder)
					{
						if (item.LedgerId > 0)
						{
							//LedgerPosting
							LedgerPosting cashPosting = new LedgerPosting();
							cashPosting.Date = model.Date;
							cashPosting.LedgerId = item.LedgerId;
							cashPosting.Debit = item.Debit;
							cashPosting.Credit = item.Credit;
							cashPosting.VoucherNo = model.VoucherNo;
							cashPosting.DetailsId = model.JournalMasterId;
							cashPosting.YearId = model.FinancialYearId;
							cashPosting.InvoiceNo = model.VoucherNo;
							cashPosting.VoucherTypeId = model.VoucherTypeId;
							cashPosting.TenantId = model.TenantId;
							cashPosting.LongReference = model.Narration;
							cashPosting.ReferenceN = item.Narration;
							cashPosting.ChequeNo = string.Empty;
							cashPosting.ChequeDate = string.Empty;
							cashPosting.AddedDate = DateTime.UtcNow;
							_context.LedgerPosting.Add(cashPosting);
							await _context.SaveChangesAsync();
							_context.Entry(cashPosting).State = EntityState.Detached;
						}
					}
					foreach (var deleteitem in model.listDelete)
					{
						JournalDetails x = _context.JournalDetails.Find(deleteitem.DeleteJournalDetailsId);
						_context.JournalDetails.Remove(x);
						await _context.SaveChangesAsync();
						_context.Entry(x).State = EntityState.Detached;
					}
					//transaction.Commit();
					return id;
				}
				catch (Exception)
				{
					//transaction.Rollback();
					return 0;
				}
			//}
		}
		public async Task<int> Update(JournalMaster model)
		{
			//using (var transaction = this._context.Database.BeginTransaction())
			//{
				try
				{
					_context.JournalMaster.Update(model);
					await _context.SaveChangesAsync();
					int id = model.JournalMasterId;
					_context.Entry(model).State = EntityState.Detached;

					
					//JournalDetails table
					foreach (var item in model.listOrder)
					{
						///AddJournalDetails
						JournalDetails details = new JournalDetails();
						if (item.JournalDetailsId == 0)
						{
							if (item.LedgerId > 0)
							{
								details.JournalMasterId = id;
								details.LedgerId = item.LedgerId;
								details.ProjectId = item.ProjectId;
								details.Debit = item.Debit;
								details.Credit = item.Credit;
								details.Narration = item.Narration;
								details.TenantId = tenantId;
								_context.JournalDetails.Add(details);
								await _context.SaveChangesAsync();
								_context.Entry(details).State = EntityState.Detached;
							}
						}
						else
						{
							if (item.LedgerId > 0)
							{
								details.JournalDetailsId = item.JournalDetailsId;
								details.JournalMasterId = id;
								details.LedgerId = item.LedgerId;
								details.ProjectId = item.ProjectId;
								details.Debit = item.Debit;
								details.Credit = item.Credit;
								details.Narration = item.Narration;
								details.TenantId = tenantId;
								_context.JournalDetails.Update(details);
								await _context.SaveChangesAsync();
								_context.Entry(details).State = EntityState.Detached;
							}
						}
					}
					//DeleteLedgerPosting
					using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
					{
						var paraScDelete = new DynamicParameters();
						paraScDelete.Add("@DetailsId", model.JournalMasterId);
						paraScDelete.Add("@VoucherTypeId", model.VoucherTypeId);
						paraScDelete.Add("@TenantId", model.TenantId);
						var valueScDelete = sqlcon.Query<int>("DELETE FROM LedgerPosting where DetailsId=@DetailsId AND VoucherTypeId=@VoucherTypeId AND TenantId=@TenantId", paraScDelete, null, true, 0, commandType: CommandType.Text);
					}
					foreach (var item in model.listOrder)
					{
						if (item.LedgerId > 0)
						{
							//LedgerPosting
							LedgerPosting cashPosting = new LedgerPosting();
							cashPosting.Date = model.Date;
							cashPosting.LedgerId = item.LedgerId;
							cashPosting.Debit = item.Debit;
							cashPosting.Credit = item.Credit;
							cashPosting.VoucherNo = model.VoucherNo;
							cashPosting.DetailsId = model.JournalMasterId;
							cashPosting.YearId = model.FinancialYearId;
							cashPosting.InvoiceNo = model.VoucherNo;
							cashPosting.VoucherTypeId = model.VoucherTypeId;
							cashPosting.TenantId = model.TenantId;
							cashPosting.LongReference = model.Narration;
							cashPosting.ReferenceN = item.Narration;
							cashPosting.ChequeNo = string.Empty;
							cashPosting.ChequeDate = string.Empty;
							cashPosting.AddedDate = DateTime.UtcNow;
							_context.LedgerPosting.Add(cashPosting);
							await _context.SaveChangesAsync();
							_context.Entry(cashPosting).State = EntityState.Detached;
						}
					}

					foreach (var deleteitem in model.listDelete)
					{
						JournalDetails x = _context.JournalDetails.Find(deleteitem.DeleteJournalDetailsId);
						_context.JournalDetails.Remove(x);
						await _context.SaveChangesAsync();
						_context.Entry(x).State = EntityState.Detached;
					}
					//transaction.Commit();
					return id;
				}
				catch (Exception)
				{
					//transaction.Rollback();
					return 0;
				}
			//}
		}
		public async Task<JournalMasterView> JournalView(int id)
		{
			var result = await (from a in _context.JournalMaster
								where a.JournalMasterId == id
								select new JournalMasterView
								{
									JournalMasterId = a.JournalMasterId,
									Amount = a.Amount,
									Narration = a.Narration,
									VoucherNo = a.VoucherNo,
									Status = a.Status,
									Date = a.Date,
									SerialNo = a.SerialNo,
									ReferenceNo = a.ReferenceNo,
									UserId = a.UserId,
									AddedDate = a.AddedDate
								}).FirstOrDefaultAsync();
			return result;
		}
		public async Task<List<JournalDetailsView>> JournalDetailsView(int id)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@JournalMasterId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<JournalDetailsView>("SELECT        DISTINCT convert(int,ROW_NUMBER() OVER (ORDER BY JournalMasterId)) AS [Id],dbo.JournalDetails.JournalDetailsId, dbo.JournalDetails.Credit, dbo.JournalDetails.Debit, dbo.JournalDetails.Narration, dbo.AccountLedger.LedgerId, dbo.AccountLedger.LedgerName, dbo.JournalDetails.ProjectId FROM dbo.JournalDetails INNER JOIN dbo.AccountLedger ON dbo.JournalDetails.LedgerId = dbo.AccountLedger.LedgerId where dbo.JournalDetails.JournalMasterId=@JournalMasterId AND JournalDetails.TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
			//var result = await (from a in _context.JournalDetails
			//					join b in _context.AccountLedger on a.LedgerId equals b.LedgerId
			//					where a.JournalMasterId == id && b.TenantId == "0" && b.TenantId == tenantId 
			//					select new JournalDetailsView
			//					{
			//						Id = id + 1,
			//						JournalDetailsId = a.JournalDetailsId,
			//						Credit = a.Credit,
			//						Debit = a.Debit,
			//						Narration = a.Narration,
			//						LedgerId = a.LedgerId,
			//						ProjectId = a.ProjectId,
			//						LedgerName = b.LedgerName
			//					}).ToListAsync();
			//return result;
		}
		public async Task<JournalMaster> GetbyId(int id)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@JournalMasterId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<JournalMaster>("SELECT *FROM JournalMaster where JournalMasterId=@JournalMasterId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
		}
		public bool VouchernoCheckExistence(string VoucherNo)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@VoucherNo", VoucherNo);
				para.Add("@TenantId", tenantId);
				return sqlcon.Query<bool>("SELECT VoucherNo FROM JournalMaster where VoucherNo=@VoucherNo AND tenantId=@tenantId", para, null, true, 0, CommandType.Text).SingleOrDefault();
			}
		}
		public async Task<bool> Delete(JournalMaster master)
		{
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			try
			{

				if (sqlcon.State == ConnectionState.Closed)
				{
					sqlcon.Open();
				}
				SqlCommand cmd = new SqlCommand("JournalVoucherDelete", sqlcon);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlParameter para = new SqlParameter();
				para = cmd.Parameters.Add("@JournalMasterId", SqlDbType.Int);
				para.Value = master.JournalMasterId;
				para = cmd.Parameters.Add("@VoucherTypeId", SqlDbType.Int);
				para.Value = master.VoucherTypeId;
				para = cmd.Parameters.Add("@VoucherNo", SqlDbType.NVarChar);
				para.Value = master.VoucherNo;
				para = cmd.Parameters.Add("@TenantId", SqlDbType.NVarChar);
				para.Value = master.TenantId;
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
		public async Task<List<LedgerPostingView>> Income()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<LedgerPostingView>("SELECT ISNULL(SUM(Credit) , 0) AS Credit,DATENAME(Month, Date) AS Month   FROM LedgerPosting  where LedgerId='53' AND TenantId=@TenantId GROUP BY MONTH([Date]),DATENAME(MONTH,[Date]) ORDER BY MONTH([Date])", para, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}
		public async Task<List<LedgerPostingView>> Expenses()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<LedgerPostingView>("SELECT ISNULL(SUM(Debit) , 0) AS Debit,DATENAME(Month, Date) AS Month   FROM LedgerPosting  where LedgerId='33' AND TenantId=@TenantId GROUP BY MONTH([Date]),DATENAME(MONTH,[Date]) ORDER BY MONTH([Date])", para, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}
	}
}
