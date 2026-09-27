using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

public class EmployeeService(AppDbContext dbContext) : IEmployeeService
{
    public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQuery query)
    {
        var employees = dbContext.Employees.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            employees = employees.Where(employee =>
                employee.FirstName.Contains(search) ||
                employee.LastName.Contains(search) ||
                employee.Email.Contains(search));
        }

        if (query.DepartmentId.HasValue)
            employees = employees.Where(employee => employee.DepartmentId == query.DepartmentId.Value);
        if (query.SkillId.HasValue)
            employees = employees.Where(employee => employee.EmployeeSkills.Any(link => link.SkillId == query.SkillId.Value));
        if (query.IsActive.HasValue)
            employees = employees.Where(employee => employee.IsActive == query.IsActive.Value);
        if (!string.IsNullOrWhiteSpace(query.Gender))
            employees = employees.Where(employee => employee.Gender == query.Gender);
        if (!string.IsNullOrWhiteSpace(query.EmploymentType))
            employees = employees.Where(employee => employee.EmploymentType == query.EmploymentType);

        var totalCount = await employees.CountAsync();
        var descending = query.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
        employees = query.SortBy.ToLowerInvariant() switch
        {
            "email" => descending ? employees.OrderByDescending(employee => employee.Email) : employees.OrderBy(employee => employee.Email),
            "salary" => descending ? employees.OrderByDescending(employee => employee.Salary) : employees.OrderBy(employee => employee.Salary),
            "joiningdate" => descending ? employees.OrderByDescending(employee => employee.JoiningDate) : employees.OrderBy(employee => employee.JoiningDate),
            "department" => descending ? employees.OrderByDescending(employee => employee.Department.Name) : employees.OrderBy(employee => employee.Department.Name),
            _ => descending
                ? employees.OrderByDescending(employee => employee.LastName).ThenByDescending(employee => employee.FirstName)
                : employees.OrderBy(employee => employee.LastName).ThenBy(employee => employee.FirstName)
        };

        var pageEntities = await employees
            .Include(employee => employee.Department)
            .Include(employee => employee.EmployeeSkills)
            .ThenInclude(link => link.Skill)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();
        var items = pageEntities.Select(ToDto).ToList();

        return new PagedResult<EmployeeDto>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<EmployeeDto?> GetEmployeeAsync(int employeeId)
    {
        var employee = await dbContext.Employees
            .AsNoTracking()
            .Include(item => item.Department)
            .Include(item => item.EmployeeSkills)
            .ThenInclude(link => link.Skill)
            .SingleOrDefaultAsync(item => item.EmployeeId == employeeId);
        return employee is null ? null : ToDto(employee);
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeRequest request)
    {
        await ValidateReferencesAndEmailAsync(request, null);
        var employee = new Employee
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Phone = request.Phone.Trim(),
            Salary = request.Salary,
            DateOfBirth = request.DateOfBirth.Date,
            JoiningDate = request.JoiningDate.Date,
            Gender = request.Gender,
            EmploymentType = request.EmploymentType,
            IsActive = request.IsActive,
            DepartmentId = request.DepartmentId,
            EmployeeSkills = request.SkillIds.Distinct().Select(skillId => new EmployeeSkill { SkillId = skillId }).ToList()
        };

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();
        return (await GetEmployeeAsync(employee.EmployeeId))!;
    }

    public async Task<EmployeeDto?> UpdateEmployeeAsync(int employeeId, EmployeeRequest request)
    {
        var employee = await dbContext.Employees
            .Include(item => item.EmployeeSkills)
            .SingleOrDefaultAsync(item => item.EmployeeId == employeeId);
        if (employee is null)
            return null;

        await ValidateReferencesAndEmailAsync(request, employeeId);
        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = request.Email.Trim().ToLowerInvariant();
        employee.Phone = request.Phone.Trim();
        employee.Salary = request.Salary;
        employee.DateOfBirth = request.DateOfBirth.Date;
        employee.JoiningDate = request.JoiningDate.Date;
        employee.Gender = request.Gender;
        employee.EmploymentType = request.EmploymentType;
        employee.IsActive = request.IsActive;
        employee.DepartmentId = request.DepartmentId;

        dbContext.EmployeeSkills.RemoveRange(employee.EmployeeSkills);
        employee.EmployeeSkills = request.SkillIds.Distinct()
            .Select(skillId => new EmployeeSkill { EmployeeId = employeeId, SkillId = skillId })
            .ToList();

        await dbContext.SaveChangesAsync();
        return await GetEmployeeAsync(employeeId);
    }

    public async Task<bool> DeleteEmployeeAsync(int employeeId)
    {
        var employee = await dbContext.Employees.FindAsync(employeeId);
        if (employee is null)
            return false;

        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync();
        return true;
    }

    private async Task ValidateReferencesAndEmailAsync(EmployeeRequest request, int? currentEmployeeId)
    {
        if (!await dbContext.Departments.AnyAsync(department => department.DepartmentId == request.DepartmentId))
            throw new ArgumentException("The selected department does not exist.");

        var requestedSkillIds = request.SkillIds.Distinct().ToList();
        var skillCount = await dbContext.Skills.CountAsync(skill => requestedSkillIds.Contains(skill.SkillId));
        if (skillCount != requestedSkillIds.Count)
            throw new ArgumentException("One or more selected skills do not exist.");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await dbContext.Employees.AnyAsync(employee => employee.Email == normalizedEmail && employee.EmployeeId != currentEmployeeId))
            throw new InvalidOperationException("An employee with this email already exists.");
    }

    private static EmployeeDto ToDto(Employee employee) => new(
        employee.EmployeeId,
        employee.FirstName,
        employee.LastName,
        employee.Email,
        employee.Phone,
        employee.Salary,
        employee.DateOfBirth,
        employee.JoiningDate,
        employee.Gender,
        employee.EmploymentType,
        employee.IsActive,
        employee.DepartmentId,
        employee.Department.Name,
        employee.EmployeeSkills.OrderBy(link => link.Skill.Name).Select(link => new LookupDto(link.SkillId, link.Skill.Name)).ToList(),
        employee.ProfileImagePath);
}
