using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs;

public class LoginRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, string Username, string Role);

public class RegisterRequest
{
    [Required, StringLength(80, MinimumLength = 3)]
    [RegularExpression(@"^[A-Za-z0-9._-]+$", ErrorMessage = "Username may contain letters, numbers, dots, underscores and hyphens only.")]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(72, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*[0-9]).+$", ErrorMessage = "Password must contain a letter and a number.")]
    public string Password { get; set; } = string.Empty;

    [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public record RegisterResponse(string Username, string Role);
