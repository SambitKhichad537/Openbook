using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openbook.Data.HrPayroll
{
    public class DailySalaryVoucherMaster : IEntidadTenant
    {
        [Key]
        public int DailySalaryVoucherMasterId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a Account.")]
        public int LedgerId { get; set; }
        public string SerialNo { get; set; }
        [Required]
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public DateTime SalaryDate { get; set; }
        public string YearMonthDay { get; set; }
        public decimal TotalAmount { get; set; }
        public string Narration { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int VoucherTypeId { get; set; }
        public int FinancialYearId { get; set; }
        public string TenantId { get; set; } = null!;
        [NotMapped]
        public List<DailySalaryVoucherDetails> listOrder { get; set; } = new List<DailySalaryVoucherDetails>();
    }
}
