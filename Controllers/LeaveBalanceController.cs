using LeaveManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeaveManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/employees")]
    public class LeaveBalanceController : ControllerBase
    {
        private readonly LeaveBalanceService _leaveBalanceService;

        public LeaveBalanceController(LeaveBalanceService leaveBalanceService)
        {
            _leaveBalanceService = leaveBalanceService;
        }


        // GET: api/employees/{employeeId}/leave-balance
        // Manager can view any employee's leave balance
        // Employee can view only their own leave balance
        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("{employeeId}/leave-balance")]
        public async Task<IActionResult> GetLeaveBalance(int employeeId)
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("You are not authenticated.");
            }

            if (!int.TryParse(userId, out int loggedInEmployeeId))
            {
                return Unauthorized("Invalid employee information.");
            }

            // Employee can only view their own leave balance
            if (User.IsInRole("Employee") &&
                loggedInEmployeeId != employeeId)
            {
                return StatusCode(
                    403,
                    "Employees can view only their own leave balance.");
            }

            var result =
                await _leaveBalanceService.GetLeaveBalance(employeeId);

            return Ok(result);
        }
    }
}