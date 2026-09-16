using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagement.Models.Entities
{
    public class Loginusers
    {
        [Key]
        public int loginuserid { get; set; }
        public int employeeid { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(30)]
        public string username { get; set; }=string.Empty;
        [Required]
        public string passwordhash{ get; set; } = string.Empty;
        [Required]
        public string role { get; set; } = string.Empty;
        [ForeignKey("employeeid")]
        public Employee? employee { get; set; }


    }
}
