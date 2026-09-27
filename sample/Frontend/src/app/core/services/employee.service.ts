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
