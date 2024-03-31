using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class FinancialYear : IEntidadTenant
	{
		[Key]
		public int FinancialYearId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
		public string TenantId { get; set; } = null!;
		public string FiscalYear { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
