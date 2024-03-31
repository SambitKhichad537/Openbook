using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class AccountGroupView
    {
        public int AccountGroupId { get; set; }
        [Required]
        public string AccountGroupName { get; set; }
        public string Under { get; set; }
        public string Nature { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
