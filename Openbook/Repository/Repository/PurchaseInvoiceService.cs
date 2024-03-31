using Dapper;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.AccountModel;
using Openbook.Data.Inventory;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
    public class PurchaseInvoiceService : IPurchaseInvoice
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseConnection _conn;
        private string tenantId;

        public PurchaseInvoiceService(ApplicationDbContext context, DatabaseConnection conn, IServicioTenant servicioTenant)
        {
            _context = context;
            _conn = conn;
            tenantId = servicioTenant.ObtenerTenant();
        }

        public bool BillnoCheckExistence(string VoucherNo)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@VoucherNo", VoucherNo);
                para.Add("@TenantId", tenantId);
                return sqlcon.Query<bool>("SELECT VoucherNo FROM PurchaseMaster where VoucherNo=@VoucherNo AND tenantId=@tenantId", para, null, true, 0, CommandType.Text).SingleOrDefault();
            }
        }

        public async Task<bool> Delete(PurchaseMaster master)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            try
            {

                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand cmd = new SqlCommand("PurchaseInvoiceDelete", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter para = new SqlParameter();
                para = cmd.Parameters.Add("@PurchaseMasterId", SqlDbType.Int);
                para.Value = master.PurchaseMasterId;
                para = cmd.Parameters.Add("@VoucherTypeId", SqlDbType.Int);
                para.Value = master.VoucherTypeId;
                para = cmd.Parameters.Add("@VoucherNo", SqlDbType.NVarChar);
                para.Value = master.VoucherNo;
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

        public async Task<int> Draft(PurchaseMaster master)
        {
				SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
				sqlcon.Open();
				SqlTransaction sql = sqlcon.BeginTransaction();
				var para = new DynamicParameters();
				para.Add("@SerialNo", master.SerialNo);
				para.Add("@VoucherNo", master.VoucherNo);
				para.Add("@WarehouseId", master.WarehouseId);
				para.Add("@OrderNo", master.OrderNo);
				para.Add("@Date", master.Date);
				para.Add("@CustomerSupplierId", master.CustomerSupplierId);
				para.Add("@DueDate", master.DueDate);
				para.Add("@Narration", master.Narration);
				para.Add("@PurchaseOrderMasterId", master.PurchaseOrderMasterId);
				para.Add("@NetAmounts", master.NetAmounts);
				para.Add("@Reference", master.Reference);
				para.Add("@TotalTax", master.TotalTax);
				para.Add("@DisPer", master.DisPer);
				para.Add("@DiscountType", master.DiscountType);
				para.Add("@BillDiscount", master.BillDiscount);
				para.Add("@GrandTotal", master.GrandTotal);
				para.Add("@TotalAmount", master.TotalAmount);
				para.Add("@PayAmount", master.PayAmount);
				para.Add("@PreviousDue", master.PreviousDue);
				para.Add("@VoucherTypeId", master.VoucherTypeId);
				para.Add("@UserId", master.UserId);
				para.Add("@FinancialYearId", master.FinancialYearId);
				para.Add("@BalanceDue", master.BalanceDue);
				para.Add("@Status", master.Status);
				para.Add("@PaymentStatus", master.PaymentStatus);
				para.Add("@AddedDate", DateTime.Now);
				para.Add("@TenantId", tenantId);
				para.Add("@MemIDOUT", dbType: DbType.Int32, direction: ParameterDirection.Output);
				sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO PurchaseMaster (SerialNo,VoucherNo,WarehouseId,OrderNo,Date,CustomerSupplierId,DueDate,Narration,PurchaseOrderMasterId,NetAmounts,Reference,TotalTax,DisPer,DiscountType,BillDiscount,GrandTotal,TotalAmount,PreviousDue,VoucherTypeId,UserId,FinancialYearId,BalanceDue,Status,PaymentStatus,PayAmount,AddedDate,TenantId)VALUES(@SerialNo,@VoucherNo,@WarehouseId,@OrderNo,@Date,@CustomerSupplierId,@DueDate,@Narration,@PurchaseOrderMasterId,@NetAmounts,@Reference,@TotalTax,@DisPer,@DiscountType,@BillDiscount,@GrandTotal,@TotalAmount,@PreviousDue,@VoucherTypeId,@UserId,@FinancialYearId,@BalanceDue,@Status,@PaymentStatus,@PayAmount,@AddedDate,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @MemIDOUT =SCOPE_IDENTITY()", para, sql, 0, CommandType.Text);
				int Id = para.Get<int>("MemIDOUT");
				if (Id > 0)
				{

					foreach (var item in master.listOrder)
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@PurchaseMasterId", Id);
						paraOpening.Add("@OrderDetailsId", 0);
						paraOpening.Add("@ProductId", item.ProductId);
						paraOpening.Add("@Qty", item.Qty);
						paraOpening.Add("@Rate", item.Rate);
						paraOpening.Add("@UnitId", item.UnitId);
						paraOpening.Add("@Discount", item.Discount);
						paraOpening.Add("@DiscountAmount", item.DiscountAmount);
						paraOpening.Add("@TaxId", item.TaxId);
						paraOpening.Add("@TaxRate", item.TaxRate);
						paraOpening.Add("@BatchId", item.BatchId);
						paraOpening.Add("@TaxAmount", item.TaxAmount);
						paraOpening.Add("@Amount", item.Amount);
						paraOpening.Add("@GrossAmount", item.GrossAmount);
						paraOpening.Add("@NetAmount", item.NetAmount);
						paraOpening.Add("@LedgerId", item.LedgerId);
						paraOpening.Add("@ProjectId", item.ProjectId);
					paraOpening.Add("@WarehouseId", item.WarehouseId);
					paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						paraOpening.Add("@DId", dbType: DbType.Int32, direction: ParameterDirection.Output);
						sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO PurchaseDetails (PurchaseMasterId,OrderDetailsId,ProductId,Qty,Rate,UnitId,Discount,DiscountAmount,TaxId,BatchId,TaxAmount,GrossAmount,NetAmount,LedgerId,ProjectId,TaxRate,Amount,Description,WarehouseId,TenantId)VALUES(@PurchaseMasterId,@OrderDetailsId,@ProductId,@Qty,@Rate,@UnitId,@Discount,@DiscountAmount,@TaxId,@BatchId,@TaxAmount,@GrossAmount,@NetAmount,@LedgerId,@ProjectId,@TaxRate,@Amount,@Description,@WarehouseId,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @DId =SCOPE_IDENTITY()", paraOpening, sql, 0, CommandType.Text);
						int longdetailsId = paraOpening.Get<int>("DId");
					//	//StockPosting
					//	var parastock = new DynamicParameters();
					//	parastock.Add("@BatchId", item.BatchId);
					//parastock.Add("@WarehouseId", 0);
					//parastock.Add("@Date", master.Date);
					//	parastock.Add("@FinancialYearId", master.FinancialYearId);
					//	parastock.Add("@InwardQty", item.Qty);
					//	parastock.Add("@OutwardQty", 0);
					//	parastock.Add("@ProductId", item.ProductId);
					//	parastock.Add("@Rate", item.Rate);
					//	parastock.Add("@UnitId", item.UnitId);
					//	parastock.Add("@DetailsId", longdetailsId);
					//	parastock.Add("@InvoiceNo", master.OrderNo);
					//	parastock.Add("@VoucherNo", master.VoucherNo);
					//	parastock.Add("@VoucherTypeId", master.VoucherTypeId);
					//	parastock.Add("@AgainstInvoiceNo", "NA");
					//	parastock.Add("@AgainstVoucherNo", "NA");
					//	parastock.Add("@AgainstVoucherTypeId", 0);
					//	parastock.Add("@StockCalculate", "Purchase");
					//	parastock.Add("@AddedDate", DateTime.Now);
					//parastock.Add("@TenantId", tenantId);
					//var valuesStock = sqlcon.Query<int>("INSERT INTO StockPosting (BatchId,WarehouseId,Date,FinancialYearId,InwardQty,OutwardQty,ProductId,Rate,UnitId,DetailsId,InvoiceNo,VoucherNo,VoucherTypeId,AgainstInvoiceNo,AgainstVoucherNo,AgainstVoucherTypeId,StockCalculate,AddedDate,TenantId)VALUES(@BatchId,@WarehouseId,@Date,@FinancialYearId,@InwardQty,@OutwardQty,@ProductId,@Rate,@UnitId,@DetailsId,@InvoiceNo,@VoucherNo,@VoucherTypeId,@AgainstInvoiceNo,@AgainstVoucherNo,@AgainstVoucherTypeId,@StockCalculate,@AddedDate,@TenantId)", parastock, sql, true, 0, commandType: CommandType.Text);
					
				}
				}
			sql.Commit();
			return Id;
		}
		public async Task<List<PurchaseMasterView>> PurchaseInvoiceFilter( DateTime fromDate, DateTime toDate, int supplierid, string strVoucherNo, string strStatus , string strFiltetype)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@fromDate", fromDate);
				para.Add("@toDate", toDate);
				para.Add("@CustomerSupplierId", supplierid);
				para.Add("@VoucherNo", strVoucherNo);
				para.Add("@Status", strStatus);
				para.Add("@FilterType", strFiltetype);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<PurchaseMasterView>("PurchaseInvoiceFilter", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}
		public async Task<List<PurchaseMasterView>> PurchaseInvoice(string strStatus , string strFiltetype)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@Status", strStatus);
				para.Add("@FilterType", strFiltetype);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<PurchaseMasterView>("PurchaseInvoiceFilter", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}

		public async Task<PurchaseMaster> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@PurchaseMasterId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<PurchaseMaster>("SELECT *FROM PurchaseMaster where PurchaseMasterId=@PurchaseMasterId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<string> GetSerialNo()
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@TenantId", tenantId);
                var val = sqlcon.Query<string>("SELECT ISNULL(MAX(SerialNo+1),1) as VoucherNo FROM PurchaseMaster where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return val;
            }
        }

        public async Task<int> Approved(PurchaseMaster master)
        {
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlTransaction sql = sqlcon.BeginTransaction();
			try
			{
				var para = new DynamicParameters();
				para.Add("@SerialNo", master.SerialNo);
				para.Add("@VoucherNo", master.VoucherNo);
				para.Add("@WarehouseId", master.WarehouseId);
				para.Add("@OrderNo", master.OrderNo);
				para.Add("@Date", master.Date);
				para.Add("@CustomerSupplierId", master.CustomerSupplierId);
				para.Add("@DueDate", master.DueDate);
				para.Add("@Narration", master.Narration);
				para.Add("@PurchaseOrderMasterId", master.PurchaseOrderMasterId);
				para.Add("@NetAmounts", master.NetAmounts);
				para.Add("@Reference", master.Reference);
				para.Add("@TotalTax", master.TotalTax);
				para.Add("@DisPer", master.DisPer);
				para.Add("@DiscountType", master.DiscountType);
				para.Add("@BillDiscount", master.BillDiscount);
				para.Add("@GrandTotal", master.GrandTotal);
				para.Add("@TotalAmount", master.TotalAmount);
				para.Add("@PayAmount", master.PayAmount);
				para.Add("@PreviousDue", master.PreviousDue);
				para.Add("@VoucherTypeId", master.VoucherTypeId);
				para.Add("@UserId", master.UserId);
				para.Add("@FinancialYearId", master.FinancialYearId);
				para.Add("@BalanceDue", master.BalanceDue);
				para.Add("@Status", master.Status);
				para.Add("@PaymentStatus", master.PaymentStatus);
				para.Add("@AddedDate", DateTime.Now);
				para.Add("@TenantId", tenantId);
				para.Add("@MemIDOUT", dbType: DbType.Int32, direction: ParameterDirection.Output);
				sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO PurchaseMaster (SerialNo,VoucherNo,WarehouseId,OrderNo,Date,CustomerSupplierId,DueDate,Narration,PurchaseOrderMasterId,NetAmounts,Reference,TotalTax,DisPer,DiscountType,BillDiscount,GrandTotal,TotalAmount,PreviousDue,VoucherTypeId,UserId,FinancialYearId,BalanceDue,Status,PaymentStatus,PayAmount,AddedDate,TenantId)VALUES(@SerialNo,@VoucherNo,@WarehouseId,@OrderNo,@Date,@CustomerSupplierId,@DueDate,@Narration,@PurchaseOrderMasterId,@NetAmounts,@Reference,@TotalTax,@DisPer,@DiscountType,@BillDiscount,@GrandTotal,@TotalAmount,@PreviousDue,@VoucherTypeId,@UserId,@FinancialYearId,@BalanceDue,@Status,@PaymentStatus,@PayAmount,@AddedDate,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @MemIDOUT =SCOPE_IDENTITY()", para, sql, 0, CommandType.Text);
				int Id = para.Get<int>("MemIDOUT");
				if (Id > 0)
				{

					foreach (var item in master.listOrder)
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@PurchaseMasterId", Id);
						paraOpening.Add("@OrderDetailsId", 0);
						paraOpening.Add("@ProductId", item.ProductId);
						paraOpening.Add("@Qty", item.Qty);
						paraOpening.Add("@Rate", item.Rate);
						paraOpening.Add("@UnitId", item.UnitId);
						paraOpening.Add("@Discount", item.Discount);
						paraOpening.Add("@DiscountAmount", item.DiscountAmount);
						paraOpening.Add("@TaxId", item.TaxId);
						paraOpening.Add("@TaxRate", item.TaxRate);
						paraOpening.Add("@BatchId", item.BatchId);
						paraOpening.Add("@TaxAmount", item.TaxAmount);
						paraOpening.Add("@Amount", item.Amount);
						paraOpening.Add("@GrossAmount", item.GrossAmount);
						paraOpening.Add("@NetAmount", item.NetAmount);
						paraOpening.Add("@LedgerId", item.LedgerId);
						paraOpening.Add("@ProjectId", item.ProjectId);
						paraOpening.Add("@WarehouseId", item.WarehouseId);
						paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						paraOpening.Add("@DId", dbType: DbType.Int32, direction: ParameterDirection.Output);
						sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO PurchaseDetails (PurchaseMasterId,OrderDetailsId,ProductId,Qty,Rate,UnitId,Discount,DiscountAmount,TaxId,BatchId,TaxAmount,GrossAmount,NetAmount,LedgerId,ProjectId,TaxRate,Amount,Description,WarehouseId,TenantId)VALUES(@PurchaseMasterId,@OrderDetailsId,@ProductId,@Qty,@Rate,@UnitId,@Discount,@DiscountAmount,@TaxId,@BatchId,@TaxAmount,@GrossAmount,@NetAmount,@LedgerId,@ProjectId,@TaxRate,@Amount,@Description,@WarehouseId,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @DId =SCOPE_IDENTITY()", paraOpening, sql, 0, CommandType.Text);
						int longdetailsId = paraOpening.Get<int>("DId");

						//StockPosting
						var parastock = new DynamicParameters();
						parastock.Add("@BatchId", item.BatchId);
						parastock.Add("@WarehouseId", item.WarehouseId);
						parastock.Add("@Date", master.Date);
						parastock.Add("@FinancialYearId", master.FinancialYearId);
						parastock.Add("@InwardQty", item.Qty);
						parastock.Add("@OutwardQty", 0);
						parastock.Add("@ProductId", item.ProductId);
						parastock.Add("@Rate", item.Rate);
						parastock.Add("@UnitId", item.UnitId);
						parastock.Add("@DetailsId", longdetailsId);
						parastock.Add("@InvoiceNo", master.OrderNo);
						parastock.Add("@VoucherNo", master.VoucherNo);
						parastock.Add("@VoucherTypeId", master.VoucherTypeId);
						parastock.Add("@AgainstInvoiceNo", "NA");
						parastock.Add("@AgainstVoucherNo", "NA");
						parastock.Add("@AgainstVoucherTypeId", 0);
						parastock.Add("@StockCalculate", "Purchase");
						parastock.Add("@AddedDate", DateTime.Now);
						parastock.Add("@TenantId", tenantId);
						var valuesStock = sqlcon.Query<int>("INSERT INTO StockPosting (BatchId,WarehouseId,Date,FinancialYearId,InwardQty,OutwardQty,ProductId,Rate,UnitId,DetailsId,InvoiceNo,VoucherNo,VoucherTypeId,AgainstInvoiceNo,AgainstVoucherNo,AgainstVoucherTypeId,StockCalculate,AddedDate,TenantId)VALUES(@BatchId,@WarehouseId,@Date,@FinancialYearId,@InwardQty,@OutwardQty,@ProductId,@Rate,@UnitId,@DetailsId,@InvoiceNo,@VoucherNo,@VoucherTypeId,@AgainstInvoiceNo,@AgainstVoucherNo,@AgainstVoucherTypeId,@StockCalculate,@AddedDate,@TenantId)", parastock, sql, true, 0, commandType: CommandType.Text);


					}
					
					foreach (var item in master.listOrder)
					{
						var paraSc = new DynamicParameters();
						paraSc.Add("@Date", master.Date);
						paraSc.Add("@LedgerId", item.LedgerId);
						paraSc.Add("@Debit", item.Amount);
						paraSc.Add("@Credit", 0);
						paraSc.Add("@VoucherNo", master.VoucherNo);
						paraSc.Add("@DetailsId", Id);
						paraSc.Add("@YearId", master.FinancialYearId);
						paraSc.Add("@InvoiceNo", master.OrderNo);
						paraSc.Add("@VoucherTypeId", master.VoucherTypeId);
						paraSc.Add("@LongReference", master.Narration);
						paraSc.Add("@ReferenceN", master.Narration);
						paraSc.Add("@ChequeNo", string.Empty);
						paraSc.Add("@ChequeDate", string.Empty);
						paraSc.Add("@AddedDate", DateTime.Now);
						paraSc.Add("@TenantId", tenantId);
						var valueSc = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraSc, sql, true, 0, commandType: CommandType.Text);
					}
					//Posting Accounts Payable
					var paraPayable = new DynamicParameters();
					paraPayable.Add("@Date", master.Date);
					paraPayable.Add("@LedgerId", 16);
					paraPayable.Add("@Debit", 0);
					paraPayable.Add("@Credit", master.GrandTotal);
					paraPayable.Add("@VoucherNo", master.VoucherNo);
					paraPayable.Add("@DetailsId", Id);
					paraPayable.Add("@YearId", master.FinancialYearId);
					paraPayable.Add("@InvoiceNo", master.OrderNo);
					paraPayable.Add("@VoucherTypeId", master.VoucherTypeId);
					paraPayable.Add("@LongReference", master.Narration);
					paraPayable.Add("@ReferenceN", master.Narration);
					paraPayable.Add("@ChequeNo", string.Empty);
					paraPayable.Add("@ChequeDate", string.Empty);
					paraPayable.Add("@AddedDate", DateTime.Now);
					paraPayable.Add("@TenantId", tenantId);
					var valuePayable = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraPayable, sql, true, 0, commandType: CommandType.Text);

					//Posting Tax Payable
					var paraTaxPayable = new DynamicParameters();
					paraTaxPayable.Add("@Date", master.Date);
					paraTaxPayable.Add("@LedgerId", 22);
					paraTaxPayable.Add("@Debit", master.TotalTax);
					paraTaxPayable.Add("@Credit", 0);
					paraTaxPayable.Add("@VoucherNo", master.VoucherNo);
					paraTaxPayable.Add("@DetailsId", Id);
					paraTaxPayable.Add("@YearId", master.FinancialYearId);
					paraTaxPayable.Add("@InvoiceNo", master.OrderNo);
					paraTaxPayable.Add("@VoucherTypeId", master.VoucherTypeId);
					paraTaxPayable.Add("@LongReference", master.Narration);
					paraTaxPayable.Add("@ReferenceN", master.Narration);
					paraTaxPayable.Add("@ChequeNo", string.Empty);
					paraTaxPayable.Add("@ChequeDate", string.Empty);
					paraTaxPayable.Add("@AddedDate", DateTime.Now);
					paraTaxPayable.Add("@TenantId", tenantId);
					var valueTaxPayable = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraTaxPayable, sql, true, 0, commandType: CommandType.Text);

					//Discount
					var paraDiscount = new DynamicParameters();
					paraDiscount.Add("@Date", master.Date);
					paraDiscount.Add("@LedgerId", 59); //DiscountAllowed
					paraDiscount.Add("@Debit", 0);
					paraDiscount.Add("@Credit", master.BillDiscount);
					paraDiscount.Add("@VoucherNo", master.VoucherNo);
					paraDiscount.Add("@DetailsId", Id);
					paraDiscount.Add("@YearId", master.FinancialYearId);
					paraDiscount.Add("@InvoiceNo", master.OrderNo);
					paraDiscount.Add("@VoucherTypeId", master.VoucherTypeId);
					paraDiscount.Add("@LongReference", master.Narration);
					paraDiscount.Add("@ReferenceN", master.Narration);
					paraDiscount.Add("@ChequeNo", string.Empty);
					paraDiscount.Add("@ChequeDate", string.Empty);
					paraDiscount.Add("@AddedDate", DateTime.Now);
					paraDiscount.Add("@TenantId", tenantId);
					var valueparaDiscount = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraDiscount, sql, true, 0, commandType: CommandType.Text);

				}
				sql.Commit();
				return Id;
			}
			catch (Exception ex)
			{
				sql.Rollback();
				return 0;
			}
		}
		public async Task<List<PurchaseMaster>> PurchaseBillView(int CustomerSupplierId)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@CustomerSupplierId", CustomerSupplierId);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<PurchaseMaster>("SELECT PurchaseMasterId,VoucherNo,Date,GrandTotal,BalanceDue,OrderNo FROM PurchaseMaster where CustomerSupplierId=@CustomerSupplierId AND TenantId=@TenantId and BalanceDue !=0 AND Status !='Draft'", para, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}
			public async Task<List<ProductView>> PurchaseDetailsView(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@PurchaseMasterId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<ProductView>("PurchaseDetailsView", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
			//var result = await (from a in _context.PurchaseDetails
			//					join b in _context.Product on a.ProductId equals b.ProductId
			//					join c in _context.Unit on a.UnitId equals c.UnitId
			//					join e in _context.AccountLedger on a.LedgerId equals e.LedgerId
			//					join d in _context.Tax on a.TaxId equals d.TaxId into lJ
			//					from res in lJ.DefaultIfEmpty()
			//					where a.PurchaseMasterId == id
			//					select new ProductView
			//					{
			//						Id = id,
 		//						   PurchaseDetailsId = a.PurchaseDetailsId,
			//						ProductId = a.ProductId,
			//						Qty = a.Qty,
			//						Rate = a.Rate,
			//						UnitId = a.UnitId,
			//						LedgerId = a.LedgerId,
			//						ProjectId = a.ProjectId,
			//						Discount = a.Discount,
			//						DiscountAmount = a.DiscountAmount,
			//						TaxId = a.TaxId,
			//						BatchId = a.BatchId,
			//						TaxRate = a.TaxRate,
			//						TaxAmount = a.TaxAmount,
			//						GrossAmount = a.GrossAmount,
			//						NetAmount = a.NetAmount,
			//						Amount = a.Amount,
			//						Description = a.Description,
			//						TenantId = a.TenantId,
			//						ProductName = b.ProductName,
			//						UnitName = c.UnitName,
			//						LedgerName = e.LedgerName,
			//						TaxName = res.TaxName
			//					}).ToListAsync();
			//return result;
		}

        public async Task<PurchaseMasterView> PurchaseInvoiceView(int id)
        {
			var result = await (from a in _context.PurchaseMaster
								join b in _context.CustomerSupplier on a.CustomerSupplierId equals b.CustomerSupplierId
								where a.PurchaseMasterId == id
								select new PurchaseMasterView
								{
									PurchaseMasterId = a.PurchaseMasterId,
									VoucherNo = a.VoucherNo,
									SerialNo = a.SerialNo,
									OrderNo = a.OrderNo,
									VoucherTypeId = a.VoucherTypeId,
									WarehouseId = a.WarehouseId,
									Date = a.Date,
									DueDate = a.DueDate,
									CustomerSupplierId = a.CustomerSupplierId,
									Reference = a.Reference,
									Narration = a.Narration,
									PurchaseOrderMasterId = a.PurchaseOrderMasterId,
									TotalTax = a.TotalTax,
									DisPer = a.DisPer,
									BillDiscount = a.BillDiscount,
									DiscountType = a.DiscountType,
									GrandTotal = a.GrandTotal,
									TotalAmount = a.TotalAmount,
									NetAmounts = a.NetAmounts,
									PayAmount = a.PayAmount,
									BalanceDue = a.BalanceDue,
									Status = a.Status,
									PreviousDue = a.PreviousDue,
									PaymentStatus = a.PaymentStatus,
									UserId = a.UserId,
									FinancialYearId = a.FinancialYearId,
									TenantId = a.TenantId,
									Email = b.Email,
									AddedDate = a.AddedDate,
									ModifyDate = a.ModifyDate,
									Name = b.Name
								}).FirstOrDefaultAsync();
			return result;
		}

        public async Task<bool> Update(PurchaseMaster master)
        {
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlTransaction sql = sqlcon.BeginTransaction();
			try
			{
				var para = new DynamicParameters();
				para.Add("@PurchaseMasterId", master.PurchaseMasterId);
				para.Add("@SerialNo", master.SerialNo);
				para.Add("@VoucherNo", master.VoucherNo);
				para.Add("@WarehouseId", master.WarehouseId);
				para.Add("@OrderNo", master.OrderNo);
				para.Add("@Date", master.Date);
				para.Add("@CustomerSupplierId", master.CustomerSupplierId);
				para.Add("@DueDate", master.DueDate);
				para.Add("@Narration", master.Narration);
				para.Add("@PurchaseOrderMasterId", master.PurchaseOrderMasterId);
				para.Add("@NetAmounts", master.NetAmounts);
				para.Add("@Reference", master.Reference);
				para.Add("@TotalTax", master.TotalTax);
				para.Add("@DisPer", master.DisPer);
				para.Add("@DiscountType", master.DiscountType);
				para.Add("@BillDiscount", master.BillDiscount);
				para.Add("@GrandTotal", master.GrandTotal);
				para.Add("@TotalAmount", master.TotalAmount);
				para.Add("@PayAmount", master.PayAmount);
				para.Add("@PreviousDue", master.PreviousDue);
				para.Add("@VoucherTypeId", master.VoucherTypeId);
				para.Add("@UserId", master.UserId);
				para.Add("@FinancialYearId", master.FinancialYearId);
				para.Add("@BalanceDue", master.BalanceDue);
				para.Add("@Status", master.Status);
				para.Add("@PaymentStatus", master.PaymentStatus);
				para.Add("@AddedDate", DateTime.Now);
				para.Add("@TenantId", tenantId);
				sqlcon.Execute("UPDATE PurchaseMaster SET SerialNo=@SerialNo,VoucherNo=@VoucherNo,WarehouseId=@WarehouseId,OrderNo=@OrderNo,Date=@Date,CustomerSupplierId=@CustomerSupplierId,DueDate=@DueDate,Narration=@Narration,PurchaseOrderMasterId=@PurchaseOrderMasterId,NetAmounts=@NetAmounts,Reference=@Reference,TotalTax=@TotalTax,DisPer=@DisPer,DiscountType=@DiscountType,BillDiscount=@BillDiscount,GrandTotal=@GrandTotal,TotalAmount=@TotalAmount,PreviousDue=@PreviousDue,VoucherTypeId=@VoucherTypeId,UserId=@UserId,FinancialYearId=@FinancialYearId,BalanceDue=@BalanceDue,Status=@Status,PaymentStatus=@PaymentStatus,PayAmount=@PayAmount,AddedDate=@AddedDate,TenantId=@TenantId where PurchaseMasterId=@PurchaseMasterId", para, sql, 0, CommandType.Text);

				foreach (var item in master.listOrder)
				{
					if (item.PurchaseDetailsId == 0)
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@PurchaseMasterId", master.PurchaseMasterId);
						paraOpening.Add("@OrderDetailsId", 0);
						paraOpening.Add("@ProductId", item.ProductId);
						paraOpening.Add("@Qty", item.Qty);
						paraOpening.Add("@Rate", item.Rate);
						paraOpening.Add("@UnitId", item.UnitId);
						paraOpening.Add("@Discount", item.Discount);
						paraOpening.Add("@DiscountAmount", item.DiscountAmount);
						paraOpening.Add("@TaxId", item.TaxId);
						paraOpening.Add("@TaxRate", item.TaxRate);
						paraOpening.Add("@BatchId", item.BatchId);
						paraOpening.Add("@TaxAmount", item.TaxAmount);
						paraOpening.Add("@Amount", item.Amount);
						paraOpening.Add("@GrossAmount", item.GrossAmount);
						paraOpening.Add("@NetAmount", item.NetAmount);
						paraOpening.Add("@LedgerId", item.LedgerId);
						paraOpening.Add("@ProjectId", item.ProjectId);
						paraOpening.Add("@WarehouseId", item.WarehouseId);
						paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						paraOpening.Add("@DId", dbType: DbType.Int32, direction: ParameterDirection.Output);
						sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO PurchaseDetails (PurchaseMasterId,OrderDetailsId,ProductId,Qty,Rate,UnitId,Discount,DiscountAmount,TaxId,BatchId,TaxAmount,GrossAmount,NetAmount,LedgerId,ProjectId,TaxRate,Amount,Description,WarehouseId,TenantId)VALUES(@PurchaseMasterId,@OrderDetailsId,@ProductId,@Qty,@Rate,@UnitId,@Discount,@DiscountAmount,@TaxId,@BatchId,@TaxAmount,@GrossAmount,@NetAmount,@LedgerId,@ProjectId,@TaxRate,@Amount,@Description,@WarehouseId,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @DId =SCOPE_IDENTITY()", paraOpening, sql, 0, CommandType.Text);
						int longdetailsId = paraOpening.Get<int>("DId");
						//AddStockPosting
						var parastock = new DynamicParameters();
						parastock.Add("@BatchId", item.BatchId);
						parastock.Add("@WarehouseId", item.WarehouseId);
						parastock.Add("@Date", master.Date);
						parastock.Add("@FinancialYearId", master.FinancialYearId);
						parastock.Add("@InwardQty", item.Qty);
						parastock.Add("@OutwardQty", 0);
						parastock.Add("@ProductId", item.ProductId);
						parastock.Add("@Rate", item.Rate);
						parastock.Add("@UnitId", item.UnitId);
						parastock.Add("@DetailsId", longdetailsId);
						parastock.Add("@InvoiceNo", master.OrderNo);
						parastock.Add("@VoucherNo", master.VoucherNo);
						parastock.Add("@VoucherTypeId", master.VoucherTypeId);
						parastock.Add("@AgainstInvoiceNo", "NA");
						parastock.Add("@AgainstVoucherNo", "NA");
						parastock.Add("@AgainstVoucherTypeId", 0);
						parastock.Add("@StockCalculate", "Purchase");
						parastock.Add("@AddedDate", DateTime.Now);
						parastock.Add("@TenantId", tenantId);
						var valuesStock = sqlcon.Query<int>("INSERT INTO StockPosting (BatchId,WarehouseId,Date,FinancialYearId,InwardQty,OutwardQty,ProductId,Rate,UnitId,DetailsId,InvoiceNo,VoucherNo,VoucherTypeId,AgainstInvoiceNo,AgainstVoucherNo,AgainstVoucherTypeId,StockCalculate,AddedDate,TenantId)VALUES(@BatchId,@WarehouseId,@Date,@FinancialYearId,@InwardQty,@OutwardQty,@ProductId,@Rate,@UnitId,@DetailsId,@InvoiceNo,@VoucherNo,@VoucherTypeId,@AgainstInvoiceNo,@AgainstVoucherNo,@AgainstVoucherTypeId,@StockCalculate,@AddedDate,@TenantId)", parastock, sql, true, 0, commandType: CommandType.Text);

					}
					else
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@PurchaseDetailsId", item.PurchaseDetailsId);
						paraOpening.Add("@PurchaseMasterId", master.PurchaseMasterId);
						paraOpening.Add("@OrderDetailsId", 0);
						paraOpening.Add("@ProductId", item.ProductId);
						paraOpening.Add("@Qty", item.Qty);
						paraOpening.Add("@Rate", item.Rate);
						paraOpening.Add("@UnitId", item.UnitId);
						paraOpening.Add("@Discount", item.Discount);
						paraOpening.Add("@DiscountAmount", item.DiscountAmount);
						paraOpening.Add("@TaxId", item.TaxId);
						paraOpening.Add("@TaxRate", item.TaxRate);
						paraOpening.Add("@BatchId", item.BatchId);
						paraOpening.Add("@TaxAmount", item.TaxAmount);
						paraOpening.Add("@Amount", item.Amount);
						paraOpening.Add("@GrossAmount", item.GrossAmount);
						paraOpening.Add("@NetAmount", item.NetAmount);
						paraOpening.Add("@LedgerId", item.LedgerId);
						paraOpening.Add("@ProjectId", item.ProjectId);
						paraOpening.Add("@WarehouseId", item.WarehouseId);
						paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						sqlcon.Execute("UPDATE PurchaseDetails SET PurchaseMasterId=@PurchaseMasterId,OrderDetailsId=@OrderDetailsId,ProductId=@ProductId,Qty=@Qty,Rate=@Rate,UnitId=@UnitId,Discount=@Discount,DiscountAmount=@DiscountAmount,TaxId=@TaxId,BatchId=@BatchId,TaxAmount=@TaxAmount,GrossAmount=@GrossAmount,NetAmount=@NetAmount,LedgerId=@LedgerId,ProjectId=@ProjectId,TaxRate=@TaxRate,Amount=@Amount,Description=@Description,WarehouseId=@WarehouseId,TenantId=@TenantId where PurchaseDetailsId=@PurchaseDetailsId", paraOpening, sql, 0, CommandType.Text);

						//GetStockPostingId
						var parameters = new DynamicParameters();
						parameters.Add("@DetailsId", item.PurchaseDetailsId);
						parameters.Add("@VoucherTypeId", master.VoucherTypeId);
						parameters.Add("@TenantId", tenantId);
						var ListofPlan = sqlcon.Query<StockPosting>("SELECT StockPostingId FROM StockPosting where DetailsId=@DetailsId and VoucherTypeId=@VoucherTypeId and TenantId=@TenantId", parameters, sql, true, 0, commandType: CommandType.Text).FirstOrDefault();
						//UpdateStockPosting
						if (ListofPlan != null)
						{
							var parastock = new DynamicParameters();
							parastock.Add("@StockPostingId", ListofPlan.StockPostingId);
							parastock.Add("@BatchId", item.BatchId);
							parastock.Add("@WarehouseId", item.WarehouseId);
							parastock.Add("@Date", master.Date);
							parastock.Add("@FinancialYearId", master.FinancialYearId);
							parastock.Add("@InwardQty", item.Qty);
							parastock.Add("@OutwardQty", 0);
							parastock.Add("@ProductId", item.ProductId);
							parastock.Add("@Rate", item.Rate);
							parastock.Add("@UnitId", item.UnitId);
							parastock.Add("@DetailsId", item.PurchaseDetailsId);
							parastock.Add("@InvoiceNo", master.OrderNo);
							parastock.Add("@VoucherNo", master.VoucherNo);
							parastock.Add("@VoucherTypeId", master.VoucherTypeId);
							parastock.Add("@AgainstInvoiceNo", "NA");
							parastock.Add("@AgainstVoucherNo", "NA");
							parastock.Add("@AgainstVoucherTypeId", 0);
							parastock.Add("@StockCalculate", "Purchase");
							parastock.Add("@AddedDate", DateTime.Now);
							parastock.Add("@TenantId", tenantId);
							var valuesStock = sqlcon.Query<int>("UPDATE StockPosting SET BatchId=@BatchId,WarehouseId=@WarehouseId,Date=@Date,FinancialYearId=@FinancialYearId,InwardQty=@InwardQty,OutwardQty=@OutwardQty,ProductId=@ProductId,Rate=@Rate,UnitId=@UnitId,DetailsId=@DetailsId,InvoiceNo=@InvoiceNo,VoucherNo=@VoucherNo,VoucherTypeId=@VoucherTypeId,AgainstInvoiceNo=@AgainstInvoiceNo,AgainstVoucherNo=@AgainstVoucherNo,AgainstVoucherTypeId=@AgainstVoucherTypeId,StockCalculate=@StockCalculate,AddedDate=@AddedDate,TenantId=@TenantId where StockPostingId=@StockPostingId", parastock, sql, true, 0, commandType: CommandType.Text);
						}
						}
				}



				//DeleteLedgerPosting
				var paraLedgerDelete = new DynamicParameters();
				paraLedgerDelete.Add("@DetailsId", master.PurchaseMasterId);
				paraLedgerDelete.Add("@VoucherTypeId", master.VoucherTypeId);
				paraLedgerDelete.Add("@TenantId", tenantId);
				var valueScDelete = sqlcon.Query<long>("DELETE FROM LedgerPosting where DetailsId=@DetailsId AND VoucherTypeId=@VoucherTypeId AND TenantId=@TenantId", paraLedgerDelete, sql, true, 0, commandType: CommandType.Text);

				//AddLedgerPosting
				foreach (var item in master.listOrder)
				{
					var paraSc = new DynamicParameters();
					paraSc.Add("@Date", master.Date);
					paraSc.Add("@LedgerId", item.LedgerId);
					paraSc.Add("@Debit", item.Amount);
					paraSc.Add("@Credit", 0);
					paraSc.Add("@VoucherNo", master.VoucherNo);
					paraSc.Add("@DetailsId", master.PurchaseMasterId);
					paraSc.Add("@YearId", master.FinancialYearId);
					paraSc.Add("@InvoiceNo", master.OrderNo);
					paraSc.Add("@VoucherTypeId", master.VoucherTypeId);
					paraSc.Add("@LongReference", master.Narration);
					paraSc.Add("@ReferenceN", master.Narration);
					paraSc.Add("@ChequeNo", string.Empty);
					paraSc.Add("@ChequeDate", string.Empty);
					paraSc.Add("@AddedDate", DateTime.Now);
					paraSc.Add("@TenantId", tenantId);
					var valueSc = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraSc, sql, true, 0, commandType: CommandType.Text);
				}
				//Posting Accounts Payable
				var paraPayable = new DynamicParameters();
				paraPayable.Add("@Date", master.Date);
				paraPayable.Add("@LedgerId", 16);
				paraPayable.Add("@Debit", 0);
				paraPayable.Add("@Credit", master.GrandTotal);
				paraPayable.Add("@VoucherNo", master.VoucherNo);
				paraPayable.Add("@DetailsId", master.PurchaseMasterId);
				paraPayable.Add("@YearId", master.FinancialYearId);
				paraPayable.Add("@InvoiceNo", master.OrderNo);
				paraPayable.Add("@VoucherTypeId", master.VoucherTypeId);
				paraPayable.Add("@LongReference", master.Narration);
				paraPayable.Add("@ReferenceN", master.Narration);
				paraPayable.Add("@ChequeNo", string.Empty);
				paraPayable.Add("@ChequeDate", string.Empty);
				paraPayable.Add("@AddedDate", DateTime.Now);
				paraPayable.Add("@TenantId", tenantId);
				var valuePayable = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraPayable, sql, true, 0, commandType: CommandType.Text);

				//Posting Tax Payable
				var paraTaxPayable = new DynamicParameters();
				paraTaxPayable.Add("@Date", master.Date);
				paraTaxPayable.Add("@LedgerId", 22);
				paraTaxPayable.Add("@Debit", master.TotalTax);
				paraTaxPayable.Add("@Credit", 0);
				paraTaxPayable.Add("@VoucherNo", master.VoucherNo);
				paraTaxPayable.Add("@DetailsId", master.PurchaseMasterId);
				paraTaxPayable.Add("@YearId", master.FinancialYearId);
				paraTaxPayable.Add("@InvoiceNo", master.OrderNo);
				paraTaxPayable.Add("@VoucherTypeId", master.VoucherTypeId);
				paraTaxPayable.Add("@LongReference", master.Narration);
				paraTaxPayable.Add("@ReferenceN", master.Narration);
				paraTaxPayable.Add("@ChequeNo", string.Empty);
				paraTaxPayable.Add("@ChequeDate", string.Empty);
				paraTaxPayable.Add("@AddedDate", DateTime.Now);
				paraTaxPayable.Add("@TenantId", tenantId);
				var valueTaxPayable = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraTaxPayable, sql, true, 0, commandType: CommandType.Text);

				//Discount
				var paraDiscount = new DynamicParameters();
				paraDiscount.Add("@Date", master.Date);
				paraDiscount.Add("@LedgerId", 59); //DiscountAllowed
				paraDiscount.Add("@Debit", 0);
				paraDiscount.Add("@Credit", master.BillDiscount);
				paraDiscount.Add("@VoucherNo", master.VoucherNo);
				paraDiscount.Add("@DetailsId", master.PurchaseMasterId);
				paraDiscount.Add("@YearId", master.FinancialYearId);
				paraDiscount.Add("@InvoiceNo", master.OrderNo);
				paraDiscount.Add("@VoucherTypeId", master.VoucherTypeId);
				paraDiscount.Add("@LongReference", master.Narration);
				paraDiscount.Add("@ReferenceN", master.Narration);
				paraDiscount.Add("@ChequeNo", string.Empty);
				paraDiscount.Add("@ChequeDate", string.Empty);
				paraDiscount.Add("@AddedDate", DateTime.Now);
				paraDiscount.Add("@TenantId", tenantId);
				var valueparaDiscount = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraDiscount, sql, true, 0, commandType: CommandType.Text);



				//Delete
				//Delete
				foreach (var item in master.listDelete)
				{
					var paraDetailsDelete = new DynamicParameters();
					paraDetailsDelete.Add("@PurchaseDetailsId", item.PurchaseDetailsId);
					paraDetailsDelete.Add("@TenantId", tenantId);
					var value = sqlcon.Query<long>("DELETE FROM PurchaseDetails where PurchaseDetailsId=@PurchaseDetailsId AND TenantId=@TenantId", paraDetailsDelete, sql, true, 0, commandType: CommandType.Text);
				}
				foreach (var item in master.listDelete)
				{
					var parPartyBalanceId = new DynamicParameters();
					parPartyBalanceId.Add("@DetailsId", item.PurchaseDetailsId);
					parPartyBalanceId.Add("@VoucherTypeId", master.VoucherTypeId);
					parPartyBalanceId.Add("@TenantId", tenantId);
					var returPartyBalanceId = sqlcon.Query<StockPosting>("SELECT StockPostingId FROM StockPosting where DetailsId=@DetailsId and VoucherTypeId=@VoucherTypeId AND TenantId=@TenantId", parPartyBalanceId, sql, true, 0, commandType: CommandType.Text).FirstOrDefault();
					if (returPartyBalanceId !=null)
					{
						var paraDetailsDelete = new DynamicParameters();
						paraDetailsDelete.Add("@StockPostingId", returPartyBalanceId.StockPostingId);
						paraDetailsDelete.Add("@TenantId", tenantId);
						var value = sqlcon.Query<long>("DELETE FROM StockPosting where StockPostingId=@StockPostingId AND TenantId=@TenantId", paraDetailsDelete, sql, true, 0, commandType: CommandType.Text);
					}
					}
				sql.Commit();
				return true;
			}
			catch (Exception ex)
			{
				sql.Rollback();
				return false;
			}
		}
		public async Task<bool> UpdateDraft(PurchaseMaster master)
		{
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlTransaction sql = sqlcon.BeginTransaction();
			try
			{
				var para = new DynamicParameters();
				para.Add("@PurchaseMasterId", master.PurchaseMasterId);
				para.Add("@SerialNo", master.SerialNo);
				para.Add("@VoucherNo", master.VoucherNo);
				para.Add("@WarehouseId", master.WarehouseId);
				para.Add("@OrderNo", master.OrderNo);
				para.Add("@Date", master.Date);
				para.Add("@CustomerSupplierId", master.CustomerSupplierId);
				para.Add("@DueDate", master.DueDate);
				para.Add("@Narration", master.Narration);
				para.Add("@PurchaseOrderMasterId", master.PurchaseOrderMasterId);
				para.Add("@NetAmounts", master.NetAmounts);
				para.Add("@Reference", master.Reference);
				para.Add("@TotalTax", master.TotalTax);
				para.Add("@DisPer", master.DisPer);
				para.Add("@DiscountType", master.DiscountType);
				para.Add("@BillDiscount", master.BillDiscount);
				para.Add("@GrandTotal", master.GrandTotal);
				para.Add("@TotalAmount", master.TotalAmount);
				para.Add("@PayAmount", master.PayAmount);
				para.Add("@PreviousDue", master.PreviousDue);
				para.Add("@VoucherTypeId", master.VoucherTypeId);
				para.Add("@UserId", master.UserId);
				para.Add("@FinancialYearId", master.FinancialYearId);
				para.Add("@BalanceDue", master.BalanceDue);
				para.Add("@Status", master.Status);
				para.Add("@PaymentStatus", master.PaymentStatus);
				para.Add("@AddedDate", DateTime.Now);
				para.Add("@TenantId", tenantId);
				sqlcon.Execute("UPDATE PurchaseMaster SET SerialNo=@SerialNo,VoucherNo=@VoucherNo,WarehouseId=@WarehouseId,OrderNo=@OrderNo,Date=@Date,CustomerSupplierId=@CustomerSupplierId,DueDate=@DueDate,Narration=@Narration,PurchaseOrderMasterId=@PurchaseOrderMasterId,NetAmounts=@NetAmounts,Reference=@Reference,TotalTax=@TotalTax,DisPer=@DisPer,DiscountType=@DiscountType,BillDiscount=@BillDiscount,GrandTotal=@GrandTotal,TotalAmount=@TotalAmount,PreviousDue=@PreviousDue,VoucherTypeId=@VoucherTypeId,UserId=@UserId,FinancialYearId=@FinancialYearId,BalanceDue=@BalanceDue,Status=@Status,PaymentStatus=@PaymentStatus,PayAmount=@PayAmount,AddedDate=@AddedDate,TenantId=@TenantId where PurchaseMasterId=@PurchaseMasterId", para, sql, 0, CommandType.Text);

				foreach (var item in master.listOrder)
				{
					if (item.PurchaseDetailsId == 0)
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@PurchaseMasterId", master.PurchaseMasterId);
						paraOpening.Add("@OrderDetailsId", 0);
						paraOpening.Add("@ProductId", item.ProductId);
						paraOpening.Add("@Qty", item.Qty);
						paraOpening.Add("@Rate", item.Rate);
						paraOpening.Add("@UnitId", item.UnitId);
						paraOpening.Add("@Discount", item.Discount);
						paraOpening.Add("@DiscountAmount", item.DiscountAmount);
						paraOpening.Add("@TaxId", item.TaxId);
						paraOpening.Add("@TaxRate", item.TaxRate);
						paraOpening.Add("@BatchId", item.BatchId);
						paraOpening.Add("@TaxAmount", item.TaxAmount);
						paraOpening.Add("@Amount", item.Amount);
						paraOpening.Add("@GrossAmount", item.GrossAmount);
						paraOpening.Add("@NetAmount", item.NetAmount);
						paraOpening.Add("@LedgerId", item.LedgerId);
						paraOpening.Add("@ProjectId", item.ProjectId);
						paraOpening.Add("@WarehouseId", item.WarehouseId);
						paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						paraOpening.Add("@DId", dbType: DbType.Int32, direction: ParameterDirection.Output);
						sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO PurchaseDetails (PurchaseMasterId,OrderDetailsId,ProductId,Qty,Rate,UnitId,Discount,DiscountAmount,TaxId,BatchId,TaxAmount,GrossAmount,NetAmount,LedgerId,ProjectId,TaxRate,Amount,Description,WarehouseId,TenantId)VALUES(@PurchaseMasterId,@OrderDetailsId,@ProductId,@Qty,@Rate,@UnitId,@Discount,@DiscountAmount,@TaxId,@BatchId,@TaxAmount,@GrossAmount,@NetAmount,@LedgerId,@ProjectId,@TaxRate,@Amount,@Description,@WarehouseId,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @DId =SCOPE_IDENTITY()", paraOpening, sql, 0, CommandType.Text);
						int longdetailsId = paraOpening.Get<int>("DId");
					}
					else
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@PurchaseDetailsId", item.PurchaseDetailsId);
						paraOpening.Add("@PurchaseMasterId", master.PurchaseMasterId);
						paraOpening.Add("@OrderDetailsId", 0);
						paraOpening.Add("@ProductId", item.ProductId);
						paraOpening.Add("@Qty", item.Qty);
						paraOpening.Add("@Rate", item.Rate);
						paraOpening.Add("@UnitId", item.UnitId);
						paraOpening.Add("@Discount", item.Discount);
						paraOpening.Add("@DiscountAmount", item.DiscountAmount);
						paraOpening.Add("@TaxId", item.TaxId);
						paraOpening.Add("@TaxRate", item.TaxRate);
						paraOpening.Add("@BatchId", item.BatchId);
						paraOpening.Add("@TaxAmount", item.TaxAmount);
						paraOpening.Add("@Amount", item.Amount);
						paraOpening.Add("@GrossAmount", item.GrossAmount);
						paraOpening.Add("@NetAmount", item.NetAmount);
						paraOpening.Add("@LedgerId", item.LedgerId);
						paraOpening.Add("@ProjectId", item.ProjectId);
						paraOpening.Add("@WarehouseId", item.WarehouseId);
						paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						sqlcon.Execute("UPDATE PurchaseDetails SET PurchaseMasterId=@PurchaseMasterId,OrderDetailsId=@OrderDetailsId,ProductId=@ProductId,Qty=@Qty,Rate=@Rate,UnitId=@UnitId,Discount=@Discount,DiscountAmount=@DiscountAmount,TaxId=@TaxId,BatchId=@BatchId,TaxAmount=@TaxAmount,GrossAmount=@GrossAmount,NetAmount=@NetAmount,LedgerId=@LedgerId,ProjectId=@ProjectId,TaxRate=@TaxRate,Amount=@Amount,Description=@Description,WarehouseId=@WarehouseId,TenantId=@TenantId where PurchaseDetailsId=@PurchaseDetailsId", paraOpening, sql, 0, CommandType.Text);
					}
				}
				sql.Commit();
				return true;
			}
			catch (Exception ex)
			{
				sql.Rollback();
				return false;
			}
		}
		public async Task<bool> ApprovedUpdate(PurchaseMaster master)
		{
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlTransaction sql = sqlcon.BeginTransaction();
			try
			{
				var para = new DynamicParameters();
				para.Add("@PurchaseMasterId", master.PurchaseMasterId);
				para.Add("@SerialNo", master.SerialNo);
				para.Add("@VoucherNo", master.VoucherNo);
				para.Add("@WarehouseId", master.WarehouseId);
				para.Add("@OrderNo", master.OrderNo);
				para.Add("@Date", master.Date);
				para.Add("@CustomerSupplierId", master.CustomerSupplierId);
				para.Add("@DueDate", master.DueDate);
				para.Add("@Narration", master.Narration);
				para.Add("@PurchaseOrderMasterId", master.PurchaseOrderMasterId);
				para.Add("@NetAmounts", master.NetAmounts);
				para.Add("@Reference", master.Reference);
				para.Add("@TotalTax", master.TotalTax);
				para.Add("@DisPer", master.DisPer);
				para.Add("@DiscountType", master.DiscountType);
				para.Add("@BillDiscount", master.BillDiscount);
				para.Add("@GrandTotal", master.GrandTotal);
				para.Add("@TotalAmount", master.TotalAmount);
				para.Add("@PayAmount", master.PayAmount);
				para.Add("@PreviousDue", master.PreviousDue);
				para.Add("@VoucherTypeId", master.VoucherTypeId);
				para.Add("@UserId", master.UserId);
				para.Add("@FinancialYearId", master.FinancialYearId);
				para.Add("@BalanceDue", master.BalanceDue);
				para.Add("@Status", master.Status);
				para.Add("@PaymentStatus", master.PaymentStatus);
				para.Add("@AddedDate", DateTime.Now);
				para.Add("@TenantId", tenantId);
				sqlcon.Execute("UPDATE PurchaseMaster SET SerialNo=@SerialNo,VoucherNo=@VoucherNo,WarehouseId=@WarehouseId,OrderNo=@OrderNo,Date=@Date,CustomerSupplierId=@CustomerSupplierId,DueDate=@DueDate,Narration=@Narration,PurchaseOrderMasterId=@PurchaseOrderMasterId,NetAmounts=@NetAmounts,Reference=@Reference,TotalTax=@TotalTax,DisPer=@DisPer,DiscountType=@DiscountType,BillDiscount=@BillDiscount,GrandTotal=@GrandTotal,TotalAmount=@TotalAmount,PreviousDue=@PreviousDue,VoucherTypeId=@VoucherTypeId,UserId=@UserId,FinancialYearId=@FinancialYearId,BalanceDue=@BalanceDue,Status=@Status,PaymentStatus=@PaymentStatus,PayAmount=@PayAmount,AddedDate=@AddedDate,TenantId=@TenantId where PurchaseMasterId=@PurchaseMasterId", para, sql, 0, CommandType.Text);

				foreach (var item in master.listOrder)
				{
						//AddStockPosting
						var parastock = new DynamicParameters();
						parastock.Add("@BatchId", item.BatchId);
						parastock.Add("@WarehouseId", item.WarehouseId);
						parastock.Add("@Date", master.Date);
						parastock.Add("@FinancialYearId", master.FinancialYearId);
						parastock.Add("@InwardQty", item.Qty);
						parastock.Add("@OutwardQty", 0);
						parastock.Add("@ProductId", item.ProductId);
						parastock.Add("@Rate", item.Rate);
						parastock.Add("@UnitId", item.UnitId);
						parastock.Add("@DetailsId", item.PurchaseDetailsId);
						parastock.Add("@InvoiceNo", master.OrderNo);
						parastock.Add("@VoucherNo", master.VoucherNo);
						parastock.Add("@VoucherTypeId", master.VoucherTypeId);
						parastock.Add("@AgainstInvoiceNo", "NA");
						parastock.Add("@AgainstVoucherNo", "NA");
						parastock.Add("@AgainstVoucherTypeId", 0);
						parastock.Add("@StockCalculate", "Purchase");
						parastock.Add("@AddedDate", DateTime.Now);
						parastock.Add("@TenantId", tenantId);
						var valuesStock = sqlcon.Query<int>("INSERT INTO StockPosting (BatchId,WarehouseId,Date,FinancialYearId,InwardQty,OutwardQty,ProductId,Rate,UnitId,DetailsId,InvoiceNo,VoucherNo,VoucherTypeId,AgainstInvoiceNo,AgainstVoucherNo,AgainstVoucherTypeId,StockCalculate,AddedDate,TenantId)VALUES(@BatchId,@WarehouseId,@Date,@FinancialYearId,@InwardQty,@OutwardQty,@ProductId,@Rate,@UnitId,@DetailsId,@InvoiceNo,@VoucherNo,@VoucherTypeId,@AgainstInvoiceNo,@AgainstVoucherNo,@AgainstVoucherTypeId,@StockCalculate,@AddedDate,@TenantId)", parastock, sql, true, 0, commandType: CommandType.Text);

				}
				//AddLedgerPosting
				foreach (var item in master.listOrder)
				{
					var paraSc = new DynamicParameters();
					paraSc.Add("@Date", master.Date);
					paraSc.Add("@LedgerId", item.LedgerId);
					paraSc.Add("@Debit", item.Amount);
					paraSc.Add("@Credit", 0);
					paraSc.Add("@VoucherNo", master.VoucherNo);
					paraSc.Add("@DetailsId", master.PurchaseMasterId);
					paraSc.Add("@YearId", master.FinancialYearId);
					paraSc.Add("@InvoiceNo", master.OrderNo);
					paraSc.Add("@VoucherTypeId", master.VoucherTypeId);
					paraSc.Add("@LongReference", master.Narration);
					paraSc.Add("@ReferenceN", master.Narration);
					paraSc.Add("@ChequeNo", string.Empty);
					paraSc.Add("@ChequeDate", string.Empty);
					paraSc.Add("@AddedDate", DateTime.Now);
					paraSc.Add("@TenantId", tenantId);
					var valueSc = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraSc, sql, true, 0, commandType: CommandType.Text);
				}
				//Posting Accounts Payable
				var paraPayable = new DynamicParameters();
				paraPayable.Add("@Date", master.Date);
				paraPayable.Add("@LedgerId", 16);
				paraPayable.Add("@Debit", 0);
				paraPayable.Add("@Credit", master.GrandTotal);
				paraPayable.Add("@VoucherNo", master.VoucherNo);
				paraPayable.Add("@DetailsId", master.PurchaseMasterId);
				paraPayable.Add("@YearId", master.FinancialYearId);
				paraPayable.Add("@InvoiceNo", master.OrderNo);
				paraPayable.Add("@VoucherTypeId", master.VoucherTypeId);
				paraPayable.Add("@LongReference", master.Narration);
				paraPayable.Add("@ReferenceN", master.Narration);
				paraPayable.Add("@ChequeNo", string.Empty);
				paraPayable.Add("@ChequeDate", string.Empty);
				paraPayable.Add("@AddedDate", DateTime.Now);
				paraPayable.Add("@TenantId", tenantId);
				var valuePayable = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraPayable, sql, true, 0, commandType: CommandType.Text);

				//Posting Tax Payable
				var paraTaxPayable = new DynamicParameters();
				paraTaxPayable.Add("@Date", master.Date);
				paraTaxPayable.Add("@LedgerId", 22);
				paraTaxPayable.Add("@Debit", master.TotalTax);
				paraTaxPayable.Add("@Credit", 0);
				paraTaxPayable.Add("@VoucherNo", master.VoucherNo);
				paraTaxPayable.Add("@DetailsId", master.PurchaseMasterId);
				paraTaxPayable.Add("@YearId", master.FinancialYearId);
				paraTaxPayable.Add("@InvoiceNo", master.OrderNo);
				paraTaxPayable.Add("@VoucherTypeId", master.VoucherTypeId);
				paraTaxPayable.Add("@LongReference", master.Narration);
				paraTaxPayable.Add("@ReferenceN", master.Narration);
				paraTaxPayable.Add("@ChequeNo", string.Empty);
				paraTaxPayable.Add("@ChequeDate", string.Empty);
				paraTaxPayable.Add("@AddedDate", DateTime.Now);
				paraTaxPayable.Add("@TenantId", tenantId);
				var valueTaxPayable = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraTaxPayable, sql, true, 0, commandType: CommandType.Text);


				//Discount
				var paraDiscount = new DynamicParameters();
				paraDiscount.Add("@Date", master.Date);
				paraDiscount.Add("@LedgerId", 59); //DiscountAllowed
				paraDiscount.Add("@Debit", 0);
				paraDiscount.Add("@Credit", master.BillDiscount);
				paraDiscount.Add("@VoucherNo", master.VoucherNo);
				paraDiscount.Add("@DetailsId", master.PurchaseMasterId);
				paraDiscount.Add("@YearId", master.FinancialYearId);
				paraDiscount.Add("@InvoiceNo", master.OrderNo);
				paraDiscount.Add("@VoucherTypeId", master.VoucherTypeId);
				paraDiscount.Add("@LongReference", master.Narration);
				paraDiscount.Add("@ReferenceN", master.Narration);
				paraDiscount.Add("@ChequeNo", string.Empty);
				paraDiscount.Add("@ChequeDate", string.Empty);
				paraDiscount.Add("@AddedDate", DateTime.Now);
				paraDiscount.Add("@TenantId", tenantId);
				var valueparaDiscount = sqlcon.Query<int>("INSERT INTO LedgerPosting (Date,VoucherTypeId,VoucherNo,LedgerId,Debit,Credit,DetailsId,YearId,InvoiceNo,ChequeNo,ChequeDate,TenantId,ReferenceN,LongReference,AddedDate)VALUES(@Date,@VoucherTypeId,@VoucherNo,@LedgerId,@Debit,@Credit,@DetailsId,@YearId,@InvoiceNo,@ChequeNo,@ChequeDate,@TenantId,@ReferenceN,@LongReference,@AddedDate)", paraDiscount, sql, true, 0, commandType: CommandType.Text);


				sql.Commit();
				return true;
			}
			catch (Exception ex)
			{
				sql.Rollback();
				return false;
			}
		}
	}
}
