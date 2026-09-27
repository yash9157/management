# Employee Management

This repository contains three runnable interview projects and a documentation site with their complete source code:

- [ASP.NET Core API](sample/EmployeeManagement.Api)
- [Angular frontend](sample/Frontend)
- [ASP.NET Core MVC app](sample/MvcEmployeeManagement)

Read the [complete source index](docs/source/index.md) to find every code and configuration file in the docs, including both projects' `appsettings` files. The [setup guide](docs/start.md) explains how to run the projects.

The development settings contain local SQL Server addresses and demonstration accounts. Replace these values before using the projects outside interview practice.

## Read the guide locally

Requirements: Node.js and npm. From this directory:

```powershell
npm ci
npm run docs:dev
```

Open the local URL printed by VitePress. For a production build, run `npm run docs:build`.

The guide starts at [docs/index.md](docs/index.md). Its [complete source pages](docs/source/index.md) embed the first-party project files so the code is readable from the docs alone. [Rebuild instructions](docs/rebuild.md) explain how to recreate the applications; the site also serves `sample-source.zip` with lockfiles and binary/vendor assets.

After changing a file under `sample/`, refresh the embedded source and bundle:

```powershell
npm run docs:sync-source
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\build-source-archive.ps1
npm run docs:verify-source
npm run docs:build
```

The docs build is static; running the API, Angular app, or MVC app requires their own tools and SQL Server, as described in [the start guide](docs/start.md).
