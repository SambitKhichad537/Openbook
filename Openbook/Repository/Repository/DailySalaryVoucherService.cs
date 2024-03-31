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
    public class DailySalaryVoucherService : IDailySalaryVoucher
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseConnection _conn;
        private string tenantId;
        public DailySalaryVoucherService(ApplicationDbContext context, DatabaseConnection conn, IServicioTenant servicioTenant)
        {
            _context = context;
            _conn = conn;
            tenantId = servicioTenant.ObtenerTenant();
        }
        public Task<bool> CheckName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<int> CheckNameId(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(int id)
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
                para.Value = id;
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

        public async Task<List<DailySalaryVoucherView>> GetAll()
        {
            var result = await(from a in _context.DailySalaryVoucherMaster
                               join b in _context.AccountLedger on a.LedgerId equals b.LedgerId
                               join c in _context.InvoiceSetting on a.VoucherTypeId equals c.VoucherTypeId
                               select new DailySalaryVoucherView
                               {
                                   DailySalaryVoucherMasterId = a.DailySalaryVoucherMasterId,
                                   VoucherNo = a.VoucherNo,
                                   TotalAmount = a.TotalAmount,
                                   Date = a.Date,
                                   LedgerName = b.LedgerName,
                                   VoucherTypeName = c.VoucherTypeName
                               }).ToListAsync();
            return result;
        }
        public async Task<IList<DailySalaryDetailsView>> GetAllEmployeeAttendance(string yearmonthday)
        {
            var result = await (from a in _context.DailyAttendanceMaster
                                join b in _context.DailyAttendanceDetails on a.DailyAttendanceMasterId equals b.DailyAttendanceMasterId
                                join c in _context.Employee on b.EmployeeId equals c.EmployeeId
                                where a.Narration == yearmonthday && b.Status == "Present"
                                select new DailySalaryDetailsView
                                {
                                    EmployeeId = c.EmployeeId,
                                    EmployeeCode = c.EmployeeCode,
                                    EmployeeName = c.EmployeeName,
                                    Wage = c.DailyWage,
                                    AttendanceStatus = b.Status
                                }).ToListAsync();
            return result;
        }

        public async Task<IList<DailySalaryVoucherView>> GetAllSalaryMonth()
        {
            throw new NotImplementedException();
        }

        public Task<IList<DailySalaryDetailsView>> GetAllSalaryVoucher(string monthYear)
        {
            throw new NotImplementedException();
        }

        public Task<DailySalaryVoucherMaster> GetbyId(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<DailySalaryVoucherDetails> GetStatusPaidUnpaid(string monthYear, int employeeid)
        {
            var result = await (from a in _context.DailySalaryVoucherMaster
                                join b in _context.DailySalaryVoucherDetails on a.DailySalaryVoucherMasterId equals b.DailySalaryVoucherMasterId
                                where a.YearMonthDay == monthYear && b.EmployeeId == employeeid
                                select new DailySalaryVoucherDetails
                                {
                                    DailySalaryVoucherMasterId = a.DailySalaryVoucherMasterId,
                                    DailySalaryVoucherDetailsId = b.DailySalaryVoucherDetailsId,
                                    YearMonthDay = a.YearMonthDay,
                                    Wage = b.Wage,
                                    Status = b.Status
                                }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<string> GetSerialNo()
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var val = sqlcon.Query<string>("SELECT ISNULL(MAX(SerialNo+1),1) as VoucherNo FROM DailySalaryVoucherMaster", null, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
                return val;
            }
        }

        public Task<IList<DailySalaryDetailsView>> MonthlySalaryReport(DateTime fromdate, DateTime todate, int employeeid, string month)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Save(DailySalaryVoucherMaster model)
        {
            var result = await (from a in _context.DailySalaryVoucherMaster
                                where a.YearMonthDay == model.YearMonthDay
                                select new DailySalaryVoucherMaster
                                {
                                    DailySalaryVoucherMasterId = a.DailySalaryVoucherMasterId
                                }).FirstOrDefaultAsync();
            int id = 0;
            if (result == null)
            {
                await _context.DailySalaryVoucherMaster.AddAsync(model);
                await _context.SaveChangesAsync();
                id = model.DailySalaryVoucherMasterId;
            }
            else
            {
                model.DailySalaryVoucherMasterId = result.DailySalaryVoucherMasterId;
                _context.DailySalaryVoucherMaster.Update(model);
                await _context.SaveChangesAsync();
            }
            //DeleteAttendanceDetails
            if (result != null)
            {
                using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
                {
                    var paraScDelete = new DynamicParameters();
                    paraScDelete.Add("@DailySalaryVoucherMasterId", result.DailySalaryVoucherMasterId);
                    var valueScDelete = sqlcon.Query<int>("DELETE FROM DailySalaryVoucherDetails where DailySalaryVoucherMasterId=@DailySalaryVoucherMasterId", paraScDelete, null, true, 0, commandType: CommandType.Text);
                }
            }
            foreach (var item in model.listOrder)
                {
                    DailySalaryVoucherDetails details = new DailySalaryVoucherDetails();
                        if (result != null)
                        {
                            details.DailySalaryVoucherMasterId = result.DailySalaryVoucherMasterId;
                        }
                        else
                        {
                            details.DailySalaryVoucherMasterId = id;
                        }
                        details.EmployeeId = item.EmployeeId;
                        details.Status = item.Status;
                        details.Wage = item.Wage;
                        await _context.DailySalaryVoucherDetails.AddAsync(details);
                        await _context.SaveChangesAsync();
                }

            //DeleteDailySalaryInvoiceLedger
            if (result != null)
            {
                using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
                {
                    var paraScDelete = new DynamicParameters();
                    paraScDelete.Add("@DetailsId", result.DailySalaryVoucherMasterId);
                    paraScDelete.Add("@VoucherTypeId", model.VoucherTypeId);
                    var valueScDelete = sqlcon.Query<int>("DELETE FROM LedgerPosting where DetailsId=@DetailsId AND VoucherTypeId=@VoucherTypeId", paraScDelete, null, true, 0, commandType: CommandType.Text);
                }
            }
                LedgerPosting cashPosting = new LedgerPosting();
            cashPosting.Date = model.Date;
            cashPosting.LedgerId = model.LedgerId;
            cashPosting.Debit = 0;
            cashPosting.Credit = model.TotalAmount;
            cashPosting.VoucherNo = model.VoucherNo;
            if (result != null)
            {
                cashPosting.DetailsId = result.DailySalaryVoucherMasterId;
            }
            else
            {
                cashPosting.DetailsId = id;
            }
            cashPosting.YearId = 1;
            cashPosting.InvoiceNo = model.VoucherNo;
            cashPosting.VoucherTypeId = model.VoucherTypeId;
            cashPosting.LongReference = model.Narration;
            cashPosting.ReferenceN = model.Narration;
            cashPosting.ChequeNo = String.Empty;
            cashPosting.ChequeDate = String.Empty;
            cashPosting.AddedDate = DateTime.UtcNow;
            _context.LedgerPosting.Add(cashPosting);
            await _context.SaveChangesAsync();

            //PostingSalary
            LedgerPosting advancePosting = new LedgerPosting();
            advancePosting.Date = model.Date;
            advancePosting.LedgerId = 6;
            advancePosting.Debit = model.TotalAmount;
            advancePosting.Credit = 0;
            advancePosting.VoucherNo = model.VoucherNo;
            if (result != null)
            {
                advancePosting.DetailsId = result.DailySalaryVoucherMasterId;
            }
            else
            {
                advancePosting.DetailsId = id;
            }
            advancePosting.YearId = 1;
            advancePosting.InvoiceNo = model.VoucherNo;
            advancePosting.VoucherTypeId = model.VoucherTypeId;
            advancePosting.LongReference = model.Narration;
            advancePosting.ReferenceN = model.Narration;
            advancePosting.ChequeNo = String.Empty;
            advancePosting.ChequeDate = String.Empty;
            advancePosting.AddedDate = DateTime.UtcNow;
            _context.LedgerPosting.Add(advancePosting);
            await _context.SaveChangesAsync();
            if (result != null)
            {
                return result.DailySalaryVoucherMasterId;
            }
            else
            {
                return id;
            }
        }

        public Task<bool> Update(DailySalaryVoucherMaster model)
        {
            throw new NotImplementedException();
        }
    }
}
