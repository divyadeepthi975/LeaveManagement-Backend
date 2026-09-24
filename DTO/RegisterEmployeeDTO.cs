using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class RegisterEmployeeDTO
    {
        [Required]
        [EmailAddress]
        public string username { get; set; } = string.Empty;

        [Required]
        public string password { get; set; } = string.Empty;

        [Required]
        [RegularExpression(
            "^(Manager|Employee)$",
            ErrorMessage = "Role must be Manager or Employee."
        )]
        public string role { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        public DateTime JoiningDate { get; set; }
    }
}