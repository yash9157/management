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
