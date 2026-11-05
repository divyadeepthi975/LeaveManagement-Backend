using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagement.Models.Entities
{
    public class Employee
    {
        [Key]
        [ForeignKey("loginuser")]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(33, ErrorMessage = "Email cannot exceed 20 characters before @techwave.com.")]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9._%+-]*@techwave\.com$",
            ErrorMessage = "Email must start with a letter and belong to the techwave.com domain."
)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        public DateTime JoiningDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int? ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public Employee? Manager { get; set; }

        public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();

        public Loginusers? loginuser { get; set; }
    }
}
