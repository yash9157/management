# Run the samples

The three projects are already present in `sample/`. You do not need to rebuild them from snippets in this guide. Choose one path below and use a separate terminal for each running process.

## Prerequisites

| Tool | Why it is needed |
| --- | --- |
| .NET 8 SDK | Build and run the API and MVC projects |
| SQL Server Express | Host the two local development databases; the checked-in connection strings target `.\\SQLEXPRESS` |
| Node.js and npm | Install and run the Angular frontend and this documentation site |
| Browser | Use Angular, Swagger, or the MVC pages |

SQL Server must be installed and the instance must be running. If its instance name is different, set a connection-string environment variable before starting the relevant .NET process. The API and MVC projects use Windows integrated authentication by default.

**Working directory: `sample/EmployeeManagement.Api`. Purpose: point the API at another SQL Server instance for this terminal session.**

```powershell
$env:ConnectionStrings__EmployeeManagement = 'Server=YOUR_SERVER;Database=EmployeeManagementInterviewDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True'
```

**Working directory: `sample/MvcEmployeeManagement`. Purpose: point MVC at another SQL Server instance for this terminal session.**

```powershell
$env:ConnectionStrings__EmployeeManagementMvc = 'Server=YOUR_SERVER;Database=EmployeeManagementMvcInterviewDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True'
```

The checked-in development settings include demo credentials and a development JWT key. Use them only for local practice. Override or replace them before any real deployment.

## Path A: API and Angular

### 1. Start the API

**Working directory: `sample/EmployeeManagement.Api`. Purpose: restore packages, build, and run the development API.**

```powershell
cd .\sample\EmployeeManagement.Api
dotnet restore
dotnet build
dotnet run --launch-profile http
```

Open Swagger at `http://localhost:5087/swagger`. In Development, startup applies the EF Core migration, creates the API database, and seeds four departments, six skills, and two demo users. The SQL Server instance must be reachable for startup to finish.

The default demo accounts are `admin` / `Admin123!` and `viewer` / `Viewer123!`. Registration creates a Viewer account. Admin can modify employees; Viewer can read them.

### 2. Start Angular

Open a **second terminal**.

**Working directory: `sample/Frontend`. Purpose: install the locked dependencies and start the Angular development server.**

```powershell
cd .\sample\Frontend
npm ci
npm start
```

Open `http://localhost:4200`, sign in as Admin, then create an employee. The client uses the API URL in `sample/Frontend/src/environments/environment.ts`, currently `http://localhost:5087/api`. The API CORS policy permits the Angular development origin `http://localhost:4200`.

### 3. Check the request

In the browser's Network tab, create an employee and look for `POST /api/employees`. The request carries a Bearer token. The API returns `201 Created` and an employee DTO. See [API and database](/api) and [Angular request flow](/angular) for the source-file trail.

## Path B: standalone MVC

The MVC app runs on its own and does not call the API. It uses `EmployeeManagementMvcInterviewDb` rather than the API's `EmployeeManagementInterviewDb`.

**Working directory: `sample/MvcEmployeeManagement`. Purpose: restore, build, and run the Razor application.**

```powershell
cd .\sample\MvcEmployeeManagement
dotnet restore
dotnet build
dotnet run --launch-profile http
```

Open `http://localhost:5269/Employees`. Development startup applies the MVC migration and seeds departments. The employee table starts empty. Add one employee, edit it, then search for it on the list page. See [Razor CRUD flow](/mvc).

For Visual Studio, open `sample/MvcEmployeeManagement/EmployeeManagement.Mvc.sln`, select the `http` launch profile, and debug with F5. Stop any terminal copy on port 5269 first.

## Run this documentation site

**Working directory: repository root (`new-interviw`). Purpose: preview the guide.**

```powershell
npm ci
npm run docs:dev
```

The site is static documentation. Building it does not start or deploy the sample applications or SQL Server.
