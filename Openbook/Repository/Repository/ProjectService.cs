using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.Inventory;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;
using System.Data;

namespace Openbook.Repository.Repository
{
    public class ProjectService : IProject
	{
		private readonly ApplicationDbContext _context;
		private readonly DatabaseConnection _conn;
		private string tenantId;
		public ProjectService(ApplicationDbContext context , DatabaseConnection conn, IServicioTenant servicioTenant)
		{
			_context = context;
			_conn = conn;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.Project
                               where progm.ProjectName == name
                               select progm.ProjectId).Count();
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
            var checkResult = (from progm in _context.Project
                               where progm.ProjectName == name
                               select progm.ProjectId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.Project
                                    where progm.ProjectName == name
                                    select progm.ProjectId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var checkResult = await (from progm in _context.JournalDetails
                                     where progm.ProjectId == id
                                     select progm.ProjectId).CountAsync();
            if (checkResult > 0)
            {
                return false;
            }
            else
            {
                Project user = await _context.Project.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<ProjectView>> GetAll()
        {
            var result = await (from a in _context.Project
                                select new ProjectView
                                {
                                    ProjectId = a.ProjectId,
                                    ProjectName = a.ProjectName
                                }).ToListAsync();
            return result;
        }

        public async Task<Project> GetbyId(int id)
        {
			using (SqlConnection sqlcon = new SqlConnection(_conn.DbConn))
			{
				var para = new DynamicParameters();
				para.Add("@ProjectId", id);
				para.Add("@TenantId", tenantId);
				var ListofPlan = sqlcon.Query<Project>("SELECT *FROM Project where ProjectId=@ProjectId AND TenantId=@TenantId", para, null, true, 0, commandType: CommandType.Text).FirstOrDefault();
				return ListofPlan;
			}
        }

        public async Task<int> Save(Project model)
        {
            await _context.Project.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.ProjectId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(Project model)
        {
            _context.Project.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
            return true;
        }
    }
}