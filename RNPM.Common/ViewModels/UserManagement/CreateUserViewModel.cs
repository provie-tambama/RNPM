using System.ComponentModel.DataAnnotations;

namespace RNPM.Common.ViewModels.UserManagement;

public class CreateUserViewModel
{
    public string? Id { get; set; }
    [Required(ErrorMessage = "Firstname is required")]
    public required string Firstname { get; set; }

    [Required(ErrorMessage = "Surname is required")]
    public required string Surname { get; set; }

    public string Fullname => $"{Firstname} {Surname}".Trim();
    public string? UserName { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^((00|\+)?(263))?0?7(1|3|7|8)[0-9]{7}$", ErrorMessage = "Enter a valid mobile number")]
    public required string PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "Email address is required")]
    [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Enter a valid email address")]
    public required string Email { get; set; }

    [DataType(DataType.Password)]
    public string? Password { get; set; }
    
    [Required(ErrorMessage = "Select a role")]
    public string? Role { get; set; }
    
    public string? Token { get; set; }
    public bool IsAdmin { get; set; }
}