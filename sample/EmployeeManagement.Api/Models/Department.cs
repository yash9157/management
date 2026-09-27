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
