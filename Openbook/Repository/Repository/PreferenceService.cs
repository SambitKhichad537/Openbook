using Microsoft.EntityFrameworkCore;
using Openbook.Data;
using Openbook.Data.Setting;
using Openbook.Data.ViewModel;
using Openbook.Repository.Interface;
using Openbook.Servicios;

namespace Openbook.Repository.Repository
{
    public class PreferenceService : IPreference
	{
		private readonly ApplicationDbContext _context;
		private string tenantId;
		public PreferenceService(ApplicationDbContext context , IServicioTenant servicioTenant)
		{
			_context = context;
			tenantId = servicioTenant.ObtenerTenant();
		}
        public async Task<GeneralSetting> GetAll()
        {
            var result = await (from a in _context.GeneralSetting
								select new GeneralSetting
                                {
                                    GeneralId = a.GeneralId,
									ShowCurrency = a.ShowCurrency,
									NegativeCash = a.NegativeCash,
									NegativeStock = a.NegativeStock,
									StockCalculationMode = a.StockCalculationMode,
									CreditLimit = a.CreditLimit,
									Discount = a.Discount,
                                    VatOnPurchase = a.VatOnPurchase,
									VatOnSales = a.VatOnSales,
                                    TenantId = a.TenantId
								}).FirstOrDefaultAsync();
            return result;
        }

        public async Task<int> Save(GeneralSetting model)
        {
            await _context.GeneralSetting.AddAsync(model);
            await _context.SaveChangesAsync();
            int id = model.GeneralId;
            _context.Entry(model).State = EntityState.Detached;
            return id;
        }

        public async Task<bool> Update(GeneralSetting model)
        {
            _context.GeneralSetting.Update(model);
            await _context.SaveChangesAsync();
            _context.Entry(model).State = EntityState.Detached;
            return true;
        }
    }
}