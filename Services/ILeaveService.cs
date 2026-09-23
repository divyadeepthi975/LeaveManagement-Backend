using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;

namespace LeaveManagement.Services
{
    public interface ILeaveService
    {
        Task<LeaveRequestGetDTO?> ApplyLeaveAsync(
     LeaverequestDTO leaverequest);

        Task<IEnumerable<Leaverequest>> GetAllLeavesAsync(
            int? employeeId = null,
            int? leaveTypeId = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        Task<IEnumerable<LeaveRequestGetDTO>> GetEmployeeLeavesAsync(
            int employeeId);

        Task<LeaveRequestGetDTO?> GetLeaveByIdAsync(int id);

        Task<string?> ApproveLeaveAsync(LeaveActionDTO leave);

        Task<string?> RejectLeaveAsync(LeaveActionDTO leave);
    }
}