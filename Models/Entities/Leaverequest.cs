using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagement.Models.Entities
{
    public class Leaverequest
    {
        [Key]
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
        public string status { get; set; } = "Pending";

        [Required]
        public DateTime applieddate { get; set; } = DateTime.Now;

        public string? approvedby { get; set; } = null;

        public string? comments { get; set; } = null;

        [ForeignKey("employeeid")]
        public Employee? Employee { get; set; }

        [ForeignKey("leavetypeid")]
        public Leavetype? Leavetype { get; set; }
    }
}