# Angular project and bootstrap

Complete package manifest, Angular/TypeScript configuration, entry points, routes, and app shell.

This page contains **complete file contents** for 19 files. Copy each block to the exact path shown. The code is embedded in this Markdown page and remains visible when the sample source directory is unavailable.

Return to [all source files](/source/) or read [setup instructions](/start). The [source bundle](/sample-source.zip) also includes binary icons and license files.

## `sample/Frontend/.editorconfig`

**File:** `sample/Frontend/.editorconfig` — **Use:** Configures editor, formatting, or ignored development files.

```text
# Editor configuration, see https://editorconfig.org
root = true

[*]
charset = utf-8
indent_style = space
indent_size = 2
insert_final_newline = true
trim_trailing_whitespace = true

[*.ts]
quote_type = single
ij_typescript_use_double_quotes = false

[*.md]
max_line_length = off
trim_trailing_whitespace = false
```

## `sample/Frontend/.gitignore`

**File:** `sample/Frontend/.gitignore` — **Use:** Configures editor, formatting, or ignored development files.

```text
# See https://docs.github.com/get-started/getting-started-with-git/ignoring-files for more about ignoring files.

# Compiled output
/dist
/tmp
/out-tsc
/bazel-out

# Node
/node_modules
npm-debug.log
yarn-error.log

# IDEs and editors
.idea/
.project
.classpath
.c9/
*.launch
.settings/
*.sublime-workspace

# Visual Studio Code
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json
!.vscode/mcp.json
.history/*

# Miscellaneous
/.angular/cache
.sass-cache/
/connect.lock
/coverage
/libpeerconnection.log
testem.log
/typings
__screenshots__/

# System files
.DS_Store
Thumbs.db
```

## `sample/Frontend/.prettierrc`

**File:** `sample/Frontend/.prettierrc` — **Use:** Configures editor, formatting, or ignored development files.

```text
{
  "printWidth": 100,
  "singleQuote": true,
  "overrides": [
    {
      "files": "*.html",
      "options": {
        "parser": "angular"
      }
    }
  ]
}
```

## `sample/Frontend/.vscode/extensions.json`

**File:** `sample/Frontend/.vscode/extensions.json` — **Use:** Supplies application code or configuration required by this sample.

```json
{
  // For more information, visit: https://go.microsoft.com/fwlink/?linkid=827846
  "recommendations": ["angular.ng-template"]
}
```

## `sample/Frontend/.vscode/launch.json`

**File:** `sample/Frontend/.vscode/launch.json` — **Use:** Supplies application code or configuration required by this sample.

```json
{
  // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
  "version": "0.2.0",
  "configurations": [
    {
      "name": "ng serve",
      "type": "chrome",
      "request": "launch",
      "preLaunchTask": "npm: start",
      "url": "http://localhost:4200/"
    },
    {
      "name": "ng test",
      "type": "chrome",
      "request": "launch",
      "preLaunchTask": "npm: test",
      "url": "http://localhost:9876/debug.html"
    }
  ]
}
```

## `sample/Frontend/.vscode/mcp.json`

**File:** `sample/Frontend/.vscode/mcp.json` — **Use:** Supplies application code or configuration required by this sample.

```json
{
  // For more information, visit: https://angular.dev/ai/mcp
  "servers": {
    "angular-cli": {
      "command": "npx",
      "args": ["-y", "@angular/cli", "mcp"]
    }
  }
}
```

## `sample/Frontend/.vscode/tasks.json`

**File:** `sample/Frontend/.vscode/tasks.json` — **Use:** Supplies application code or configuration required by this sample.

```json
{
  // For more information, visit: https://go.microsoft.com/fwlink/?LinkId=733558
  "version": "2.0.0",
  "tasks": [
    {
      "type": "npm",
      "script": "start",
      "isBackground": true,
      "problemMatcher": {
        "owner": "typescript",
        "pattern": "$tsc",
        "background": {
          "activeOnStart": true,
          "beginsPattern": {
            "regexp": "Changes detected"
          },
          "endsPattern": {
            "regexp": "bundle generation (complete|failed)"
          }
        }
      }
    },
    {
      "type": "npm",
      "script": "test",
      "isBackground": true,
      "problemMatcher": {
        "owner": "typescript",
        "pattern": "$tsc",
        "background": {
          "activeOnStart": true,
          "beginsPattern": {
            "regexp": "Changes detected"
          },
          "endsPattern": {
            "regexp": "bundle generation (complete|failed)"
          }
        }
      }
    }
  ]
}
```

## `sample/Frontend/angular.json`

**File:** `sample/Frontend/angular.json` — **Use:** Configures Angular build, assets, styles, and test targets.

```json
{
  "$schema": "./node_modules/@angular/cli/lib/config/schema.json",
  "version": 1,
  "cli": {
    "packageManager": "npm"
  },
  "newProjectRoot": "projects",
  "projects": {
    "employee-management-ui": {
      "projectType": "application",
      "schematics": {
        "@schematics/angular:component": {
          "style": "scss"
        }
      },
      "root": "",
      "sourceRoot": "src",
      "prefix": "app",
      "architect": {
        "build": {
          "builder": "@angular/build:application",
          "options": {
            "browser": "src/main.ts",
            "polyfills": ["zone.js"],
            "tsConfig": "tsconfig.app.json",
            "inlineStyleLanguage": "scss",
            "assets": [
              {
                "glob": "**/*",
                "input": "public"
              }
            ],
            "styles": [
              "src/styles.scss"
            ]
          },
          "configurations": {
            "production": {
              "budgets": [
                {
                  "type": "initial",
                  "maximumWarning": "600kB",
                  "maximumError": "1MB"
                },
                {
                  "type": "anyComponentStyle",
                  "maximumWarning": "4kB",
                  "maximumError": "8kB"
                }
              ],
              "outputHashing": "all"
            },
            "development": {
              "optimization": false,
              "extractLicenses": false,
              "sourceMap": true
            }
          },
          "defaultConfiguration": "production"
        },
        "serve": {
          "builder": "@angular/build:dev-server",
          "configurations": {
            "production": {
              "buildTarget": "employee-management-ui:build:production"
            },
            "development": {
              "buildTarget": "employee-management-ui:build:development"
            }
          },
          "defaultConfiguration": "development"
        },
        "test": {
          "builder": "@angular/build:unit-test"
        }
      }
    }
  }
}
```

## `sample/Frontend/package.json`

**File:** `sample/Frontend/package.json` — **Use:** Defines Angular dependencies and npm commands.

```json
{
  "name": "employee-management-ui",
  "version": "0.0.0",
  "scripts": {
    "ng": "ng",
    "start": "ng serve",
    "build": "ng build",
    "watch": "ng build --watch --configuration development",
    "test": "ng test"
  },
  "private": true,
  "packageManager": "npm@11.11.0",
  "dependencies": {
    "@angular/common": "^21.2.0",
    "@angular/compiler": "^21.2.0",
    "@angular/core": "^21.2.0",
    "@angular/forms": "^21.2.0",
    "@angular/platform-browser": "^21.2.0",
    "@angular/router": "^21.2.0",
    "bootstrap": "^5.3.8",
    "rxjs": "~7.8.0",
    "tslib": "^2.3.0",
    "zone.js": "^0.15.1"
  },
  "devDependencies": {
    "@angular/build": "^21.2.6",
    "@angular/cli": "^21.2.6",
    "@angular/compiler-cli": "^21.2.0",
    "jsdom": "^28.0.0",
    "prettier": "^3.8.1",
    "typescript": "~5.9.2",
    "vitest": "^4.0.8"
  }
}
```

## `sample/Frontend/README.md`

**File:** `sample/Frontend/README.md` — **Use:** Provides the sample project’s original README instructions.

````markdown
# EmployeeManagementUi

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 21.2.6.

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
````

## `sample/Frontend/src/app/app.config.ts`

**File:** `sample/Frontend/src/app/app.config.ts` — **Use:** Registers Angular router and HTTP services.

```typescript
import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor]))
  ]
};
```

## `sample/Frontend/src/app/app.html`

**File:** `sample/Frontend/src/app/app.html` — **Use:** Supplies application code or configuration required by this sample.

```html
@if (authService.currentUser$ | async; as user) {
  <nav class="navbar navbar-dark bg-primary shadow-sm">
    <div class="container d-flex flex-nowrap">
      <a class="navbar-brand fw-semibold" routerLink="/dashboard">Employee Manager</a>
      <div class="navbar-nav flex-row gap-2 me-auto">
        <a class="nav-link" routerLink="/dashboard" routerLinkActive="active">Dashboard</a>
        <a class="nav-link" routerLink="/employees" routerLinkActive="active">Employees</a>
      </div>
      <span class="navbar-text me-3 d-none d-md-inline">{{ user.username }} · {{ user.role }}</span>
      <button class="btn btn-outline-light btn-sm" type="button" (click)="logout()">Logout</button>
    </div>
  </nav>
}

<app-notification />
<router-outlet />
```

## `sample/Frontend/src/app/app.routes.ts`

**File:** `sample/Frontend/src/app/app.routes.ts` — **Use:** Maps Angular URLs to pages and guards.

```typescript
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent) },
  { path: 'dashboard', canActivate: [authGuard], loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
  { path: 'employees', canActivate: [authGuard], loadComponent: () => import('./features/employees/employee-list/employee-list.component').then(m => m.EmployeeListComponent) },
  { path: 'employees/new', canActivate: [adminGuard], loadComponent: () => import('./features/employees/employee-form/employee-form.component').then(m => m.EmployeeFormComponent) },
  { path: 'employees/:id/edit', canActivate: [adminGuard], loadComponent: () => import('./features/employees/employee-form/employee-form.component').then(m => m.EmployeeFormComponent) },
  { path: 'employees/:id', canActivate: [authGuard], loadComponent: () => import('./features/employees/employee-detail/employee-detail.component').then(m => m.EmployeeDetailComponent) },
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: '**', redirectTo: 'dashboard' }
];
```

## `sample/Frontend/src/app/app.ts`

**File:** `sample/Frontend/src/app/app.ts` — **Use:** Supplies application code or configuration required by this sample.

```typescript
import { AsyncPipe } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { NotificationComponent } from './shared/notification/notification.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, AsyncPipe, NotificationComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  constructor(public authService: AuthService, private router: Router) {}

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
```

## `sample/Frontend/src/environments/environment.ts`

**File:** `sample/Frontend/src/environments/environment.ts` — **Use:** Defines the API base URL used by Angular.

```typescript
export const environment = { apiUrl: 'http://localhost:5087/api' };
```

## `sample/Frontend/src/index.html`

**File:** `sample/Frontend/src/index.html` — **Use:** Hosts the Angular application root.

```html
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <title>EmployeeManagementUi</title>
  <base href="/">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="icon" type="image/x-icon" href="favicon.ico">
</head>
<body>
  <app-root></app-root>
</body>
</html>
```

## `sample/Frontend/src/main.ts`

**File:** `sample/Frontend/src/main.ts` — **Use:** Bootstraps the Angular application.

```typescript
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
```

## `sample/Frontend/tsconfig.app.json`

**File:** `sample/Frontend/tsconfig.app.json` — **Use:** Configures TypeScript compilation for this target.

```json
/* To learn more about Typescript configuration file: https://www.typescriptlang.org/docs/handbook/tsconfig-json.html. */
/* To learn more about Angular compiler options: https://angular.dev/reference/configs/angular-compiler-options. */
{
  "extends": "./tsconfig.json",
  "compilerOptions": {
    "outDir": "./out-tsc/app",
    "types": []
  },
  "include": [
    "src/**/*.ts"
  ],
  "exclude": [
    "src/**/*.spec.ts"
  ]
}
```

## `sample/Frontend/tsconfig.json`

**File:** `sample/Frontend/tsconfig.json` — **Use:** Configures TypeScript compilation for this target.

```json
/* To learn more about Typescript configuration file: https://www.typescriptlang.org/docs/handbook/tsconfig-json.html. */
/* To learn more about Angular compiler options: https://angular.dev/reference/configs/angular-compiler-options. */
{
  "compileOnSave": false,
  "compilerOptions": {
    "strict": true,
    "noImplicitOverride": true,
    "noPropertyAccessFromIndexSignature": true,
    "noImplicitReturns": true,
    "noFallthroughCasesInSwitch": true,
    "skipLibCheck": true,
    "isolatedModules": true,
    "experimentalDecorators": true,
    "importHelpers": true,
    "target": "ES2022",
    "module": "preserve"
  },
  "angularCompilerOptions": {
    "enableI18nLegacyMessageIdFormat": false,
    "strictInjectionParameters": true,
    "strictInputAccessModifiers": true,
    "strictTemplates": true
  },
  "files": [],
  "references": [
    {
      "path": "./tsconfig.app.json"
    },
    {
      "path": "./tsconfig.spec.json"
    }
  ]
}
```
