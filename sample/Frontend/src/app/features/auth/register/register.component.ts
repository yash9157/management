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
