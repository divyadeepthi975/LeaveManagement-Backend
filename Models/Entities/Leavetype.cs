using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Models.Entities
{
    public class Leavetype
    {
        [Key]
        public int leavetypeid { get; set;}
        [Required]
        [StringLength(20)]
        public string leavetypename { get; set; } = string.Empty;
        [Required]
        public int maximumdays {  get; set;}
        public bool IsActive { get; set; } = true;
    }
}
