using LeaveManagement.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class EmployeeDTO
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

        [Required]
        public int? ManagerId { get; set; }
    }
}
