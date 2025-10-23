using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PLANMHE.Service;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult LoginBasic(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginBasic(string username, string password, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var user = await _authService.AuthenticateAsync(username, password);
            if (user == null)
            {
                ViewData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            // Tạo claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserTypeId", user.UserTypeId.ToString()) // Thêm claim UserTypeId
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Lưu phiên đăng nhập vào cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = Request.Form["remember-me"] == "on",
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                AllowRefresh = true
            });

            // Luôn chuyển hướng đến /Dashboards/Index
            return RedirectToAction("Index", "Dashboards");
        }
    }
}
