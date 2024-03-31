using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.HrPayroll;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
    public class BonusDeductionService : IBonusDeduction
    {
        private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
        private string tenantId;
        public BonusDeductionService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
        {
            _context = context;
            _conn = conn;
            tenantId = servicioTenant.ObtenerTenant();
        }
        public async Task<bool> CheckName(string name , int employeeid)
        {
            var checkResult = (from progm in _context.BonusDeduction
                               where progm.YearMonth == name && progm.EmployeeId == employeeid
                               select progm.BonusDeductionId).Count();
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
            var checkResult = (from progm in _context.BonusDeduction
							   where progm.YearMonth == name
                               select progm.BonusDeductionId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.BonusDeduction
									where progm.YearMonth == name
                                    select progm.BonusDeductionId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }
		public async Task<bool> Delete(int BonusDeductionId)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand cmd = new SqlCommand("BonusDeductionDelete", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter para = new SqlParameter();
                para = cmd.Parameters.Add("@BonusDeductionId", SqlDbType.Int);
                para.Value = BonusDeductionId;
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

        public async Task<List<BonusDeductionView>> GetAll()
        {
            var result = await (from a in _context.BonusDeduction
                                join b in _context.Employee on a.EmployeeId equals b.EmployeeId
                                select new BonusDeductionView
                                {
                                    BonusDeductionId = a.BonusDeductionId,
                                    Date = a.Date,
                                    Month = a.Month,
                                    BonusAmount = a.BonusAmount,
                                    DeductionAmount = a.DeductionAmount,
                                    YearMonth = a.YearMonth,
                                    EmployeeCode = b.EmployeeCode,
                                    EmployeeName = b.EmployeeName,
                                    Narration = a.Narration
                                }).ToListAsync();
            return result;
        }
        public async Task<BonusDeduction> GetBonusDeductionAmount(string name , int employeeid)
        {
            var result = await(from a in _context.BonusDeduction
                               where a.YearMonth == name && a.EmployeeId == employeeid
                               select new BonusDeduction
							   {
                                   BonusDeductionId = a.BonusDeductionId,
                                   BonusAmount = a.BonusAmount,
                                   DeductionAmount = a.DeductionAmount
							   }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<BonusDeduction> GetbyId(int id)
        {
			BonusDeduction model = await _context.BonusDeduction.FindAsync(id);
            return model;
        }

        public async Task<int> Save(BonusDeduction model)
        {
            await _context.BonusDeduction.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.BonusDeductionId;
			return id;
        }

        public async Task<bool> Update(BonusDeduction model)
        {
            _context.BonusDeduction.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IList<BonusDeductionView>> GetAllSalaryMonth()
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<BonusDeductionView>("SELECT DISTINCT YearMonth From BonusDeduction where TenantId=@TenantId", null, null, true, 0, commandType: CommandType.Text).ToList();
                return ListofPlan;
            }
        }
        public async Task<IList<BonusDeductionView>> BonusDeductionReport(DateTime fromdate, DateTime todate, int employeeid, string month)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@FromDate", fromdate);
                para.Add("@ToDate", todate);
                para.Add("@EmployeeId", employeeid);
                para.Add("@YearMonth", month);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<BonusDeductionView>("BonusDeductionReport", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
                return ListofPlan;
            }
        }
    }
}
