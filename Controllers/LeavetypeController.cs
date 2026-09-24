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
        [Authorize(Roles = "Manager,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var leavetypes =
                await _leavetypeService.GetAllLeavetypeAsync();

            return Ok(leavetypes);
        }


        // GET: api/Leavetypes/1
        // Manager and Employee can view a leave type
        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
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
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Create(LeavetypeDTO leavetype)
        {
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
        [Authorize(Roles = "Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            LeavetypeIsactiveDTO leavetype)
        {
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


        // DELETE: api/Leavetypes/1
        // Only Manager can delete a leave type
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result =
                await _leavetypeService.DeleteAsync(id);

            return Ok(result);
        }
    }
}