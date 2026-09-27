using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Models;

public class AppUser
{
    public int AppUserId { get; set; }

    [MaxLength(80)]
    public required string Username { get; set; }

    [MaxLength(200)]
    public required string PasswordHash { get; set; }

    [MaxLength(30)]
    public required string Role { get; set; }
}
