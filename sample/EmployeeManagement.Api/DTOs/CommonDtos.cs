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
