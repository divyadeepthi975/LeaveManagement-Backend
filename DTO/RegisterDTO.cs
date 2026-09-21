namespace LeaveManagement.DTO
{
    public class RegisterDTO
    {
        public int employeecode { get; set; }

        public string username { get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;

        public string role { get; set; } = string.Empty;
    }
}