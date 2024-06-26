﻿using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Inventory
{
    public class PurchaseDetailsView
	{
		public int PurchaseDetailsId { get; set; }
        public int PurchaseMasterId { get; set; }
        public int OrderDetailsId { get; set; }
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public int TaxId { get; set; }
        public int BatchId { get; set; }
        public int LedgerId { get; set; }
        public int ProjectId { get; set; }
		public string ProductName { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public string UnitName { get; set; }

		public decimal Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string TaxName { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
		public string TenantId { get; set; } = null!;
	}
}
