using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.ViewModel
{
	public class TaxView
	{
        public int TaxId { get; set; }
        public string TaxName { get; set; }
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }
        public string TenantId { get; set; } = null!;
    }
}
