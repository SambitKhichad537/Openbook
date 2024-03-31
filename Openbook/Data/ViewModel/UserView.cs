namespace Openbook.Data.ViewModel
{
    public class UserView
    {
        public int UserId { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
    }
}
