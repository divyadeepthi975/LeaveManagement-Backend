using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;

public interface IEmployeeService
{
    Task<EmployeewithIDDTO> CreateEmployeeAsync(EmployeeDTO employee);

    Task<string> DeleteAsync(int id);

    Task<List<EmployeewithIDDTO>> GetAllEmployeesAsync();

    Task<EmployeewithIDDTO?> GetByIdAsync(int id);

    Task<EmployeewithIDDTO?> UpdateAsync(
        EmployeewithIDDTO employee);

    Task<Employee> AddWithIdAsync(
        EmployeeDTO employeeDTO,
        int employeeId);
}