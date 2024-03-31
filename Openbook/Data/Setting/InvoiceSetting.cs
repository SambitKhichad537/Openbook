using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class InvoiceSetting : IEntidadTenant
	{
		[Key]
		public int VoucherTypeId { get; set; }
        public string VoucherTypeName { get; set; }
        public int StartIndex { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int CompanyId { get; set; }
        public string PONumber { get; set; }
        public string EwayBillNo { get; set; }
        public string VechileNo { get; set; }
        public bool ShowDiscount { get; set; }
        public bool ShowTax { get; set; }
        public bool ShowBarcode { get; set; }
        public bool IsActive { get; set; }
		public string TenantId { get; set; } = null!;
	}
}
