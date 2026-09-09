namespace LeaveManagement.DTO
{
    public class LeaveBalanceDTO
    {
        public string LeaveType { get; set; } = string.Empty;

        public int TotalDays { get; set; }

        public int UsedDays { get; set; }

        public int RemainingDays { get; set; }
    }
}