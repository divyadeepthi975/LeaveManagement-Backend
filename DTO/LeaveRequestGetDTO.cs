using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class LeaveRequestGetDTO
    {
        [Required]
        public int leaverequestid { get; set; }

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

        [Required]
        public string status { get; set; } = string.Empty;

        [Required]
        public DateTime applieddate { get; set; }

        public string? approvedby { get; set; }

        public string? comments { get; set; }
    }
}