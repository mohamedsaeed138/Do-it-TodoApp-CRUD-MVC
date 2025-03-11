using System.ComponentModel.DataAnnotations;

namespace TodoApp.ViewModels
{
    public class RegisterViewModel
    {
        public string Name { get; set; }

        [MaxLength(100)]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords doesn't match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
