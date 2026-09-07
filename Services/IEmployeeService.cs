using LeaveManagement.Models.Entities;

namespace LeaveManagement.Services
{
    public interface IEmployeeService
    {
        Task<Employee> AddAsync(Employee employee);

        Task<IEnumerable<Employee>> GetAllAsync();

        Task<Employee?> GetByIdAsync(int id);

        Task<Employee?> UpdateAsync(Employee employee);

        Task<bool> DeleteAsync(int id);
    }
}