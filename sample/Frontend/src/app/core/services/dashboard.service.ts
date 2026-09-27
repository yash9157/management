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
