using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.SaasModels
{
    public class Features : IEntidadComn
	{

        [Key]
        public int FeaturesId { get; set; }
    [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
