# Angular project and bootstrap

## `sample/Frontend/angular.json`

Configures Angular build, assets, styles, and test targets.

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

Defines Angular dependencies and npm commands.

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

## `sample/Frontend/src/app/app.config.ts`

Registers Angular router and HTTP services.

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

Supplies application code or configuration required by this sample.

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

Maps Angular URLs to pages and guards.

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

Supplies application code or configuration required by this sample.

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

Defines the API base URL used by Angular.

```typescript
export const environment = { apiUrl: 'http://localhost:5087/api' };
```

## `sample/Frontend/src/index.html`

Hosts the Angular application root.

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

Bootstraps the Angular application.

```typescript
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
```

## `sample/Frontend/tsconfig.app.json`

Configures TypeScript compilation for this target.

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

Configures TypeScript compilation for this target.

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
