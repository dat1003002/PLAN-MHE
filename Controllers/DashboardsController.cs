using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AspnetCoreMvcFull.Controllers;

public class DashboardsController : Controller
{
  [Authorize]
  public IActionResult Index()
  {
    return View();
  }

  [HttpPost]
  public async Task<IActionResult> Logout()
  {
    // Đăng xuất người dùng bằng cách xóa cookie xác thực
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    // Thêm các tiêu đề để ngăn lưu cache
    Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    Response.Headers["Pragma"] = "no-cache";
    Response.Headers["Expires"] = "0";

    // Trả về OK để JavaScript phía client xử lý chuyển hướng
    return Ok();
  }
}
