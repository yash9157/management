# Angular models and core services

## `sample/Frontend/src/app/core/guards/admin.guard.ts`

Controls navigation to protected Angular routes.

```typescript
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if (!authService.isAuthenticated) return router.createUrlTree(['/login']);
  return authService.isAdmin ? true : router.createUrlTree(['/employees']);
};
```

## `sample/Frontend/src/app/core/guards/auth.guard.ts`

Controls navigation to protected Angular routes.

```typescript
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  return authService.isAuthenticated ? true : inject(Router).createUrlTree(['/login']);
};
```

## `sample/Frontend/src/app/core/interceptors/auth.interceptor.ts`

Adds authentication to requests and handles unauthenticated responses.

```typescript
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const token = authService.accessToken;
  const authorizedRequest = token ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : request;
  return next(authorizedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !request.url.endsWith('/auth/login')) {
        authService.logout();
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};
```

## `sample/Frontend/src/app/core/models/auth.model.ts`

Defines typed data exchanged with the API.

```typescript
export interface LoginResponse { accessToken: string; expiresAtUtc: string; username: string; role: string; }

export interface RegisterRequest { username: string; password: string; confirmPassword: string; }
export interface RegisterResponse { username: string; role: string; }
```

## `sample/Frontend/src/app/core/models/employee.model.ts`

Defines typed data exchanged with the API.

```typescript
export interface LookupItem { id: number; name: string; }

export interface Employee {
  employeeId: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  salary: number;
  dateOfBirth: string;
  joiningDate: string;
  gender: string;
  employmentType: string;
  isActive: boolean;
  departmentId: number;
  departmentName: string;
  skills: LookupItem[];
  profileImageUrl: string | null;
}

export interface EmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  salary: number;
  dateOfBirth: string;
  joiningDate: string;
  gender: string;
  employmentType: string;
  isActive: boolean;
  departmentId: number;
  skillIds: number[];
}

export interface EmployeeQuery {
  search?: string;
  departmentId?: number;
  skillId?: number;
  isActive?: boolean;
  employmentType?: string;
  sortBy: string;
  sortDirection: 'asc' | 'desc';
  page: number;
  pageSize: number;
}

export interface PagedResult<T> { items: T[]; page: number; pageSize: number; totalCount: number; totalPages: number; }
export interface DashboardSummary { totalEmployees: number; activeEmployees: number; departmentCount: number; skillCount: number; }
export interface DepartmentEmployeeCount { departmentId: number; departmentName: string; employeeCount: number; }
```

## `sample/Frontend/src/app/core/services/auth.service.ts`

Supplies application code or configuration required by this sample.

```typescript
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginResponse, RegisterRequest, RegisterResponse } from '../models/auth.model';

const storageKey = 'employee-management-session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly currentUserSubject = new BehaviorSubject<LoginResponse | null>(this.readSession());
  readonly currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, { username, password }).pipe(
      tap(response => {
        localStorage.setItem(storageKey, JSON.stringify(response));
        this.currentUserSubject.next(response);
      })
    );
  }

  register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${environment.apiUrl}/auth/register`, request);
  }

  logout(): void {
    localStorage.removeItem(storageKey);
    this.currentUserSubject.next(null);
  }

  get accessToken(): string | null { return this.currentUserSubject.value?.accessToken ?? null; }
  get isAdmin(): boolean { return this.isAuthenticated && this.currentUserSubject.value?.role === 'Admin'; }

  get isAuthenticated(): boolean {
    const session = this.currentUserSubject.value;
    if (!session || new Date(session.expiresAtUtc).getTime() <= Date.now()) {
      if (session) this.logout();
      return false;
    }
    return true;
  }

  private readSession(): LoginResponse | null {
    const savedSession = localStorage.getItem(storageKey);
    if (!savedSession) return null;
    try {
      const session = JSON.parse(savedSession) as LoginResponse;
      if (new Date(session.expiresAtUtc).getTime() <= Date.now()) {
        localStorage.removeItem(storageKey);
        return null;
      }
      return session;
    } catch {
      localStorage.removeItem(storageKey);
      return null;
    }
  }
}
```

## `sample/Frontend/src/app/core/services/dashboard.service.ts`

Supplies application code or configuration required by this sample.

```typescript
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardSummary, DepartmentEmployeeCount } from '../models/employee.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  constructor(private http: HttpClient) {}
  getSummary(): Observable<DashboardSummary> { return this.http.get<DashboardSummary>(`${environment.apiUrl}/dashboard`); }
  getDepartmentCounts(): Observable<DepartmentEmployeeCount[]> { return this.http.get<DepartmentEmployeeCount[]>(`${environment.apiUrl}/dashboard/department-counts`); }
}
```

## `sample/Frontend/src/app/core/services/employee.service.ts`

Supplies application code or configuration required by this sample.

```typescript
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Employee, EmployeeQuery, EmployeeRequest, PagedResult } from '../models/employee.model';

@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private readonly url = `${environment.apiUrl}/employees`;

  constructor(private http: HttpClient) {}

  getEmployees(query: EmployeeQuery): Observable<PagedResult<Employee>> {
    let params = new HttpParams()
      .set('page', query.page).set('pageSize', query.pageSize)
      .set('sortBy', query.sortBy).set('sortDirection', query.sortDirection);
    if (query.search) params = params.set('search', query.search);
    if (query.departmentId) params = params.set('departmentId', query.departmentId);
    if (query.skillId) params = params.set('skillId', query.skillId);
    if (query.isActive !== undefined) params = params.set('isActive', query.isActive);
    if (query.employmentType) params = params.set('employmentType', query.employmentType);
    return this.http.get<PagedResult<Employee>>(this.url, { params });
  }

  getEmployee(id: number): Observable<Employee> { return this.http.get<Employee>(`${this.url}/${id}`); }
  createEmployee(request: EmployeeRequest): Observable<Employee> { return this.http.post<Employee>(this.url, request); }
  updateEmployee(id: number, request: EmployeeRequest): Observable<Employee> { return this.http.put<Employee>(`${this.url}/${id}`, request); }
  deleteEmployee(id: number): Observable<void> { return this.http.delete<void>(`${this.url}/${id}`); }

  uploadProfileImage(id: number, file: File): Observable<{ profileImageUrl: string }> {
    const data = new FormData();
    data.append('file', file);
    return this.http.post<{ profileImageUrl: string }>(`${this.url}/${id}/profile-image`, data);
  }
}
```

## `sample/Frontend/src/app/core/services/lookup.service.ts`

Supplies application code or configuration required by this sample.

```typescript
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LookupItem } from '../models/employee.model';

@Injectable({ providedIn: 'root' })
export class LookupService {
  constructor(private http: HttpClient) {}
  getDepartments(): Observable<LookupItem[]> { return this.http.get<LookupItem[]>(`${environment.apiUrl}/departments/lookup`); }
  getSkills(): Observable<LookupItem[]> { return this.http.get<LookupItem[]>(`${environment.apiUrl}/skills/lookup`); }
}
```

## `sample/Frontend/src/app/core/services/notification.service.ts`

Supplies application code or configuration required by this sample.

```typescript
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface NotificationMessage { text: string; type: 'success' | 'danger'; }

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly messageSubject = new BehaviorSubject<NotificationMessage | null>(null);
  readonly message$ = this.messageSubject.asObservable();
  success(text: string): void { this.show(text, 'success'); }
  error(text: string): void { this.show(text, 'danger'); }
  clear(): void { this.messageSubject.next(null); }
  private show(text: string, type: 'success' | 'danger'): void {
    this.messageSubject.next({ text, type });
    window.setTimeout(() => this.clear(), 4000);
  }
}
```
