using System.ComponentModel.DataAnnotations;

namespace Openbook.Data
{
    public class DeleteItem
    {
        [Key]
        public int id { get; set; }
        public int PurchaseDetailsId { get; set; }
        public int DeleteItemIdnext { get; set; }
        public int DeleteExpenditureId { get; set; }
        public int DeleteIncomeDetailsId { get; set; }
        public int DeleteJournalDetailsId { get; set; }
        public int SalesDetailsId { get; set; }
        public int DeletePurchaseReturnId { get; set; }
        public int DeleteSalesReturnId { get; set; }
        public int DeleteQuotationId { get; set; }
        public int PaymentDetailsId { get; set; }
        public int ReceiptDetailsId { get; set; }
		public int SalaryPackageDetailsId { get; set; }
        public int BudgetDetailsId { get; set; }
	}
}
