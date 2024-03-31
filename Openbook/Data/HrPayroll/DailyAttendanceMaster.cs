using Openbook.Entidades;
using Openbook.Data.Inventory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openbook.Data.HrPayroll
{
    public class DailyAttendanceMaster : IEntidadTenant
    {
        [Key]
        public int DailyAttendanceMasterId { get; set; }
        public DateTime Date { get; set; }
        public string Narration { get; set; }
        public string TenantId { get; set; } = null!;
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
		[NotMapped]
		public List<DailyAttendanceDetails> listOrder { get; set; } = new List<DailyAttendanceDetails>();
	}
}
