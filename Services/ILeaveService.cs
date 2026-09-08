using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;

namespace LeaveManagement.Services
{
    public interface ILeaveService
    {
        Task<Leaverequest?> ApplyLeaveAsync(LeaverequestDTO leaverequest);

        Task<IEnumerable<Leaverequest>> GetAllLeavesAsync(
            int? employeeId = null,
            int? leaveTypeId = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        Task<IEnumerable<Leaverequest>> GetEmployeeLeavesAsync(
            int employeeId);

        Task<Leaverequest?> GetLeaveByIdAsync(
            int id);

        Task<Leaverequest?> ApproveLeaveAsync(LeaveActionDTO leave);

        Task<Leaverequest?> RejectLeaveAsync(LeaveActionDTO leave);
    }
}