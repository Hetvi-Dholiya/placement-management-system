using System.ComponentModel.DataAnnotations;

namespace PlacementManagementSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email or Enrollment Number is required.")]
        [Display(Name = "Email or Enrollment No")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
