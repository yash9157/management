using EmployeeManagement.Mvc.Data;
using EmployeeManagement.Mvc.Models;
using EmployeeManagement.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Mvc.Controllers;

[AutoValidateAntiforgeryToken]
public class EmployeesController(AppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? search, int? departmentId, string sortBy = "name", string direction = "asc", int page = 1)
    {
        sortBy = sortBy is "email" or "salary" or "joiningDate" or "department" ? sortBy : "name";
        direction = direction == "desc" ? "desc" : "asc";
        page = Math.Max(page, 1);

        var employees = dbContext.Employees.AsNoTracking().Include(employee => employee.Department).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            employees = employees.Where(employee => employee.FirstName.Contains(term)
                || employee.LastName.Contains(term) || employee.Email.Contains(term));
        }
        if (departmentId.HasValue)
            employees = employees.Where(employee => employee.DepartmentId == departmentId.Value);

        var totalCount = await employees.CountAsync();
        const int pageSize = 10;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(totalPages, 1));
        var descending = direction == "desc";
        employees = sortBy switch
        {
            "email" => descending ? employees.OrderByDescending(employee => employee.Email) : employees.OrderBy(employee => employee.Email),
            "salary" => descending ? employees.OrderByDescending(employee => employee.Salary) : employees.OrderBy(employee => employee.Salary),
            "joiningDate" => descending ? employees.OrderByDescending(employee => employee.JoiningDate) : employees.OrderBy(employee => employee.JoiningDate),
            "department" => descending ? employees.OrderByDescending(employee => employee.Department.Name) : employees.OrderBy(employee => employee.Department.Name),
            _ => descending
                ? employees.OrderByDescending(employee => employee.LastName).ThenByDescending(employee => employee.FirstName)
                : employees.OrderBy(employee => employee.LastName).ThenBy(employee => employee.FirstName)
        };

        var model = new EmployeeListViewModel
        {
            Search = search,
            DepartmentId = departmentId,
            SortBy = sortBy,
            Direction = direction,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Employees = await employees.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
            Departments = await GetDepartmentOptionsAsync()
        };
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var employee = await dbContext.Employees.AsNoTracking()
            .Include(item => item.Department)
            .SingleOrDefaultAsync(item => item.EmployeeId == id);
        return employee is null ? NotFound() : View(employee);
    }

    public async Task<IActionResult> Create()
    {
        var model = new EmployeeFormViewModel { Departments = await GetDepartmentOptionsAsync() };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(EmployeeFormViewModel model)
    {
        if (ModelState.IsValid)
            await ValidateReferencesAndEmailAsync(model, null);
        if (!ModelState.IsValid)
        {
            model.Departments = await GetDepartmentOptionsAsync();
            return View(model);
        }

        var employee = new Employee();
        CopyFormToEmployee(model, employee);
        dbContext.Employees.Add(employee);
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (IsDuplicateEmail(exception))
        {
            ModelState.AddModelError(nameof(model.Email), "An employee with this email already exists.");
            model.Departments = await GetDepartmentOptionsAsync();
            return View(model);
        }

        TempData["SuccessMessage"] = "Employee created successfully.";
        return RedirectToAction(nameof(Details), new { id = employee.EmployeeId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await dbContext.Employees.FindAsync(id);
        if (employee is null) return NotFound();

        return View(new EmployeeFormViewModel
        {
            EmployeeId = employee.EmployeeId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            Salary = employee.Salary,
            DateOfBirth = employee.DateOfBirth,
            JoiningDate = employee.JoiningDate,
            Gender = employee.Gender,
            EmploymentType = employee.EmploymentType,
            IsActive = employee.IsActive,
            DepartmentId = employee.DepartmentId,
            Departments = await GetDepartmentOptionsAsync()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, EmployeeFormViewModel model)
    {
        var employee = await dbContext.Employees.FindAsync(id);
        if (employee is null) return NotFound();

        if (ModelState.IsValid)
            await ValidateReferencesAndEmailAsync(model, id);
        if (!ModelState.IsValid)
        {
            model.EmployeeId = id;
            model.Departments = await GetDepartmentOptionsAsync();
            return View(model);
        }

        CopyFormToEmployee(model, employee);
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (IsDuplicateEmail(exception))
        {
            ModelState.AddModelError(nameof(model.Email), "An employee with this email already exists.");
            model.EmployeeId = id;
            model.Departments = await GetDepartmentOptionsAsync();
            return View(model);
        }

        TempData["SuccessMessage"] = "Employee updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var employee = await dbContext.Employees.AsNoTracking()
            .Include(item => item.Department)
            .SingleOrDefaultAsync(item => item.EmployeeId == id);
        return employee is null ? NotFound() : View(employee);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await dbContext.Employees.FindAsync(id);
        if (employee is null) return NotFound();

        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Employee deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateReferencesAndEmailAsync(EmployeeFormViewModel model, int? currentEmployeeId)
    {
        if (!await dbContext.Departments.AnyAsync(department => department.DepartmentId == model.DepartmentId))
            ModelState.AddModelError(nameof(model.DepartmentId), "Select an existing department.");

        var email = model.Email.Trim().ToLowerInvariant();
        if (await dbContext.Employees.AnyAsync(employee => employee.Email == email && employee.EmployeeId != currentEmployeeId))
            ModelState.AddModelError(nameof(model.Email), "An employee with this email already exists.");
    }

    private async Task<List<SelectListItem>> GetDepartmentOptionsAsync() =>
        await dbContext.Departments.AsNoTracking().OrderBy(department => department.Name)
            .Select(department => new SelectListItem(department.Name, department.DepartmentId.ToString()))
            .ToListAsync();

    private static void CopyFormToEmployee(EmployeeFormViewModel form, Employee employee)
    {
        employee.FirstName = form.FirstName.Trim();
        employee.LastName = form.LastName.Trim();
        employee.Email = form.Email.Trim().ToLowerInvariant();
        employee.Phone = form.Phone.Trim();
        employee.Salary = form.Salary;
        employee.DateOfBirth = form.DateOfBirth!.Value.Date;
        employee.JoiningDate = form.JoiningDate!.Value.Date;
        employee.Gender = form.Gender;
        employee.EmploymentType = form.EmploymentType;
        employee.IsActive = form.IsActive;
        employee.DepartmentId = form.DepartmentId;
    }

    private static bool IsDuplicateEmail(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 2601 or 2627 };
}
