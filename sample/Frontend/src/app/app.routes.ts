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
