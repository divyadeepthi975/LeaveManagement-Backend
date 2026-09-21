using LeaveManagement.DTO;
using LeaveManagement.Models.DTOs;

namespace LeaveManagement.Services
{
    public interface ILoginService
    {
        Task<LoginResponseDTO> RegisterAsync(RegisterDTO registerDTO);

        Task<LoginResponseDTO> LoginAsync(LoginDTO loginDTO);
    }
}