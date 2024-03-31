using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
	public class CustomerSupplier : IEntidadTenant
	{
		[Key]
		public int CustomerSupplierId { get; set; }
		[Required]
		public string Name { get; set; }
		public string WorkPhone { get; set; }
		public string Mobile { get; set; }
		[Required]
		public string Email { get; set; }
		public int CountryId { get; set; }
		public string City { get; set; }
		[Required]
		public string Address { get; set; }
		public decimal CreditLimit { get; set; }
		public string CompanyName { get; set; }
		public string Website { get; set; }
		public bool SameasShipping { get; set; }
		public string Type { get; set; }
		public decimal OpeningBalance { get; set; }
		public string DrCr { get; set; }
		public string TenantId { get; set; } = null!;
		public DateTime? Date { get; set; }
		public DateTime? AddedDate { get; set; }
		public DateTime? ModifyDate { get; set; }
	}
}
