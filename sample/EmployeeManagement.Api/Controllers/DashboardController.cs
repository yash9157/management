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
