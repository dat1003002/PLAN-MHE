using AspnetCoreMvcFull.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AspnetCoreMvcFull.Repositories;
using AspnetCoreMvcFull.Repository;
using AspnetCoreMvcFull.Service;
using AspnetCoreMvcFull.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();


// Kết nối đến SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Đăng ký Repository và Service
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductLTRepository, ProductLTRepository>();
builder.Services.AddScoped<IProductLTService, ProductLTService>();
builder.Services.AddScoped<IProductCvRepository, ProductCvRepository>();
builder.Services.AddScoped<IProductCvService, ProductCvService>();
builder.Services.AddScoped<IProductCvCTLRepository, ProductCvCTLRepository>();
builder.Services.AddScoped<IProductCvCTLService, ProductCvCTLService>();
builder.Services.AddScoped<IProductCvGCRepository, ProductCvGCRepository>();
builder.Services.AddScoped<IProductCvGCService, ProductCvGCService>();
builder.Services.AddScoped<IProductCSDRepository, ProductCSDRepository>();
builder.Services.AddScoped<IProductCSDService, ProductCSDService>();
builder.Services.AddScoped<IProductCSCTLRepository, ProductCSCTLRepository>();
builder.Services.AddScoped<IProductCSCTLService, ProductCSCTLService>();
builder.Services.AddScoped<IGangCauCTLRepository, GangCauCTLRepository>();
builder.Services.AddScoped<IGangCauCTLService, GangCauCTLService>();
builder.Services.AddScoped<ILuuHoaCTLRepository, LuuHoaCTLRepository>();
builder.Services.AddScoped<ILuuHoaCTLSevice, LuuHoaCTLSevice>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboards}/{action=Index}/{id?}");

app.Run();
