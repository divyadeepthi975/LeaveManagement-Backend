using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;

namespace LeaveManagement.Services
{
    public interface ILeaveService
    {
        Task<LeaveRequestGetDTO?> ApplyLeaveAsync(
     LeaverequestDTO leaverequest);

        Task<IEnumerable<LeaveRequestGetDTO>> GetAllLeavesAsync(
            int? employeeId = null,
            int? leaveTypeId = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? managerId = null);

        Task<IEnumerable<LeaveRequestGetDTO>> GetEmployeeLeavesAsync(
            int employeeId,
            int? managerId = null);

        Task<LeaveRequestGetDTO?> GetLeaveByIdAsync(int id, int? managerId = null);

        Task<string?> ApproveLeaveAsync(LeaveActionDTO leave, int managerId);

        Task<string?> RejectLeaveAsync(LeaveActionDTO leave, int managerId);
    }
}