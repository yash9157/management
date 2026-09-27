# Troubleshooting

Start by identifying which process failed: documentation site, API, Angular, MVC, or SQL Server. The API and MVC use separate databases and separate HTTP ports.

| Symptom | Check | Source or next action |
| --- | --- | --- |
| API fails at startup with SQL connection error | SQL Server Express running? Does `.\\SQLEXPRESS` match your instance? | Override `ConnectionStrings__EmployeeManagement`; see [Run the samples](/start#prerequisites). |
| MVC fails at startup with SQL connection error | Same instance check, but for the MVC database | Override `ConnectionStrings__EmployeeManagementMvc`. |
| API database tables or report procedure missing | Were you running in Development, and could the app connect to SQL Server? | Development startup applies `sample/EmployeeManagement.Api/Data/Migrations/20260927123728_InitialCreate.cs`. |
| Angular cannot load data | Is the API running on port 5087? | Check `sample/Frontend/src/environments/environment.ts` and browser Network. |
| Browser reports a CORS failure | Are you using Angular at `http://localhost:4200`? | The API's `AngularClient` policy in `sample/EmployeeManagement.Api/Program.cs` allows that origin. |
| API returns 401 | Token missing, invalid, or expired | Sign in again; inspect Bearer header and `auth.interceptor.ts`. |
| API returns 403 | Signed-in account lacks Admin role for a write | Use the local Admin account for Admin-only actions. |
| Create/edit returns 400 | Field or reference validation failed | Inspect response details, `EmployeeDtos.cs`, and `EmployeeService.ValidateReferencesAndEmailAsync`. |
| Create/edit returns 409 | Duplicate email or another application conflict | Check the email; `ExceptionHandlingMiddleware` maps `InvalidOperationException` to 409. |
| MVC form returns 400 | Missing anti-forgery token on a manual POST | Submit the rendered Razor form; `EmployeesController` applies `[AutoValidateAntiforgeryToken]`. |
| Port 5087, 4200, or 5269 is in use | Another instance is still running | Stop the old process or change the launch URL and dependent client configuration. |
| `npm ci` fails | Wrong directory or lockfile absent | Use the root for docs and `sample/Frontend` for Angular; each has its own `package-lock.json`. |

## Useful checks

**Working directory: `sample/EmployeeManagement.Api`. Purpose: verify that the API source builds without starting SQL Server.**

```powershell
dotnet build
```

**Working directory: `sample/MvcEmployeeManagement`. Purpose: verify that the MVC source builds without starting SQL Server.**

```powershell
dotnet build
```

**Working directory: `sample/Frontend`. Purpose: verify the Angular production bundle.**

```powershell
npm run build
```

**Working directory: repository root (`new-interviw`). Purpose: verify the documentation site.**

```powershell
npm run docs:build
```

These builds check compilation and documentation output. They do not prove a live SQL Server connection or a full browser workflow.
