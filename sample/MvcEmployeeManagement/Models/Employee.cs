using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Mvc.Models;

public class Employee
{
    public int EmployeeId { get; set; }

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

    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [DataType(DataType.Date)]
    public DateTime JoiningDate { get; set; }

    [Required, StringLength(20)]
    public string Gender { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string EmploymentType { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
}
