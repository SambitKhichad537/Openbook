using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.SaasModels
{
    public class PaymentTypeView
    {
        public int PaymentId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
