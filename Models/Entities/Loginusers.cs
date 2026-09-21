using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Models.Entities
{
    public class Loginusers
    {
        [Key]
        public int employeeid { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(30)]
        public string username { get; set; } = string.Empty;

        [Required]
        public string passwordhash { get; set; } = string.Empty;

        [Required]
        public string role { get; set; } = string.Empty;

        public Employee? employee { get; set; }
    }
}