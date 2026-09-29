using LeaveManagement.DTO;
using LeaveManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
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
        [HttpPut("users/{employeeId}/change-to-manager")]
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

        [HttpPut("users/{employeeId}/change-to-employee")]
        public async Task<IActionResult> ChangeToEmployee(
            int employeeId)
        {
            try
            {
                var result =
                    await _adminService.ChangeToEmployeeAsync(
                        employeeId);

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

        [HttpPut("users/{employeeId}/status")]
        public async Task<IActionResult> ChangeStatus(
            int employeeId,
            [FromBody] ChangeStatusDTO dto)
        {
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
[HttpGet("employees/{employeeId}")]
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
        [HttpPut("employees/{employeeId}")]
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
                if (employeeId == 1)
                {
                    return BadRequest(
                        "You can't edit this employee");
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




    }
}