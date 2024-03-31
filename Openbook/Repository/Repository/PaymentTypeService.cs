using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.SaasModels;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
	public class PaymentTypeService : IPaymentType
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public PaymentTypeService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.PaymentType
                               where progm.Name == name
                               select progm.PaymentId).Count();
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
            var checkResult = (from progm in _context.PaymentType
							   where progm.Name == name
                               select progm.PaymentId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.PaymentType
									where progm.Name == name
                                    select progm.PaymentId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            PaymentType user = await _context.PaymentType.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
        }

        public async Task<List<PaymentTypeView>> GetAll()
        {
            var result = await (from a in _context.PaymentType
								select new PaymentTypeView
								{
                                    PaymentId = a.PaymentId,
                                    Name = a.Name,
                                    IsActive = a.IsActive
                                }).ToListAsync();
            return result;
        }

        public async Task<PaymentType> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@PaymentId", id);
				var ListofPlan = sqlcon.Query<PaymentType>("SELECT *FROM PaymentType where PaymentId=@PaymentId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(PaymentType model)
        {
            await _context.PaymentType.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.PaymentId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(PaymentType model)
        {
            _context.PaymentType.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
            return true;
        }
    }
}