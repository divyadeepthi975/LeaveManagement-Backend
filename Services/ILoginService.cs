using LeaveManagement.DTO;

namespace LeaveManagement.Services
{
    public interface ILoginService
    {
        Task<LoginResponseDTO> LoginAsync(
            LoginDTO loginDTO);
    }
}