using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openbook.Data.Setting
{
    public class Product : IEntidadTenant
	{
		[Key]
		public int ProductId { get; set; }
        [Required]
        public string ProductCode { get; set; }
        public string Snno { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public int CategoriesId { get; set; }
        public int BrandId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a unit.")]
        public int UnitId { get; set; }
        public int TaxId { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal SalesRate { get; set; }
        public string Narration { get; set; }
        public string ItemType { get; set; }
        public bool IsActive { get; set; }
        public string Barcode { get; set; }
        public string Image { get; set; }
        public int PurchaseAccount { get; set; }
		public int SalesAccount { get; set; }
		public string TenantId { get; set; } = null!;
		public int OpeningStock { get; set; }
        public DateTime ExiparyDate { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        [NotMapped]
        public List<ProductView> Variants { get; set; } = new List<ProductView>();
    }
}
