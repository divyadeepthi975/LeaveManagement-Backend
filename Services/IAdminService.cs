using LeaveManagement.DTO;

namespace LeaveManagement.Services
{
    public interface IAdminService
    {
        Task<EmployeewithIDDTO> CreateUserAsync(CreateUserDTO dto,string role);

        Task<bool> ChangeToManagerAsync(int employeeId);

        Task<bool> ChangeToEmployeeAsync(int employeeId);

        Task<bool> ChangeStatusAsync(int employeeId,ChangeStatusDTO dto);
        Task<List<GetAllEmployeesDTO>> GetAllUsersAsync();
        Task<EmployeewithIDDTO?> GetEmployeeByIdAsync(int employeeId);

        Task<EmployeewithIDDTO?> UpdateEmployeeAsync(
            EmployeewithIDDTO employee);
    }
}