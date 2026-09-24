using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class LoginDTO
    {
        [EmailAddress]
        [Required]
        public string username { get; set; } = string.Empty;
        [Required]
        public string password {  get; set; }=string.Empty;
    }
}
