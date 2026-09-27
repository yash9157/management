# Angular employee screens

## `sample/Frontend/src/app/features/employees/employee-detail/employee-detail.component.html`

Renders the markup for this Angular screen.

```html
<main class="container py-4">
  <a class="text-decoration-none" routerLink="/employees">← Back to employees</a>
  @if (loading) {
    <div class="text-center py-5"><div class="spinner-border text-primary"></div></div>
  } @else if (errorMessage) {
    <div class="alert alert-danger mt-4">{{ errorMessage }}</div>
  } @else if (employee) {
    <section class="page-card p-4 mt-3">
      <div class="d-flex flex-column flex-sm-row align-items-sm-center gap-4 border-bottom pb-4 mb-4">
        @if (profileImageUrl) { <img class="profile-image rounded-circle border" [src]="profileImageUrl" alt="Profile image"> }
        @else { <div class="initials rounded-circle bg-primary-subtle text-primary d-flex align-items-center justify-content-center fw-bold">{{ employee.firstName[0] }}{{ employee.lastName[0] }}</div> }
        <div class="flex-grow-1"><div class="d-flex align-items-center gap-2"><h1 class="h3 mb-0">{{ employee.firstName }} {{ employee.lastName }}</h1><span class="badge" [class.text-bg-success]="employee.isActive" [class.text-bg-secondary]="!employee.isActive">{{ employee.isActive ? 'Active' : 'Inactive' }}</span></div><p class="text-secondary mb-0 mt-1">{{ employee.departmentName }} · {{ employee.employmentType }}</p></div>
        @if (authService.isAdmin) { <a class="btn btn-primary" [routerLink]="['/employees', employee.employeeId, 'edit']">Edit employee</a> }
      </div>
      <div class="row g-4">
        <div class="col-md-6 col-lg-4"><div class="text-secondary small">Email</div><div>{{ employee.email }}</div></div>
        <div class="col-md-6 col-lg-4"><div class="text-secondary small">Phone</div><div>{{ employee.phone }}</div></div>
        <div class="col-md-6 col-lg-4"><div class="text-secondary small">Salary</div><div>{{ employee.salary | currency:'INR' }}</div></div>
        <div class="col-md-6 col-lg-4"><div class="text-secondary small">Date of birth</div><div>{{ employee.dateOfBirth | date:'longDate' }}</div></div>
        <div class="col-md-6 col-lg-4"><div class="text-secondary small">Joining date</div><div>{{ employee.joiningDate | date:'longDate' }}</div></div>
        <div class="col-md-6 col-lg-4"><div class="text-secondary small">Gender</div><div>{{ employee.gender }}</div></div>
        <div class="col-12"><div class="text-secondary small mb-2">Skills</div>@if (employee.skills.length) { @for (skill of employee.skills; track skill.id) { <span class="badge text-bg-light border me-2 mb-2">{{ skill.name }}</span> } } @else { <span class="text-secondary">No skills selected</span> }</div>
      </div>
    </section>
  }
</main>
```

## `sample/Frontend/src/app/features/employees/employee-detail/employee-detail.component.ts`

Implements the behavior for this Angular screen or shared control.

```typescript
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Employee } from '../../../core/models/employee.model';
import { EmployeeService } from '../../../core/services/employee.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-employee-detail',
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './employee-detail.component.html',
  styles: `.profile-image { width: 7rem; height: 7rem; object-fit: cover; } .initials { width: 7rem; height: 7rem; font-size: 2rem; }`
})
export class EmployeeDetailComponent implements OnInit {
  employee: Employee | null = null;
  loading = true;
  errorMessage = '';

  constructor(private route: ActivatedRoute, private employeeService: EmployeeService, public authService: AuthService) {}

  ngOnInit(): void {
    const employeeId = Number(this.route.snapshot.paramMap.get('id'));
    this.employeeService.getEmployee(employeeId).pipe(finalize(() => this.loading = false)).subscribe({
      next: employee => this.employee = employee,
      error: error => this.errorMessage = error.status === 404 ? 'Employee not found.' : 'Employee details could not be loaded.'
    });
  }

  get profileImageUrl(): string | null {
    return this.employee?.profileImageUrl ? environment.apiUrl.replace(/\/api$/, '') + this.employee.profileImageUrl : null;
  }
}
```

## `sample/Frontend/src/app/features/employees/employee-form/employee-form.component.html`

Renders the markup for this Angular screen.

```html
<main class="container py-4">
  <div class="mb-4"><a class="text-decoration-none" routerLink="/employees">← Back to employees</a><h1 class="h3 mt-2 mb-1">{{ employeeId ? 'Edit employee' : 'Add employee' }}</h1><p class="text-secondary mb-0">Fields marked with * are required.</p></div>
  @if (loading) {
    <div class="text-center py-5"><div class="spinner-border text-primary"></div></div>
  } @else {
    <form class="page-card p-3 p-lg-4" [formGroup]="form" (ngSubmit)="save()" novalidate>
      @if (errorMessage) { <div class="alert alert-danger">{{ errorMessage }}</div> }
      <div class="row g-3">
        <div class="col-md-6"><label class="form-label required-label" for="firstName">First name</label><input class="form-control" id="firstName" formControlName="firstName" [class.is-invalid]="form.controls.firstName.touched && form.controls.firstName.invalid"><div class="invalid-feedback">First name is required (maximum 80 characters).</div></div>
        <div class="col-md-6"><label class="form-label required-label" for="lastName">Last name</label><input class="form-control" id="lastName" formControlName="lastName" [class.is-invalid]="form.controls.lastName.touched && form.controls.lastName.invalid"><div class="invalid-feedback">Last name is required (maximum 80 characters).</div></div>
        <div class="col-md-6"><label class="form-label required-label" for="email">Email</label><input class="form-control" id="email" type="email" formControlName="email" [class.is-invalid]="form.controls.email.touched && form.controls.email.invalid"><div class="invalid-feedback">Enter a valid email address.</div></div>
        <div class="col-md-6"><label class="form-label required-label" for="phone">Phone</label><input class="form-control" id="phone" formControlName="phone" [class.is-invalid]="form.controls.phone.touched && form.controls.phone.invalid"><div class="invalid-feedback">Enter a valid phone number.</div></div>
        <div class="col-md-4"><label class="form-label required-label" for="salary">Salary</label><input class="form-control" id="salary" type="number" min="0" step="0.01" formControlName="salary" [class.is-invalid]="form.controls.salary.touched && form.controls.salary.invalid"><div class="invalid-feedback">Salary must be zero or greater.</div></div>
        <div class="col-md-4"><label class="form-label required-label" for="dateOfBirth">Date of birth</label><input class="form-control" id="dateOfBirth" type="date" formControlName="dateOfBirth" [class.is-invalid]="form.controls.dateOfBirth.touched && form.controls.dateOfBirth.invalid"><div class="invalid-feedback">Date of birth is required.</div></div>
        <div class="col-md-4"><label class="form-label required-label" for="joiningDate">Joining date</label><input class="form-control" id="joiningDate" type="date" formControlName="joiningDate" [class.is-invalid]="form.controls.joiningDate.touched && form.controls.joiningDate.invalid"><div class="invalid-feedback">Joining date is required.</div></div>
        <div class="col-md-4"><label class="form-label required-label" for="gender">Gender</label><select class="form-select" id="gender" formControlName="gender" [class.is-invalid]="form.controls.gender.touched && form.controls.gender.invalid"><option value="">Select gender</option>@for (item of genders; track item) { <option [value]="item">{{ item }}</option> }</select><div class="invalid-feedback">Select a gender.</div></div>
        <div class="col-md-4"><label class="form-label required-label" for="employmentType">Employment type</label><select class="form-select" id="employmentType" formControlName="employmentType" [class.is-invalid]="form.controls.employmentType.touched && form.controls.employmentType.invalid"><option value="">Select type</option>@for (item of employmentTypes; track item) { <option [value]="item">{{ item }}</option> }</select><div class="invalid-feedback">Select an employment type.</div></div>
        <div class="col-md-4"><label class="form-label required-label" for="departmentId">Department</label><select class="form-select" id="departmentId" formControlName="departmentId" [class.is-invalid]="form.controls.departmentId.touched && form.controls.departmentId.invalid"><option [ngValue]="0">Select department</option>@for (item of departments; track item.id) { <option [ngValue]="item.id">{{ item.name }}</option> }</select><div class="invalid-feedback">Select a department.</div></div>
        <div class="col-md-8"><label class="form-label" for="skillIds">Skills</label><select class="form-select" id="skillIds" formControlName="skillIds" multiple size="5">@for (item of skills; track item.id) { <option [ngValue]="item.id">{{ item.name }}</option> }</select><div class="form-text">Hold Ctrl (Windows) or Command (macOS) to select several skills.</div></div>
        <div class="col-md-4"><label class="form-label" for="profileImage">Profile image</label><input class="form-control" id="profileImage" type="file" accept="image/png,image/jpeg" (change)="selectImage($event)"><div class="form-text">Optional JPG or PNG, maximum 2 MB.</div><div class="form-check mt-3"><input class="form-check-input" id="isActive" type="checkbox" formControlName="isActive"><label class="form-check-label" for="isActive">Active employee</label></div></div>
      </div>
      <div class="d-flex justify-content-end gap-2 mt-4 pt-3 border-top"><a class="btn btn-outline-secondary" routerLink="/employees">Cancel</a><button class="btn btn-primary" type="submit" [disabled]="saving">@if (saving) { <span class="spinner-border spinner-border-sm me-2"></span> }Save employee</button></div>
    </form>
  }
</main>
```

## `sample/Frontend/src/app/features/employees/employee-form/employee-form.component.ts`

Implements the behavior for this Angular screen or shared control.

```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, forkJoin, map, of, switchMap } from 'rxjs';
import { EmployeeRequest, LookupItem } from '../../../core/models/employee.model';
import { EmployeeService } from '../../../core/services/employee.service';
import { LookupService } from '../../../core/services/lookup.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-employee-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './employee-form.component.html'
})
export class EmployeeFormComponent implements OnInit {
  readonly genders = ['Female', 'Male', 'Non-binary', 'Prefer not to say'];
  readonly employmentTypes = ['Full-time', 'Part-time', 'Contract', 'Intern'];
  readonly form;
  departments: LookupItem[] = [];
  skills: LookupItem[] = [];
  employeeId: number | null = null;
  loading = true;
  saving = false;
  errorMessage = '';
  profileFile: File | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private employeeService: EmployeeService,
    private lookupService: LookupService,
    private notifications: NotificationService
  ) {
    this.form = this.formBuilder.nonNullable.group({
      firstName: ['', [Validators.required, Validators.maxLength(80)]],
      lastName: ['', [Validators.required, Validators.maxLength(80)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
      phone: ['', [Validators.required, Validators.pattern(/^[0-9+() -]{7,25}$/)]],
      salary: [0, [Validators.required, Validators.min(0)]],
      dateOfBirth: ['', Validators.required],
      joiningDate: ['', Validators.required],
      gender: ['', Validators.required],
      employmentType: ['', Validators.required],
      isActive: [true],
      departmentId: [0, [Validators.required, Validators.min(1)]],
      skillIds: [[] as number[]]
    });
  }

  ngOnInit(): void {
    const routeId = this.route.snapshot.paramMap.get('id');
    this.employeeId = routeId ? Number(routeId) : null;
    const employeeRequest = this.employeeId ? this.employeeService.getEmployee(this.employeeId) : of(null);
    forkJoin({
      departments: this.lookupService.getDepartments(),
      skills: this.lookupService.getSkills(),
      employee: employeeRequest
    }).pipe(finalize(() => this.loading = false)).subscribe({
      next: data => {
        this.departments = data.departments;
        this.skills = data.skills;
        if (data.employee) {
          this.form.patchValue({
            ...data.employee,
            dateOfBirth: data.employee.dateOfBirth.substring(0, 10),
            joiningDate: data.employee.joiningDate.substring(0, 10),
            skillIds: data.employee.skills.map(skill => skill.id)
          });
        }
      },
      error: () => this.errorMessage = 'The employee form could not be loaded.'
    });
  }

  selectImage(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0] ?? null;
    this.errorMessage = '';
    if (file && (!['image/jpeg', 'image/png'].includes(file.type) || file.size > 2_000_000)) {
      this.profileFile = null;
      this.errorMessage = 'Profile image must be a JPG or PNG smaller than 2 MB.';
      return;
    }
    this.profileFile = file;
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    const request: EmployeeRequest = { ...value };
    this.saving = true;
    this.errorMessage = '';
    const saveRequest = this.employeeId
      ? this.employeeService.updateEmployee(this.employeeId, request)
      : this.employeeService.createEmployee(request);

    saveRequest.pipe(
      switchMap(employee => this.profileFile
        ? this.employeeService.uploadProfileImage(employee.employeeId, this.profileFile).pipe(map(() => employee))
        : of(employee)),
      finalize(() => this.saving = false)
    ).subscribe({
      next: employee => {
        this.notifications.success(`Employee ${this.employeeId ? 'updated' : 'created'} successfully.`);
        this.router.navigate(['/employees', employee.employeeId]);
      },
      error: error => this.errorMessage = this.getApiError(error)
    });
  }

  private getApiError(error: any): string {
    if (error.error?.errors) {
      const messages = Object.values(error.error.errors).flat() as string[];
      return messages[0] ?? 'Please correct the highlighted fields.';
    }
    return error.error?.detail ?? error.error?.message ?? 'The employee could not be saved.';
  }
}
```

## `sample/Frontend/src/app/features/employees/employee-list/employee-list.component.html`

Renders the markup for this Angular screen.

```html
<main class="container py-4">
  <div class="d-flex justify-content-between align-items-center mb-4">
    <div><h1 class="h3 mb-1">Employees</h1><p class="text-secondary mb-0">Search, filter and manage employee records.</p></div>
    @if (authService.isAdmin) { <a class="btn btn-primary" routerLink="/employees/new">Add employee</a> }
  </div>

  <section class="page-card p-3 p-lg-4 mb-3">
    <div class="row g-3">
      <div class="col-12 col-lg-4">
        <label class="form-label" for="search">Search</label>
        <input class="form-control" id="search" [formControl]="searchControl" placeholder="Name or email">
      </div>
      <div class="col-6 col-lg-2">
        <label class="form-label" for="department">Department</label>
        <select class="form-select" id="department" [(ngModel)]="departmentFilter" (ngModelChange)="filtersChanged()">
          <option [ngValue]="undefined">All</option>
          @for (item of departments; track item.id) { <option [ngValue]="item.id">{{ item.name }}</option> }
        </select>
      </div>
      <div class="col-6 col-lg-2">
        <label class="form-label" for="skill">Skill</label>
        <select class="form-select" id="skill" [(ngModel)]="skillFilter" (ngModelChange)="filtersChanged()">
          <option [ngValue]="undefined">All</option>
          @for (item of skills; track item.id) { <option [ngValue]="item.id">{{ item.name }}</option> }
        </select>
      </div>
      <div class="col-6 col-lg-2">
        <label class="form-label" for="active">Status</label>
        <select class="form-select" id="active" [(ngModel)]="activeFilter" (ngModelChange)="filtersChanged()">
          <option value="">All</option><option value="true">Active</option><option value="false">Inactive</option>
        </select>
      </div>
      <div class="col-6 col-lg-2">
        <label class="form-label" for="employmentType">Type</label>
        <select class="form-select" id="employmentType" [(ngModel)]="employmentTypeFilter" (ngModelChange)="filtersChanged()">
          <option value="">All</option><option>Full-time</option><option>Part-time</option><option>Contract</option><option>Intern</option>
        </select>
      </div>
    </div>
  </section>

  <section class="page-card overflow-hidden">
    @if (loading) {
      <div class="text-center py-5"><div class="spinner-border text-primary"></div><p class="text-secondary mt-2 mb-0">Loading employees…</p></div>
    } @else if (errorMessage) {
      <div class="p-4"><div class="alert alert-danger mb-0">{{ errorMessage }}</div></div>
    } @else if (employees.length === 0) {
      <div class="text-center py-5"><h2 class="h5">No employees found</h2><p class="text-secondary">Try changing your filters.</p></div>
    } @else {
      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light"><tr>
            <th><button class="btn btn-link p-0" type="button" (click)="sort('name')">Name {{ sortBy === 'name' ? (sortDirection === 'asc' ? '↑' : '↓') : '' }}</button></th>
            <th><button class="btn btn-link p-0" type="button" (click)="sort('email')">Email {{ sortBy === 'email' ? (sortDirection === 'asc' ? '↑' : '↓') : '' }}</button></th>
            <th><button class="btn btn-link p-0" type="button" (click)="sort('department')">Department {{ sortBy === 'department' ? (sortDirection === 'asc' ? '↑' : '↓') : '' }}</button></th>
            <th>Type</th><th><button class="btn btn-link p-0" type="button" (click)="sort('joiningDate')">Joined</button></th><th>Status</th><th class="text-end">Actions</th>
          </tr></thead>
          <tbody>
            @for (employee of employees; track employee.employeeId) {
              <tr>
                <td><div class="fw-semibold">{{ employee.firstName }} {{ employee.lastName }}</div><small class="text-secondary">{{ employee.salary | currency:'INR':'symbol':'1.0-0' }}</small></td>
                <td>{{ employee.email }}</td><td>{{ employee.departmentName }}</td><td>{{ employee.employmentType }}</td><td>{{ employee.joiningDate | date:'mediumDate' }}</td>
                <td><span class="badge" [class.text-bg-success]="employee.isActive" [class.text-bg-secondary]="!employee.isActive">{{ employee.isActive ? 'Active' : 'Inactive' }}</span></td>
                <td class="text-end text-nowrap">
                  <a class="btn btn-sm btn-outline-primary me-1" [routerLink]="['/employees', employee.employeeId]">View</a>
                  @if (authService.isAdmin) {
                    <a class="btn btn-sm btn-outline-secondary me-1" [routerLink]="['/employees', employee.employeeId, 'edit']">Edit</a>
                    <button class="btn btn-sm btn-outline-danger" type="button" (click)="confirmDelete(employee)">Delete</button>
                  }
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
      <div class="d-flex flex-wrap justify-content-between align-items-center gap-3 p-3 border-top">
        <div class="d-flex align-items-center gap-2 text-secondary"><span>{{ totalCount }} total</span><select class="form-select form-select-sm w-auto" [(ngModel)]="pageSize" (ngModelChange)="changePageSize()"><option [ngValue]="5">5</option><option [ngValue]="10">10</option><option [ngValue]="25">25</option></select></div>
        <app-pagination [page]="page" [totalPages]="totalPages" (pageChange)="changePage($event)" />
      </div>
    }
  </section>
</main>

<app-confirm-dialog
  [open]="employeeToDelete !== null"
  title="Delete employee"
  [message]="'Delete ' + (employeeToDelete?.firstName ?? '') + ' ' + (employeeToDelete?.lastName ?? '') + '? This cannot be undone.'"
  (confirmed)="deleteEmployee()"
  (cancelled)="employeeToDelete = null" />
```

## `sample/Frontend/src/app/features/employees/employee-list/employee-list.component.ts`

Implements the behavior for this Angular screen or shared control.

```typescript
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, finalize, Subject, takeUntil } from 'rxjs';
import { Employee, EmployeeQuery, LookupItem } from '../../../core/models/employee.model';
import { EmployeeService } from '../../../core/services/employee.service';
import { LookupService } from '../../../core/services/lookup.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { PaginationComponent } from '../../../shared/pagination/pagination.component';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-employee-list',
  imports: [CurrencyPipe, DatePipe, FormsModule, ReactiveFormsModule, RouterLink, ConfirmDialogComponent, PaginationComponent],
  templateUrl: './employee-list.component.html'
})
export class EmployeeListComponent implements OnInit, OnDestroy {
  readonly searchControl = new FormControl('', { nonNullable: true });
  departments: LookupItem[] = [];
  skills: LookupItem[] = [];
  employees: Employee[] = [];
  loading = false;
  errorMessage = '';
  departmentFilter?: number;
  skillFilter?: number;
  activeFilter = '';
  employmentTypeFilter = '';
  sortBy = 'name';
  sortDirection: 'asc' | 'desc' = 'asc';
  page = 1;
  pageSize = Number(localStorage.getItem('employee-page-size')) || 10;
  totalCount = 0;
  totalPages = 0;
  employeeToDelete: Employee | null = null;
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private employeeService: EmployeeService,
    private lookupService: LookupService,
    private notifications: NotificationService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.lookupService.getDepartments().subscribe(items => this.departments = items);
    this.lookupService.getSkills().subscribe(items => this.skills = items);
    this.searchControl.valueChanges.pipe(debounceTime(350), distinctUntilChanged(), takeUntil(this.destroyed$))
      .subscribe(() => { this.page = 1; this.loadEmployees(); });
    this.loadEmployees();
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  loadEmployees(): void {
    this.loading = true;
    this.errorMessage = '';
    const query: EmployeeQuery = {
      search: this.searchControl.value.trim() || undefined,
      departmentId: this.departmentFilter || undefined,
      skillId: this.skillFilter || undefined,
      isActive: this.activeFilter === '' ? undefined : this.activeFilter === 'true',
      employmentType: this.employmentTypeFilter || undefined,
      sortBy: this.sortBy,
      sortDirection: this.sortDirection,
      page: this.page,
      pageSize: this.pageSize
    };
    this.employeeService.getEmployees(query).pipe(finalize(() => this.loading = false)).subscribe({
      next: result => {
        this.employees = result.items;
        this.totalCount = result.totalCount;
        this.totalPages = result.totalPages;
      },
      error: () => this.errorMessage = 'Employees could not be loaded. Please try again.'
    });
  }

  filtersChanged(): void { this.page = 1; this.loadEmployees(); }

  changePage(page: number): void { this.page = page; this.loadEmployees(); }

  changePageSize(): void {
    localStorage.setItem('employee-page-size', this.pageSize.toString());
    this.page = 1;
    this.loadEmployees();
  }

  sort(field: string): void {
    if (this.sortBy === field) this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    else { this.sortBy = field; this.sortDirection = 'asc'; }
    this.page = 1;
    this.loadEmployees();
  }

  confirmDelete(employee: Employee): void { this.employeeToDelete = employee; }

  deleteEmployee(): void {
    if (!this.employeeToDelete) return;
    this.employeeService.deleteEmployee(this.employeeToDelete.employeeId).subscribe({
      next: () => {
        this.notifications.success('Employee deleted successfully.');
        this.employeeToDelete = null;
        this.loadEmployees();
      },
      error: () => this.notifications.error('The employee could not be deleted.')
    });
  }
}
```

## `sample/Frontend/src/app/shared/confirm-dialog/confirm-dialog.component.ts`

Implements the behavior for this Angular screen or shared control.

```typescript
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  template: `
    @if (open) {
      <div class="modal d-block" tabindex="-1" role="dialog" aria-modal="true">
        <div class="modal-dialog modal-dialog-centered">
          <div class="modal-content shadow">
            <div class="modal-header"><h2 class="modal-title fs-5">{{ title }}</h2></div>
            <div class="modal-body"><p class="mb-0">{{ message }}</p></div>
            <div class="modal-footer">
              <button class="btn btn-outline-secondary" type="button" (click)="cancelled.emit()">Cancel</button>
              <button class="btn btn-danger" type="button" (click)="confirmed.emit()">Delete</button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop show"></div>
    }
  `
})
export class ConfirmDialogComponent {
  @Input() open = false;
  @Input() title = 'Confirm action';
  @Input() message = 'Are you sure?';
  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();
}
```

## `sample/Frontend/src/app/shared/notification/notification.component.ts`

Implements the behavior for this Angular screen or shared control.

```typescript
import { AsyncPipe } from '@angular/common';
import { Component } from '@angular/core';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-notification',
  imports: [AsyncPipe],
  template: `
    @if (notificationService.message$ | async; as message) {
      <div class="notification alert alert-{{ message.type }} alert-dismissible shadow" role="alert">
        {{ message.text }}
        <button type="button" class="btn-close" aria-label="Close" (click)="notificationService.clear()"></button>
      </div>
    }
  `,
  styles: `.notification { position: fixed; top: 4.5rem; right: 1rem; z-index: 1100; min-width: 20rem; max-width: calc(100vw - 2rem); }`
})
export class NotificationComponent {
  constructor(public notificationService: NotificationService) {}
}
```

## `sample/Frontend/src/app/shared/pagination/pagination.component.ts`

Implements the behavior for this Angular screen or shared control.

```typescript
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  template: `
    @if (totalPages > 1) {
      <nav aria-label="Employee pages">
        <ul class="pagination mb-0">
          <li class="page-item" [class.disabled]="page === 1">
            <button class="page-link" type="button" (click)="changePage(page - 1)">Previous</button>
          </li>
          @for (pageNumber of visiblePages; track pageNumber) {
            <li class="page-item" [class.active]="pageNumber === page">
              <button class="page-link" type="button" (click)="changePage(pageNumber)">{{ pageNumber }}</button>
            </li>
          }
          <li class="page-item" [class.disabled]="page === totalPages">
            <button class="page-link" type="button" (click)="changePage(page + 1)">Next</button>
          </li>
        </ul>
      </nav>
    }
  `
})
export class PaginationComponent {
  @Input() page = 1;
  @Input() totalPages = 0;
  @Output() pageChange = new EventEmitter<number>();

  get visiblePages(): number[] {
    const start = Math.max(1, Math.min(this.page - 2, this.totalPages - 4));
    const end = Math.min(this.totalPages, start + 4);
    return Array.from({ length: end - start + 1 }, (_, index) => start + index);
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.page) this.pageChange.emit(page);
  }
}
```
