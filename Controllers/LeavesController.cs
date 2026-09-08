using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;
using LeaveManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeavesController : ControllerBase
    {
        private readonly LeaveService _leaveService;

        public LeavesController(LeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpPost]
        public async Task<IActionResult> ApplyLeave([FromBody] LeaverequestDTO leaverequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _leaveService.ApplyLeaveAsync(leaverequest);

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

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? employeeId,
            [FromQuery] int? leaveTypeId,
            [FromQuery] string? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await _leaveService.GetAllLeavesAsync(
                employeeId,
                leaveTypeId,
                status,
                fromDate,
                toDate);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _leaveService.GetLeaveByIdAsync(id);

            if (result == null)
                return NotFound($"Leave request with ID {id} not found.");

            return Ok(result);
        }

        [HttpGet("/api/employees/{employeeId}/leaves")]
        public async Task<IActionResult> GetEmployeeLeaves(int employeeId)
        {
            var result = await _leaveService.GetEmployeeLeavesAsync(employeeId);

            return Ok(result);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromBody]LeaveActionDTO leave)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _leaveService.ApproveLeaveAsync(leave);

                if (result == null)
                    return NotFound($"Leave request with ID {leave.id} not found.");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] LeaveActionDTO leave)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _leaveService.RejectLeaveAsync(leave);

                if (result == null)
                    return NotFound($"Leave request with ID {id} not found.");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}