using LeaveManagement.Data;
using LeaveManagement.DTO;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _db;

        public DashboardService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardDTO> GetDashboardSummary()
        {
            int totalEmployees = await _db.Employees.CountAsync();

            int activeEmployees = await _db.Employees.CountAsync();

            int pendingRequests = await _db.Leaverequests
                .CountAsync(l => l.status == "Pending");

            int approvedRequests = await _db.Leaverequests
                .CountAsync(l => l.status == "Approved");

            int rejectedRequests = await _db.Leaverequests
                .CountAsync(l => l.status == "Rejected");

            int totalLeavesTaken = await _db.Leaverequests
                .Where(l => l.status == "Approved")
                .SumAsync(l=>(l.todate - l.fromdate).Days + 1);

            return new DashboardDTO
            {
                TotalEmployees = totalEmployees,
                ActiveEmployees = activeEmployees,
                PendingRequests = pendingRequests,
                ApprovedRequests = approvedRequests,
                RejectedRequests = rejectedRequests,
                TotalLeavesTaken = totalLeavesTaken
            };
        }
    }
}