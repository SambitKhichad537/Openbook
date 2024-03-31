using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.ViewModel
{
    public class UnitView
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int NoOfDecimalplaces { get; set; }
    }
}
