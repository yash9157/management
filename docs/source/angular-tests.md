# Angular tests

Complete checked-in component tests and test TypeScript configuration.

This page contains **complete file contents** for 3 files. Copy each block to the exact path shown. The code is embedded in this Markdown page and remains visible when the sample source directory is unavailable.

Return to [all source files](/source/) or read [setup instructions](/start). The [source bundle](/sample-source.zip) also includes binary icons and license files.

## `sample/Frontend/src/app/app.spec.ts`

**File:** `sample/Frontend/src/app/app.spec.ts` — **Use:** Tests this Angular component.

```typescript
import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideRouter([])]
    }).compileComponents();
  });

  it('creates the application shell', () => {
    expect(TestBed.createComponent(App).componentInstance).toBeTruthy();
  });
});
```

## `sample/Frontend/src/app/features/auth/register/register.component.spec.ts`

**File:** `sample/Frontend/src/app/features/auth/register/register.component.spec.ts` — **Use:** Tests this Angular component.

```typescript
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
```

## `sample/Frontend/tsconfig.spec.json`

**File:** `sample/Frontend/tsconfig.spec.json` — **Use:** Configures TypeScript compilation for this target.

```json
/* To learn more about Typescript configuration file: https://www.typescriptlang.org/docs/handbook/tsconfig-json.html. */
/* To learn more about Angular compiler options: https://angular.dev/reference/configs/angular-compiler-options. */
{
  "extends": "./tsconfig.json",
  "compilerOptions": {
    "outDir": "./out-tsc/spec",
    "types": [
      "vitest/globals"
    ]
  },
  "include": [
    "src/**/*.d.ts",
    "src/**/*.spec.ts"
  ]
}
```
