using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class LeaveActionDTO
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string approvedby { get; set; } = string.Empty;

        public string? comments { get; set; }
    }
}