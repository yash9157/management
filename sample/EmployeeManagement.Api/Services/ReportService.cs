using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

public class ReportService(AppDbContext dbContext)
{
    public Task<List<DepartmentEmployeeCount>> GetDepartmentEmployeeCountsAsync() =>
        dbContext.Set<DepartmentEmployeeCount>()
            .FromSqlRaw("EXEC dbo.GetDepartmentEmployeeCounts")
            .AsNoTracking()
            .ToListAsync();
}
