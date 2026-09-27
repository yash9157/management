# Rebuild from the documentation

This page is for the interview constraint: **you can read the docs but cannot browse the source repository**. The [complete source pages](/source/) show each first-party file in full, with its path and purpose. The [downloadable bundle](/sample-source.zip) provides an exact copy when downloads are allowed.

## Fastest path: download the bundle

1. Download [sample-source.zip](/sample-source.zip) and extract it into a working directory. The ZIP contains `sample/EmployeeManagement.Api`, `sample/Frontend`, and `sample/MvcEmployeeManagement`.
2. Install .NET 8 SDK, SQL Server Express, Node.js, and npm. The sample does not contain their installers.
3. Choose the API + Angular path or the MVC path below. They can be practiced independently.

The archive includes project files, migration code and metadata, Angular lockfile, Razor/Angular templates, icons, and the MVC project's bundled browser libraries. It excludes build outputs, installed `node_modules`, and machine-specific Visual Studio settings.

## Manual path: copy files from the pages

Create the exact directory tree shown by each **File** label and copy the entire code block into that path. Start with the project/startup page, then contracts/models, data/migrations, behavior, and UI files. Keep file extensions and capitalization. The source pages include the migration designer and snapshot files for a faithful EF Core project.

The source pages embed all first-party text. For the exact Angular dependency tree and the MVC project's bundled browser libraries, use the ZIP. If the ZIP is unavailable, `npm install` can produce a new Angular lockfile from `package.json`; the MVC layout's vendor CSS/JS must be restored separately before the pages look and validate exactly like this sample.

## API + Angular steps

1. Copy or extract all files listed under the [API source pages](/source/#aspnet-core-api) and [Angular source pages](/source/#angular-frontend).
2. Start SQL Server Express. The API development setting targets `.\\SQLEXPRESS` and database `EmployeeManagementInterviewDb`.
3. In the API directory, restore/build/run. Development startup applies the migration and seeds reference data plus local demo accounts.

**Working directory: `sample/EmployeeManagement.Api`. Purpose: restore and run the reconstructed API.**

```powershell
dotnet restore
dotnet build
dotnet run --launch-profile http
```

4. Open Swagger at `http://localhost:5087/swagger`. Sign in with `admin` / `Admin123!` or `viewer` / `Viewer123!` only for local practice.
5. In a second terminal, install and run Angular.

**Working directory: `sample/Frontend`. Purpose: install dependencies and run the reconstructed client.**

```powershell
npm ci
npm start
```

Open `http://localhost:4200`. The API URL comes from `sample/Frontend/src/environments/environment.ts`. `npm ci` requires the lockfile from the bundle; use `npm install` if manually recreating only from text pages.

## MVC steps

1. Copy or extract all files listed under the [MVC source pages](/source/#standalone-mvc).
2. Start SQL Server Express. MVC targets its own database, `EmployeeManagementMvcInterviewDb`.

**Working directory: `sample/MvcEmployeeManagement`. Purpose: restore and run the reconstructed Razor app.**

```powershell
dotnet restore
dotnet build
dotnet run --launch-profile http
```

Open `http://localhost:5269/Employees`. Development startup applies the MVC migration. The dropdown has seeded departments; the employee list starts empty. Visual Studio can open `EmployeeManagement.Mvc.sln` from this same directory.

## Verify the result

Use [hands-on exercises](/practice) to create, read, update, and delete an employee and to trace the corresponding code. For common failures, see [troubleshooting](/troubleshooting). A successful build alone does not prove SQL Server connectivity; the live app must start and serve a request.
