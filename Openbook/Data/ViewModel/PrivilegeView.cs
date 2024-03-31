namespace Openbook.Data.ViewModel
{
	public class PrivilegeView
	{
        public int PrivilegeId { get; set; }
        public string FormName { get; set; }
        public bool AddAction { get; set; }
        public bool EditAction { get; set; }
        public bool DeleteAction { get; set; }
        public bool ShowAction { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int CompanyId { get; set; }
        public string SettingType { get; set; }
        public bool IsActive { get; set; }
    }
}
