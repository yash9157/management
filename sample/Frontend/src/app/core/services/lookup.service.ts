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
