namespace Openbook.Data.ViewModel
{
    public class CategoriesView
    {
        public int CategoriesId { get; set; }
        public string CategoryName { get; set; }
        public string Image { get; set; }
        public string TenantId { get; set; } = null!;
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
