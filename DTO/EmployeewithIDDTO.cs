

using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class EmployeewithIDDTO
    {

        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
        public bool IsActive { get; set; }
    }
}
