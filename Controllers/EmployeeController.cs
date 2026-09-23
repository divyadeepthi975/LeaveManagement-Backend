
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
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            if (!User.IsInRole("Manager"))
            {
                return StatusCode(403, "You are not authorized to view all employees.");
            }

            var employees = await _employeeService.GetAllAsync();

            return Ok(employees);
        }


        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("You are not authenticated.");
            }

            if (!int.TryParse(userId, out int employeeId))
            {
                return Unauthorized("Invalid employee information.");
            }

            // Employee can only view their own details
            if (User.IsInRole("Employee") && employeeId != id)
            {
                return StatusCode(403, "Employees can view only their own details.");
            }

            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            return Ok(employee);
        }


        [HttpGet("me")]
        public async Task<IActionResult> GetMyDetails()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("You are not authenticated.");
            }

            if (!int.TryParse(userId, out int employeeId))
            {
                return Unauthorized("Invalid employee information.");
            }

            var employee = await _employeeService.GetByIdAsync(employeeId);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(
            int id,
            EmployeewithIDDTO employee)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("You are not authenticated.");
            }

            if (!User.IsInRole("Manager") &&
                !User.IsInRole("Employee"))
            {
                return StatusCode(403, "You are not authorized to perform this action.");
            }

            if (User.IsInRole("Employee") &&
                userId != id.ToString())
            {
                return StatusCode(403, "You are not authorized to update this employee.");
            }

            if (id != employee.EmployeeId)
            {
                return BadRequest("Employee Id mismatch.");
            }

            var updatedEmployee =
                await _employeeService.UpdateAsync(employee);

            if (updatedEmployee == null)
            {
                return NotFound("Employee is inactive or not found.");
            }

            return Ok(updatedEmployee);
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (!User.IsInRole("Manager"))
            {
                return StatusCode(403, "You are not authorized to delete an employee.");
            }

            var result = await _employeeService.DeleteAsync(id);

            return Ok(result);
        }
    }
}

