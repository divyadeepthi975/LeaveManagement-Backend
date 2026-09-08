using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class LeavetypeDTO
    {
        [Required]
        [StringLength(100)]
        public string leavetypename { get; set; } = string.Empty;

        [Required]
        [Range(1, 365)]
        public int maximumdays { get; set; }
    }
}