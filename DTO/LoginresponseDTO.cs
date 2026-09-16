namespace LeaveManagement.DTO
{
    public class LoginResponseDTO
    {
        public string token { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
    }
}