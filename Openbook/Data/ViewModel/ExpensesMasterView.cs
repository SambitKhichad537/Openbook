namespace Openbook.Data.Setting
{
    public class ExpensesMasterView
    {
        public int ExpensiveMasterId { get; set; }
        public int LedgerId { get; set; }
        public string VoucherNo { get; set; }
        public DateTime? Date { get; set; }
        public string NepaliDate { get; set; }
        public string VoucherTypeName { get; set; }
        public string LedgerName { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }
        public string WarehouseName { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public int WarehouseId { get; set; }
        public int VoucherTypeId { get; set; }
        public string SerialNo { get; set; }
        public int FinancialYearId { get; set; }
		public DateTime AddedDate { get; set; }
        public DateTime ModifyDate { get; set; }
        //LedgerInfo
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
