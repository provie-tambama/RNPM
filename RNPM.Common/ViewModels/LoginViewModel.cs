using System.ComponentModel.DataAnnotations;

namespace RNPM.Common.ViewModels;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}