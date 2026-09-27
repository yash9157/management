# MVC models, view models, and migration

Complete EF Core mapping, entities, form/list models, and schema migration.

This page contains **complete file contents** for 7 files. Copy each block to the exact path shown. The code is embedded in this Markdown page and remains visible when the sample source directory is unavailable.

Return to [all source files](/source/) or read [setup instructions](/start). The [source bundle](/sample-source.zip) also includes binary icons and license files.

## `sample/MvcEmployeeManagement/Data/AppDbContext.cs`

**File:** `sample/MvcEmployeeManagement/Data/AppDbContext.cs` — **Use:** Maps entities, relationships, indexes, and seed data to SQL Server.

```csharp
using EmployeeManagement.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Mvc.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasIndex(employee => employee.Email).IsUnique();
        modelBuilder.Entity<Employee>().Property(employee => employee.Salary).HasPrecision(18, 2);
        modelBuilder.Entity<Employee>()
            .HasOne(employee => employee.Department)
            .WithMany(department => department.Employees)
            .HasForeignKey(employee => employee.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Department>().HasData(
            new Department { DepartmentId = 1, Name = "Engineering" },
            new Department { DepartmentId = 2, Name = "Human Resources" },
            new Department { DepartmentId = 3, Name = "Finance" },
            new Department { DepartmentId = 4, Name = "Sales" });
    }
}
```

## `sample/MvcEmployeeManagement/Data/Migrations/20260927154654_InitialCreate.cs`

**File:** `sample/MvcEmployeeManagement/Data/Migrations/20260927154654_InitialCreate.cs` — **Use:** Creates the initial database schema and seeded reference data.

```csharp
﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeManagement.Mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmploymentType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "Name" },
                values: new object[,]
                {
                    { 1, "Engineering" },
                    { 2, "Human Resources" },
                    { 3, "Finance" },
                    { 4, "Sales" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
```

## `sample/MvcEmployeeManagement/Models/Department.cs`

**File:** `sample/MvcEmployeeManagement/Models/Department.cs` — **Use:** Supplies application code or configuration required by this sample.

```csharp
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Mvc.Models;

public class Department
{
    public int DepartmentId { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
```

## `sample/MvcEmployeeManagement/Models/Employee.cs`

**File:** `sample/MvcEmployeeManagement/Models/Employee.cs` — **Use:** Supplies application code or configuration required by this sample.

```csharp
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Mvc.Models;

public class Employee
{
    public int EmployeeId { get; set; }

    [Required, StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, StringLength(25)]
    public string Phone { get; set; } = string.Empty;

    [Range(0, 100000000)]
    public decimal Salary { get; set; }

    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [DataType(DataType.Date)]
    public DateTime JoiningDate { get; set; }

    [Required, StringLength(20)]
    public string Gender { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string EmploymentType { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
}
```

## `sample/MvcEmployeeManagement/Models/ErrorViewModel.cs`

**File:** `sample/MvcEmployeeManagement/Models/ErrorViewModel.cs` — **Use:** Supplies application code or configuration required by this sample.

```csharp
namespace EmployeeManagement.Mvc.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
```

## `sample/MvcEmployeeManagement/ViewModels/EmployeeFormViewModel.cs`

**File:** `sample/MvcEmployeeManagement/ViewModels/EmployeeFormViewModel.cs` — **Use:** Supplies application code or configuration required by this sample.

```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagement.Mvc.ViewModels;

public class EmployeeFormViewModel : IValidatableObject
{
    public static readonly string[] Genders = ["Female", "Male", "Non-binary", "Prefer not to say"];
    public static readonly string[] EmploymentTypes = ["Full-time", "Part-time", "Contract", "Intern"];

    public int EmployeeId { get; set; }

    [Required, StringLength(80)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(80)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, StringLength(25)]
    public string Phone { get; set; } = string.Empty;

    [Range(0, 100000000)]
    public decimal Salary { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Date of birth")]
    public DateTime? DateOfBirth { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Joining date")]
    public DateTime? JoiningDate { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Employment type")]
    public string EmploymentType { get; set; } = string.Empty;

    [Display(Name = "Active employee")]
    public bool IsActive { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "Select a department.")]
    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [ValidateNever]
    public List<SelectListItem> Departments { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOfBirth.HasValue && DateOfBirth.Value.Date >= DateTime.Today)
            yield return new ValidationResult("Date of birth must be in the past.", [nameof(DateOfBirth)]);
        if (DateOfBirth.HasValue && JoiningDate.HasValue && JoiningDate.Value.Date < DateOfBirth.Value.Date)
            yield return new ValidationResult("Joining date cannot be before date of birth.", [nameof(JoiningDate)]);
        if (!string.IsNullOrWhiteSpace(Gender) && !Genders.Contains(Gender))
            yield return new ValidationResult("Select a valid gender.", [nameof(Gender)]);
        if (!string.IsNullOrWhiteSpace(EmploymentType) && !EmploymentTypes.Contains(EmploymentType))
            yield return new ValidationResult("Select a valid employment type.", [nameof(EmploymentType)]);
    }
}
```

## `sample/MvcEmployeeManagement/ViewModels/EmployeeListViewModel.cs`

**File:** `sample/MvcEmployeeManagement/ViewModels/EmployeeListViewModel.cs` — **Use:** Supplies application code or configuration required by this sample.

```csharp
using EmployeeManagement.Mvc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagement.Mvc.ViewModels;

public class EmployeeListViewModel
{
    public string? Search { get; set; }
    public int? DepartmentId { get; set; }
    public string SortBy { get; set; } = "name";
    public string Direction { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public List<Employee> Employees { get; set; } = [];
    public List<SelectListItem> Departments { get; set; } = [];
}
```
