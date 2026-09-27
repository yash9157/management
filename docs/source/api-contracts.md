# API entities and contracts

## `sample/EmployeeManagement.Api/DTOs/AuthDtos.cs`

Supplies application code or configuration required by this sample.

```csharp
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
```

## `sample/EmployeeManagement.Api/DTOs/CommonDtos.cs`

Supplies application code or configuration required by this sample.

```csharp
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs;

public record LookupDto(int Id, string Name);

public class NamedResourceRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }
}

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public record DashboardDto(int TotalEmployees, int ActiveEmployees, int DepartmentCount, int SkillCount);
```

## `sample/EmployeeManagement.Api/DTOs/EmployeeDtos.cs`

Supplies application code or configuration required by this sample.

```csharp
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs;

public class EmployeeRequest : IValidatableObject
{
    [Required, StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, StringLength(25)]
    public string Phone { get; set; } = string.Empty;

    [Range(0, 100000000)]
    public decimal Salary { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public DateTime JoiningDate { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public string EmploymentType { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; set; }

    public List<int> SkillIds { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOfBirth.Date >= DateTime.UtcNow.Date)
            yield return new ValidationResult("Date of birth must be in the past.", [nameof(DateOfBirth)]);
        if (JoiningDate.Date < DateOfBirth.Date)
            yield return new ValidationResult("Joining date cannot be before date of birth.", [nameof(JoiningDate)]);
        if (!new[] { "Female", "Male", "Non-binary", "Prefer not to say" }.Contains(Gender))
            yield return new ValidationResult("Select a valid gender.", [nameof(Gender)]);
        if (!new[] { "Full-time", "Part-time", "Contract", "Intern" }.Contains(EmploymentType))
            yield return new ValidationResult("Select a valid employment type.", [nameof(EmploymentType)]);
    }
}

public record EmployeeDto(
    int EmployeeId,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    decimal Salary,
    DateTime DateOfBirth,
    DateTime JoiningDate,
    string Gender,
    string EmploymentType,
    bool IsActive,
    int DepartmentId,
    string DepartmentName,
    IReadOnlyList<LookupDto> Skills,
    string? ProfileImageUrl);

public class EmployeeQuery
{
    public string? Search { get; set; }
    public int? DepartmentId { get; set; }
    public int? SkillId { get; set; }
    public bool? IsActive { get; set; }
    public string? Gender { get; set; }
    public string? EmploymentType { get; set; }
    public string SortBy { get; set; } = "name";
    public string SortDirection { get; set; } = "asc";

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}
```

## `sample/EmployeeManagement.Api/Interfaces/IEmployeeService.cs`

Implements Employee operations.

```csharp
using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQuery query);
    Task<EmployeeDto?> GetEmployeeAsync(int employeeId);
    Task<EmployeeDto> CreateEmployeeAsync(EmployeeRequest request);
    Task<EmployeeDto?> UpdateEmployeeAsync(int employeeId, EmployeeRequest request);
    Task<bool> DeleteEmployeeAsync(int employeeId);
}
```

## `sample/EmployeeManagement.Api/Models/AppUser.cs`

Supplies application code or configuration required by this sample.

```csharp
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
```

## `sample/EmployeeManagement.Api/Models/Department.cs`

Supplies application code or configuration required by this sample.

```csharp
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Models;

public class Department
{
    public int DepartmentId { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
```

## `sample/EmployeeManagement.Api/Models/DepartmentEmployeeCount.cs`

Supplies application code or configuration required by this sample.

```csharp
namespace EmployeeManagement.Api.Models;

public class DepartmentEmployeeCount
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
}
```

## `sample/EmployeeManagement.Api/Models/Employee.cs`

Supplies application code or configuration required by this sample.

```csharp
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Models;

public class Employee
{
    public int EmployeeId { get; set; }

    [MaxLength(80)]
    public required string FirstName { get; set; }

    [MaxLength(80)]
    public required string LastName { get; set; }

    [MaxLength(150)]
    public required string Email { get; set; }

    [MaxLength(25)]
    public required string Phone { get; set; }

    public decimal Salary { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime JoiningDate { get; set; }

    [MaxLength(20)]
    public required string Gender { get; set; }

    [MaxLength(30)]
    public required string EmploymentType { get; set; }

    public bool IsActive { get; set; }
    public int DepartmentId { get; set; }
    public string? ProfileImagePath { get; set; }
    public Department Department { get; set; } = null!;
    public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
}
```

## `sample/EmployeeManagement.Api/Models/EmployeeSkill.cs`

Supplies application code or configuration required by this sample.

```csharp
namespace EmployeeManagement.Api.Models;

public class EmployeeSkill
{
    public int EmployeeId { get; set; }
    public int SkillId { get; set; }
    public Employee Employee { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
```

## `sample/EmployeeManagement.Api/Models/Skill.cs`

Supplies application code or configuration required by this sample.

```csharp
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Models;

public class Skill
{
    public int SkillId { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
}
```
