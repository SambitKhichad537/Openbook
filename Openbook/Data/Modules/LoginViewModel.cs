using System.ComponentModel.DataAnnotations;

namespace Openbook.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "required email")]
        [EmailAddress(ErrorMessage = "please type valid email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "required password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember")]
        public bool Recuerdame { get; set; }

    }
}
