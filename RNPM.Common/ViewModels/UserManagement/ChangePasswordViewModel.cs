using System.ComponentModel.DataAnnotations;

namespace RNPM.Common.ViewModels.UserManagement;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Old Password is required")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "New Password is required")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}