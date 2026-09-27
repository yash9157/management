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
