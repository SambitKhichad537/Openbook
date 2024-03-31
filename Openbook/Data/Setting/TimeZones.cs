using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
	public class TimeZones : IEntidadComn
	{
		[Key]
		public int TimeZoneId { get; set; }
		[Required]
		public string Name { get; set; }
	}
}
