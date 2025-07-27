using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MKxStore247.Data;
using MKxStore247.Models;
using MKxStore247.Models.HelperModel;
using MKxStore247.Services;
using MKxStore247.Services.Implementation;
using MKxStore247.Services.Interface;

namespace MKxStore247
{
    public class program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("MKxStore247ContextConnection") ?? throw new InvalidOperationException("Connection string 'MKxStore247ContextConnection' not found.");

            builder.Services.AddDbContext<MKxStore247Context>(options => options.UseSqlServer(connectionString));

            
            builder.Services.AddDefaultIdentity<UserApplication>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            }).AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<MKxStore247Context>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("CloudinarySetting"));
            builder.Services.AddScoped<IUserApplicationService, UserApplicationService>();
            builder.Services.AddScoped<ICategoryProductService, CategoryProductService>();
            builder.Services.AddScoped<IShopService, ShopService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IFileUploadService, FileUploadService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedRolesAndAdminAsync(services);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.MapRazorPages();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapAreaControllerRoute(
                name: "admin",
                areaName: "Admin",
                pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static readonly string[] Roles = new[] { "Admin", "Customer", "Seller" };

        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<UserApplication>>();

            // Tạo Roles nếu chưa có
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Tạo user Admin mặc định
            string adminEmail = "admin";
            string adminPassword = "1234qwer!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail+"@gmail.com");
            if (adminUser == null)
            {
                adminUser = new UserApplication
                {
                    UserName = adminEmail,
                    Email = adminEmail+"@gmail.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "System"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

    }
}

