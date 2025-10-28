using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using PLANMHE.Service;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AspnetCoreMvcFull.Controllers
{
  [Authorize]
  public class DashboardsController : Controller
  {
    private readonly IDashboardsService _dashboardsService;

    public DashboardsController(IDashboardsService dashboardsService)
    {
      _dashboardsService = dashboardsService;
    }

    public async Task<IActionResult> Index()
    {
      var recentPlans = await _dashboardsService.GetTop5RecentPlansWithStatusAsync();

      // Lấy dòng tóm tắt (dòng cuối cùng)
      var summary = recentPlans.LastOrDefault() ?? new Dictionary<string, object>();

      // Truyền dữ liệu cho View
      ViewData["RecentPlans"] = recentPlans;
      ViewData["TotalPlans"] = summary.GetValueOrDefault("TotalPlans", 0);
      ViewData["CompletedPlans"] = summary.GetValueOrDefault("CompletedPlans", 0);
      ViewData["ActivePlans"] = summary.GetValueOrDefault("ActivePlans", 0);

      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
      Response.Headers["Pragma"] = "no-cache";
      Response.Headers["Expires"] = "0";
      return Ok();
    }
  }
}
