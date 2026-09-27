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
