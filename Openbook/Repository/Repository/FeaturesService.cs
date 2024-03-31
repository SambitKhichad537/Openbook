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
    public class FeaturesService : IFeatures
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public FeaturesService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Features
                               where progm.Title == name
                               select progm.FeaturesId).Count();
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
            var checkResult = (from progm in _context.Features
							   where progm.Title == name
                               select progm.FeaturesId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Features
									where progm.Title == name
                                    select progm.FeaturesId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            Features user = await _context.Features.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
        }

        public async Task<List<FeaturesView>> GetAll()
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
                var ListofPlan = sqlcon.Query<FeaturesView>("SELECT *FROM Features", null, null, true, 0, commandType: CommandType.Text).ToList ();
				return ListofPlan;
			}
		}

        public async Task<Features> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@FeaturesId", id);
				var ListofPlan = sqlcon.Query<Features>("SELECT *FROM Features where FeaturesId=@FeaturesId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(Features model)
        {
            await _context.Features.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.FeaturesId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(Features model)
        {
            _context.Features.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
            return true;
        }
    }
}