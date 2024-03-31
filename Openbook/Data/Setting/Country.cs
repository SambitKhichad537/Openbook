using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
	public class Country : IEntidadComn
	{
		[Key]
		public int CountryId { get; set; }
		[Required]
		public string Name { get; set; }
	}
}
