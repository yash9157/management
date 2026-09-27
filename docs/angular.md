# Angular request flow

Angular runs in the browser and calls the API at `http://localhost:5087/api` using `HttpClient`. This page follows the login and employee CRUD path. The complete source pages also include the other checked-in features.

## Routes

| Route | Page | Access |
| --- | --- | --- |
| `/login`, `/register` | Authentication forms | Public |
| `/employees` | Employee list | Signed in |
| `/employees/:id` | Employee detail | Signed in |
| `/employees/new`, `/employees/:id/edit` | Create/edit form | Admin |
| `/dashboard` | Optional summary page | Signed in |

The complete route table is in [app.routes.ts](/source/angular-project). The sample's default route opens `/dashboard`; use `/employees` to follow the CRUD exercise.

## Search and page

The list component builds an `EmployeeQuery` from search text, department, skill, active state, employment type, sort, page, and page size. Search waits 350 ms after typing. Changing a filter resets the page to 1. `EmployeeService` turns the query into HTTP parameters.

**File: `sample/Frontend/src/app/core/services/employee.service.ts` — Use: send typed list parameters to the API.**

```typescript
let params = new HttpParams()
  .set('page', query.page).set('pageSize', query.pageSize)
  .set('sortBy', query.sortBy).set('sortDirection', query.sortDirection);
if (query.search) params = params.set('search', query.search);
```

The API returns `items`, `page`, `pageSize`, `totalCount`, and `totalPages`. The list shows loading, error, and empty states and asks for confirmation before deletion.

## Create or edit

1. An Admin opens `/employees/new` or `/employees/:id/edit`.
2. `EmployeeFormComponent` loads departments, skills, and, for editing, the existing employee.
3. The reactive form validates required fields and basic formats.
4. `EmployeeService` sends `POST` or `PUT`.
5. The API checks the request and database references, saves through EF Core, and returns an employee DTO.
6. If an image was selected, Angular sends a separate upload request after the employee is saved.
7. Angular navigates to the employee detail page.

**File: `sample/Frontend/src/app/features/employees/employee-form/employee-form.component.ts` — Use: choose create or update before an optional image upload.**

```typescript
const saveRequest = this.employeeId
  ? this.employeeService.updateEmployee(this.employeeId, request)
  : this.employeeService.createEmployee(request);

saveRequest.pipe(
  switchMap(employee => this.profileFile
    ? this.employeeService.uploadProfileImage(employee.employeeId, this.profileFile).pipe(map(() => employee))
    : of(employee)),
  finalize(() => this.saving = false)
).subscribe({
  next: employee => {
    this.notifications.success(`Employee ${this.employeeId ? 'updated' : 'created'} successfully.`);
    this.router.navigate(['/employees', employee.employeeId]);
  },
  error: error => this.errorMessage = this.getApiError(error)
});
```

The image request is a second write, so an upload failure can happen after the employee record is saved. The [full file](/source/angular-employees) shows the surrounding form logic.

Read the [complete Angular source](/source/#angular-frontend) to see each component, HTML template, service, guard, and model without opening the project files.

## Debugging checkpoints

- Browser Network shows the request URL, JSON body, status, and Bearer header. Login has no token.
- `sample/Frontend/src/environments/environment.ts` defines the API URL.
- The API breakpoints `EmployeesController.GetEmployees` and `EmployeeService.GetEmployeesAsync` show query values.
- A 401 means the token is absent or invalid. A 403 on a write means the caller is signed in without the Admin role.
