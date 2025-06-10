using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MKxStore247.Data;
using MKxStore247.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MKxStore247ContextConnection") ?? throw new InvalidOperationException("Connection string 'MKxStore247ContextConnection' not found.");

builder.Services.AddDbContext<MKxStore247Context>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<UserApplication>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MKxStore247Context>();

// Add services to the container.
builder.Services.AddControllersWithViews();


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
