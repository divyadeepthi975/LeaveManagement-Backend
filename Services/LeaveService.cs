using LeaveManagement.Data;
using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly AppDbContext _db;

        public LeaveService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<LeaveRequestGetDTO?> ApplyLeaveAsync(LeaverequestDTO leaverequest)
        {
            var employee = await _db.Employees
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == leaverequest.employeeid);

            if (employee == null || employee.IsActive==false)
            {
                throw new ArgumentException("Employee not found or inactive.");
            }

            var leavetype = await _db.Leavetypes
                .FirstOrDefaultAsync(l =>
                    l.leavetypeid == leaverequest.leavetypeid);

            if (leavetype == null || leavetype.IsActive==false)
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
            Leaverequest leave = new Leaverequest();
            leave.employeeid= leaverequest.employeeid;
            leave.leavetypeid= leaverequest.leavetypeid;
            leave.fromdate= leaverequest.fromdate;
            leave.todate= leaverequest.todate;
            leave.reason= leaverequest.reason;

            _db.Leaverequests.Add(leave);

            await _db.SaveChangesAsync();

            return MapToDTO(leave);
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

        public async Task<IEnumerable<LeaveRequestGetDTO>>GetEmployeeLeavesAsync(int employeeId)
        {
            var leaves = await _db.Leaverequests
                .Where(l => l.employeeid == employeeId)
                .OrderByDescending(l => l.applieddate)
                .ToListAsync();

            return leaves.Select(MapToDTO).ToList();
        }

        public async Task<LeaveRequestGetDTO?> GetLeaveByIdAsync(int id)
        {
            var leave = await _db.Leaverequests
                .FirstOrDefaultAsync(l =>
                    l.leaverequestid == id);

            if (leave == null)
            {
                return null;
            }

            return MapToDTO(leave);
        }

        public async Task<string?> ApproveLeaveAsync(LeaveActionDTO leave)
        {
            var leaveRequest = await _db.Leaverequests
                .FirstOrDefaultAsync(l =>
                    l.leaverequestid == leave.id);

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

            int numberOfDays =(leaveRequest.todate.Date-leaveRequest.fromdate.Date).Days + 1;

            int remainingDays =balance.totaldays - balance.useddays;

            if (numberOfDays > remainingDays)
            {
                throw new ArgumentException("Insufficient leave balance.");
            }

            leaveRequest.status = "Approved";
            leaveRequest.approvedby = leave.approvedby;
            leaveRequest.comments = leave.comments;

            balance.useddays += numberOfDays;

            await _db.SaveChangesAsync();

            return "Your Leave is approved";
        }

        public async Task<string?> RejectLeaveAsync(LeaveActionDTO leave)
        {
            var leaveRequest = await _db.Leaverequests
                .FirstOrDefaultAsync(l =>
                    l.leaverequestid == leave.id);

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
            leaveRequest.approvedby = "Rejected";
            leaveRequest.comments = leave.comments;

            await _db.SaveChangesAsync();

            return "Your Leave is rejected";
        }
        private LeaveRequestGetDTO MapToDTO(Leaverequest leave)
        {
            return new LeaveRequestGetDTO
            {
                leaverequestid = leave.leaverequestid,
                employeeid = leave.employeeid,
                leavetypeid = leave.leavetypeid,
                fromdate = leave.fromdate,
                todate = leave.todate,
                reason = leave.reason,
                status = leave.status,
                applieddate = leave.applieddate,
                approvedby = leave.approvedby,
                comments = leave.comments
            };
        }
    }
}