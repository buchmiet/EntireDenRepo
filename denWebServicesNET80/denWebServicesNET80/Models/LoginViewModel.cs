using System.ComponentModel.DataAnnotations;

namespace denWebServicesNET80.Models;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
    }
}