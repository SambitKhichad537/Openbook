using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.ViewModel
{
    public class CurrencyView
    {
        public int CurrencyId { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyName { get; set; }
	}
}
