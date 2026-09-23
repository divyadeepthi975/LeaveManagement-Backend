using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;

public interface IEmployeeService
{
    Task<Employee> AddAsync(EmployeeDTO employee);

    Task<string> DeleteAsync(int id);

    Task<IEnumerable<EmployeewithIDDTO>> GetAllAsync();

    Task<EmployeewithIDDTO?> GetByIdAsync(int id);

    Task<EmployeewithIDDTO?> UpdateAsync(
        EmployeewithIDDTO employee);

    Task<Employee> AddWithIdAsync(
        EmployeeDTO employeeDTO,
        int employeeId);
}