using System.ComponentModel.DataAnnotations;

namespace Iowa.Authentication.Register;

public class Payload
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
}
