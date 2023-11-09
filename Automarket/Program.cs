using Microsoft.EntityFrameworkCore;
using Automarket.DataAccessLayer;
using Automarket.DataAccessLayer.Inerfaces;
using Automarket.DataAccessLayer.Repositories;
using Automarket.Service.Interfaces;
using Automarket.Service.Implementations;
using Automarket.Domain.Entity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Automarket;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => 
                        options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                        .UseSqlServer(connection, b => b.MigrationsAssembly("Automarket.DataAccessLayer")));
//options.UseSqlServer(connection, b => b.MigrationsAssembly("Automarket"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Account/Login");
        options.AccessDeniedPath = new PathString("/Account/Login");
    });

builder.Services.InitializeRepositories();
builder.Services.InitializeServices();
builder.Services.AddEntityFrameworkSqlServer();

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
