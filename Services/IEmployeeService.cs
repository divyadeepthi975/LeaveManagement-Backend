using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;

namespace LeaveManagement.Services
{
    public interface IEmployeeService
    {
        Task<Employee> AddAsync(EmployeeDTO employee);

        Task<IEnumerable<Employee>> GetAllAsync();

        Task<Employee?> GetByIdAsync(int id);

        Task<Employee?> UpdateAsync(Employee employee);

        Task<string> DeleteAsync(int id);
    }
}