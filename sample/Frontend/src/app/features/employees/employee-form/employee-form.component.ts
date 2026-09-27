import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, forkJoin, map, of, switchMap } from 'rxjs';
import { EmployeeRequest, LookupItem } from '../../../core/models/employee.model';
import { EmployeeService } from '../../../core/services/employee.service';
import { LookupService } from '../../../core/services/lookup.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-employee-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './employee-form.component.html'
})
export class EmployeeFormComponent implements OnInit {
  readonly genders = ['Female', 'Male', 'Non-binary', 'Prefer not to say'];
  readonly employmentTypes = ['Full-time', 'Part-time', 'Contract', 'Intern'];
  readonly form;
  departments: LookupItem[] = [];
  skills: LookupItem[] = [];
  employeeId: number | null = null;
  loading = true;
  saving = false;
  errorMessage = '';
  profileFile: File | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private employeeService: EmployeeService,
    private lookupService: LookupService,
    private notifications: NotificationService
  ) {
    this.form = this.formBuilder.nonNullable.group({
      firstName: ['', [Validators.required, Validators.maxLength(80)]],
      lastName: ['', [Validators.required, Validators.maxLength(80)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
      phone: ['', [Validators.required, Validators.pattern(/^[0-9+() -]{7,25}$/)]],
      salary: [0, [Validators.required, Validators.min(0)]],
      dateOfBirth: ['', Validators.required],
      joiningDate: ['', Validators.required],
      gender: ['', Validators.required],
      employmentType: ['', Validators.required],
      isActive: [true],
      departmentId: [0, [Validators.required, Validators.min(1)]],
      skillIds: [[] as number[]]
    });
  }

  ngOnInit(): void {
    const routeId = this.route.snapshot.paramMap.get('id');
    this.employeeId = routeId ? Number(routeId) : null;
    const employeeRequest = this.employeeId ? this.employeeService.getEmployee(this.employeeId) : of(null);
    forkJoin({
      departments: this.lookupService.getDepartments(),
      skills: this.lookupService.getSkills(),
      employee: employeeRequest
    }).pipe(finalize(() => this.loading = false)).subscribe({
      next: data => {
        this.departments = data.departments;
        this.skills = data.skills;
        if (data.employee) {
          this.form.patchValue({
            ...data.employee,
            dateOfBirth: data.employee.dateOfBirth.substring(0, 10),
            joiningDate: data.employee.joiningDate.substring(0, 10),
            skillIds: data.employee.skills.map(skill => skill.id)
          });
        }
      },
      error: () => this.errorMessage = 'The employee form could not be loaded.'
    });
  }

  selectImage(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0] ?? null;
    this.errorMessage = '';
    if (file && (!['image/jpeg', 'image/png'].includes(file.type) || file.size > 2_000_000)) {
      this.profileFile = null;
      this.errorMessage = 'Profile image must be a JPG or PNG smaller than 2 MB.';
      return;
    }
    this.profileFile = file;
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    const request: EmployeeRequest = { ...value };
    this.saving = true;
    this.errorMessage = '';
    const saveRequest = this.employeeId
      ? this.employeeService.updateEmployee(this.employeeId, request)
      : this.employeeService.createEmployee(request);

    saveRequest.pipe(
      switchMap(employee => this.profileFile
        ? this.employeeService.uploadProfileImage(employee.employeeId, this.profileFile).pipe(map(() => employee))
        : of(employee)),
      finalize(() => this.saving = false)
    ).subscribe({
      next: employee => {
        this.notifications.success(`Employee ${this.employeeId ? 'updated' : 'created'} successfully.`);
        this.router.navigate(['/employees', employee.employeeId]);
      },
      error: error => this.errorMessage = this.getApiError(error)
    });
  }

  private getApiError(error: any): string {
    if (error.error?.errors) {
      const messages = Object.values(error.error.errors).flat() as string[];
      return messages[0] ?? 'Please correct the highlighted fields.';
    }
    return error.error?.detail ?? error.error?.message ?? 'The employee could not be saved.';
  }
}
