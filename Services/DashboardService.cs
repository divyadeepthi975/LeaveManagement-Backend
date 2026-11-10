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

        public async Task<DashboardDTO> GetDashboardSummary(int? managerId = null, string? managerUsername = null)
        {
            var employeeQuery = _db.Employees.AsQueryable();
            var leaveQuery = _db.Leaverequests.AsQueryable();

            if (managerId.HasValue)
            {
                employeeQuery = employeeQuery.Where(e => e.ManagerId == managerId.Value);
                leaveQuery = leaveQuery.Where(l => _db.Employees.Any(e =>
                   e.EmployeeId == l.employeeid && e.ManagerId == managerId.Value));

            }

            int totalEmployees = await employeeQuery.CountAsync();
            int activeEmployees = managerId.HasValue
                ? await employeeQuery.CountAsync(e => e.IsActive)
                : await employeeQuery.CountAsync();

            int pendingRequests = await leaveQuery
                .CountAsync(l => l.status == "Pending");

            int approvedRequests = await leaveQuery.CountAsync(l =>
                l.status == "Approved" &&
                (!managerId.HasValue || l.approvedby == managerUsername));

            int rejectedRequests = await leaveQuery.CountAsync(l =>
                l.status == "Rejected" &&
                (!managerId.HasValue || l.approvedby == managerUsername));

            var approvedLeaves = await leaveQuery
                    .Where(l => l.status == "Approved" &&
                        (!managerId.HasValue || l.approvedby == managerUsername))
                    .Select(l => new
                    {
                        l.fromdate,
                        l.todate
                    })
                    .ToListAsync();

                    var totalApprovedDays = approvedLeaves.Sum(l => (l.todate - l.fromdate).Days + 1);
            return new DashboardDTO
            {
                TotalEmployees = totalEmployees,
                ActiveEmployees = activeEmployees,
                PendingRequests = pendingRequests,
                ApprovedRequests = approvedRequests,
                RejectedRequests = rejectedRequests,
                TotalLeavesTaken = totalApprovedDays
            };
        }
    }
}