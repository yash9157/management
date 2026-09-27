# Interview practice

Use these exercises after [starting the samples](/start). The [code pages](/source/api-project) show the application files used in the exercises.

## 1. Trace employee creation

1. Sign in to Angular as `admin` / `Admin123!`.
2. Open `/employees/new`, enter valid details, choose a department, and save.
3. In browser Network, inspect `POST /api/employees`, the JSON body, Bearer header, and `201 Created` response.
4. Follow `employee-form.component.ts` → `employee.service.ts` → `EmployeesController.cs` → `EmployeeService.cs` → `AppDbContext.cs` in the [source index](/source/).

**Explain:** Angular validates the form for feedback; the API validates the request and current department; SQL Server enforces unique email even if requests race.

## 2. Compare Viewer and Admin

1. Sign in as `viewer` / `Viewer123!` and open the employee list.
2. Observe that the create/edit routes are guarded.
3. In Swagger, authorize with a Viewer token and attempt `POST /api/employees`.

**Expected:** read succeeds; write returns 403. Angular guards guide navigation, and server authorization enforces access.

## 3. Check server paging

Create more than ten employees, then request `GET /api/employees?page=2&pageSize=10&sortBy=name&sortDirection=asc`. Compare `items`, `totalCount`, and `totalPages` with the Angular list. Explain the order: filter, count, sort, `Skip`, `Take`.

## 4. Debug the MVC form

Start the separate MVC app and create an employee. Submit a duplicate email. Break at the POST `EmployeesController.Create` action and inspect `ModelState`, department validation, `SaveChangesAsync`, and the redirect on success.

## Questions to rehearse

| Question | Answer from this sample |
| --- | --- |
| Why separate request/view models from entities? | They define allowed input and validation; `Employee` is the EF Core persistence model. |
| Why check email in code and add a unique index? | Code gives a clear error; the database index handles races. |
| Why use `AsNoTracking`? | List and detail queries do not change entities. |
| What do 401 and 403 mean? | 401 cannot authenticate; 403 is authenticated without the required role. |
| MVC versus Angular? | MVC returns server-rendered HTML; Angular renders in the browser and calls JSON endpoints. |

Lockfiles, icons, generated files, and vendor assets are in the [source bundle](/sample-source.zip).
