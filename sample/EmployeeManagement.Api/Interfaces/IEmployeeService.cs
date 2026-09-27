using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQuery query);
    Task<EmployeeDto?> GetEmployeeAsync(int employeeId);
    Task<EmployeeDto> CreateEmployeeAsync(EmployeeRequest request);
    Task<EmployeeDto?> UpdateEmployeeAsync(int employeeId, EmployeeRequest request);
    Task<bool> DeleteEmployeeAsync(int employeeId);
}
