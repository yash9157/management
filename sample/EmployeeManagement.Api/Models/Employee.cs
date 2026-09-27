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
