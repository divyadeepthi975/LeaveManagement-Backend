using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class LoginDTO
    {
        [Required]
        public string username { get; set; } = string.Empty;
        [Required]
        public string passwordhash { get; set; } = string.Empty;
    }
}
