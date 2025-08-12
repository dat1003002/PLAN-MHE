using AspnetCoreMvcFull.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using PLANMHE.Repositories.Interfaces;
using PLANMHE.Services.Interfaces;
using PLANMHE.Repositories;
using PLANMHE.Services;
using PLANMHE.Repository;
using PLANMHE.Service; // Thêm namespace này

var builder = WebApplication.CreateBuilder(args);

// Đăng ký IHttpContextAccessor
builder.Services.AddHttpContextAccessor(); // Thay vì AddSingleton trực tiếp

// Kết nối đến SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();
builder.Services.AddScoped<IUserTypeService, UserTypeService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IKehoachRepository, KehoachRepository>();
builder.Services.AddScoped<IKehoachService, KehoachService>();
// Đăng ký Controllers và Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Cấu hình pipeline
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Cấu hình route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboards}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
