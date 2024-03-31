namespace Openbook.Data.Setting
{
    public class FinancialReport
    {
        public int ID { get; set; }
        public int LedgerId { get; set; }
        public int SubledgerId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Balance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string VoucherTypeName { get; set; }
        public string VoucherNo { get; set; }
        public DateTime Date { get; set; }
        public string LedgerCode { get; set; }
        public string LedgerName { get; set; }
        public decimal Qty { get; set; }
        public string UnitName { get; set; }
        public string ProductName { get; set; }

        public string UserName { get; set; }
        public string Narration { get; set; }

    }
}
