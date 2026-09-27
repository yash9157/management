# MVC Razor views and assets

Complete Razor pages, form partial, layout, styles, and application JavaScript.

This page contains **complete file contents** for 16 files. Copy each block to the exact path shown. The code is embedded in this Markdown page and remains visible when the sample source directory is unavailable.

Return to [all source files](/source/) or read [setup instructions](/start). The [source bundle](/sample-source.zip) also includes binary icons and license files.

## `sample/MvcEmployeeManagement/Views/_ViewImports.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/_ViewImports.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
﻿@using EmployeeManagement.Mvc
@using EmployeeManagement.Mvc.Models
@using EmployeeManagement.Mvc.ViewModels
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

## `sample/MvcEmployeeManagement/Views/_ViewStart.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/_ViewStart.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
﻿@{
    Layout = "_Layout";
}
```

## `sample/MvcEmployeeManagement/Views/Employees/_EmployeeForm.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/Employees/_EmployeeForm.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
@model EmployeeFormViewModel

<div asp-validation-summary="ModelOnly" class="text-danger mb-3"></div>
<div class="row g-3">
    <div class="col-md-6">
        <label asp-for="FirstName" class="form-label"></label>
        <input asp-for="FirstName" class="form-control" />
        <span asp-validation-for="FirstName" class="text-danger small"></span>
    </div>
    <div class="col-md-6">
        <label asp-for="LastName" class="form-label"></label>
        <input asp-for="LastName" class="form-control" />
        <span asp-validation-for="LastName" class="text-danger small"></span>
    </div>
    <div class="col-md-6">
        <label asp-for="Email" class="form-label"></label>
        <input asp-for="Email" class="form-control" />
        <span asp-validation-for="Email" class="text-danger small"></span>
    </div>
    <div class="col-md-6">
        <label asp-for="Phone" class="form-label"></label>
        <input asp-for="Phone" class="form-control" />
        <span asp-validation-for="Phone" class="text-danger small"></span>
    </div>
    <div class="col-md-6">
        <label asp-for="Salary" class="form-label"></label>
        <input asp-for="Salary" class="form-control" type="number" min="0" step="0.01" />
        <span asp-validation-for="Salary" class="text-danger small"></span>
    </div>
    <div class="col-md-6">
        <label asp-for="DepartmentId" class="form-label"></label>
        <select asp-for="DepartmentId" asp-items="Model.Departments" class="form-select">
            <option value="0">Select a department</option>
        </select>
        <span asp-validation-for="DepartmentId" class="text-danger small"></span>
    </div>
    <div class="col-md-6">
        <label asp-for="DateOfBirth" class="form-label"></label>
        <input asp-for="DateOfBirth" class="form-control" type="date" />
        <span asp-validation-for="DateOfBirth" class="text-danger small"></span>
    </div>
    <div class="col-md-6">
        <label asp-for="JoiningDate" class="form-label"></label>
        <input asp-for="JoiningDate" class="form-control" type="date" />
        <span asp-validation-for="JoiningDate" class="text-danger small"></span>
    </div>
    <div class="col-md-6">
        <label asp-for="Gender" class="form-label"></label>
        <select asp-for="Gender" class="form-select">
            <option value="">Select gender</option>
            @foreach (var gender in EmployeeFormViewModel.Genders)
            {
                <option value="@gender">@gender</option>
            }
        </select>
        <span asp-validation-for="Gender" class="text-danger small"></span>
    </div>
    <div class="col-md-6">
        <label asp-for="EmploymentType" class="form-label"></label>
        <select asp-for="EmploymentType" class="form-select">
            <option value="">Select employment type</option>
            @foreach (var type in EmployeeFormViewModel.EmploymentTypes)
            {
                <option value="@type">@type</option>
            }
        </select>
        <span asp-validation-for="EmploymentType" class="text-danger small"></span>
    </div>
    <div class="col-12">
        <div class="form-check">
            <input asp-for="IsActive" class="form-check-input" />
            <label asp-for="IsActive" class="form-check-label"></label>
        </div>
    </div>
</div>
```

## `sample/MvcEmployeeManagement/Views/Employees/Create.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/Employees/Create.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
@model EmployeeFormViewModel
@{
    ViewData["Title"] = "Add employee";
}

<a asp-action="Index" class="text-decoration-none">← Back to employees</a>
<h1 class="h3 mt-3 mb-4">Add employee</h1>
<form asp-action="Create" method="post" class="content-card p-4">
    <partial name="_EmployeeForm" model="Model" />
    <div class="d-flex gap-2 mt-4">
        <button type="submit" class="btn btn-primary">Save employee</button>
        <a asp-action="Index" class="btn btn-outline-secondary">Cancel</a>
    </div>
</form>

@section Scripts { <partial name="_ValidationScriptsPartial" /> }
```

## `sample/MvcEmployeeManagement/Views/Employees/Delete.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/Employees/Delete.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
@model Employee
@{
    ViewData["Title"] = "Delete employee";
}

<a asp-action="Details" asp-route-id="@Model.EmployeeId" class="text-decoration-none">← Back to employee</a>
<section class="content-card p-4 mt-3">
    <h1 class="h3">Delete employee</h1>
    <p>Are you sure you want to delete <strong>@Model.FirstName @Model.LastName</strong> from @Model.Department.Name?</p>
    <p class="text-danger">This cannot be undone.</p>
    <form asp-action="Delete" asp-route-id="@Model.EmployeeId" method="post" class="d-flex gap-2">
        <button type="submit" class="btn btn-danger">Delete employee</button>
        <a asp-action="Details" asp-route-id="@Model.EmployeeId" class="btn btn-outline-secondary">Cancel</a>
    </form>
</section>
```

## `sample/MvcEmployeeManagement/Views/Employees/Details.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/Employees/Details.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
@model Employee
@{
    ViewData["Title"] = "Employee details";
}

<a asp-action="Index" class="text-decoration-none">← Back to employees</a>
@if (TempData["SuccessMessage"] is string message)
{
    <div class="alert alert-success mt-3" role="status">@message</div>
}
<section class="content-card p-4 mt-3">
    <div class="d-flex justify-content-between align-items-start gap-3 border-bottom pb-3 mb-4">
        <div>
            <h1 class="h3 mb-1">@Model.FirstName @Model.LastName</h1>
            <span class="text-secondary">@Model.Department.Name · @Model.EmploymentType</span>
        </div>
        <div class="d-flex gap-2">
            <a asp-action="Edit" asp-route-id="@Model.EmployeeId" class="btn btn-primary">Edit</a>
            <a asp-action="Delete" asp-route-id="@Model.EmployeeId" class="btn btn-outline-danger">Delete</a>
        </div>
    </div>
    <div class="row g-4">
        <div class="col-md-6"><div class="text-secondary small">Email</div><div>@Model.Email</div></div>
        <div class="col-md-6"><div class="text-secondary small">Phone</div><div>@Model.Phone</div></div>
        <div class="col-md-6"><div class="text-secondary small">Salary</div><div>@Model.Salary.ToString("N2")</div></div>
        <div class="col-md-6"><div class="text-secondary small">Date of birth</div><div>@Model.DateOfBirth.ToString("dd MMM yyyy")</div></div>
        <div class="col-md-6"><div class="text-secondary small">Joining date</div><div>@Model.JoiningDate.ToString("dd MMM yyyy")</div></div>
        <div class="col-md-6"><div class="text-secondary small">Gender</div><div>@Model.Gender</div></div>
        <div class="col-md-6"><div class="text-secondary small">Status</div><div>@(Model.IsActive ? "Active" : "Inactive")</div></div>
    </div>
</section>
```

## `sample/MvcEmployeeManagement/Views/Employees/Edit.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/Employees/Edit.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
@model EmployeeFormViewModel
@{
    ViewData["Title"] = "Edit employee";
}

<a asp-action="Details" asp-route-id="@Model.EmployeeId" class="text-decoration-none">← Back to employee</a>
<h1 class="h3 mt-3 mb-4">Edit employee</h1>
<form asp-action="Edit" asp-route-id="@Model.EmployeeId" method="post" class="content-card p-4">
    <partial name="_EmployeeForm" model="Model" />
    <div class="d-flex gap-2 mt-4">
        <button type="submit" class="btn btn-primary">Save changes</button>
        <a asp-action="Details" asp-route-id="@Model.EmployeeId" class="btn btn-outline-secondary">Cancel</a>
    </div>
</form>

@section Scripts { <partial name="_ValidationScriptsPartial" /> }
```

## `sample/MvcEmployeeManagement/Views/Employees/Index.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/Employees/Index.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
@model EmployeeListViewModel
@{
    ViewData["Title"] = "Employees";
}

<div class="d-flex justify-content-between align-items-center gap-3 mb-4">
    <div>
        <h1 class="h3 mb-1">Employees</h1>
        <p class="text-secondary mb-0">Search and manage employee records.</p>
    </div>
    <a class="btn btn-primary" asp-action="Create">Add employee</a>
</div>

@if (TempData["SuccessMessage"] is string message)
{
    <div class="alert alert-success" role="status">@message</div>
}

<form asp-action="Index" method="get" class="content-card p-3 p-lg-4 mb-3">
    <div class="row g-3 align-items-end">
        <div class="col-md-4">
            <label asp-for="Search" class="form-label"></label>
            <input asp-for="Search" class="form-control" placeholder="Name or email" />
        </div>
        <div class="col-md-3">
            <label asp-for="DepartmentId" class="form-label">Department</label>
            <select asp-for="DepartmentId" asp-items="Model.Departments" class="form-select">
                <option value="">All departments</option>
            </select>
        </div>
        <div class="col-md-2">
            <label asp-for="SortBy" class="form-label">Sort by</label>
            <select asp-for="SortBy" class="form-select">
                <option value="name">Name</option>
                <option value="email">Email</option>
                <option value="salary">Salary</option>
                <option value="joiningDate">Joining date</option>
                <option value="department">Department</option>
            </select>
        </div>
        <div class="col-md-2">
            <label asp-for="Direction" class="form-label">Direction</label>
            <select asp-for="Direction" class="form-select">
                <option value="asc">Ascending</option>
                <option value="desc">Descending</option>
            </select>
        </div>
        <div class="col-md-1 d-grid">
            <button class="btn btn-outline-primary" type="submit">Apply</button>
        </div>
    </div>
</form>

<section class="content-card overflow-hidden">
    @if (Model.Employees.Count == 0)
    {
        <div class="text-center p-5">
            <h2 class="h5">No employees found</h2>
            <p class="text-secondary mb-0">Try another search, or add your first employee.</p>
        </div>
    }
    else
    {
        <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
                <thead class="table-light">
                    <tr><th>Name</th><th>Email</th><th>Department</th><th>Salary</th><th>Joined</th><th>Status</th><th class="text-end">Actions</th></tr>
                </thead>
                <tbody>
                    @foreach (var employee in Model.Employees)
                    {
                        <tr>
                            <td class="fw-semibold">@employee.FirstName @employee.LastName</td>
                            <td>@employee.Email</td>
                            <td>@employee.Department.Name</td>
                            <td>@employee.Salary.ToString("N2")</td>
                            <td>@employee.JoiningDate.ToString("dd MMM yyyy")</td>
                            <td><span class="badge @(employee.IsActive ? "text-bg-success" : "text-bg-secondary")">@(employee.IsActive ? "Active" : "Inactive")</span></td>
                            <td class="text-end text-nowrap">
                                <a class="btn btn-sm btn-outline-primary" asp-action="Details" asp-route-id="@employee.EmployeeId">View</a>
                                <a class="btn btn-sm btn-outline-secondary" asp-action="Edit" asp-route-id="@employee.EmployeeId">Edit</a>
                                <a class="btn btn-sm btn-outline-danger" asp-action="Delete" asp-route-id="@employee.EmployeeId">Delete</a>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
        <div class="d-flex justify-content-between align-items-center p-3 border-top">
            <span class="text-secondary">@Model.TotalCount employee(s) · Page @Model.Page of @Model.TotalPages</span>
            <div class="btn-group" role="group" aria-label="Pagination">
                @if (Model.Page > 1)
                {
                    <a class="btn btn-outline-primary" asp-action="Index" asp-route-search="@Model.Search" asp-route-departmentId="@Model.DepartmentId" asp-route-sortBy="@Model.SortBy" asp-route-direction="@Model.Direction" asp-route-page="@(Model.Page - 1)">Previous</a>
                }
                @if (Model.Page < Model.TotalPages)
                {
                    <a class="btn btn-outline-primary" asp-action="Index" asp-route-search="@Model.Search" asp-route-departmentId="@Model.DepartmentId" asp-route-sortBy="@Model.SortBy" asp-route-direction="@Model.Direction" asp-route-page="@(Model.Page + 1)">Next</a>
                }
            </div>
        </div>
    }
</section>
```

## `sample/MvcEmployeeManagement/Views/Home/Index.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/Home/Index.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
﻿@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>Learn about <a href="https://learn.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
</div>
```

## `sample/MvcEmployeeManagement/Views/Home/Privacy.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/Home/Privacy.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
﻿@{
    ViewData["Title"] = "Privacy Policy";
}
<h1>@ViewData["Title"]</h1>

<p>Use this page to detail your site's privacy policy.</p>
```

## `sample/MvcEmployeeManagement/Views/Shared/_Layout.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/Shared/_Layout.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
﻿<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Employee Management</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/EmployeeManagement.Mvc.styles.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-light bg-white border-bottom mb-3">
            <div class="container-fluid">
                <a class="navbar-brand fw-semibold" asp-controller="Employees" asp-action="Index">Employee Management</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-controller="Employees" asp-action="Index">Employees</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-controller="Employees" asp-action="Create">Add employee</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            Employee Management MVC interview practice
        </div>
    </footer>
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

## `sample/MvcEmployeeManagement/Views/Shared/_Layout.cshtml.css`

**File:** `sample/MvcEmployeeManagement/Views/Shared/_Layout.cshtml.css` — **Use:** Styles this page or application.

```css
﻿/* Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
for details on configuring this project to bundle and minify static web assets. */

a.navbar-brand {
  white-space: normal;
  text-align: center;
  word-break: break-all;
}

a {
  color: #0077cc;
}

.btn-primary {
  color: #fff;
  background-color: #1b6ec2;
  border-color: #1861ac;
}

.nav-pills .nav-link.active, .nav-pills .show > .nav-link {
  color: #fff;
  background-color: #1b6ec2;
  border-color: #1861ac;
}

.border-top {
  border-top: 1px solid #e5e5e5;
}
.border-bottom {
  border-bottom: 1px solid #e5e5e5;
}

.box-shadow {
  box-shadow: 0 .25rem .75rem rgba(0, 0, 0, .05);
}

button.accept-policy {
  font-size: 1rem;
  line-height: inherit;
}

.footer {
  position: absolute;
  bottom: 0;
  width: 100%;
  white-space: nowrap;
  line-height: 60px;
}
```

## `sample/MvcEmployeeManagement/Views/Shared/_ValidationScriptsPartial.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/Shared/_ValidationScriptsPartial.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
﻿<script src="~/lib/jquery-validation/dist/jquery.validate.min.js"></script>
<script src="~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"></script>
```

## `sample/MvcEmployeeManagement/Views/Shared/Error.cshtml`

**File:** `sample/MvcEmployeeManagement/Views/Shared/Error.cshtml` — **Use:** Renders this Razor page or reusable partial.

```html
﻿@model ErrorViewModel
@{
    ViewData["Title"] = "Error";
}

<h1 class="text-danger">Error.</h1>
<h2 class="text-danger">An error occurred while processing your request.</h2>

@if (Model.ShowRequestId)
{
    <p>
        <strong>Request ID:</strong> <code>@Model.RequestId</code>
    </p>
}

<h3>Development Mode</h3>
<p>
    Swapping to <strong>Development</strong> environment will display more detailed information about the error that occurred.
</p>
<p>
    <strong>The Development environment shouldn't be enabled for deployed applications.</strong>
    It can result in displaying sensitive information from exceptions to end users.
    For local debugging, enable the <strong>Development</strong> environment by setting the <strong>ASPNETCORE_ENVIRONMENT</strong> environment variable to <strong>Development</strong>
    and restarting the app.
</p>
```

## `sample/MvcEmployeeManagement/wwwroot/css/site.css`

**File:** `sample/MvcEmployeeManagement/wwwroot/css/site.css` — **Use:** Styles this page or application.

```css
html {
  font-size: 14px;
}

@media (min-width: 768px) {
  html {
    font-size: 16px;
  }
}

.btn:focus, .btn:active:focus, .btn-link.nav-link:focus, .form-control:focus, .form-check-input:focus {
  box-shadow: 0 0 0 0.1rem white, 0 0 0 0.25rem #258cfb;
}

html {
  position: relative;
  min-height: 100%;
}

body {
  margin-bottom: 60px;
  background: #f6f8fb;
}

.content-card {
  background: #fff;
  border: 1px solid #e5e9f0;
  border-radius: 0.75rem;
  box-shadow: 0 4px 20px rgba(15, 23, 42, 0.04);
}

.table th { white-space: nowrap; }
```

## `sample/MvcEmployeeManagement/wwwroot/js/site.js`

**File:** `sample/MvcEmployeeManagement/wwwroot/js/site.js` — **Use:** Supplies application code or configuration required by this sample.

```javascript
﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
```
