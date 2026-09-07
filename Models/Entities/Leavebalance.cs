using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagement.Models.Entities
{
    public class Leavebalance
    {
        [Key]
        public int leavebalanceid { get; set; }

        [Required]
        public int employeeid { get; set; }

        [Required]
        public int leavetypeid { get; set; }

        [Required]
        public int totaldays { get; set; }

        public int useddays { get; set; } = 0;

        [ForeignKey("employeeid")]
        public Employee? Employee { get; set; }

        [ForeignKey("leavetypeid")]
        public Leavetype? Leavetype { get; set; }
    }
}