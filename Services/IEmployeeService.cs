using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;

public interface IEmployeeService
{
    Task<EmployeewithIDDTO> CreateEmployeeAsync(EmployeeDTO employee);

    Task<string> DeleteAsync(int id);

    Task<List<EmployeewithIDDTO>> GetEmployeesByManagerAsync(int managerId);

    Task<EmployeewithIDDTO?> GetByIdAsync(int id);

    Task<EmployeewithIDDTO?> GetByIdForManagerAsync(int employeeId, int managerId);

    Task<object?> UpdateAsync(
        EmployeewithIDDTO employee);

    Task<bool> ChangeStatusByManagerAsync(
        int managerId,
        int employeeId,
        bool isActive);

    Task<Employee> AddWithIdAsync(
        EmployeeDTO employeeDTO,
        int employeeId);
}
