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
