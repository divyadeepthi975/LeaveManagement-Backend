using LeaveManagement.Data;
using LeaveManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class LeaveService
    {
        private readonly AppDbContext _db;

        public LeaveService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Leaverequest?> ApplyLeaveAsync(Leaverequest leaverequest)
        {
            var employee = await _db.Employees
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == leaverequest.employeeid);

            if (employee == null)
            {
                throw new ArgumentException("Employee not found or inactive.");
            }

            var leavetype = await _db.Leavetypes
                .FirstOrDefaultAsync(l =>
                    l.leavetypeid == leaverequest.leavetypeid);

            if (leavetype == null)
            {
                throw new ArgumentException("Leave type not found or inactive.");
            }

            if (leaverequest.fromdate > leaverequest.todate)
            {
                throw new ArgumentException(
                    "From date cannot be after To date.");
            }

            int numberOfDays =
                (leaverequest.todate.Date -
                 leaverequest.fromdate.Date).Days + 1;

            var balance = await _db.Leavebalances
                .FirstOrDefaultAsync(b =>
                    b.employeeid == leaverequest.employeeid &&
                    b.leavetypeid == leaverequest.leavetypeid);

            if (balance == null)
            {
                throw new ArgumentException(
                    "Leave balance not found.");
            }

            int remainingDays =
                balance.totaldays - balance.useddays;

            if (numberOfDays > remainingDays)
            {
                throw new ArgumentException(
                    $"Insufficient leave balance. Remaining days: {remainingDays}.");
            }

            var overlappingLeave = await _db.Leaverequests
                .AnyAsync(l =>
                    l.employeeid == leaverequest.employeeid &&
                    l.status != "Rejected" &&
                    leaverequest.fromdate.Date <= l.todate.Date &&
                    leaverequest.todate.Date >= l.fromdate.Date);

            if (overlappingLeave)
            {
                throw new ArgumentException(
                    "Leave request overlaps with an existing leave period.");
            }

            leaverequest.status = "Pending";
            leaverequest.applieddate = DateTime.Now;
            leaverequest.approvedby = null;
            leaverequest.comments = null;

            _db.Leaverequests.Add(leaverequest);

            await _db.SaveChangesAsync();

            return leaverequest;
        }

        public async Task<IEnumerable<Leaverequest>> GetAllLeavesAsync(
            int? employeeId = null,
            int? leaveTypeId = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _db.Leaverequests.AsQueryable();

            if (employeeId.HasValue)
            {
                query = query.Where(l =>
                    l.employeeid == employeeId.Value);
            }

            if (leaveTypeId.HasValue)
            {
                query = query.Where(l =>
                    l.leavetypeid == leaveTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(l =>
                    l.status == status);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(l =>
                    l.fromdate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(l =>
                    l.todate <= toDate.Value);
            }

            return await query
                .OrderByDescending(l => l.applieddate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Leaverequest>> GetEmployeeLeavesAsync(
            int employeeId)
        {
            return await _db.Leaverequests
                .Where(l => l.employeeid == employeeId)
                .OrderByDescending(l => l.applieddate)
                .ToListAsync();
        }

        public async Task<Leaverequest?> GetLeaveByIdAsync(int id)
        {
            return await _db.Leaverequests
                .FirstOrDefaultAsync(l =>
                    l.leaverequestid == id);
        }

        public async Task<Leaverequest?> ApproveLeaveAsync(
            int id,
            string approvedBy,
            string? comments)
        {
            var leaveRequest = await _db.Leaverequests
                .FirstOrDefaultAsync(l =>
                    l.leaverequestid == id);

            if (leaveRequest == null)
            {
                return null;
            }

            if (leaveRequest.status != "Pending")
            {
                throw new ArgumentException(
                    "Only pending leave requests can be approved.");
            }

            var balance = await _db.Leavebalances
                .FirstOrDefaultAsync(b =>
                    b.employeeid == leaveRequest.employeeid &&
                    b.leavetypeid == leaveRequest.leavetypeid);

            if (balance == null)
            {
                throw new ArgumentException(
                    "Leave balance not found.");
            }

            int numberOfDays =
                (leaveRequest.todate.Date -
                 leaveRequest.fromdate.Date).Days + 1;

            int remainingDays =
                balance.totaldays - balance.useddays;

            if (numberOfDays > remainingDays)
            {
                throw new ArgumentException(
                    "Insufficient leave balance.");
            }

            leaveRequest.status = "Approved";
            leaveRequest.approvedby = approvedBy;
            leaveRequest.comments = comments;

            balance.useddays += numberOfDays;

            await _db.SaveChangesAsync();

            return leaveRequest;
        }

        public async Task<Leaverequest?> RejectLeaveAsync(
            int id,
            string approvedBy,
            string? comments)
        {
            var leaveRequest = await _db.Leaverequests
                .FirstOrDefaultAsync(l =>
                    l.leaverequestid == id);

            if (leaveRequest == null)
            {
                return null;
            }

            if (leaveRequest.status != "Pending")
            {
                throw new ArgumentException(
                    "Only pending leave requests can be rejected.");
            }

            leaveRequest.status = "Rejected";
            leaveRequest.approvedby = approvedBy;
            leaveRequest.comments = comments;

            await _db.SaveChangesAsync();

            return leaveRequest;
        }
    }
}