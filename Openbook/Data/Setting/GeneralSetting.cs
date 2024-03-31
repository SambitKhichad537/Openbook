using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
	public class GeneralSetting : IEntidadTenant
	{
		[Key]
		public int GeneralId { get; set; }
		public bool ShowCurrency { get; set; }
        public bool NegativeCash { get; set; }
        public bool NegativeStock { get; set; }
        public bool StockCalculationMode { get; set; }
        public bool CreditLimit { get; set; }
        public bool Discount { get; set; }
        public bool VatOnPurchase { get; set; }
        public bool VatOnSales { get; set; }
		public string TenantId { get; set; } = null!;

	}
}
