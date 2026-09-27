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
