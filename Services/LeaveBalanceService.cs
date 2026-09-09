using LeaveManagement.Data;
using LeaveManagement.DTO;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class LeaveBalanceService
    {
        private readonly AppDbContext _db;

        public LeaveBalanceService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<LeaveBalanceDTO>> GetLeaveBalance(int employeeId)
        {
            var employeeExists = await _db.Employees.AnyAsync(e => e.EmployeeId == employeeId);

            if (!employeeExists)
            {
                throw new KeyNotFoundException(
                    $"Employee with ID {employeeId} not found.");
            }

            var balances = await _db.Leavebalances
                .Where(lb => lb.employeeid == employeeId)
                .Include(lb=>lb.Leavetype)
                .Select(lb => new LeaveBalanceDTO
                {
                    LeaveType = lb.Leavetype.leavetypename,
                    TotalDays = lb.totaldays,
                    UsedDays = lb.useddays,
                    RemainingDays = lb.totaldays - lb.useddays
                })
                .ToListAsync();

            return balances;
        }
    }
}