using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.SaasModels
{
	public class PlanMasterView
	{
		public int PlanId { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string Duration { get; set; }
		public int MaximumUser { get; set; }
		[Required]
		public int MaximumCustomer { get; set; }
		[Required]
		public int MaximumSupplier { get; set; }
		[Required]
		public int MaximumProduct { get; set; }
		[Required]
		public int MaximumInvoice { get; set; }
		public string Description { get; set; }
		public bool Crm { get; set; }
		public bool Project { get; set; }
		public bool Hrm { get; set; }
		public bool Account { get; set; }
		public bool Pos { get; set; }
		public string OrderNo { get; set; }
		public bool IsActive { get; set; }
		public int TotalUser { get; set; }
		public int PaidUser { get; set; }
		public decimal TotalorderAmount { get; set; }
		public int TotalPlan { get; set; }
		public DateTime? AddedDate { get; set; }
		public DateTime? ModifyDate { get; set; }
	}
}
