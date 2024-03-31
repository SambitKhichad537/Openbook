using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class ProductGroupView
    {
        [Key]
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public long groupUnder { get; set; }
        public string Under { get; set; }
       public string Image { get; set; }
        public DateTime addedDate { get; set; }
        public DateTime modifyDate { get; set; }
    }
}
