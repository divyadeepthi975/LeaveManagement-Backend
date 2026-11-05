using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;

public interface IEmployeeService
{
    Task<EmployeewithIDDTO> CreateEmployeeAsync(EmployeeDTO employee);

    Task<string> DeleteAsync(int id);

    Task<List<EmployeewithIDDTO>> GetAllEmployeesAsync();

    Task<EmployeewithIDDTO?> GetByIdAsync(int id);

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
