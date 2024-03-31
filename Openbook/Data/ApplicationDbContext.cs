using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Openbook.Data.AccountModel;
using Openbook.Data.HrPayroll;
using Openbook.Data.Inventory;
using Openbook.Data.Modules;
using Openbook.Data.SaasModels;
using Openbook.Data.Setting;
using Openbook.Entidades;
using Openbook.Servicios;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Openbook.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
		private string tenantId;

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
			IServicioTenant servicioTenant)
			: base(options)
		{
			tenantId = servicioTenant.ObtenerTenant();
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			foreach (var item in ChangeTracker.Entries().Where(e => e.State == EntityState.Added
			&& e.Entity is IEntidadTenant))
			{
				if (string.IsNullOrEmpty(tenantId))
				{
					throw new Exception("TenantId no encontrado al momento de crear el registro.");
				}

				var entidad = item.Entity as IEntidadTenant;
				entidad!.TenantId = tenantId;
			}

			return base.SaveChangesAsync(cancellationToken);
		}


		protected override void OnModelCreating(ModelBuilder builder)
		{
            base.OnModelCreating(builder);
			//builder.Entity<Currency>().HasData(new Currency[]
			//	{
			//		new Currency{CurrencyId = 1, CurrencySymbol = "USD" , CurrencyName = "USA"},
			//		new Currency{CurrencyId = 2, CurrencySymbol = "Nrs" , CurrencyName = "Nepal"},
   //                 new Currency{CurrencyId = 3, CurrencySymbol = "Rs" , CurrencyName = "India"},
   //                 new Currency{CurrencyId = 4, CurrencySymbol = "BDT" , CurrencyName = "Bangladesh"}
			//	});
			//builder.Entity<TimeZones>().HasData(new TimeZones[]
			//	{
			//		new TimeZones{TimeZoneId = 1, Name = "Asia/Calcutta"},
			//		new TimeZones{TimeZoneId = 2, Name = "Europe/Andorra"},
			//		new TimeZones{TimeZoneId = 3, Name = "Asia/Dubai"},
			//		new TimeZones{TimeZoneId = 4, Name = "Asia/Kabul"}
			//	});
			//builder.Entity<PaymentMode>().HasData(new PaymentMode[]
   //             {
   //                 new PaymentMode{PaymentmodeId = 1, Name = "Cash"},
   //                 new PaymentMode{PaymentmodeId = 2, Name = "Bank"},
   //                 new PaymentMode{PaymentmodeId = 3, Name = "Credit"},
   //                 new PaymentMode{PaymentmodeId = 4, Name = "Online Payment"}
   //             });

            foreach (var entidad in builder.Model.GetEntityTypes())
			{
				var tipo = entidad.ClrType;

				if (typeof(IEntidadTenant).IsAssignableFrom(tipo))
				{
					var método = typeof(ApplicationDbContext)
						.GetMethod(nameof(ArmarFiltroGlobalTenant),
						BindingFlags.NonPublic | BindingFlags.Static
						   )?.MakeGenericMethod(tipo);

					var filtro = método?.Invoke(null, new object[] { this })!;
					entidad.SetQueryFilter((LambdaExpression)filtro);
					entidad.AddIndex(entidad.FindProperty(nameof(IEntidadTenant.TenantId))!);
				}
				else if (tipo.DebeSaltarValidaciónTenant())
				{
					continue;
				}
				else
				{
					throw new Exception($"La entidad {entidad} no ha sido marcada como tenant o común");
				}
			}
		}

		private static LambdaExpression ArmarFiltroGlobalTenant<TEntidad>(
			ApplicationDbContext context)
			where TEntidad : class, IEntidadTenant
		{
			Expression<Func<TEntidad, bool>> filtro = x => x.TenantId == context.tenantId;
			return filtro;
		}
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<UserCompany> UserCompany { get; set; }
        public DbSet<Priviliage> Priviliage { get; set; }
        public DbSet<Categories> Categories { get; set; }
		public DbSet<AccountLedger> AccountLedger { get; set; }
		public DbSet<Currency> Currency { get; set; }
		public DbSet<ExpenseMaster> ExpenseMaster { get; set; }
		public DbSet<ExpensesDetails> ExpensesDetails { get; set; }
		public DbSet<FinancialYear> FinancialYear { get; set; }
		public DbSet<IncomeMaster> IncomeMaster { get; set; }
		public DbSet<LedgerPosting> LedgerPosting { get; set; }
		public DbSet<PaymentMaster> PaymentMaster { get; set; }
		public DbSet<PaymentDetails> PaymentDetails { get; set; }
		public DbSet<Product> Product { get; set; }
		public DbSet<PurchaseDetails> PurchaseDetails { get; set; }
		public DbSet<PurchaseMaster> PurchaseMaster { get; set; }
		public DbSet<PurchaseReturnDetails> PurchaseReturnDetails { get; set; }
		public DbSet<PurchaseReturnMaster> PurchaseReturnMaster { get; set; }
		public DbSet<ReceiptMaster> ReceiptMaster { get; set; }
		public DbSet<ReceiptDetails> ReceiptDetails { get; set; }
		public DbSet<SalesDetails> SalesDetails { get; set; }
		public DbSet<SalesMaster> SalesMaster { get; set; }
		public DbSet<SalesQuotationMaster> SalesQuotationMaster { get; set; }
		public DbSet<SalesQuotationDetails> SalesQuotationDetails { get; set; }
		public DbSet<SalesReturnDetails> SalesReturnDetails { get; set; }
		public DbSet<SalesReturnMaster> SalesReturnMaster { get; set; }
		public DbSet<Tax> Tax { get; set; }
		public DbSet<StockPosting> StockPosting { get; set; }
		public DbSet<Unit> Unit { get; set; }
		public DbSet<InvoiceSetting> InvoiceSetting { get; set; }
		public DbSet<Company> Company => Set<Company>();
		public DbSet<Brand> Brand { get; set; }
		public DbSet<EmailSetting> EmailSetting { get; set; }
		public DbSet<GeneralSetting> GeneralSetting { get; set; }
		public DbSet<JournalMaster> JournalMaster { get; set; }
		public DbSet<JournalDetails> JournalDetails { get; set; }
		public DbSet<TimeZones> TimeZones { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
		public DbSet<Country> Country { get; set; }
		public DbSet<CustomerSupplier> CustomerSupplier { get; set; }
		public DbSet<PlanMaster> PlanMaster { get; set; }
        public DbSet<PlanUpgrade> PlanUpgrade { get; set; }
        public DbSet<Coupons> Coupons { get; set; }
		public DbSet<Features> Features { get; set; }
		public DbSet<PaymentType> PaymentType { get; set; }
		public DbSet<WebsiteSetting> WebsiteSetting { get; set; }
		//Payroll
		public DbSet<AdvancePayment> AdvancePayment { get; set; }
        public DbSet<BonusDeduction> BonusDeduction { get; set; }
        public DbSet<DailyAttendanceMaster> DailyAttendanceMaster { get; set; }
        public DbSet<DailyAttendanceDetails> DailyAttendanceDetails { get; set; }
        public DbSet<DailySalaryVoucherMaster> DailySalaryVoucherMaster { get; set; }
        public DbSet<DailySalaryVoucherDetails> DailySalaryVoucherDetails { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<PayHead> PayHead { get; set; }
        public DbSet<SalaryPackage> SalaryPackage { get; set; }
        public DbSet<SalaryPackageDetails> SalaryPackageDetails { get; set; }
        public DbSet<MonthlySalary> MonthlySalary { get; set; }
        public DbSet<MonthlySalaryDetails> MonthlySalaryDetails { get; set; }
        public DbSet<SalaryVoucherDetails> SalaryVoucherDetails { get; set; }
        public DbSet<SalaryVoucherMaster> SalaryVoucherMaster { get; set; }
    }
}