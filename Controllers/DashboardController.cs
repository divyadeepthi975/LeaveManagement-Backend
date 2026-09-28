using LeaveManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var result = await _dashboardService.GetDashboardSummary();

            return Ok(result);
        }
    }
}