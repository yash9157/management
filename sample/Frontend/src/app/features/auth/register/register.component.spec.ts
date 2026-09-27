import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { RegisterComponent } from './register.component';

describe('RegisterComponent', () => {
  beforeEach(async () => {
    localStorage.removeItem('employee-management-session');
    await TestBed.configureTestingModule({
      imports: [RegisterComponent],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])]
    }).compileComponents();
  });

  it('does not send mismatched passwords', () => {
    const component = TestBed.createComponent(RegisterComponent).componentInstance;
    component.form.setValue({ username: 'newuser', password: 'Example123!', confirmPassword: 'Different123!' });

    component.register();

    expect(component.passwordsDoNotMatch).toBe(true);
    TestBed.inject(HttpTestingController).expectNone(`${environment.apiUrl}/auth/register`);
  });

  it('sends a valid registration to the API', () => {
    const component = TestBed.createComponent(RegisterComponent).componentInstance;
    component.form.setValue({ username: 'newuser', password: 'Example123!', confirmPassword: 'Example123!' });

    component.register();

    const request = TestBed.inject(HttpTestingController).expectOne(`${environment.apiUrl}/auth/register`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body.username).toBe('newuser');
    request.flush({ username: 'newuser', role: 'Viewer' });
    expect(component.loading).toBe(false);
  });
});
