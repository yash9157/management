# Employee Management MVC practice project

This is a separate ASP.NET Core 8 MVC project. Razor views render the pages on the server; there is no Angular frontend or Web API in this project. It is intentionally small enough for a practical interview: employee CRUD, validation, a department dropdown, search, sorting, and pagination.

## Build order

1. `Models/Employee.cs` and `Models/Department.cs` describe the data and the one-department-to-many-employees relationship.
2. `Data/AppDbContext.cs` exposes the tables, configures the foreign key and unique email index, and seeds four departments.
3. `Data/Migrations/` contains the Code First migration that creates the separate SQL Server database.
4. `ViewModels/EmployeeFormViewModel.cs` defines the form fields and validation rules. `EmployeeListViewModel.cs` holds the list filters, page, and results.
5. `Controllers/EmployeesController.cs` receives requests, runs async EF Core queries, validates form values, and chooses the Razor view or redirect.
6. `Views/Employees/` contains the List, Add, Edit, Details, and Delete pages. `_EmployeeForm.cshtml` is shared by Add and Edit.
7. `Program.cs` configures dependency injection, SQL Server, MVC routes, and development migrations.

## Requirements

- .NET 8 SDK
- SQL Server Express at `.\SQLEXPRESS`

The development database is **`EmployeeManagementMvcInterviewDb`**. It is separate from the Angular/Web API project's database. If your SQL Server instance has a different name, set `ConnectionStrings:EmployeeManagementMvc` in `appsettings.Development.json` or provide an environment variable:

```powershell
$env:ConnectionStrings__EmployeeManagementMvc = 'Server=YOUR_SERVER;Database=EmployeeManagementMvcInterviewDb;Trusted_Connection=True;TrustServerCertificate=True'
```

## Run

Open a terminal in this directory:

```powershell
dotnet restore
dotnet tool restore
dotnet tool run dotnet-ef database update
dotnet run --launch-profile http
```

Open **http://localhost:5269**. Development startup also applies pending migrations. The initial employee list is empty so you can practise creating records yourself; the department dropdown is already populated.

To run the build check:

```powershell
dotnet build
```

Do not start a second copy on port 5269 while one is already running. Stop the current terminal with Ctrl+C before rebuilding and restarting if Windows reports that the executable is in use.

## Open and debug in Visual Studio

1. Open `EmployeeManagement.Mvc.sln` from this directory. Do not open the other `EmployeeManagement.slnx` in the parent directory; that belongs to the Angular/Web API project.
2. Set `EmployeeManagement.Mvc` as the startup project and choose the **http** launch profile.
3. Press **F9** on a line in `Controllers/EmployeesController.cs` to add a breakpoint. Useful lines are `Index`, the POST `Create` method, `ValidateReferencesAndEmailAsync`, and `SaveChangesAsync`.
4. Press **F5**. Visual Studio starts the app on `http://localhost:5269`. Open the Add employee form and submit it to hit the POST breakpoint.
5. Inspect `model`, `ModelState.IsValid`, and `dbContext` in Locals or Watch. Use **F10** to step over a line and **F11** to step into a method.
6. Press **Shift+F5** to stop. Use **Ctrl+F5** to run without debugging. Do not run the terminal server and Visual Studio server at the same time on port 5269.

For browser errors, press **F12**, open **Network**, submit the form, and inspect the request status and response. For data, inspect `EmployeeManagementMvcInterviewDb` in SQL Server Object Explorer.

## Pages and request flow

| URL | Purpose |
|---|---|
| `/Employees` | Search, filter, sort, and page employee records |
| `/Employees/Create` | Add employee with server-side validation |
| `/Employees/Details/{id}` | View one employee |
| `/Employees/Edit/{id}` | Update one employee |
| `/Employees/Delete/{id}` | Confirm and delete one employee |

A browser GET asks the controller for a Razor page. A POST sends the form to the controller. The controller checks `ModelState`, validates the department and email, saves through EF Core, and redirects after success. POST forms include an anti-forgery token.

The list query filters first, counts results, sorts by an allowed field, then uses `Skip` and `Take` to load one page. For example:

```csharp
employees = employees.Where(employee => employee.DepartmentId == departmentId);
var total = await employees.CountAsync();
var page = await employees.OrderBy(employee => employee.LastName)
    .Skip((pageNumber - 1) * 10).Take(10).ToListAsync();
```

Approximate SQL:

```sql
SELECT e.* FROM Employees e
WHERE e.DepartmentId = @departmentId
ORDER BY e.LastName
OFFSET (@pageNumber - 1) * 10 ROWS FETCH NEXT 10 ROWS ONLY;
```

## Common errors

- **Port 5269 already in use:** another copy is running; stop it or select another port.
- **SQL connection failed:** check that SQL Server Express is running and that the instance name in `appsettings.Development.json` matches yours.
- **Migration or table missing:** run `dotnet tool run dotnet-ef database update` in this directory.
- **Form does not save:** read the field-level validation messages; dates, department, email, and dropdown values are checked on the server.
- **Duplicate email:** the form shows an error, and a unique SQL index also prevents duplicates.
- **HTTP 400 on a manual POST:** MVC requires an anti-forgery token. Use the rendered form, not an unprotected raw POST.
- **HTTP 404:** the employee ID does not exist, or the route is incorrect.

This is a CRUD interview exercise, not a production HR system. It has **no login or role-based authorization**; keep it on your local development machine. The separate Angular/Web API project contains those features.
