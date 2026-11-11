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
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILeavetypeService _leavetypeService;

        public AdminController(IAdminService adminService, ILeavetypeService leavetypeService)
        {
            _adminService = adminService;
            _leavetypeService = leavetypeService;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("employees")]
        public async Task<IActionResult> CreateEmployee(
            [FromBody] CreateUserDTO dto)
        {
            try
            {
                var result = await _adminService.CreateUserAsync(
                    dto,
                    "Employee");

                return Ok(new
                {
                    message = "Employee created successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpPost("managers")]
        public async Task<IActionResult> CreateManager(
            [FromBody] CreateUserDTO dto)
        {
            try
            {
                var result = await _adminService.CreateUserAsync(
                    dto,
                    "Manager");

                return Ok(new
                {
                    message = "Manager created successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("users/{UserId}/change-to-manager")]
        public async Task<IActionResult> ChangeToManager(
            int employeeId)
        {
            try
            {
                var result =
                    await _adminService.ChangeToManagerAsync(
                        employeeId);

                if (!result)
                {
                    return NotFound("User not found.");
                }

                return Ok(
                    "User role changed to Manager successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("users/{UserId}/change-to-employee")]
        public async Task<IActionResult> ChangeToEmployee(
            int employeeId,
            [FromBody] ChangeToEmployeeDTO dto)
        {
            try
            {
                var result =
                    await _adminService.ChangeToEmployeeAsync(
                        employeeId,
                        dto.ManagerId);

                if (!result)
                {
                    return NotFound("User not found.");
                }

                return Ok(
                    "User role changed to Employee successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]

        [HttpPut("users/{UserId}/status")]
        public async Task<IActionResult> ChangeStatus(
            int employeeId,
            [FromBody] ChangeStatusDTO dto)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var adminId))
            {
                return Unauthorized("Invalid admin information.");
            }

            if (employeeId == adminId)
            {
                return BadRequest("You cannot change your own account status.");
            }

            try
            {
                var result =
                    await _adminService.ChangeStatusAsync(
                        employeeId,
                        dto);

                if (!result)
                {
                    return NotFound("User not found.");
                }

                if (dto.IsActive)
                {
                    return Ok("User activated successfully.");
                }

                return Ok("User deactivated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


[Authorize(Roles = "Admin")]
[HttpGet("employees/{UserId}")]
public async Task<IActionResult> GetEmployeeById(
    int employeeId)
        {
            try
            {
                var employee =
                    await _adminService.GetEmployeeByIdAsync(
                        employeeId);

                if (employee == null)
                {
                    return NotFound(
                        "Employee not found.");
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("employees/{UserId}")]
        public async Task<IActionResult> UpdateEmployee(
            int employeeId,
            [FromBody] EmployeewithIDDTO employee)
        {
            try
            {
                if (employeeId != employee.EmployeeId)
                {
                    return BadRequest(
                        "Employee ID in URL and request body do not match.");
                }
                if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var adminId))
                {
                    return Unauthorized("Invalid admin information.");
                }

                if (employeeId == adminId)
                {
                    return BadRequest(
                        "You cannot edit your own account.");
                }

                var result =
                    await _adminService.UpdateEmployeeAsync(
                        employee);

                if (result == null)
                {
                    return NotFound(
                        "Employee not found.");
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("leave-types")]
        public async Task<IActionResult> GetLeaveTypes()
        {
            var leaveTypes = await _leavetypeService.GetAllLeavetypeAsync();
            return Ok(leaveTypes);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("leave-types")]
        public async Task<IActionResult> CreateLeaveType([FromBody] LeavetypeDTO leaveType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _leavetypeService.AddAsync(leaveType);
                return CreatedAtAction(nameof(GetLeaveTypes), new { id = created.leavetypeid }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("leave-types/{id}")]
        public async Task<IActionResult> UpdateLeaveType(int id, [FromBody] LeavetypeIsactiveDTO leaveType)
        {
            if (id != leaveType.leavetypeid)
            {
                return BadRequest("ID in URL and request body do not match.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _leavetypeService.UpdateLeavetypeAsync(leaveType);
            if (updated == null)
            {
                return NotFound($"Leavetype with this ID-{id} not found.");
            }

            return Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("leave-types/{id}/activate")]
        public async Task<IActionResult> ActivateLeaveType(int id)
        {
            var result = await _leavetypeService.SetActiveStatusAsync(id, true);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("leave-types/{id}/deactivate")]
        public async Task<IActionResult> DeactivateLeaveType(int id)
        {
            var result = await _leavetypeService.SetActiveStatusAsync(id, false);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("leave-types/{id}")]
        public async Task<IActionResult> DeleteLeaveType(int id)
        {
            var result = await _leavetypeService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
