using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.account;

public class RegisterDto
{
    [Required]
    [StringLength(40, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
