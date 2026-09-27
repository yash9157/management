# Other Angular files

Complete remaining Angular component, template, and application styles.

This page contains **complete file contents** for 4 files. Copy each block to the exact path shown. The code is embedded in this Markdown page and remains visible when the sample source directory is unavailable.

Return to [all source files](/source/) or read [setup instructions](/start). The [source bundle](/sample-source.zip) also includes binary icons and license files.

## `sample/Frontend/src/app/app.scss`

**File:** `sample/Frontend/src/app/app.scss` — **Use:** Styles this page or application.

```scss
.navbar-brand { letter-spacing: .01em; }
```

## `sample/Frontend/src/app/features/dashboard/dashboard.component.html`

**File:** `sample/Frontend/src/app/features/dashboard/dashboard.component.html` — **Use:** Renders the markup for this Angular screen.

```html
<main class="container py-4">
  <div class="d-flex justify-content-between align-items-center mb-4">
    <div><h1 class="h3 mb-1">Dashboard</h1><p class="text-secondary mb-0">A quick view of your workforce.</p></div>
    @if (authService.isAdmin) { <a class="btn btn-primary" routerLink="/employees/new">Add employee</a> }
  </div>
  @if (loading) {
    <div class="text-center py-5"><div class="spinner-border text-primary"></div><p class="text-secondary mt-2">Loading dashboard…</p></div>
  } @else if (errorMessage) {
    <div class="alert alert-danger">{{ errorMessage }}</div>
  } @else if (summary) {
    <div class="row g-3 mb-4">
      <div class="col-6 col-lg-3"><div class="page-card p-4"><div class="text-secondary">Employees</div><div class="display-6 fw-semibold">{{ summary.totalEmployees | number }}</div></div></div>
      <div class="col-6 col-lg-3"><div class="page-card p-4"><div class="text-secondary">Active</div><div class="display-6 fw-semibold text-success">{{ summary.activeEmployees | number }}</div></div></div>
      <div class="col-6 col-lg-3"><div class="page-card p-4"><div class="text-secondary">Departments</div><div class="display-6 fw-semibold">{{ summary.departmentCount }}</div></div></div>
      <div class="col-6 col-lg-3"><div class="page-card p-4"><div class="text-secondary">Skills</div><div class="display-6 fw-semibold">{{ summary.skillCount }}</div></div></div>
    </div>
    <section class="page-card p-4">
      <h2 class="h5 mb-3">Employees by department</h2>
      <div class="table-responsive">
        <table class="table align-middle mb-0">
          <thead><tr><th>Department</th><th class="text-end">Employees</th></tr></thead>
          <tbody>@for (item of departmentCounts; track item.departmentId) { <tr><td>{{ item.departmentName }}</td><td class="text-end">{{ item.employeeCount }}</td></tr> }</tbody>
        </table>
      </div>
      <p class="small text-secondary mt-3 mb-0">This table is populated by the SQL Server stored procedure example.</p>
    </section>
  }
</main>
```

## `sample/Frontend/src/app/features/dashboard/dashboard.component.ts`

**File:** `sample/Frontend/src/app/features/dashboard/dashboard.component.ts` — **Use:** Implements the behavior for this Angular screen or shared control.

```typescript
import { DecimalPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin, finalize } from 'rxjs';
import { DashboardSummary, DepartmentEmployeeCount } from '../../core/models/employee.model';
import { DashboardService } from '../../core/services/dashboard.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  summary: DashboardSummary | null = null;
  departmentCounts: DepartmentEmployeeCount[] = [];
  loading = true;
  errorMessage = '';

  constructor(private dashboardService: DashboardService, public authService: AuthService) {}

  ngOnInit(): void {
    forkJoin({ summary: this.dashboardService.getSummary(), counts: this.dashboardService.getDepartmentCounts() })
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: data => { this.summary = data.summary; this.departmentCounts = data.counts; },
        error: () => this.errorMessage = 'Dashboard data could not be loaded.'
      });
  }
}
```

## `sample/Frontend/src/styles.scss`

**File:** `sample/Frontend/src/styles.scss` — **Use:** Styles this page or application.

```scss
@import 'bootstrap/dist/css/bootstrap.min.css';

body { background: #f4f6f9; color: #253047; }
.page-card { background: #fff; border: 1px solid #e4e8ef; border-radius: .75rem; box-shadow: 0 .25rem 1rem rgba(26, 39, 64, .05); }
.required-label::after { color: #dc3545; content: ' *'; }
.table th button { color: inherit; font-weight: 600; text-decoration: none; }
```
