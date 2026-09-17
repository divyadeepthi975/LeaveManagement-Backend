using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;
using LeaveManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var leavetypes = await _leavetypeService.GetAllLeavetypeAsync();

            return Ok(leavetypes);
        }

        // GET: api/Leavetypes/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var leavetype = await _leavetypeService.GetLeavetypeAsync(id);

            if (leavetype == null)
            {
                return NotFound($"Leave type with ID {id} not found.");
            }

            return Ok(leavetype);
        }

        // POST: api/Leavetypes
        [HttpPost]
        public async Task<IActionResult> Create(LeavetypeDTO leavetype)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _leavetypeService.AddAsync(leavetype);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.leavetypeid },
                result);
        }

        // PUT: api/Leavetypes/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, LeavetypeIsactiveDTO leavetype)
        {
            if (id != leavetype.leavetypeid)
            {
                return BadRequest("ID in URL and request body do not match.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _leavetypeService.UpdateLeavetypeAsync(leavetype);

            if (result == null)
            {
                return NotFound($"Leave type with ID {id} not found.");
            }

            return Ok(result);
        }

        // DELETE: api/Leavetypes/1
        [HttpDelete("{id}")]
        public async Task<string> Delete(int id)
        {
            var result = await _leavetypeService.DeleteAsync(id);
            return result;
        }
    }
}

