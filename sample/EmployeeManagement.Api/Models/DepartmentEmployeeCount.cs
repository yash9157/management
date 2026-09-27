namespace EmployeeManagement.Api.Models;

public class DepartmentEmployeeCount
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
}
