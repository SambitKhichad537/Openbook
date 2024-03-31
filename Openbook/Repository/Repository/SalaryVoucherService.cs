using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.HrPayroll;
using Openbook.Data.Inventory;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
    public class SalaryVoucherService : ISalaryVoucher
    {
        private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
        private string tenantId;
        public SalaryVoucherService(ApplicationDbContext context, DatabaseConnection conn, IServicioTenant servicioTenant)
		{
            _context = context;
            _conn = conn;
            tenantId = servicioTenant.ObtenerTenant();
        }
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.SalaryVoucherMaster
                               where progm.YearMonth == name
                               select progm.SalaryVoucherMasterId).Count();
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
            var checkResult = (from progm in _context.SalaryVoucherMaster
                               where progm.VoucherNo == name
                               select progm.SalaryVoucherMasterId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.SalaryVoucherMaster
                                    where progm.VoucherNo == name
                                    select progm.SalaryVoucherMasterId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int SalaryVoucherMasterId)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand cmd = new SqlCommand("SalaryVoucherDelete", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter para = new SqlParameter();
                para = cmd.Parameters.Add("@SalaryVoucherMasterId", SqlDbType.Int);
                para.Value = SalaryVoucherMasterId;
                para = cmd.Parameters.Add("@TenantId", SqlDbType.NVarChar);
                para.Value = tenantId;
                long rowAffacted = cmd.ExecuteNonQuery();
                if (rowAffacted > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                sqlcon.Close();
            }
        }
        public async Task<AdvancePayment> GetAdvanceAmount(string name, int employeeid)
        {
            var result = await (from a in _context.AdvancePayment
                                where a.YearMonth == name && a.EmployeeId == employeeid
                                select new AdvancePayment
                                {
                                    AdvancePaymentId = a.AdvancePaymentId,
                                    Amount = a.Amount
                                }).FirstOrDefaultAsync();
            return result;
        }
        public async Task<string> GetSerialNo()
		{
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
                var para = new DynamicParameters();
                para.Add("@TenantId", tenantId);
                var val = sqlcon.Query<string>("SELECT ISNULL(MAX(SerialNo+1),1) as VoucherNo FROM SalaryVoucherMaster where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return val;
			}
		}
		public async Task<List<SalaryVoucherMasterView>> GetAll()
        {
            var result = await(from a in _context.SalaryVoucherMaster
                               select new SalaryVoucherMasterView
                               {
                                   SalaryVoucherMasterId = a.SalaryVoucherMasterId,
                                   VoucherNo = a.VoucherNo,
                                   TotalAmount = a.TotalAmount,
                                   Date = a.Date
							   }).ToListAsync();
            return result;
        }
        public async Task<SalaryVoucherMaster> GetbyId(int id)
        {
            SalaryVoucherMaster model = await _context.SalaryVoucherMaster.FindAsync(id);
            return model;
        }
        public async Task<SalaryVoucherDetails> GetPaidUnpaid(string month , int employeeid)
        {
            var result = await (from a in _context.SalaryVoucherMaster
                                join b in _context.SalaryVoucherDetails on a.SalaryVoucherMasterId equals b.SalaryVoucherMasterId
                                where a.YearMonth == month && b.EmployeeId == employeeid
                                select new SalaryVoucherDetails
                                {
                                    Status = b.Status
                                }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<int> Save(SalaryVoucherMaster model)
        {
            await _context.SalaryVoucherMaster.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.SalaryVoucherMasterId;
            foreach (var item in model.listOrder)
            {
                SalaryVoucherDetails details = new SalaryVoucherDetails();
                details.SalaryVoucherMasterId = id;
                details.EmployeeId = item.EmployeeId;
                details.Bonus = item.Bonus;
                details.Deduction = item.Deduction;
                details.Advance = item.Advance;
                details.Lop = item.Lop;
                details.Salary = item.Salary;
                details.Status = item.Status;
                details.TenantId = tenantId;
                await _context.SalaryVoucherDetails.AddAsync(details);
                await _context.SaveChangesAsync();
            }
            //CashBankPosting
            LedgerPosting cashPosting = new LedgerPosting();
            cashPosting.Date = model.Date;
            cashPosting.LedgerId = 10;
            cashPosting.Debit = 0;
            cashPosting.Credit = model.TotalAmount;
            cashPosting.VoucherNo = model.VoucherNo;
            cashPosting.DetailsId = id;
            cashPosting.YearId = 1;
            cashPosting.InvoiceNo = model.VoucherNo;
            cashPosting.VoucherTypeId = model.VoucherTypeId;
            cashPosting.LongReference = model.Narration;
            cashPosting.ReferenceN = model.Narration;
            cashPosting.ChequeNo = String.Empty;
            cashPosting.ChequeDate = String.Empty;
            cashPosting.AddedDate = DateTime.UtcNow;
            cashPosting.TenantId = tenantId;
            _context.LedgerPosting.Add(cashPosting);
            await _context.SaveChangesAsync();

            //PostingSalary
            LedgerPosting advancePosting = new LedgerPosting();
            advancePosting.Date = model.Date;
            advancePosting.LedgerId = 48;
            advancePosting.Debit = model.TotalAmount;
            advancePosting.Credit = 0;
            advancePosting.VoucherNo = model.VoucherNo;
            advancePosting.DetailsId = id;
            advancePosting.YearId = 1;
            advancePosting.InvoiceNo = model.VoucherNo;
            advancePosting.VoucherTypeId = model.VoucherTypeId;
            advancePosting.LongReference = model.Narration;
            advancePosting.ReferenceN = model.Narration;
            advancePosting.ChequeNo = String.Empty;
            advancePosting.ChequeDate = String.Empty;
            advancePosting.AddedDate = DateTime.UtcNow;
            advancePosting.TenantId = tenantId;
            _context.LedgerPosting.Add(advancePosting);
            await _context.SaveChangesAsync();
            return id;
        }

        public async Task<bool> Update(SalaryVoucherMaster model)
        {
            _context.SalaryVoucherMaster.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IList<SalaryVoucherDetailsView>> GetAllSalaryVoucher(string monthYear)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@monthYear", monthYear);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<SalaryVoucherDetailsView>("MonthlySalaryVoucherDetailsViewAll", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
                return ListofPlan;
            }
        }
        public async Task<IList<SalaryVoucherMasterView>> GetAllSalaryMonth()
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<SalaryVoucherMasterView>("SELECT DISTINCT YearMonth From SalaryVoucherMaster where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).ToList();
                return ListofPlan;
            }
        }
        public async Task<IList<SalaryVoucherDetailsView>> MonthlySalaryReport(DateTime fromdate, DateTime todate, int employeeid, string month)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@FromDate", fromdate);
                para.Add("@ToDate", todate);
                para.Add("@EmployeeId", employeeid);
                para.Add("@YearMonth", month);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<SalaryVoucherDetailsView>("MonthlySalaryReport", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
                return ListofPlan;
            }
        }
    }
}
