using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.Inventory;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
    public class PaymentReceiptRepository : IPaymentReceipt
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public PaymentReceiptRepository(ApplicationDbContext context, DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
		public async Task<bool> Delete(ReceiptMaster master)
		{
			try
			{
				//GetBalance
				var result = await (from a in _context.ReceiptDetails
									where a.ReceiptMasterId == master.ReceiptMasterId && a.TenantId == tenantId
									select new
									{
										SalesMasterId = a.SalesMasterId,
										ReceiveAmount = a.ReceiveAmount
									}).ToListAsync();
				if (result != null)
				{
					foreach (var item in result)
					{
						SalesMaster purchase = new SalesMaster();
						using (SqlConnection sqlconn = new SqlConnection(_conn.DbConn))
						{
							var paras = new DynamicParameters();
							paras.Add("@SalesMasterId", item.SalesMasterId);
                            paras.Add("@TenantId",tenantId);
                            purchase = sqlconn.Query<SalesMaster>("SELECT *FROM SalesMaster where SalesMasterId=@SalesMasterId AND TenantId=@TenantId", paras, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
						}
						if (purchase != null)
						{
							//UpdatePurchaseBalance
							using (SqlConnection sqlconn = new SqlConnection(_conn.DbConn))
							{
								var paraPurchase = new DynamicParameters();
								paraPurchase.Add("@SalesMasterId", item.SalesMasterId);
								paraPurchase.Add("@BalanceDue", purchase.GrandTotal);
								paraPurchase.Add("@Status", "PartiallyPaid");
								paraPurchase.Add("@PaymentStatus", "PartiallyPaid");
                                paraPurchase.Add("@TenantId", tenantId);
                                sqlconn.Execute("UPDATE SalesMaster SET BalanceDue=@BalanceDue,Status=@Status,PaymentStatus=@PaymentStatus where SalesMasterId=@SalesMasterId AND TenantId=@TenantId", paraPurchase, null, 0, CommandType.Text);
							}
						}
					}
				}
				SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
				if (sqlcon.State == ConnectionState.Closed)
				{
					sqlcon.Open();
				}
				SqlCommand cmd = new SqlCommand("PaymentReceiptDelete", sqlcon);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlParameter para = new SqlParameter();
				para = cmd.Parameters.Add("@ReceiptMasterId", SqlDbType.Int);
				para.Value = master.ReceiptMasterId;
				para = cmd.Parameters.Add("@VoucherTypeId", SqlDbType.Int);
				para.Value = master.VoucherTypeId;
                para = cmd.Parameters.Add("@TenantId", SqlDbType.NVarChar);
                para.Value = tenantId;
                int rowAffacted = cmd.ExecuteNonQuery();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<IList<ReceiptMasterView>> GetAll(DateTime FromDate, DateTime ToDate, int SupplierId, string VoucherNo, string strType)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@FromDate", FromDate);
				para.Add("@ToDate", ToDate);
				para.Add("@CustomerSupplierId", SupplierId);
				para.Add("@VoucherNo", VoucherNo);
				para.Add("@ReportType", strType);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<ReceiptMasterView>("ReceiptViewAll", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}
		public async Task<ReceiptMasterView> GetbyIdView(int ReceiptMasterId)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@ReceiptMasterId", ReceiptMasterId);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<ReceiptMasterView>("ReceiptViewPrint", para, null, true, 0, commandType: CommandType.StoredProcedure).FirstOrDefault();
				return ListofPlan;
			}
		}
			public async Task<ReceiptMaster> GetbyId(int id)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@ReceiptMasterId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<ReceiptMaster>("SELECT *FROM ReceiptMaster where ReceiptMasterId=@ReceiptMasterId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
		}

		public async Task<string> GetSerialNo()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TenantId", tenantId);
				var val = sqlcon.Query<string>("SELECT ISNULL(MAX(SerialNo+1),1) as VoucherNo FROM ReceiptMaster where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return val;
			}
		}

		public async Task<IList<ReceiptMaster>> ReceiptMadeSales(int id)
		{
			var result = await (from a in _context.ReceiptMaster
                                join b in _context.ReceiptDetails on a.ReceiptMasterId equals b.ReceiptMasterId
                                where b.SalesMasterId == id
								select new ReceiptMaster
                                {
                                    ReceiptMasterId = a.ReceiptMasterId,
									Date = a.Date,
									VoucherNo = a.VoucherNo,
									Amount = a.Amount,
									Narration = a.Narration,
									PaymentType = a.PaymentType,
									Type = a.Type,
									UserId = a.UserId
								}).ToListAsync();
			return result;
		}
		public async Task<IList<ReceiptDetailsView>> ReceiptFor(int id)
		{
			var result = await (from a in _context.ReceiptDetails
								join b in _context.SalesMaster on a.SalesMasterId equals b.SalesMasterId
                                where a.ReceiptMasterId == id
								select new ReceiptDetailsView
                                {
                                    ReceiptMasterId = a.ReceiptMasterId,
                                    ReceiptDetailsId = a.ReceiptDetailsId,
									BillDate = b.Date,
									BillNo = b.VoucherNo,
									PurchaseAmount = b.GrandTotal,
									TotalAmount = a.TotalAmount
								}).ToListAsync();
			return result;
		}
		public async Task<IList<ReceiptDetailsView>> ReceiptDetailsView(int id)
		{
			var result = await (from a in _context.ReceiptDetails
								join b in _context.SalesMaster on a.SalesMasterId equals b.SalesMasterId
								where a.ReceiptMasterId == id
								select new ReceiptDetailsView
                                {
                                    ReceiptMasterId = a.ReceiptMasterId,
                                    ReceiptDetailsId = a.ReceiptDetailsId,
                                    SalesMasterId = a.SalesMasterId,
									BillDate = b.Date,
									BillNo = b.VoucherNo,
									TotalAmount = a.TotalAmount,
									ReceiveAmount = a.ReceiveAmount,
									DueAmount = a.DueAmount,

								}).ToListAsync();
			return result;
		}
		public async Task<int> Save(ReceiptMaster model)
		{
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlTransaction sql = sqlcon.BeginTransaction();
			try
			{
				var para = new DynamicParameters();
				para.Add("@SerialNo", model.SerialNo);
				para.Add("@VoucherNo", model.VoucherNo);
				para.Add("@LedgerId", model.LedgerId);
				para.Add("@CustomerSupplierId", model.CustomerSupplierId);
				para.Add("@Date", model.Date);
				para.Add("@PaidAmount", model.PaidAmount);
				para.Add("@Amount", model.Amount);
				para.Add("@Narration", model.Narration);
				para.Add("@VoucherTypeId", model.VoucherTypeId);
				para.Add("@UserId", model.UserId);
				para.Add("@Type", model.Type);
				para.Add("@PaymentType", model.PaymentType);
				para.Add("@FinancialYearId", model.FinancialYearId);
				para.Add("@Reference", model.Reference);
				para.Add("@TenantId", tenantId);
				para.Add("@AddedDate", model.AddedDate);
				para.Add("@MemIDOUT", dbType: DbType.Int32, direction: ParameterDirection.Output);
				sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO ReceiptMaster (SerialNo,VoucherNo,LedgerId,CustomerSupplierId,Date,PaidAmount,Amount,Narration,VoucherTypeId,UserId,Type,PaymentType,FinancialYearId,TenantId,AddedDate,Reference)VALUES(@SerialNo,@VoucherNo,@LedgerId,@CustomerSupplierId,@Date,@PaidAmount,@Amount,@Narration,@VoucherTypeId,@UserId,@Type,@PaymentType,@FinancialYearId,@TenantId,@AddedDate,@Reference) SELECT @ReturnValue = SCOPE_IDENTITY() set @MemIDOUT =SCOPE_IDENTITY()", para, sql, 0, CommandType.Text);
				int Id = para.Get<int>("MemIDOUT");
				if (Id > 0)
				{

					foreach (var item in model.listOrder)
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@ReceiptMasterId", Id);
						paraOpening.Add("@SalesMasterId", item.SalesMasterId);
						paraOpening.Add("@TotalAmount", item.TotalAmount);
						paraOpening.Add("@ReceiveAmount", item.ReceiveAmount);
						paraOpening.Add("@DueAmount", item.DueAmount);
						paraOpening.Add("@TenantId", tenantId);
						paraOpening.Add("@DId", dbType: DbType.Int32, direction: ParameterDirection.Output);
						sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO ReceiptDetails (ReceiptMasterId,SalesMasterId,TotalAmount,ReceiveAmount,DueAmount,TenantId)VALUES(@ReceiptMasterId,@SalesMasterId,@TotalAmount,@ReceiveAmount,@DueAmount,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @DId =SCOPE_IDENTITY()", paraOpening, sql, 0, CommandType.Text);
						
						int longdetailsId = paraOpening.Get<int>("DId");

						//UpdatePurchaseBalance
						var paraPurchase = new DynamicParameters();
						paraPurchase.Add("@SalesMasterId", item.SalesMasterId);
						paraPurchase.Add("@BalanceDue", 0);
						paraPurchase.Add("@Status", "Paid");
						paraPurchase.Add("@PaymentStatus", "Paid");
                        paraPurchase.Add("@TenantId", tenantId);
                        sqlcon.Execute("UPDATE SalesMaster SET BalanceDue=@BalanceDue,Status=@Status,PaymentStatus=@PaymentStatus where SalesMasterId=@SalesMasterId AND TenantId=@TenantId", paraPurchase, sql, 0, CommandType.Text);

					}
					//PostingCash
					var paraCash = new DynamicParameters();
					paraCash.Add("@Date", model.Date);
					paraCash.Add("@LedgerId", model.LedgerId);
					paraCash.Add("@Debit", model.Amount);
					paraCash.Add("@Credit", 0);
					paraCash.Add("@VoucherNo", model.VoucherNo);
					paraCash.Add("@DetailsId", Id);
					paraCash.Add("@YearId", model.FinancialYearId);
					paraCash.Add("@InvoiceNo", model.VoucherNo);
					paraCash.Add("@VoucherTypeId", model.VoucherTypeId);
					paraCash.Add("@LongReference", model.Narration);
					paraCash.Add("@ReferenceN", model.Narration);
					paraCash.Add("@ChequeNo", string.Empty);
					paraCash.Add("@ChequeDate", string.Empty);
					paraCash.Add("@AddedDate", DateTime.Now);
					paraCash.Add("@TenantId", tenantId);
					var valuePayable = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraCash, sql, true, 0, commandType: CommandType.Text);

					//AccountReceiable
					var paraPrepaidExpenses = new DynamicParameters();
					paraPrepaidExpenses.Add("@Date", model.Date);
					paraPrepaidExpenses.Add("@LedgerId", 5);
					paraPrepaidExpenses.Add("@Debit", 0);
					paraPrepaidExpenses.Add("@Credit", model.Amount);
                    paraPrepaidExpenses.Add("@VoucherNo", model.VoucherNo);
					paraPrepaidExpenses.Add("@DetailsId", Id);
					paraPrepaidExpenses.Add("@YearId", model.FinancialYearId);
					paraPrepaidExpenses.Add("@InvoiceNo", model.VoucherNo);
					paraPrepaidExpenses.Add("@VoucherTypeId", model.VoucherTypeId);
					paraPrepaidExpenses.Add("@LongReference", model.Narration);
					paraPrepaidExpenses.Add("@ReferenceN", model.Narration);
					paraPrepaidExpenses.Add("@ChequeNo", string.Empty);
					paraPrepaidExpenses.Add("@ChequeDate", string.Empty);
					paraPrepaidExpenses.Add("@AddedDate", DateTime.Now);
					paraPrepaidExpenses.Add("@TenantId", tenantId);
					var valueparaPrepaidExpenses = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraPrepaidExpenses, sql, true, 0, commandType: CommandType.Text);

				}
				sql.Commit();
				return Id;
			}
			catch (Exception ex)
			{
				sql.Rollback();
				return 0;
				throw;
			}
		}
		public async Task<bool> Update(ReceiptMaster model)
		{
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlTransaction sql = sqlcon.BeginTransaction();
			try
			{
				var para = new DynamicParameters();
				para.Add("@ReceiptMasterId", model.ReceiptMasterId);
				para.Add("@SerialNo", model.SerialNo);
				para.Add("@VoucherNo", model.VoucherNo);
				para.Add("@LedgerId", model.LedgerId);
				para.Add("@CustomerSupplierId", model.CustomerSupplierId);
				para.Add("@Date", model.Date);
				para.Add("@PaidAmount", model.PaidAmount);
				para.Add("@Amount", model.Amount);
				para.Add("@Narration", model.Narration);
				para.Add("@VoucherTypeId", model.VoucherTypeId);
				para.Add("@UserId", model.UserId);
				para.Add("@Type", model.Type);
				para.Add("@PaymentType", model.PaymentType);
				para.Add("@FinancialYearId", model.FinancialYearId);
				para.Add("@Reference", model.Reference);
				para.Add("@TenantId", tenantId);
				para.Add("@ModifyDate", model.ModifyDate);
				sqlcon.Execute("UPDATE ReceiptMaster SET SerialNo=@SerialNo,VoucherNo=@VoucherNo,LedgerId=@LedgerId,CustomerSupplierId=@CustomerSupplierId,Date=@Date,PaidAmount=@PaidAmount,Amount=@Amount,Narration=@Narration,VoucherTypeId=@VoucherTypeId,UserId=@UserId,Type=@Type,PaymentType=@PaymentType,FinancialYearId=@FinancialYearId,TenantId=@TenantId,ModifyDate=@ModifyDate,Reference=@Reference where ReceiptMasterId=@ReceiptMasterId", para, sql, 0, CommandType.Text);

					foreach (var item in model.listOrder)
					{
						var paraOpening = new DynamicParameters();
					paraOpening.Add("@ReceiptDetailsId", item.ReceiptDetailsId);
					paraOpening.Add("@ReceiptMasterId", item.ReceiptMasterId);
						paraOpening.Add("@SalesMasterId", item.SalesMasterId);
						paraOpening.Add("@TotalAmount", item.TotalAmount);
						paraOpening.Add("@ReceiveAmount", item.ReceiveAmount);
						paraOpening.Add("@DueAmount", item.DueAmount);
						paraOpening.Add("@TenantId", tenantId);
						sqlcon.Execute("UPDATE ReceiptDetails SET ReceiptMasterId=@ReceiptMasterId,SalesMasterId=@SalesMasterId,TotalAmount=@TotalAmount,ReceiveAmount=@ReceiveAmount,DueAmount=@DueAmount,TenantId=@TenantId where ReceiptDetailsId=@ReceiptDetailsId", paraOpening, sql, 0, CommandType.Text);

						//UpdatePurchaseBalance
						var paraPurchase = new DynamicParameters();
						paraPurchase.Add("@SalesMasterId", item.SalesMasterId);
						paraPurchase.Add("@BalanceDue", 0);
						paraPurchase.Add("@Status", "Paid");
						paraPurchase.Add("@PaymentStatus", "Paid");
                    paraPurchase.Add("@TenantId", tenantId);
                    sqlcon.Execute("UPDATE SalesMaster SET BalanceDue=@BalanceDue,Status=@Status,PaymentStatus=@PaymentStatus where SalesMasterId=@SalesMasterId AND TenantId=@TenantId", paraPurchase, sql, 0, CommandType.Text);

					}



				//DeleteLedgerPosting
				var paraLedgerDelete = new DynamicParameters();
				paraLedgerDelete.Add("@DetailsId", model.ReceiptMasterId);
				paraLedgerDelete.Add("@VoucherTypeId", model.VoucherTypeId);
				paraLedgerDelete.Add("@TenantId", tenantId);
				var valueScDelete = sqlcon.Query<long>("DELETE FROM LedgerPosting where DetailsId=@DetailsId AND VoucherTypeId=@VoucherTypeId AND TenantId=@TenantId", paraLedgerDelete, sql, true, 0, commandType: CommandType.Text);

				//PostingCash
				var paraCash = new DynamicParameters();
					paraCash.Add("@Date", model.Date);
					paraCash.Add("@LedgerId", model.LedgerId);
					paraCash.Add("@Debit", model.Amount);
				paraCash.Add("@Credit", 0);
					paraCash.Add("@VoucherNo", model.VoucherNo);
					paraCash.Add("@DetailsId", model.ReceiptMasterId);
					paraCash.Add("@YearId", model.FinancialYearId);
					paraCash.Add("@InvoiceNo", model.VoucherNo);
					paraCash.Add("@VoucherTypeId", model.VoucherTypeId);
					paraCash.Add("@LongReference", model.Narration);
					paraCash.Add("@ReferenceN", model.Narration);
					paraCash.Add("@ChequeNo", string.Empty);
					paraCash.Add("@ChequeDate", string.Empty);
					paraCash.Add("@AddedDate", DateTime.Now);
					paraCash.Add("@TenantId", tenantId);
					var valuePayable = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraCash, sql, true, 0, commandType: CommandType.Text);

					//PostingPrepaidExpenses
					var paraPrepaidExpenses = new DynamicParameters();
					paraPrepaidExpenses.Add("@Date", model.Date);
					paraPrepaidExpenses.Add("@LedgerId", 5);
				paraPrepaidExpenses.Add("@Debit", 0);
					paraPrepaidExpenses.Add("@Credit", model.Amount);
                paraPrepaidExpenses.Add("@VoucherNo", model.VoucherNo);
					paraPrepaidExpenses.Add("@DetailsId", model.ReceiptMasterId);
					paraPrepaidExpenses.Add("@YearId", model.FinancialYearId);
					paraPrepaidExpenses.Add("@InvoiceNo", model.VoucherNo);
					paraPrepaidExpenses.Add("@VoucherTypeId", model.VoucherTypeId);
					paraPrepaidExpenses.Add("@LongReference", model.Narration);
					paraPrepaidExpenses.Add("@ReferenceN", model.Narration);
					paraPrepaidExpenses.Add("@ChequeNo", string.Empty);
					paraPrepaidExpenses.Add("@ChequeDate", string.Empty);
					paraPrepaidExpenses.Add("@AddedDate", DateTime.Now);
					paraPrepaidExpenses.Add("@TenantId", tenantId);
					var valueparaPrepaidExpenses = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraPrepaidExpenses, sql, true, 0, commandType: CommandType.Text);
				sql.Commit();
				return true;
			}
			catch (Exception ex)
			{
				sql.Rollback();
				return false;
				throw;
			}
		}
		public async Task<decimal> Payable()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@TenantId", tenantId);
				var val = sqlcon.Query<decimal>("SELECT ISNULL(SUM(Credit),0) - ISNULL(SUM(Debit),0) as Payable FROM LedgerPosting where LedgerId='16' AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return val;
			}
		}
	}
}
