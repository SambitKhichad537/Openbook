using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.SaasModels
{
    public class FeaturesView
    {

        public int FeaturesId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
