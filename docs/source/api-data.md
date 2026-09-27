# API EF Core and migration

## `sample/EmployeeManagement.Api/Data/AppDbContext.cs`

Maps entities, relationships, indexes, and seed data to SQL Server.

```csharp
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<EmployeeSkill> EmployeeSkills => Set<EmployeeSkill>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasIndex(employee => employee.Email).IsUnique();
        modelBuilder.Entity<Employee>().Property(employee => employee.Salary).HasPrecision(18, 2);
        modelBuilder.Entity<Employee>()
            .HasOne(employee => employee.Department)
            .WithMany(department => department.Employees)
            .HasForeignKey(employee => employee.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeSkill>().HasKey(link => new { link.EmployeeId, link.SkillId });
        modelBuilder.Entity<EmployeeSkill>()
            .HasOne(link => link.Employee)
            .WithMany(employee => employee.EmployeeSkills)
            .HasForeignKey(link => link.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EmployeeSkill>()
            .HasOne(link => link.Skill)
            .WithMany(skill => skill.EmployeeSkills)
            .HasForeignKey(link => link.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Department>().HasIndex(department => department.Name).IsUnique();
        modelBuilder.Entity<Skill>().HasIndex(skill => skill.Name).IsUnique();
        modelBuilder.Entity<AppUser>().HasIndex(user => user.Username).IsUnique();
        modelBuilder.Entity<DepartmentEmployeeCount>().HasNoKey().ToView("DepartmentEmployeeCounts");

        modelBuilder.Entity<Department>().HasData(
            new Department { DepartmentId = 1, Name = "Engineering", Description = "Software delivery and platform engineering" },
            new Department { DepartmentId = 2, Name = "Human Resources", Description = "People operations and recruitment" },
            new Department { DepartmentId = 3, Name = "Finance", Description = "Financial planning and accounting" },
            new Department { DepartmentId = 4, Name = "Sales", Description = "Sales and customer relationships" });

        modelBuilder.Entity<Skill>().HasData(
            new Skill { SkillId = 1, Name = "C#" },
            new Skill { SkillId = 2, Name = "ASP.NET Core" },
            new Skill { SkillId = 3, Name = "Angular" },
            new Skill { SkillId = 4, Name = "SQL Server" },
            new Skill { SkillId = 5, Name = "TypeScript" },
            new Skill { SkillId = 6, Name = "Communication" });
    }
}
```

## `sample/EmployeeManagement.Api/Data/DatabaseInitializer.cs`

Applies the API migration and seeds local demo users.

```csharp
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

public class DatabaseInitializer(AppDbContext dbContext, IConfiguration configuration)
{
    public async Task InitializeAsync()
    {
        await dbContext.Database.MigrateAsync();

        await SeedUserAsync("DemoAdmin", "Admin");
        await SeedUserAsync("DemoViewer", "Viewer");
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedUserAsync(string configurationSection, string role)
    {
        var username = configuration[$"{configurationSection}:Username"];
        var password = configuration[$"{configurationSection}:Password"];
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return;
        if (await dbContext.AppUsers.AnyAsync(user => user.Username == username))
            return;

        dbContext.AppUsers.Add(new AppUser
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        });
    }
}
```

## `sample/EmployeeManagement.Api/Data/Migrations/20260927123728_InitialCreate.cs`

Creates the initial database schema and seeded reference data.

```csharp
﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeManagement.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    AppUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.AppUserId);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillId);
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
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "EmployeeSkills",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSkills", x => new { x.EmployeeId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Software delivery and platform engineering", "Engineering" },
                    { 2, "People operations and recruitment", "Human Resources" },
                    { 3, "Financial planning and accounting", "Finance" },
                    { 4, "Sales and customer relationships", "Sales" }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "SkillId", "Name" },
                values: new object[,]
                {
                    { 1, "C#" },
                    { 2, "ASP.NET Core" },
                    { 3, "Angular" },
                    { 4, "SQL Server" },
                    { 5, "TypeScript" },
                    { 6, "Communication" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Username",
                table: "AppUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_SkillId",
                table: "EmployeeSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Name",
                table: "Skills",
                column: "Name",
                unique: true);

            migrationBuilder.Sql("""
                CREATE PROCEDURE dbo.GetDepartmentEmployeeCounts
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT d.DepartmentId,
                           d.Name AS DepartmentName,
                           COUNT(e.EmployeeId) AS EmployeeCount
                    FROM Departments d
                    LEFT JOIN Employees e ON e.DepartmentId = d.DepartmentId
                    GROUP BY d.DepartmentId, d.Name
                    ORDER BY d.Name;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.GetDepartmentEmployeeCounts");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "EmployeeSkills");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
```
