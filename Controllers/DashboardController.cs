using LeaveManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("leave-summary")]
        public async Task<IActionResult> GetLeaveSummary()
        {
            var result = await _dashboardService.GetDashboardSummary();

            return Ok(result);
        }
    }
}