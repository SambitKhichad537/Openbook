using Microsoft.Build.Framework;
using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.SaasModels
{
	public class WebsiteSetting : IEntidadComn
	{
		[Key]
		public int WebId { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Footer { get; set; }
		public string Email { get; set; }
		public string CopyRight { get; set; }
		public int NoofDecimal { get; set; }
		public string Facebook { get; set; }
		public string Twitter { get; set; }
		public string Youtube { get; set; }
		public string Linkedin { get; set; }
		public string Instagram { get; set; }
		public string Logo { get; set; }
	}
}
