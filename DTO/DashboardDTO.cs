namespace LeaveManagement.DTO
{
    public class DashboardDTO
    {
        public int TotalEmployees { get; set; }

        public int ActiveEmployees { get; set; }

        public int PendingRequests { get; set; }

        public int ApprovedRequests { get; set; }

        public int RejectedRequests { get; set; }

        public int TotalLeavesTaken { get; set; }
    }
}