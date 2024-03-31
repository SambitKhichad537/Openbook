using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.SaasModels
{
    public class Coupons : IEntidadComn
	{
        [Key]
        public int CouponId { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Discount { get; set; }
        public decimal Limit { get; set; }
        public string Code { get; set; }
    }
}
