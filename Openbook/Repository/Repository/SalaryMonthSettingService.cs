using Dapper;
using Microsoft.Data.SqlClient;
using Openbook.Data;
using Openbook.Data.HrPayroll;
using Openbook.Data.HrPayrollModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
    public class SalaryMonthSettingService : ISalaryMonthSetting
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseConnection _conn;
        private string tenantId;
        public SalaryMonthSettingService(ApplicationDbContext context, DatabaseConnection conn, IServicioTenant servicioTenant)
        {
            _context = context;
            _conn = conn;
            tenantId = servicioTenant.ObtenerTenant();
        }
        public async Task<bool> CheckName(string name)
        {
            //var checkResult = (from progm in _context.MonthlySalary
            //                   where progm.PayHeadName == name
            //                   select progm.PayHeadId).Count();
            //if (checkResult > 0)
            //{
            //    return true;
            //}
            //else
            //{
                return false;
            //}
        }

        public async Task<int> CheckNameId(string name)
        {
            var checkResult = (from progm in _context.PayHead
							   where progm.PayHeadName == name
                               select progm.PayHeadId).Count();
         //   if (checkResult > 0)
         //   {

         //       var checkAccount = (from progm in _context.PayHead
									//where progm.PayHeadName == name
         //                           select progm.PayHeadId).FirstOrDefault();
         //       return checkAccount;
         //   }
         //   else
         //   {
                return 0;
            //}
        }

        public async Task<bool> Delete(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var paraScDelete = new DynamicParameters();
                paraScDelete.Add("@MonthlySalaryId", id);
                paraScDelete.Add("@TenantId", tenantId);
                var valueScDelete = sqlcon.Query<int>("DELETE FROM MonthlySalaryDetails where MonthlySalaryId=@MonthlySalaryId AND TenantId=@TenantId DELETE FROM MonthlySalary where MonthlySalaryId=@MonthlySalaryId AND TenantId=@TenantId", paraScDelete, null, true, 0, commandType: CommandType.Text);
            }
            return true;
        }

        public async Task<List<MonthlySalarySettingView>> GetAll(string YearMonth)
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@YearMonth", YearMonth);
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<MonthlySalarySettingView>("MonthlySalarySettingsEmployeeViewAll", para, null, true, 0, commandType: CommandType.StoredProcedure).ToList();
                return ListofPlan;
            }
        }
        public async Task<List<MonthlySalary>> GetAllMonth()
        {
            using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
            {
                var para = new DynamicParameters();
                para.Add("@TenantId", tenantId);
                var ListofPlan = sqlcon.Query<MonthlySalary>("SELECT MonthlySalaryId,YearMonth,Narration FROM MonthlySalary where TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).ToList();
                return ListofPlan;
            }
        }

        public async Task<MonthlySalary> GetbyId(int id)
        {
            MonthlySalary model = await _context.MonthlySalary.FindAsync(id);
            return model;
        }

        public async Task<int> Save(MonthlySalary model)
        {
            var returnSalaryId = (from progm in _context.MonthlySalary
                                       where progm.YearMonth == model.YearMonth
                                  select progm.MonthlySalaryId).FirstOrDefault();
            //DeleteSalaryMonthSetting
            if (returnSalaryId != null)
            {
                using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
                {
                    var paraScDelete = new DynamicParameters();
                    paraScDelete.Add("@MonthlySalaryId", returnSalaryId);
                    paraScDelete.Add("@TenantId", tenantId);
                    var valueScDelete = sqlcon.Query<int>("DELETE FROM MonthlySalaryDetails where MonthlySalaryId=@MonthlySalaryId AND TenantId=@TenantId DELETE FROM MonthlySalary where MonthlySalaryId=@MonthlySalaryId AND TenantId=@TenantId", paraScDelete, null, true, 0, commandType: CommandType.Text);
                }
            }
            await _context.MonthlySalary.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.MonthlySalaryId;
            foreach(var item in model.listOrder)
            {
                MonthlySalaryDetails details = new MonthlySalaryDetails();
                details.MonthlySalaryId = id;
                details.SalaryPackageId = item.SalaryPackageId;
                details.EmployeeId = item.EmployeeId;
                details.TenantId = tenantId;
                _context.MonthlySalaryDetails.Update(details);
                await _context.SaveChangesAsync();
            }
            return id;
        }

        public async Task<bool> Update(MonthlySalary model)
        {
            _context.MonthlySalary.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
