using Microsoft.Build.Framework;
using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.SaasModels
{
	public class PlanUpgrade : IEntidadTenant
	{
		[Key]
		public int PlanUpgradeId { get; set; }
		public int PlanId { get; set; }
		public string OrderNo { get; set; }
		public bool IsActive { get; set; }
		public string TenantId { get; set; } = null!;
	}
}
