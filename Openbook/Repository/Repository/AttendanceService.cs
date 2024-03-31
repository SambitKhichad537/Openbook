using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.HrPayroll;
using Openbook.Data.HrPayrollModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
    public class AttendanceService : IAttendance
    {
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
        private string tenantId;
        public AttendanceService(ApplicationDbContext context, DatabaseConnection conn, IServicioTenant servicioTenant)
        {
			_context = context;
			_conn = conn;
            tenantId = servicioTenant.ObtenerTenant();
        }

		public decimal HolliDayChecking(DateTime date)
		{
			decimal decResult = 0;
			SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlCommand sccmd = new SqlCommand("HolliDayChecking", sqlcon);
				sccmd.CommandType = CommandType.StoredProcedure;
				SqlParameter sprmparam = new SqlParameter();
				sprmparam = sccmd.Parameters.Add("@date", SqlDbType.DateTime);
				sprmparam.Value = date;
            sprmparam = sccmd.Parameters.Add("@TenantId", SqlDbType.NVarChar);
            sprmparam.Value = tenantId;
            decResult = Convert.ToDecimal(sccmd.ExecuteScalar());
				sqlcon.Close();
			return decResult;
		}
        public decimal HolidaySettings(DateTime date)
        {
            decimal decResult = 0;
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            sqlcon.Open();
            SqlCommand sccmd = new SqlCommand("HolliDayChecking", sqlcon);
                sccmd.CommandType = CommandType.StoredProcedure;
                SqlParameter sprmparam = new SqlParameter();
                sprmparam = sccmd.Parameters.Add("@date", SqlDbType.DateTime);
                sprmparam.Value = date;
                decResult = Convert.ToDecimal(sccmd.ExecuteScalar());
                sqlcon.Close();
            return decResult;
        }
        public async Task<bool> Delete(int DailyAttendanceMasterId)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand cmd = new SqlCommand("AttendanceDelete", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter para = new SqlParameter();
                para = cmd.Parameters.Add("@DailyAttendanceMasterId", SqlDbType.Int);
                para.Value = DailyAttendanceMasterId;
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
        public async Task<DailyAttendanceDetails> GetAttandanceDetails(string date, int employeeid)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@Narration", date);
                para.Add("@Employeeid", employeeid);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<DailyAttendanceDetails>("GetAttandanceDetails", para, null, true, 0, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return ListofPlan;
            }
        }
        public IList<DailyAttendanceView> DailyAttendanceDetailsSearchGridFill()
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
                var para = new DynamicParameters();
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<DailyAttendanceView>("SELECT  EmployeeId,EmployeeCode,EmployeeName FROM Employee Where IsActive=1 AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).ToList();
				return ListofPlan;
			}
		}
		public bool DailyAttendanceMasterMasterIdSearch(DateTime strDate)
		{

			decimal deccountMasterId = 0;
				SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
			sqlcon.Open();
			SqlCommand sqlcmd = new SqlCommand("Select DailyAttendanceMasterId  from DailyAttendanceMaster  where Date= @date AND TenantId=@TenantId", sqlcon);
				sqlcmd.CommandType = CommandType.Text;
				sqlcmd.Parameters.Add("@date", SqlDbType.NVarChar).Value = strDate;
            sqlcmd.Parameters.Add("@TenantId", SqlDbType.NVarChar).Value = tenantId;
            Object obj = sqlcmd.ExecuteScalar();
				if (obj != null)
				{
					deccountMasterId = decimal.Parse(obj.ToString());
				}
				if (deccountMasterId > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
				sqlcon.Close();
		}
		public async Task<int> Save(DailyAttendanceMaster model)
        {
            var result = await (from a in _context.DailyAttendanceMaster
                                where a.Narration == model.Narration
                                select new DailyAttendanceMaster
                                {
                                    DailyAttendanceMasterId = a.DailyAttendanceMasterId
                                }).FirstOrDefaultAsync();
            int id = 0;
            if (result == null)
            {
                await _context.DailyAttendanceMaster.AddAsync(model);
                await _context.SaveChangesAsync();
                id = model.DailyAttendanceMasterId;
            }
            else
            {
                model.DailyAttendanceMasterId = result.DailyAttendanceMasterId;
                _context.DailyAttendanceMaster.Update(model);
                await _context.SaveChangesAsync();
            }
            //DeleteAttendanceDetails
            if (result != null)
            {
                using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
                {
                    var paraScDelete = new DynamicParameters();
                    paraScDelete.Add("@DailyAttendanceMasterId", result.DailyAttendanceMasterId);
                    paraScDelete.Add("@TenantId", tenantId);
                    var valueScDelete = sqlcon.Query<int>("DELETE FROM DailyAttendanceDetails where DailyAttendanceMasterId=@DailyAttendanceMasterId AND TenantId=@TenantId", paraScDelete, null, true, 0, commandType: CommandType.Text);
                }
            }
            foreach (var item in model.listOrder)
            {
                DailyAttendanceDetails details = new DailyAttendanceDetails();
                    if (result != null)
                    {
                        details.DailyAttendanceMasterId = result.DailyAttendanceMasterId;
                    }
                    else
                    {
                        details.DailyAttendanceMasterId = id;
                    }
                    details.EmployeeId = item.EmployeeId;
                    details.Status = item.Status;
                    details.Narration = item.Narration;
                details.TenantId = tenantId;
                await _context.DailyAttendanceDetails.AddAsync(details);
                    await _context.SaveChangesAsync();
            }
            if (result != null)
            {
                return result.DailyAttendanceMasterId;
            }
            else
            {
                return id;
            }
        }

        public async Task<bool> Update(DailyAttendanceMaster model)
        {
            _context.DailyAttendanceMaster.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IList<DailyAttendanceView>> GetAll()
        {
            var result = await (from a in _context.DailyAttendanceMaster
                                select new DailyAttendanceView
                                {
                                    DailyAttendanceMasterId = a.DailyAttendanceMasterId,
                                    Date = a.Date
                                }).ToListAsync();
            return result;
        }
    }
}
