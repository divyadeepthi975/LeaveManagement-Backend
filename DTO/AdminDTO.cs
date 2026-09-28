using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class CreateUserDTO
    {
        [Required]
        [StringLength(50)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [Required]
        public DateTime JoiningDate { get; set; }
    }


    public class ChangeStatusDTO
    {
        [Required]
        public bool IsActive { get; set; }
    }
    public class GetAllEmployeesDTO
    {
        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public DateTime JoiningDate { get; set; }

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}