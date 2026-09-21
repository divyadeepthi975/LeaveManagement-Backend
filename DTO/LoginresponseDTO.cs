namespace LeaveManagement.Models.DTOs
{
    public class LoginResponseDTO
    {
        public string employeecode { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;

        public string username { get; set; } = string.Empty;

        public string role { get; set; } = string.Empty;
    }
}