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
    public class EmployeeService : IEmployee
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseConnection _conn;
        private string tenantId;
        public EmployeeService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
        {
            _context = context;
            _conn = conn;
            tenantId = servicioTenant.ObtenerTenant();
        }
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Employee
                               where progm.EmployeeName == name
                               select progm.EmployeeId).Count();
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
            var checkResult = (from progm in _context.Employee
							   where progm.EmployeeName == name
                               select progm.EmployeeId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Employee
									where progm.EmployeeName == name
                                    select progm.EmployeeId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int EmployeeId)
        {
            SqlConnection sqlcon = new SqlConnection(_conn.DbConn);
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand cmd = new SqlCommand("EmployeeDelete", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter para = new SqlParameter();
                para = cmd.Parameters.Add("@EmployeeId", SqlDbType.Int);
                para.Value = EmployeeId;
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

        public async Task<List<EmployeeView>> GetAll()
        {
            var result = await(from a in _context.Employee
							   join b in _context.Designation on a.DesignationId equals b.DesignationId
							   select new EmployeeView
							   {
                                   EmployeeId = a.EmployeeId,
                                   EmployeeName = a.EmployeeName,
                                   EmployeeCode = a.EmployeeCode,
                                   Dob = a.Dob,
                                   Gender = a.Gender,
                                   Address = a.Address,
                                   PhoneNumber = a.PhoneNumber,
                                   Email = a.Email,
                                   DesignationName = b.DesignationName
							   }).ToListAsync();
            return result;
        }

        public async Task<Employee> GetbyId(int id)
        {
			Employee model = await _context.Employee.FindAsync(id);
            return model;
        }

        public async Task<int> Save(Employee model)
        {
            await _context.Employee.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.EmployeeId;
            return id;
        }

        public async Task<bool> Update(Employee model)
        {
            _context.Employee.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
