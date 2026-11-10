using LeaveManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeaveManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;
        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("leave-summary")]
        public async Task<IActionResult> GetLeaveSummary()
        {
            int? managerId = null;
            string? managerUsername = null;

            if (User.IsInRole("Manager"))
            {
                if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedManagerId))
                {
                    return Unauthorized("Invalid manager information.");
                }

                managerId = parsedManagerId;
                managerUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            }

            var result = await _dashboardService.GetDashboardSummary(managerId, managerUsername);

            return Ok(result);
        }
    }
}