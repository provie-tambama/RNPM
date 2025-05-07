using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RNPM.Common.ViewModels.UserManagement;

public class RegisterUserViewModel
{
    [Required(ErrorMessage = "Firstname is required")]
    public required string Firstname { get; set; }

    [Required(ErrorMessage = "Surname is required")]
    public required string Surname { get; set; }

    public string Fullname => $"{Firstname} {Surname}".Trim();
    [Required]
    public Role Role { get; set; }
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email (optional)")]
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public enum Role
{
    [Description("SystemAdministrator")]
    SystemAdministrator,
    [Description("Administrator")]
    Administrator,
    [Description("User")]
    User

}