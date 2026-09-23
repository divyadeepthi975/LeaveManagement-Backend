using LeaveManagement.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class EmployeeDTO
    {
    
        public string EmployeeCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public DateTime JoiningDate { get; set; }

        public Loginusers loginuser { get; set; }
    }
}