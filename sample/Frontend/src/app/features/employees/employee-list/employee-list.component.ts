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
