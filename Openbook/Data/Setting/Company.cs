using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class Company : IEntidadTenant
    {
        public int CompanyId { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string Address { get; set; }
        public string City { get; set; }
        public string CountryName { get; set; }
        public string TaxId { get; set; }
        public int TimeZoneId { get; set; }
        public string DateFormat { get; set; }
        public string PhoneNo { get; set; }
        [Required]
        public string Email { get; set; }
        public int CurrencyId { get; set; }
		public int FinancialYearId { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
        public DateTime? StartDate { get; set; }
		public string TenantId { get; set; } = null!;
		public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
