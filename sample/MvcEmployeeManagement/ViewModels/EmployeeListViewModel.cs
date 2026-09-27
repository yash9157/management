using EmployeeManagement.Mvc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagement.Mvc.ViewModels;

public class EmployeeListViewModel
{
    public string? Search { get; set; }
    public int? DepartmentId { get; set; }
    public string SortBy { get; set; } = "name";
    public string Direction { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public List<Employee> Employees { get; set; } = [];
    public List<SelectListItem> Departments { get; set; } = [];
}
