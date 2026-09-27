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
