
using LeaveManagement.DTO;
using LeaveManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LeavetypesController : ControllerBase
    {
        private readonly ILeavetypeService _leavetypeService;

        public LeavetypesController(ILeavetypeService leavetypeService)
        {
            _leavetypeService = leavetypeService;
        }

        // GET: api/Leavetypes
        // Manager and Employee can view all leave types
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!User.IsInRole("Manager") &&
                !User.IsInRole("Employee"))
            {
                return StatusCode(403,
                    "You are not authorized to view leave types.");
            }

            var leavetypes =
                await _leavetypeService.GetAllLeavetypeAsync();

            return Ok(leavetypes);
        }


        // GET: api/Leavetypes/1
        // Manager and Employee can view a leave type
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!User.IsInRole("Manager") &&
                !User.IsInRole("Employee"))
            {
                return StatusCode(403,
                    "You are not authorized to view this leave type.");
            }

            var leavetype =
                await _leavetypeService.GetLeavetypeAsync(id);

            if (leavetype == null)
            {
                return NotFound(
                    $"Leave type with ID {id} not found.");
            }

            return Ok(leavetype);
        }


        // POST: api/Leavetypes
        // Only Manager can create a leave type
        [HttpPost]
        public async Task<IActionResult> Create(LeavetypeDTO leavetype)
        {
            if (!User.IsInRole("Manager"))
            {
                return StatusCode(403,
                    "Only Managers are authorized to create leave types.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await _leavetypeService.AddAsync(leavetype);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.leavetypeid },
                result);
        }


        // PUT: api/Leavetypes/1
        // Only Manager can update a leave type
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            LeavetypeIsactiveDTO leavetype)
        {
            if (!User.IsInRole("Manager"))
            {
                return StatusCode(403,
                    "Only Managers are authorized to update leave types.");
            }

            if (id != leavetype.leavetypeid)
            {
                return BadRequest(
                    "ID in URL and request body do not match.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await _leavetypeService.UpdateLeavetypeAsync(leavetype);

            if (result == null)
            {
                return NotFound(
                    $"Leave type with ID {id} not found.");
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("Manager"))
            {
                return StatusCode(403,
                    "Only Managers are authorized to delete leave types.");
            }

            var result =
                await _leavetypeService.DeleteAsync(id);

            return Ok(result);
        }
    }
}

