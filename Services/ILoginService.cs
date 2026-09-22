using LeaveManagement.DTO;

namespace LeaveManagement.Services
{
    public interface ILoginService
    {
        Task<LoginResponseDTO> RegisterEmployeeAsync(
            RegisterEmployeeDTO registerDTO);

        Task<LoginResponseDTO> LoginAsync(
            LoginDTO loginDTO);
    }
}