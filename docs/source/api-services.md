# API controllers and services

Complete routes, authentication, employee CRUD, reporting, and error middleware.

This page contains **complete file contents** for 9 files. Copy each block to the exact path shown. The code is embedded in this Markdown page and remains visible when the sample source directory is unavailable.

Return to [all source files](/source/) or read [setup instructions](/start). The [source bundle](/sample-source.zip) also includes binary icons and license files.

## `sample/EmployeeManagement.Api/Controllers/AuthController.cs`

**File:** `sample/EmployeeManagement.Api/Controllers/AuthController.cs` — **Use:** Implements the Auth HTTP actions.

```csharp
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request)
    {
        var response = await authService.RegisterAsync(request);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        return response is null ? Unauthorized(new { message = "Invalid username or password." }) : Ok(response);
    }
}
```

## `sample/EmployeeManagement.Api/Controllers/DashboardController.cs`

**File:** `sample/EmployeeManagement.Api/Controllers/DashboardController.cs` — **Use:** Implements the Dashboard HTTP actions.

```csharp
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public class DashboardController(AppDbContext dbContext, ReportService reportService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard() => Ok(new DashboardDto(
        await dbContext.Employees.CountAsync(),
        await dbContext.Employees.CountAsync(employee => employee.IsActive),
        await dbContext.Departments.CountAsync(),
        await dbContext.Skills.CountAsync()));

    [HttpGet("department-counts")]
    public async Task<ActionResult<IEnumerable<DepartmentEmployeeCount>>> GetDepartmentCounts() => Ok(await reportService.GetDepartmentEmployeeCountsAsync());
}
```

## `sample/EmployeeManagement.Api/Controllers/DepartmentsController.cs`

**File:** `sample/EmployeeManagement.Api/Controllers/DepartmentsController.cs` — **Use:** Implements the Departments HTTP actions.

```csharp
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/departments")]
public class DepartmentsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Department>>> GetDepartments() => Ok(await dbContext.Departments.AsNoTracking().OrderBy(item => item.Name).ToListAsync());

    [HttpGet("lookup")]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetLookup() => Ok(await dbContext.Departments.AsNoTracking().OrderBy(item => item.Name).Select(item => new LookupDto(item.DepartmentId, item.Name)).ToListAsync());

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Department>> Create(NamedResourceRequest request)
    {
        var name = request.Name.Trim();
        if (await dbContext.Departments.AnyAsync(item => item.Name == name))
            return Conflict(new { message = "A department with this name already exists." });
        var department = new Department { Name = name, Description = request.Description?.Trim() };
        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDepartments), new { id = department.DepartmentId }, department);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Department>> Update(int id, NamedResourceRequest request)
    {
        var department = await dbContext.Departments.FindAsync(id);
        if (department is null) return NotFound();
        var name = request.Name.Trim();
        if (await dbContext.Departments.AnyAsync(item => item.Name == name && item.DepartmentId != id))
            return Conflict(new { message = "A department with this name already exists." });
        department.Name = name;
        department.Description = request.Description?.Trim();
        await dbContext.SaveChangesAsync();
        return Ok(department);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var department = await dbContext.Departments.Include(item => item.Employees).SingleOrDefaultAsync(item => item.DepartmentId == id);
        if (department is null) return NotFound();
        if (department.Employees.Count > 0) return Conflict(new { message = "Move or delete the department's employees first." });
        dbContext.Departments.Remove(department);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
```

## `sample/EmployeeManagement.Api/Controllers/EmployeesController.cs`

**File:** `sample/EmployeeManagement.Api/Controllers/EmployeesController.cs` — **Use:** Implements the Employees HTTP actions.

```csharp
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/employees")]
public class EmployeesController(IEmployeeService employeeService, AppDbContext dbContext, IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> GetEmployees([FromQuery] EmployeeQuery query) =>
        Ok(await employeeService.GetEmployeesAsync(query));

    [HttpGet("{employeeId:int}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int employeeId)
    {
        var employee = await employeeService.GetEmployeeAsync(employeeId);
        return employee is null ? NotFound() : Ok(employee);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(EmployeeRequest request)
    {
        var employee = await employeeService.CreateEmployeeAsync(request);
        return CreatedAtAction(nameof(GetEmployee), new { employeeId = employee.EmployeeId }, employee);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{employeeId:int}")]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int employeeId, EmployeeRequest request)
    {
        var employee = await employeeService.UpdateEmployeeAsync(employeeId, request);
        return employee is null ? NotFound() : Ok(employee);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{employeeId:int}")]
    public async Task<IActionResult> DeleteEmployee(int employeeId) =>
        await employeeService.DeleteEmployeeAsync(employeeId) ? NoContent() : NotFound();

    [Authorize(Roles = "Admin")]
    [HttpPost("{employeeId:int}/profile-image")]
    [RequestSizeLimit(2_000_000)]
    public async Task<ActionResult<object>> UploadProfileImage(int employeeId, IFormFile file)
    {
        var employee = await dbContext.Employees.FindAsync(employeeId);
        if (employee is null)
            return NotFound();
        if (file.Length == 0 || file.Length > 2_000_000)
            return BadRequest(new { message = "Choose an image smaller than 2 MB." });

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(extension) ||
            !new[] { "image/jpeg", "image/png" }.Contains(file.ContentType.ToLowerInvariant()))
            return BadRequest(new { message = "Only JPG and PNG images are allowed." });

        var uploadsFolder = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "uploads", "profiles");
        Directory.CreateDirectory(uploadsFolder);
        var fileName = $"{employeeId}-{Guid.NewGuid():N}{extension}";
        await using (var stream = System.IO.File.Create(Path.Combine(uploadsFolder, fileName)))
            await file.CopyToAsync(stream);

        employee.ProfileImagePath = $"/uploads/profiles/{fileName}";
        await dbContext.SaveChangesAsync();
        return Ok(new { profileImageUrl = employee.ProfileImagePath });
    }
}
```

## `sample/EmployeeManagement.Api/Controllers/SkillsController.cs`

**File:** `sample/EmployeeManagement.Api/Controllers/SkillsController.cs` — **Use:** Implements the Skills HTTP actions.

```csharp
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/skills")]
public class SkillsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [HttpGet("lookup")]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetSkills() => Ok(await dbContext.Skills.AsNoTracking().OrderBy(item => item.Name).Select(item => new LookupDto(item.SkillId, item.Name)).ToListAsync());

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Skill>> Create(NamedResourceRequest request)
    {
        var name = request.Name.Trim();
        if (await dbContext.Skills.AnyAsync(item => item.Name == name))
            return Conflict(new { message = "A skill with this name already exists." });
        var skill = new Skill { Name = name };
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSkills), new { id = skill.SkillId }, skill);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Skill>> Update(int id, NamedResourceRequest request)
    {
        var skill = await dbContext.Skills.FindAsync(id);
        if (skill is null) return NotFound();
        var name = request.Name.Trim();
        if (await dbContext.Skills.AnyAsync(item => item.Name == name && item.SkillId != id))
            return Conflict(new { message = "A skill with this name already exists." });
        skill.Name = name;
        await dbContext.SaveChangesAsync();
        return Ok(skill);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var skill = await dbContext.Skills.Include(item => item.EmployeeSkills).SingleOrDefaultAsync(item => item.SkillId == id);
        if (skill is null) return NotFound();
        if (skill.EmployeeSkills.Count > 0) return Conflict(new { message = "Remove this skill from employees first." });
        dbContext.Skills.Remove(skill);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
```

## `sample/EmployeeManagement.Api/Middleware/ExceptionHandlingMiddleware.cs`

**File:** `sample/EmployeeManagement.Api/Middleware/ExceptionHandlingMiddleware.cs` — **Use:** Converts application exceptions into HTTP problem responses.

```csharp
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled error for {Method} {Path}", context.Request.Method, context.Request.Path);
            var (status, title, detail) = exception switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request", exception.Message),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Request conflict", exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Server error", "An unexpected error occurred.")
            };

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            });
        }
    }
}
```

## `sample/EmployeeManagement.Api/Services/AuthService.cs`

**File:** `sample/EmployeeManagement.Api/Services/AuthService.cs` — **Use:** Implements Auth operations.

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Api.Services;

public class AuthService(AppDbContext dbContext, IConfiguration configuration)
{
    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        if (await dbContext.AppUsers.AnyAsync(user => user.Username == username))
            throw new InvalidOperationException("This username is already taken.");

        var user = new AppUser
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Viewer"
        };
        dbContext.AppUsers.Add(user);
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new InvalidOperationException("This username is already taken.", exception);
        }

        return new RegisterResponse(user.Username, user.Role);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var user = await dbContext.AppUsers.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Username == username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var expiresAt = DateTime.UtcNow.AddHours(1);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.AppUserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresAt, user.Username, user.Role);
    }
}
```

## `sample/EmployeeManagement.Api/Services/EmployeeService.cs`

**File:** `sample/EmployeeManagement.Api/Services/EmployeeService.cs` — **Use:** Implements Employee operations.

```csharp
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
```

## `sample/EmployeeManagement.Api/Services/ReportService.cs`

**File:** `sample/EmployeeManagement.Api/Services/ReportService.cs` — **Use:** Implements Report operations.

```csharp
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

public class ReportService(AppDbContext dbContext)
{
    public Task<List<DepartmentEmployeeCount>> GetDepartmentEmployeeCountsAsync() =>
        dbContext.Set<DepartmentEmployeeCount>()
            .FromSqlRaw("EXEC dbo.GetDepartmentEmployeeCounts")
            .AsNoTracking()
            .ToListAsync();
}
```
