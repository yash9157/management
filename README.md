# Employee Management

This repository contains three runnable interview projects and documentation for their application code:

- [ASP.NET Core API](sample/EmployeeManagement.Api)
- [Angular frontend](sample/Frontend)
- [ASP.NET Core MVC app](sample/MvcEmployeeManagement)

Read the [code pages](docs/source/index.md) for application files, including both projects' `appsettings` files. The [setup guide](docs/start.md) explains how to run the projects. Dependency lockfiles, editor settings, generated migration metadata, and bundled libraries remain in the project folders and source ZIP.

The development settings contain local SQL Server addresses and demonstration accounts. Replace these values before using the projects outside interview practice.

## Read the guide locally

Requirements: Node.js and npm. From this directory:

```powershell
npm ci
npm run docs:dev
```

Open the local URL printed by VitePress. For a production build, run `npm run docs:build`.

The guide starts at [docs/index.md](docs/index.md). Its [code pages](docs/source/index.md) show the application files. [Rebuild instructions](docs/rebuild.md) explain how to recreate the applications; the site also serves `sample-source.zip` with the full project trees.

After changing a file under `sample/`, refresh the embedded source and bundle:

```powershell
npm run docs:sync-source
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\build-source-archive.ps1
npm run docs:verify-source
npm run docs:build
```

The docs build is static; running the API, Angular app, or MVC app requires their own tools and SQL Server, as described in [the start guide](docs/start.md).
