using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.SaasModels;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;
using System.Drawing;

namespace Openbook.Repository.Repository
{
    public class CouponsService : ICoupons
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public CouponsService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Coupons
                               where progm.Name == name
                               select progm.CouponId).Count();
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
            var checkResult = (from progm in _context.Coupons
                               where progm.Name == name
                               select progm.CouponId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Coupons
                                    where progm.Name == name
                                    select progm.CouponId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            Coupons user = await _context.Coupons.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
        }

        public async Task<List<CouponsView>> GetAll()
        {
            var result = await (from a in _context.Coupons
                                select new CouponsView
                                {
                                    CouponId = a.CouponId,
                                    Name = a.Name,
                                    Discount = a.Discount,
                                    Limit = a.Limit,
                                    Code = a.Code
                                }).ToListAsync();
            return result;
        }

        public async Task<Coupons> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@CouponId", id);
				var ListofPlan = sqlcon.Query<Coupons>("SELECT *FROM Coupons where CouponId=@CouponId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(Coupons model)
        {
            await _context.Coupons.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.CouponId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(Coupons model)
        {
            _context.Coupons.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
            return true;
        }
    }
}