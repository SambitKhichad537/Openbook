namespace Openbook.Data.ViewModel
{
    public class UserListDataModel
    {

        public string? Name { get; set; }
        public string? RoleName { get; set; }
        public string? Email { get; set; }
        public string? PlanName { get; set; }

        public string? TenantId {  get; set; }

        public bool? IsActive {  get; set; }
    }
}
