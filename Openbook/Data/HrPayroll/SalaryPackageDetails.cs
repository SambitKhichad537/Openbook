using Openbook.Entidades;
using System;
using System.ComponentModel.DataAnnotations;
namespace Openbook.Data.HrPayroll
{
	public class SalaryPackageDetails : IEntidadTenant
    {
		[Key]
		public int SalaryPackageDetailsId { get; set; }
		[Required]
		public int SalaryPackageId { get; set; }
		public int PayHeadId { get; set; }
		public decimal Amount { get; set; }
		public string Narration { get; set; }
        public string TenantId { get; set; } = null!;
    }
}
