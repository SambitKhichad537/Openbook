using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Openbook.Data;
using Openbook.Repository.Interface;
using Openbook.Repository.Repository;
using Openbook.Servicios;
using Radzen;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
});



builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
// Add services to the container.

builder.Services.AddTransient<IServicioTenant, ServicioTenant>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<ICompany, CompanyService>();
builder.Services.AddScoped<IPriviliage, PriviliageService>();
builder.Services.AddScoped<ICategories, CategoriesService>();
builder.Services.AddScoped<ITax, TaxService>();
builder.Services.AddScoped<ICurrency, CurrencyService>();
builder.Services.AddScoped<ITimeZone, TimezoneService>();
builder.Services.AddScoped<IPreference, PreferenceService>();
builder.Services.AddScoped<IEmails, EmailsService>();
builder.Services.AddScoped<IUnits, UnitService>();
builder.Services.AddScoped<IProduct, ProductService>();
builder.Services.AddScoped<IChartofAccount, ChartofAccountService>();
builder.Services.AddScoped<IProject, ProjectService>();
builder.Services.AddScoped<IWarehouse, WarehouseService>();
builder.Services.AddScoped<IJournal, JournalServices>();
builder.Services.AddScoped<ICountry, CountryService>();
builder.Services.AddScoped<ICustomerSupplier, CustomerSupplierService>();
builder.Services.AddScoped<IPurchaseInvoice, PurchaseInvoiceService>();
builder.Services.AddScoped<IPaymentMade, PaymentMadeRepository>();
builder.Services.AddScoped<IPurchaseReturn, PurchaseReturnInvoiceService>();
builder.Services.AddScoped<ISalesInvoice, SalesInvoiceService>();
builder.Services.AddScoped<IPaymentReceipt, PaymentReceiptRepository>();
builder.Services.AddScoped<ISalesReturn, SalesReturnInvoiceService>();
builder.Services.AddScoped<IInventoryReport, InventoryReportService>();
builder.Services.AddScoped<IAccountReports, AccountReportService>();
builder.Services.AddScoped<IPlans, PlanService>();
builder.Services.AddScoped<IPlanUpgrade, PlanUpgradeRepository>();
builder.Services.AddScoped<ICoupons, CouponsService>();
builder.Services.AddScoped<IPaymentType, PaymentTypeService>();
builder.Services.AddTransient<IWebsiteSetting, WebsiteService>();
builder.Services.AddScoped<IFeatures, FeaturesService>();
builder.Services.AddTransient<IPayHead, PayheadService>();
builder.Services.AddTransient<IDesignation, DesignationService>();
builder.Services.AddTransient<IEmployee, EmployeeService>();
builder.Services.AddTransient<ISalaryPackage, SalaryPackageService>();
builder.Services.AddTransient<ISalaryMonthSetting, SalaryMonthSettingService>();
builder.Services.AddTransient<ISalaryVoucher, SalaryVoucherService>();
builder.Services.AddTransient<IAdvancePayment, AdvancePaymentService>();
builder.Services.AddTransient<IBonusDeduction, BonusDeductionService>();
builder.Services.AddTransient<IAttendance, AttendanceService>();
builder.Services.AddTransient<IDailySalaryVoucher, DailySalaryVoucherService>();
builder.Services.AddScoped<DatabaseConnection>();


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
