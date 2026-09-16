using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;
using LeaveManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
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
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeDTO employee)
        {
            var createdEmployee = await _employeeService.AddAsync(employee);

            return CreatedAtAction(
                nameof(GetEmployeeById),
                new { id = createdEmployee.EmployeeId },
                createdEmployee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest("Employee Id mismatch.");
            }

            var updatedEmployee = await _employeeService.UpdateAsync(employee);

            if (updatedEmployee == null)
            {
                return NotFound();
            }

            return Ok(updatedEmployee);
        }

        [HttpDelete("{id}")]
        public async Task<string> DeleteEmployee(int id)
        {
            var result = await _employeeService.DeleteAsync(id);

            return result;
        }
    }
}