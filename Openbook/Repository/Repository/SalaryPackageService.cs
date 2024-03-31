using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.HrPayroll;
using Openbook.Repository.Interface;
using Openbook.Servicios;

namespace Openbook.Repository.Repository
{
    public class SalaryPackageService : ISalaryPackage
    {
        private readonly ApplicationDbContext _context;
        private string tenantId;
        public SalaryPackageService(ApplicationDbContext context , IServicioTenant servicioTenant)
        {
            _context = context;
            tenantId = servicioTenant.ObtenerTenant();
        }
        public async Task<bool> CheckName(string name)
        {
            var checkResult = (from progm in _context.SalaryPackage
                               where progm.SalaryPackageName == name
                               select progm.SalaryPackageId).Count();
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
            var checkResult = (from progm in _context.SalaryPackage
                               where progm.SalaryPackageName == name
                               select progm.SalaryPackageId).Count();
            if (checkResult > 0)
            {

                var checkAccount = (from progm in _context.SalaryPackage
                                    where progm.SalaryPackageName == name
                                    select progm.SalaryPackageId).FirstOrDefault();
                return checkAccount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var checkResult = await(from progm in _context.Employee
                                    where progm.DesignationId == id
                                    select progm.DesignationId).CountAsync();
            if (checkResult > 0)
            {
                return false;
            }
            else
            {
                SalaryPackage user = await _context.SalaryPackage.FindAsync(id);
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<SalaryPackage>> GetAll()
        {
            var result = await(from a in _context.SalaryPackage
                               select new SalaryPackage
                               {
                                   SalaryPackageId = a.SalaryPackageId,
                                   SalaryPackageName = a.SalaryPackageName,
                                   IsActive = a.IsActive,
                                   Narration = a.Narration
							   }).ToListAsync();
            return result;
        }
        public async Task<List<SalaryPackageDetailsView>> GetAllView(int id)
        {
            var result = await (from a in _context.SalaryPackageDetails
                                join b in _context.PayHead on a.PayHeadId equals b.PayHeadId
                                where a.SalaryPackageId == id
                                select new SalaryPackageDetailsView
                                {
                                    SalaryPackageDetailsId = a.SalaryPackageDetailsId,
                                    SalaryPackageId = a.SalaryPackageId,
                                    PayHeadId = a.PayHeadId,
                                    Amount = a.Amount,
                                    Narration = a.Narration,
                                    PayHeadName = b.PayHeadName,
                                    Type = b.Type
                                }).ToListAsync();
            return result;
        }

        public async Task<SalaryPackage> GetbyId(int id)
        {
            SalaryPackage model = await _context.SalaryPackage.FindAsync(id);
            return model;
        }

        public async Task<int> Save(SalaryPackage model)
        {
            await _context.SalaryPackage.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.SalaryPackageId;

           foreach(var item in model.listPackage)
            {
                SalaryPackageDetails details = new SalaryPackageDetails();
                details.SalaryPackageId = id;
                details.PayHeadId = item.PayHeadId;
                details.Amount = item.Amount;
                details.Narration = item.Narration;
                details.TenantId = tenantId;
                await _context.SalaryPackageDetails.AddAsync(details);
                await _context.SaveChangesAsync();
            }
            return id;
        }

        public async Task<bool> Update(SalaryPackage model)
        {
            _context.SalaryPackage.Update(model);
            await _context.SaveChangesAsync();
            foreach (var item in model.listPackage)
            {
                SalaryPackageDetails details = new SalaryPackageDetails();
                if (item.SalaryPackageDetailsId == 0)
                {
                    details.SalaryPackageId = model.SalaryPackageId;
                    details.PayHeadId = item.PayHeadId;
                    details.Amount = item.Amount;
                    details.Narration = item.Narration;
                    details.TenantId = tenantId;
                    await _context.SalaryPackageDetails.AddAsync(details);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    details.SalaryPackageDetailsId = item.SalaryPackageDetailsId;
                    details.SalaryPackageId = model.SalaryPackageId;
                    details.PayHeadId = item.PayHeadId;
                    details.Amount = item.Amount;
                    details.Narration = item.Narration;
                    details.TenantId = tenantId;
                    _context.SalaryPackageDetails.Update(details);
                    await _context.SaveChangesAsync();
                }
            }
            foreach (var deleteitem in model.listDelete)
            {
                SalaryPackageDetails x = _context.SalaryPackageDetails.Find(deleteitem.SalaryPackageDetailsId);
                _context.SalaryPackageDetails.Remove(x);
                await _context.SaveChangesAsync();
            }
            return true;
        }
    }
}
