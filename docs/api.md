# API and database

The ASP.NET Core 8 API exposes employee records to Angular. Controllers accept HTTP requests, `EmployeeService` handles employee rules, and EF Core stores data in SQL Server. Development startup applies the initial migration and creates the local demo accounts.

## Database

| Table | Purpose | Important rule |
| --- | --- | --- |
| `Employees` | Employee data and department reference | Email has a unique index |
| `Departments` | Department choices | Name has a unique index; an in-use department cannot be deleted |
| `Skills`, `EmployeeSkills` | Skills and employee-skill links | Composite key for each employee-skill pair |
| `AppUsers` | Demo login accounts | Username has a unique index; passwords are BCrypt hashed |

The complete mapping and initial migration are in the [API data source page](/source/api-data). The migration creates the tables, seeds four departments and six skills, and creates one stored procedure used by the extra report endpoint.

**File: `sample/EmployeeManagement.Api/Data/AppDbContext.cs` — Use: connect each employee to one department.**

```csharp
modelBuilder.Entity<Employee>()
    .HasOne(employee => employee.Department)
    .WithMany(department => department.Employees)
    .HasForeignKey(employee => employee.DepartmentId)
    .OnDelete(DeleteBehavior.Restrict);
```

## Endpoint map

| Route | Access | Purpose |
| --- | --- | --- |
| `POST /api/auth/login`, `POST /api/auth/register` | Anonymous | Sign in or create a Viewer account |
| `GET /api/employees`, `GET /api/employees/{id}` | Signed in | Paged list or detail |
| `POST /api/employees`, `PUT /api/employees/{id}`, `DELETE /api/employees/{id}` | Admin | Employee CRUD writes |
| `GET /api/departments`, `GET /api/departments/lookup` | Signed in | Department list and form options |
| `POST /api/departments`, `PUT /api/departments/{id}`, `DELETE /api/departments/{id}` | Admin | Manage departments |
| `GET /api/skills/lookup` | Signed in | Skill choices for the employee form |
| `POST /api/skills`, `PUT /api/skills/{id}`, `DELETE /api/skills/{id}` | Admin | Manage skills |
| `POST /api/employees/{id}/profile-image` | Admin | Optional profile image upload |
| `GET /api/dashboard`, `GET /api/dashboard/department-counts` | Signed in | Optional totals and report |

The core interview flow below uses login and employee CRUD. The [complete source](/source/) also contains the optional skill, image, and report paths so every checked-in coding file remains available.

## Trace a list request

1. `EmployeesController.GetEmployees` receives `EmployeeQuery` from the query string.
2. `EmployeeService.GetEmployeesAsync` applies search and optional department, status, gender, and employment-type filters.
3. It counts matches, chooses an allowed sort, and loads one page.
4. It loads department and skill data, maps entities to `EmployeeDto`, and returns `PagedResult<EmployeeDto>`.

**File: `sample/EmployeeManagement.Api/Services/EmployeeService.cs` — Use: load one sorted page with department names.**

```csharp
var pageEntities = await employees
    .Include(employee => employee.Department)
    .Include(employee => employee.EmployeeSkills)
    .ThenInclude(link => link.Skill)
    .Skip((query.Page - 1) * query.PageSize)
    .Take(query.PageSize)
    .ToListAsync();
```

`EmployeeQuery` permits page sizes from 1 to 100. For writes, the service checks that the department and selected skills exist and that the email is not already used; the database index protects the email rule under concurrency. See the [complete contracts](/source/api-contracts) and [complete services](/source/api-services).

## Try the API

Start the API using [Run the samples](/start). Open `http://localhost:5087/swagger`, call `POST /api/auth/login` with the local Admin account, copy `accessToken`, and use Swagger's **Authorize** button. Then try `GET /api/employees?page=1&pageSize=10`.

The full `sample/EmployeeManagement.Api/EmployeeManagement.Api.http` file is embedded on the [API project page](/source/api-project). It includes login, employee list, and department lookup requests.
