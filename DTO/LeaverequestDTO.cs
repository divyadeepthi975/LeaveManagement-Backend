using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class LeaverequestDTO
    {
        [Required]
        public int employeeid { get; set; }

        [Required]
        public int leavetypeid { get; set; }

        [Required]
        public DateTime fromdate { get; set; }

        [Required]
        public DateTime todate { get; set; }

        [Required]
        public string reason { get; set; } = string.Empty;
    }
}