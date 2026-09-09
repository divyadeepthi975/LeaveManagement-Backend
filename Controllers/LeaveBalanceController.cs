using LeaveManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class LeaveBalanceController : ControllerBase
    {
        private readonly LeaveBalanceService _leaveBalanceService;

        public LeaveBalanceController(LeaveBalanceService leaveBalanceService)
        {
            _leaveBalanceService = leaveBalanceService;
        }

        [HttpGet("{employeeId}/leave-balance")]
        public async Task<IActionResult> GetLeaveBalance(int employeeId)
        {
            var result = await _leaveBalanceService
                .GetLeaveBalance(employeeId);

            return Ok(result);
        }
    }
}