namespace Openbook.Data.ViewModel
{
	public class CustomerSupplierView
	{
		public int CustomerSupplierId { get; set; }
		public string Name { get; set; }
		public string WorkPhone { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public int CountryId { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
		public decimal CreditLimit { get; set; }
		public string CompanyName { get; set; }
		public string Website { get; set; }
		public string SameasShipping { get; set; }
		public string Type { get; set; }
		public decimal OpeningBalance { get; set; }
		public string DrCr { get; set; }
		public string TenantId { get; set; } = null!;
		public DateTime? Date { get; set; }
		public DateTime? AddedDate { get; set; }
		public DateTime? ModifyDate { get; set; }
	}
}
