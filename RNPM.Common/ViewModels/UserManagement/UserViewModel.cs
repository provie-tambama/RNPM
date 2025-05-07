namespace RNPM.Common.ViewModels;

public class UserViewModel
{
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; } = string.Empty;
    
    public string? Firstname { get; set; }
    
    public string? Surname { get; set; }
    public string Fullname { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
    public string Token { get; set; }
    
    public bool IsAdmin { get; set; }
    public bool LoggedIn { get; set; } = false;
    public string AccessCode { get; set; }
    
}