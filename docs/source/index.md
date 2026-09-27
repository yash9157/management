# Complete source, file by file

These pages contain the **full text of 119 coding and configuration files** from `sample/`. Each code block names its exact path and purpose. The code is embedded directly in the documentation, so it remains readable without repository access.

For an exact runnable copy, [download the source bundle](/sample-source.zip). It contains 126 files, including the source below, icons, licenses, and bundled browser assets. It excludes generated `bin`, `obj`, `dist`, `node_modules`, Angular cache output, and the machine-specific `.csproj.user` file.

Use [setup instructions](/start) to run the projects, or [rebuild instructions](/rebuild) to recreate them from these pages.

## ASP.NET Core API

| Read in this order | Contents |
| --- | --- |
| [Project and startup](/source/api-project) | `.csproj`, settings, launch profiles, `Program.cs`, local tool and HTTP requests |
| [Entities and contracts](/source/api-contracts) | Models, DTO validation, service interface |
| [EF Core and migration](/source/api-data) | `AppDbContext`, startup seeding, initial migration and stored procedure |
| [Controllers and services](/source/api-services) | Routes, authorization, CRUD, auth, reports, error handling |
| [Migration metadata](/source/api-generated) | Designer and model snapshot needed for migration tooling |

## Angular frontend

| Read in this order | Contents |
| --- | --- |
| [Project and bootstrap](/source/angular-project) | `package.json`, Angular/TS config, entry points, routing, app shell |
| [Dependency lockfile](/source/angular-lockfile) | Complete `package-lock.json` for `npm ci` |
| [Models and core services](/source/angular-core) | Typed models, HTTP services, session, guards, interceptor |
| [Login and registration](/source/angular-auth) | Authentication components and templates |
| [Employee screens](/source/angular-employees) | List, detail, form, pagination, dialog, notification |
| [Other Angular files](/source/angular-extra) | Remaining component, template, and global styles |
| [Tests](/source/angular-tests) | Checked-in component tests |

## Standalone MVC

| Read in this order | Contents |
| --- | --- |
| [Project and startup](/source/mvc-project) | `.sln`, `.csproj`, settings, launch profiles, `Program.cs` |
| [Models, EF Core, migration](/source/mvc-data) | Entities, form/list view models, schema migration |
| [Controllers](/source/mvc-controller) | Complete employee CRUD and home actions |
| [Razor views and assets](/source/mvc-views) | Pages, partials, layout, first-party CSS/JS |
| [Bundled validation JavaScript](/source/mvc-vendor) | Complete checked-in vendor `.js` files |
| [Migration metadata](/source/mvc-generated) | Designer and model snapshot |

## Non-code assets in the bundle

Icons are binary, and the bundle includes third-party license files and an empty directory marker. These are not printable source code. The full Angular lockfile and checked-in validation JavaScript **are** printed in the source pages above.
