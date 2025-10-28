using AspnetCoreMvcFull.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PLANMHE.Repository;
using PLANMHE.Service;
using PLANMHE.Repositories;
using PLANMHE.Services;
using PLANMHE.Repositories.Interfaces;
using PLANMHE.Services.Interfaces;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
      options.LoginPath = "/Auth/LoginBasic";
      options.LogoutPath = "/Auth/Logout";
      options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
      options.SlidingExpiration = true;
    });

builder.Services.AddHttpContextAccessor();

// Kết nối đến SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Đăng ký các repository và service
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();
builder.Services.AddScoped<IUserTypeService, UserTypeService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IKehoachRepository, KehoachRepository>();
builder.Services.AddScoped<IKehoachService, KehoachService>();
builder.Services.AddScoped<ITHPlanRepository, THPlanRepository>();
builder.Services.AddScoped<ITHPlanService, THPlanService>();
builder.Services.AddScoped<IDetailkehoachReposive, DetailkehoachReposive>();
builder.Services.AddScoped<IDetailkehoachService, DetailkehoachService>();
builder.Services.AddScoped<ILichSuPlanReponsitory, LichSuPlanReponsitory>();
builder.Services.AddScoped<ILichSuPlanService, LichSuPlanService>();
builder.Services.AddScoped<IDashboardsRepository, DashboardsRepository>();
builder.Services.AddScoped<IDashboardsService, DashboardsService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
// EPPlus License (Non-Commercial)
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=LoginBasic}/{id?}");
app.MapRazorPages();

// Tạo tài khoản admin
using (var scope = app.Services.CreateScope())
{
  var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
  await authService.CreateAdminUserIfNotExistsAsync();
}

app.Run();
