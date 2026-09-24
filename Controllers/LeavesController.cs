using LeaveManagement.DTO;
using LeaveManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeaveManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LeavesController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeavesController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }


        // POST: api/Leaves
        // Employee can apply for leave
        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> ApplyLeave(
            [FromBody] LeaverequestDTO leaverequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get logged-in employee ID from JWT
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("You are not authenticated.");
            }

            if (!int.TryParse(userId, out int employeeId))
            {
                return Unauthorized("Invalid employee information.");
            }

            // Employee can apply leave only for themselves
            if (employeeId != leaverequest.employeeid)
            {
                return StatusCode(
                    403,
                    "Employees can apply for leave only for themselves.");
            }

            try
            {
                var result =
                    await _leaveService.ApplyLeaveAsync(leaverequest);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result!.leaverequestid },
                    result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET: api/Leaves
        // Manager can view all leaves
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? employeeId,
            [FromQuery] int? leaveTypeId,
            [FromQuery] string? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result =
                await _leaveService.GetAllLeavesAsync(
                    employeeId,
                    leaveTypeId,
                    status,
                    fromDate,
                    toDate);

            return Ok(result);
        }


        // GET: api/Leaves/{id}
        // Manager can view any leave
        // Employee can view only their own leave
        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("You are not authenticated.");
            }

            if (!int.TryParse(userId, out int employeeId))
            {
                return Unauthorized("Invalid employee information.");
            }

            var result =
                await _leaveService.GetLeaveByIdAsync(id);

            if (result == null)
            {
                return NotFound(
                    $"Leave request with ID {id} not found.");
            }

            // Employee can only view their own leave
            if (User.IsInRole("Employee") &&
                result.employeeid != employeeId)
            {
                return StatusCode(
                    403,
                    "Employees can view only their own leave requests.");
            }

            LeaveRequestGetDTO leaverequest =
                new LeaveRequestGetDTO();

            leaverequest.leaverequestid =
                result.leaverequestid;

            leaverequest.employeeid =
                result.employeeid;

            leaverequest.leavetypeid =
                result.leavetypeid;

            leaverequest.fromdate =
                result.fromdate;

            leaverequest.todate =
                result.todate;

            leaverequest.reason =
                result.reason;

            leaverequest.status =
                result.status;

            leaverequest.applieddate =
                result.applieddate;

            leaverequest.approvedby =
                result.approvedby;

            leaverequest.comments =
                result.comments;

            return Ok(leaverequest);
        }


        // GET: /api/employees/{employeeId}/leaves
        // Manager can view any employee's leaves
        // Employee can view only their own leaves
        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("/api/employees/{employeeId}/leaves")]
        public async Task<IActionResult> GetEmployeeLeaves(
            int employeeId)
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

            // Employee can only view their own leaves
            if (User.IsInRole("Employee") &&
                loggedInEmployeeId != employeeId)
            {
                return StatusCode(
                    403,
                    "Employees can view only their own leave requests.");
            }

            var result =
                await _leaveService.GetEmployeeLeavesAsync(
                    employeeId);

            return Ok(result);
        }


        // PUT: api/Leaves/{id}/approve
        // Only Manager can approve leave
        [Authorize(Roles = "Manager")]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(
            int id,
            [FromBody] LeaveActionDTO leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Make sure URL ID and body ID match
            if (id != leave.id)
            {
                return BadRequest(
                    "Leave request ID in URL and request body do not match.");
            }

            try
            {
                var result =
                    await _leaveService.ApproveLeaveAsync(leave);

                if (result == null)
                {
                    return NotFound(
                        $"Leave request with ID {id} not found.");
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // PUT: api/Leaves/{id}/reject
        // Only Manager can reject leave
        [Authorize(Roles = "Manager")]
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(
            int id,
            [FromBody] LeaveActionDTO leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Make sure URL ID and body ID match
            if (id != leave.id)
            {
                return BadRequest(
                    "Leave request ID in URL and request body do not match.");
            }

            try
            {
                var result =
                    await _leaveService.RejectLeaveAsync(leave);

                if (result == null)
                {
                    return NotFound(
                        $"Leave request with ID {id} not found.");
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}