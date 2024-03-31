using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class PaymentMode : IEntidadComn
	{
        public int PaymentmodeId { get; set; }
        public string Name { get; set; }
	}
}
