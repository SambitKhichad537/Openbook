using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
    public class Currency : IEntidadComn
	{
		[Key]
		public int CurrencyId { get; set; }
        [Required]
        public string CurrencySymbol { get; set; }
		[Required]
		public string CurrencyName { get; set; }
    }
}
