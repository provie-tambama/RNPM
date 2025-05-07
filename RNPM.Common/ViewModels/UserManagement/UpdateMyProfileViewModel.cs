using System.ComponentModel.DataAnnotations;

namespace RNPM.Common.ViewModels.UserManagement;

public class UpdateMyProfileViewModel
{
    public string? Id { get; set; }
    [Required(ErrorMessage = "Full name is required")]
    public required string Fullname { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^((00|\+)?(263))?0?7(1|3|7|8)[0-9]{7}$", ErrorMessage = "Enter a valid mobile number")]
    public required string PhoneNumber { get; set; }
    
}