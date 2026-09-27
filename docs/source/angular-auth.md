# Angular login and registration

## `sample/Frontend/src/app/features/auth/login/login.component.html`

Renders the markup for this Angular screen.

```html
<main class="login-page d-flex align-items-center py-5">
  <div class="container">
    <div class="row justify-content-center">
      <div class="col-sm-10 col-md-7 col-lg-5 col-xl-4">
        <div class="card border-0 shadow-lg">
          <div class="card-body p-4 p-lg-5">
            <h1 class="h3 fw-bold mb-2">Employee Manager</h1>
            <p class="text-secondary mb-4">Sign in to manage employee records.</p>
            @if (errorMessage) { <div class="alert alert-danger" role="alert">{{ errorMessage }}</div> }
            <form [formGroup]="form" (ngSubmit)="login()" novalidate>
              <div class="mb-3">
                <label class="form-label required-label" for="username">Username</label>
                <input class="form-control" id="username" formControlName="username" autocomplete="username"
                       [class.is-invalid]="form.controls.username.touched && form.controls.username.invalid">
                <div class="invalid-feedback">Username is required.</div>
              </div>
              <div class="mb-4">
                <label class="form-label required-label" for="password">Password</label>
                <input class="form-control" id="password" type="password" formControlName="password" autocomplete="current-password"
                       [class.is-invalid]="form.controls.password.touched && form.controls.password.invalid">
                <div class="invalid-feedback">Password is required.</div>
              </div>
              <button class="btn btn-primary w-100" type="submit" [disabled]="loading">
                @if (loading) { <span class="spinner-border spinner-border-sm me-2"></span> }
                Sign in
              </button>
            </form>
            <p class="text-center text-secondary small mb-0 mt-4">New here? <a routerLink="/register">Create an account</a></p>
          </div>
        </div>
      </div>
    </div>
  </div>
</main>
```

## `sample/Frontend/src/app/features/auth/login/login.component.scss`

Styles this page or application.

```scss
.login-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #e8f0ff, #f8fafc 50%, #e5f5f1);
}
```

## `sample/Frontend/src/app/features/auth/login/login.component.ts`

Implements the behavior for this Angular screen or shared control.

```typescript
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loading = false;
  errorMessage = '';
  readonly form;

  constructor(private formBuilder: FormBuilder, private authService: AuthService, private router: Router) {
    this.form = this.formBuilder.nonNullable.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
    if (this.authService.isAuthenticated) this.router.navigate(['/dashboard']);
  }

  login(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    this.errorMessage = '';
    this.authService.login(this.form.controls.username.value, this.form.controls.password.value)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: () => this.router.navigate(['/dashboard']),
        error: error => this.errorMessage = error.error?.message ?? 'Unable to sign in. Check that the API is running.'
      });
  }
}
```

## `sample/Frontend/src/app/features/auth/register/register.component.html`

Renders the markup for this Angular screen.

```html
<main class="login-page d-flex align-items-center py-5">
  <div class="container">
    <div class="row justify-content-center">
      <div class="col-sm-10 col-md-7 col-lg-5 col-xl-4">
        <div class="card border-0 shadow-lg">
          <div class="card-body p-4 p-lg-5">
            <h1 class="h3 fw-bold mb-2">Create an account</h1>
            <p class="text-secondary mb-4">Registered accounts can view employee records.</p>
            @if (errorMessage) { <div class="alert alert-danger" role="alert">{{ errorMessage }}</div> }
            <form [formGroup]="form" (ngSubmit)="register()" novalidate>
              <div class="mb-3">
                <label class="form-label required-label" for="username">Username</label>
                <input class="form-control" id="username" formControlName="username" autocomplete="username"
                       [class.is-invalid]="form.controls.username.touched && form.controls.username.invalid">
                <div class="invalid-feedback">Use 3–80 letters, numbers, dots, underscores or hyphens.</div>
              </div>
              <div class="mb-3">
                <label class="form-label required-label" for="password">Password</label>
                <input class="form-control" id="password" type="password" formControlName="password" autocomplete="new-password"
                       [class.is-invalid]="form.controls.password.touched && form.controls.password.invalid">
                <div class="invalid-feedback">Use 8–72 characters with at least one letter and one number.</div>
              </div>
              <div class="mb-4">
                <label class="form-label required-label" for="confirmPassword">Confirm password</label>
                <input class="form-control" id="confirmPassword" type="password" formControlName="confirmPassword" autocomplete="new-password"
                       [class.is-invalid]="form.controls.confirmPassword.touched && (form.controls.confirmPassword.invalid || passwordsDoNotMatch)">
                <div class="invalid-feedback">Passwords must match.</div>
              </div>
              <button class="btn btn-primary w-100" type="submit" [disabled]="loading">
                @if (loading) { <span class="spinner-border spinner-border-sm me-2"></span> }
                Create account
              </button>
            </form>
            <p class="text-center text-secondary small mb-0 mt-4">Already have an account? <a routerLink="/login">Sign in</a></p>
          </div>
        </div>
      </div>
    </div>
  </div>
</main>
```

## `sample/Frontend/src/app/features/auth/register/register.component.ts`

Implements the behavior for this Angular screen or shared control.

```typescript
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: '../login/login.component.scss'
})
export class RegisterComponent {
  loading = false;
  errorMessage = '';
  readonly form;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private notifications: NotificationService,
    private router: Router
  ) {
    this.form = this.formBuilder.nonNullable.group({
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(80), Validators.pattern(/^[A-Za-z0-9._-]+$/)]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(72), Validators.pattern(/^(?=.*[A-Za-z])(?=.*[0-9]).+$/)]],
      confirmPassword: ['', Validators.required]
    });
    if (this.authService.isAuthenticated) this.router.navigate(['/dashboard']);
  }

  get passwordsDoNotMatch(): boolean {
    return this.form.controls.confirmPassword.touched &&
      this.form.controls.confirmPassword.value !== this.form.controls.password.value;
  }

  register(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.passwordsDoNotMatch) return;

    this.loading = true;
    this.errorMessage = '';
    this.authService.register(this.form.getRawValue())
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: () => {
          this.notifications.success('Account created. Sign in to continue.');
          this.router.navigate(['/login']);
        },
        error: error => {
          const validationErrors = error.error?.errors;
          this.errorMessage = validationErrors
            ? (Object.values(validationErrors).flat() as string[])[0]
            : error.error?.detail ?? 'Registration failed. Please try again.';
        }
      });
  }
}
```
