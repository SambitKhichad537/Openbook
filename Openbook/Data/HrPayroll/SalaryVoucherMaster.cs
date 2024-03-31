using Openbook.Data.Inventory;
using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openbook.Data.HrPayroll
{
    public class SalaryVoucherMaster : IEntidadTenant
    {
        [Key]
        public int SalaryVoucherMasterId { get; set; }
        public int LedgerId { get; set; }
        public string SerialNo { get; set; }
		public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public string YearMonth { get; set; }
        public DateTime Month { get; set; }
        public decimal TotalAmount { get; set; }
        public string Narration { get; set; }
        public int VoucherTypeId { get; set; }
        public int financialYearId { get; set; }
        public string TenantId { get; set; } = null!;
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        [NotMapped]
        public List<SalaryVoucherDetails> listOrder { get; set; } = new List<SalaryVoucherDetails>();
    }
}
