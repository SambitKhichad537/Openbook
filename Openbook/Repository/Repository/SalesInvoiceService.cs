﻿using Dapper;
using Microsoft.CodeAnalysis;
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
    public class SalesInvoiceService : ISalesInvoice
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseConnection _conn;
        private string tenantId;

        public SalesInvoiceService(ApplicationDbContext context, DatabaseConnection conn, IServicioTenant servicioTenant)
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
                return sqlcon.Query<bool>("SELECT VoucherNo FROM SalesMaster where VoucherNo=@VoucherNo AND tenantId=@tenantId", para, null, true, 0, CommandType.Text).SingleOrDefault();
            }
        }

        public async Task<bool> Delete(SalesMaster master)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            try
            {

                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand cmd = new SqlCommand("SalesInvoiceDelete", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter para = new SqlParameter();
                para = cmd.Parameters.Add("@SalesMasterId", SqlDbType.Int);
                para.Value = master.SalesMasterId;
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

        public async Task<int> Draft(SalesMaster master)
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
				para.Add("@OrderMasterId", master.OrderMasterId);
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
				sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO SalesMaster (SerialNo,VoucherNo,WarehouseId,OrderNo,Date,CustomerSupplierId,DueDate,Narration,OrderMasterId,NetAmounts,Reference,TotalTax,DisPer,DiscountType,BillDiscount,GrandTotal,TotalAmount,PreviousDue,VoucherTypeId,UserId,FinancialYearId,BalanceDue,Status,PaymentStatus,PayAmount,AddedDate,TenantId)VALUES(@SerialNo,@VoucherNo,@WarehouseId,@OrderNo,@Date,@CustomerSupplierId,@DueDate,@Narration,@OrderMasterId,@NetAmounts,@Reference,@TotalTax,@DisPer,@DiscountType,@BillDiscount,@GrandTotal,@TotalAmount,@PreviousDue,@VoucherTypeId,@UserId,@FinancialYearId,@BalanceDue,@Status,@PaymentStatus,@PayAmount,@AddedDate,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @MemIDOUT =SCOPE_IDENTITY()", para, sql, 0, CommandType.Text);
				int Id = para.Get<int>("MemIDOUT");
				if (Id > 0)
				{

					foreach (var item in master.listOrder)
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@SalesMasterId", Id);
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
						paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						paraOpening.Add("@DId", dbType: DbType.Int32, direction: ParameterDirection.Output);
						sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO SalesDetails (SalesMasterId,OrderDetailsId,ProductId,Qty,Rate,UnitId,Discount,DiscountAmount,TaxId,BatchId,TaxAmount,GrossAmount,NetAmount,LedgerId,ProjectId,TaxRate,Amount,Description,TenantId)VALUES(@SalesMasterId,@OrderDetailsId,@ProductId,@Qty,@Rate,@UnitId,@Discount,@DiscountAmount,@TaxId,@BatchId,@TaxAmount,@GrossAmount,@NetAmount,@LedgerId,@ProjectId,@TaxRate,@Amount,@Description,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @DId =SCOPE_IDENTITY()", paraOpening, sql, 0, CommandType.Text);
						int longdetailsId = paraOpening.Get<int>("DId");
				}
				}
			sql.Commit();
			return Id;
		}
		public async Task<List<SalesMasterView>> SalesInvoiceFilter( DateTime fromDate, DateTime toDate, int supplierid, string strVoucherNo, string strStatus , string strFiltetype)
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
				var ListofPlan = sqlcon.Query<SalesMasterView>("SalesInvoiceFilter", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}
		public async Task<SalesMaster> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@SalesMasterId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<SalesMaster>("SELECT *FROM SalesMaster where SalesMasterId=@SalesMasterId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }
		public SalesMaster GetbyIdView(int id)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@SalesMasterId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<SalesMaster>("SELECT *FROM SalesMaster where SalesMasterId=@SalesMasterId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
		}

		public async Task<string> GetSerialNo()
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@TenantId", tenantId);
                var val = sqlcon.Query<string>("SELECT ISNULL(MAX(SerialNo+1),1) as VoucherNo FROM SalesMaster where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return val;
            }
        }

        public async Task<int> Approved(SalesMaster master)
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
				para.Add("@OrderMasterId", master.OrderMasterId);
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
				sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO SalesMaster (SerialNo,VoucherNo,WarehouseId,OrderNo,Date,CustomerSupplierId,DueDate,Narration,OrderMasterId,NetAmounts,Reference,TotalTax,DisPer,DiscountType,BillDiscount,GrandTotal,TotalAmount,PreviousDue,VoucherTypeId,UserId,FinancialYearId,BalanceDue,Status,PaymentStatus,PayAmount,AddedDate,TenantId)VALUES(@SerialNo,@VoucherNo,@WarehouseId,@OrderNo,@Date,@CustomerSupplierId,@DueDate,@Narration,@OrderMasterId,@NetAmounts,@Reference,@TotalTax,@DisPer,@DiscountType,@BillDiscount,@GrandTotal,@TotalAmount,@PreviousDue,@VoucherTypeId,@UserId,@FinancialYearId,@BalanceDue,@Status,@PaymentStatus,@PayAmount,@AddedDate,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @MemIDOUT =SCOPE_IDENTITY()", para, sql, 0, CommandType.Text);
				int Id = para.Get<int>("MemIDOUT");
				if (Id > 0)
				{

					foreach (var item in master.listOrder)
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@SalesMasterId", Id);
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
						paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						paraOpening.Add("@DId", dbType: DbType.Int32, direction: ParameterDirection.Output);
						sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO SalesDetails (SalesMasterId,OrderDetailsId,ProductId,Qty,Rate,UnitId,Discount,DiscountAmount,TaxId,BatchId,TaxAmount,GrossAmount,NetAmount,LedgerId,ProjectId,TaxRate,Amount,Description,TenantId)VALUES(@SalesMasterId,@OrderDetailsId,@ProductId,@Qty,@Rate,@UnitId,@Discount,@DiscountAmount,@TaxId,@BatchId,@TaxAmount,@GrossAmount,@NetAmount,@LedgerId,@ProjectId,@TaxRate,@Amount,@Description,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @DId =SCOPE_IDENTITY()", paraOpening, sql, 0, CommandType.Text);
						int longdetailsId = paraOpening.Get<int>("DId");

						//StockPosting
						var parastock = new DynamicParameters();
						parastock.Add("@BatchId", item.BatchId);
						parastock.Add("@WarehouseId", 0);
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
						paraSc.Add("@Debit", 0);
						paraSc.Add("@Credit", item.Amount);
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
                    //Posting Accounts Receivable
                    var paraPayable = new DynamicParameters();
					paraPayable.Add("@Date", master.Date);
					paraPayable.Add("@LedgerId", 5);
					paraPayable.Add("@Debit", master.GrandTotal);
					paraPayable.Add("@Credit", 0);
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
					paraTaxPayable.Add("@Debit", 0);
					paraTaxPayable.Add("@Credit", master.TotalTax);
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
					paraDiscount.Add("@LedgerId", 58); //DiscountReceived
					paraDiscount.Add("@Debit", master.BillDiscount);
					paraDiscount.Add("@Credit", 0);
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
		public async Task<List<SalesMaster>> SalesBillView(int CustomerSupplierId)
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@CustomerSupplierId", CustomerSupplierId);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<SalesMaster>("SELECT SalesMasterId,VoucherNo,Date,GrandTotal,BalanceDue,OrderNo FROM SalesMaster where CustomerSupplierId=@CustomerSupplierId AND TenantId=@TenantId and BalanceDue !=0 AND Status !='Draft'", para, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}
			public async Task<List<ProductView>> SalesDetailsView(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@SalesMasterId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<ProductView>("SalesDetailsView", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
				return ListofPlan;
			}
		}

        public async Task<SalesMasterView> SalesInvoiceView(int id)
        {
			var result = await (from a in _context.SalesMaster
								join b in _context.CustomerSupplier on a.CustomerSupplierId equals b.CustomerSupplierId
								where a.SalesMasterId == id
								select new SalesMasterView
								{
                                    SalesMasterId = a.SalesMasterId,
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
									OrderMasterId = a.OrderMasterId,
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

        public async Task<bool> Update(SalesMaster master)
        {
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlTransaction sql = sqlcon.BeginTransaction();
			try
			{
				var para = new DynamicParameters();
				para.Add("@SalesMasterId", master.SalesMasterId);
				para.Add("@SerialNo", master.SerialNo);
				para.Add("@VoucherNo", master.VoucherNo);
				para.Add("@WarehouseId", master.WarehouseId);
				para.Add("@OrderNo", master.OrderNo);
				para.Add("@Date", master.Date);
				para.Add("@CustomerSupplierId", master.CustomerSupplierId);
				para.Add("@DueDate", master.DueDate);
				para.Add("@Narration", master.Narration);
				para.Add("@OrderMasterId", master.OrderMasterId);
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
				sqlcon.Execute("UPDATE SalesMaster SET SerialNo=@SerialNo,VoucherNo=@VoucherNo,WarehouseId=@WarehouseId,OrderNo=@OrderNo,Date=@Date,CustomerSupplierId=@CustomerSupplierId,DueDate=@DueDate,Narration=@Narration,OrderMasterId=@OrderMasterId,NetAmounts=@NetAmounts,Reference=@Reference,TotalTax=@TotalTax,DisPer=@DisPer,DiscountType=@DiscountType,BillDiscount=@BillDiscount,GrandTotal=@GrandTotal,TotalAmount=@TotalAmount,PreviousDue=@PreviousDue,VoucherTypeId=@VoucherTypeId,UserId=@UserId,FinancialYearId=@FinancialYearId,BalanceDue=@BalanceDue,Status=@Status,PaymentStatus=@PaymentStatus,PayAmount=@PayAmount,AddedDate=@AddedDate,TenantId=@TenantId where SalesMasterId=@SalesMasterId", para, sql, 0, CommandType.Text);

				foreach (var item in master.listOrder)
				{
					if (item.SalesDetailsId == 0)
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@SalesMasterId", master.SalesMasterId);
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
						paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						paraOpening.Add("@DId", dbType: DbType.Int32, direction: ParameterDirection.Output);
						sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO SalesDetails (SalesMasterId,OrderDetailsId,ProductId,Qty,Rate,UnitId,Discount,DiscountAmount,TaxId,BatchId,TaxAmount,GrossAmount,NetAmount,LedgerId,ProjectId,TaxRate,Amount,Description,TenantId)VALUES(@SalesMasterId,@OrderDetailsId,@ProductId,@Qty,@Rate,@UnitId,@Discount,@DiscountAmount,@TaxId,@BatchId,@TaxAmount,@GrossAmount,@NetAmount,@LedgerId,@ProjectId,@TaxRate,@Amount,@Description,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @DId =SCOPE_IDENTITY()", paraOpening, sql, 0, CommandType.Text);
						int longdetailsId = paraOpening.Get<int>("DId");
						//AddStockPosting
						var parastock = new DynamicParameters();
						parastock.Add("@BatchId", item.BatchId);
						parastock.Add("@WarehouseId", 0);
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
						paraOpening.Add("@SalesDetailsId", item.SalesDetailsId);
						paraOpening.Add("@SalesMasterId", master.SalesMasterId);
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
						paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						sqlcon.Execute("UPDATE SalesDetails SET SalesMasterId=@SalesMasterId,OrderDetailsId=@OrderDetailsId,ProductId=@ProductId,Qty=@Qty,Rate=@Rate,UnitId=@UnitId,Discount=@Discount,DiscountAmount=@DiscountAmount,TaxId=@TaxId,BatchId=@BatchId,TaxAmount=@TaxAmount,GrossAmount=@GrossAmount,NetAmount=@NetAmount,LedgerId=@LedgerId,ProjectId=@ProjectId,TaxRate=@TaxRate,Amount=@Amount,Description=@Description,TenantId=@TenantId where SalesDetailsId=@SalesDetailsId", paraOpening, sql, 0, CommandType.Text);

						//GetStockPostingId
						var parameters = new DynamicParameters();
						parameters.Add("@DetailsId", item.SalesDetailsId);
						parameters.Add("@VoucherTypeId", master.VoucherTypeId);
						parameters.Add("@TenantId", tenantId);
						var ListofPlan = sqlcon.Query<StockPosting>("SELECT StockPostingId FROM StockPosting where DetailsId=@DetailsId and VoucherTypeId=@VoucherTypeId and TenantId=@TenantId", parameters, sql, true, 0, commandType: CommandType.Text).FirstOrDefault();
						//UpdateStockPosting
						if (ListofPlan != null)
						{
							var parastock = new DynamicParameters();
							parastock.Add("@StockPostingId", ListofPlan.StockPostingId);
							parastock.Add("@BatchId", item.BatchId);
							parastock.Add("@WarehouseId", 0);
							parastock.Add("@Date", master.Date);
							parastock.Add("@FinancialYearId", master.FinancialYearId);
							parastock.Add("@InwardQty", item.Qty);
							parastock.Add("@OutwardQty", 0);
							parastock.Add("@ProductId", item.ProductId);
							parastock.Add("@Rate", item.Rate);
							parastock.Add("@UnitId", item.UnitId);
							parastock.Add("@DetailsId", item.SalesDetailsId);
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
				paraLedgerDelete.Add("@DetailsId", master.SalesMasterId);
				paraLedgerDelete.Add("@VoucherTypeId", master.VoucherTypeId);
				paraLedgerDelete.Add("@TenantId", tenantId);
				var valueScDelete = sqlcon.Query<long>("DELETE FROM LedgerPosting where DetailsId=@DetailsId AND VoucherTypeId=@VoucherTypeId AND TenantId=@TenantId", paraLedgerDelete, sql, true, 0, commandType: CommandType.Text);

				//AddLedgerPosting
				foreach (var item in master.listOrder)
				{
					var paraSc = new DynamicParameters();
					paraSc.Add("@Date", master.Date);
					paraSc.Add("@LedgerId", item.LedgerId);
					paraSc.Add("@Debit", 0);
					paraSc.Add("@Credit", item.Amount);
                    paraSc.Add("@VoucherNo", master.VoucherNo);
					paraSc.Add("@DetailsId", master.SalesMasterId);
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
				//Posting AccountsReceiable
				var paraPayable = new DynamicParameters();
				paraPayable.Add("@Date", master.Date);
				paraPayable.Add("@LedgerId", 5);
				paraPayable.Add("@Debit", master.GrandTotal);
				paraPayable.Add("@Credit", 0);
				paraPayable.Add("@VoucherNo", master.VoucherNo);
				paraPayable.Add("@DetailsId", master.SalesMasterId);
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
				paraTaxPayable.Add("@Debit", 0);
				paraTaxPayable.Add("@Credit", master.TotalTax);
                paraTaxPayable.Add("@VoucherNo", master.VoucherNo);
				paraTaxPayable.Add("@DetailsId", master.SalesMasterId);
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
				paraDiscount.Add("@LedgerId", 58); //DiscountReceived
				paraDiscount.Add("@Debit", master.BillDiscount);
				paraDiscount.Add("@Credit", 0);
				paraDiscount.Add("@VoucherNo", master.VoucherNo);
				paraDiscount.Add("@DetailsId", master.SalesMasterId);
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
					paraDetailsDelete.Add("@SalesDetailsId", item.SalesDetailsId);
					paraDetailsDelete.Add("@TenantId", tenantId);
					var value = sqlcon.Query<long>("DELETE FROM SalesDetails where SalesDetailsId=@SalesDetailsId AND TenantId=@TenantId", paraDetailsDelete, sql, true, 0, commandType: CommandType.Text);
				}
				foreach (var item in master.listDelete)
				{
					var parPartyBalanceId = new DynamicParameters();
					parPartyBalanceId.Add("@DetailsId", item.SalesDetailsId);
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
		public async Task<bool> UpdateDraft(SalesMaster master)
		{
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlTransaction sql = sqlcon.BeginTransaction();
			try
			{
				var para = new DynamicParameters();
				para.Add("@SalesMasterId", master.SalesMasterId);
				para.Add("@SerialNo", master.SerialNo);
				para.Add("@VoucherNo", master.VoucherNo);
				para.Add("@WarehouseId", master.WarehouseId);
				para.Add("@OrderNo", master.OrderNo);
				para.Add("@Date", master.Date);
				para.Add("@CustomerSupplierId", master.CustomerSupplierId);
				para.Add("@DueDate", master.DueDate);
				para.Add("@Narration", master.Narration);
				para.Add("@OrderMasterId", master.OrderMasterId);
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
				sqlcon.Execute("UPDATE SalesMaster SET SerialNo=@SerialNo,VoucherNo=@VoucherNo,WarehouseId=@WarehouseId,OrderNo=@OrderNo,Date=@Date,CustomerSupplierId=@CustomerSupplierId,DueDate=@DueDate,Narration=@Narration,OrderMasterId=@OrderMasterId,NetAmounts=@NetAmounts,Reference=@Reference,TotalTax=@TotalTax,DisPer=@DisPer,DiscountType=@DiscountType,BillDiscount=@BillDiscount,GrandTotal=@GrandTotal,TotalAmount=@TotalAmount,PreviousDue=@PreviousDue,VoucherTypeId=@VoucherTypeId,UserId=@UserId,FinancialYearId=@FinancialYearId,BalanceDue=@BalanceDue,Status=@Status,PaymentStatus=@PaymentStatus,PayAmount=@PayAmount,AddedDate=@AddedDate,TenantId=@TenantId where SalesMasterId=@SalesMasterId", para, sql, 0, CommandType.Text);

				foreach (var item in master.listOrder)
				{
					if (item.SalesDetailsId == 0)
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@SalesMasterId", master.SalesMasterId);
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
						paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						paraOpening.Add("@DId", dbType: DbType.Int32, direction: ParameterDirection.Output);
						sqlcon.Execute("DECLARE @ReturnValue int INSERT INTO SalesDetails (SalesMasterId,OrderDetailsId,ProductId,Qty,Rate,UnitId,Discount,DiscountAmount,TaxId,BatchId,TaxAmount,GrossAmount,NetAmount,LedgerId,ProjectId,TaxRate,Amount,Description,TenantId)VALUES(@SalesMasterId,@OrderDetailsId,@ProductId,@Qty,@Rate,@UnitId,@Discount,@DiscountAmount,@TaxId,@BatchId,@TaxAmount,@GrossAmount,@NetAmount,@LedgerId,@ProjectId,@TaxRate,@Amount,@Description,@TenantId) SELECT @ReturnValue = SCOPE_IDENTITY() set @DId =SCOPE_IDENTITY()", paraOpening, sql, 0, CommandType.Text);
						int longdetailsId = paraOpening.Get<int>("DId");
					}
					else
					{
						var paraOpening = new DynamicParameters();
						paraOpening.Add("@SalesDetailsId", item.SalesDetailsId);
						paraOpening.Add("@SalesMasterId", master.SalesMasterId);
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
						paraOpening.Add("@Description", item.Description);
						paraOpening.Add("@TenantId", tenantId);
						sqlcon.Execute("UPDATE SalesDetails SET SalesMasterId=@SalesMasterId,OrderDetailsId=@OrderDetailsId,ProductId=@ProductId,Qty=@Qty,Rate=@Rate,UnitId=@UnitId,Discount=@Discount,DiscountAmount=@DiscountAmount,TaxId=@TaxId,BatchId=@BatchId,TaxAmount=@TaxAmount,GrossAmount=@GrossAmount,NetAmount=@NetAmount,LedgerId=@LedgerId,ProjectId=@ProjectId,TaxRate=@TaxRate,Amount=@Amount,Description=@Description,TenantId=@TenantId where SalesDetailsId=@SalesDetailsId", paraOpening, sql, 0, CommandType.Text);
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
		public async Task<bool> ApprovedUpdate(SalesMaster master)
		{
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlTransaction sql = sqlcon.BeginTransaction();
			try
			{
				var para = new DynamicParameters();
				para.Add("@SalesMasterId", master.SalesMasterId);
				para.Add("@SerialNo", master.SerialNo);
				para.Add("@VoucherNo", master.VoucherNo);
				para.Add("@WarehouseId", master.WarehouseId);
				para.Add("@OrderNo", master.OrderNo);
				para.Add("@Date", master.Date);
				para.Add("@CustomerSupplierId", master.CustomerSupplierId);
				para.Add("@DueDate", master.DueDate);
				para.Add("@Narration", master.Narration);
				para.Add("@OrderMasterId", master.OrderMasterId);
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
				sqlcon.Execute("UPDATE SalesMaster SET SerialNo=@SerialNo,VoucherNo=@VoucherNo,WarehouseId=@WarehouseId,OrderNo=@OrderNo,Date=@Date,CustomerSupplierId=@CustomerSupplierId,DueDate=@DueDate,Narration=@Narration,OrderMasterId=@OrderMasterId,NetAmounts=@NetAmounts,Reference=@Reference,TotalTax=@TotalTax,DisPer=@DisPer,DiscountType=@DiscountType,BillDiscount=@BillDiscount,GrandTotal=@GrandTotal,TotalAmount=@TotalAmount,PreviousDue=@PreviousDue,VoucherTypeId=@VoucherTypeId,UserId=@UserId,FinancialYearId=@FinancialYearId,BalanceDue=@BalanceDue,Status=@Status,PaymentStatus=@PaymentStatus,PayAmount=@PayAmount,AddedDate=@AddedDate,TenantId=@TenantId where SalesMasterId=@SalesMasterId", para, sql, 0, CommandType.Text);

				foreach (var item in master.listOrder)
				{
						//AddStockPosting
						var parastock = new DynamicParameters();
						parastock.Add("@BatchId", item.BatchId);
						parastock.Add("@WarehouseId", 0);
						parastock.Add("@Date", master.Date);
						parastock.Add("@FinancialYearId", master.FinancialYearId);
						parastock.Add("@InwardQty", item.Qty);
						parastock.Add("@OutwardQty", 0);
						parastock.Add("@ProductId", item.ProductId);
						parastock.Add("@Rate", item.Rate);
						parastock.Add("@UnitId", item.UnitId);
						parastock.Add("@DetailsId", item.SalesDetailsId);
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
					paraSc.Add("@Debit", 0);
					paraSc.Add("@Credit", item.Amount);
                    paraSc.Add("@VoucherNo", master.VoucherNo);
					paraSc.Add("@DetailsId", master.SalesMasterId);
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
				//Posting AccountsReceiable
				var paraPayable = new DynamicParameters();
				paraPayable.Add("@Date", master.Date);
				paraPayable.Add("@LedgerId", 5);
				paraPayable.Add("@Debit", master.GrandTotal);
				paraPayable.Add("@Credit", 0);
				paraPayable.Add("@VoucherNo", master.VoucherNo);
				paraPayable.Add("@DetailsId", master.SalesMasterId);
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
				paraTaxPayable.Add("@Debit", 0);
				paraTaxPayable.Add("@Credit", master.TotalTax);
                paraTaxPayable.Add("@VoucherNo", master.VoucherNo);
				paraTaxPayable.Add("@DetailsId", master.SalesMasterId);
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
				paraDiscount.Add("@LedgerId", 58); //DiscountReceived
				paraDiscount.Add("@Debit", master.BillDiscount);
				paraDiscount.Add("@Credit", 0);
				paraDiscount.Add("@VoucherNo", master.VoucherNo);
				paraDiscount.Add("@DetailsId", master.SalesMasterId);
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
