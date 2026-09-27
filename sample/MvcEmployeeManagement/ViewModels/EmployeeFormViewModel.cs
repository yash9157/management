using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagement.Mvc.ViewModels;

public class EmployeeFormViewModel : IValidatableObject
{
    public static readonly string[] Genders = ["Female", "Male", "Non-binary", "Prefer not to say"];
    public static readonly string[] EmploymentTypes = ["Full-time", "Part-time", "Contract", "Intern"];

    public int EmployeeId { get; set; }

    [Required, StringLength(80)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(80)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, StringLength(25)]
    public string Phone { get; set; } = string.Empty;

    [Range(0, 100000000)]
    public decimal Salary { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Date of birth")]
    public DateTime? DateOfBirth { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Joining date")]
    public DateTime? JoiningDate { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Employment type")]
    public string EmploymentType { get; set; } = string.Empty;

    [Display(Name = "Active employee")]
    public bool IsActive { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "Select a department.")]
    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [ValidateNever]
    public List<SelectListItem> Departments { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOfBirth.HasValue && DateOfBirth.Value.Date >= DateTime.Today)
            yield return new ValidationResult("Date of birth must be in the past.", [nameof(DateOfBirth)]);
        if (DateOfBirth.HasValue && JoiningDate.HasValue && JoiningDate.Value.Date < DateOfBirth.Value.Date)
            yield return new ValidationResult("Joining date cannot be before date of birth.", [nameof(JoiningDate)]);
        if (!string.IsNullOrWhiteSpace(Gender) && !Genders.Contains(Gender))
            yield return new ValidationResult("Select a valid gender.", [nameof(Gender)]);
        if (!string.IsNullOrWhiteSpace(EmploymentType) && !EmploymentTypes.Contains(EmploymentType))
            yield return new ValidationResult("Select a valid employment type.", [nameof(EmploymentType)]);
    }
}
