using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.SaasModels
{
    public class CouponsView
    {
        public int CouponId { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Discount { get; set; }
        public decimal Limit { get; set; }
        public string Code { get; set; }
    }
}
