using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.DTO
{
    public class LeavetypeIsactiveDTO
    {
        public int leavetypeid { get; set; }
        public string leavetypename { get; set; } = string.Empty;
        public int maximumdays { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
