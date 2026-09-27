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
