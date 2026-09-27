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
