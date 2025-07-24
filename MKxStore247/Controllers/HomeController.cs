using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MKxStore247.Models;
using MKxStore247.Services.Interface;

namespace MKxStore247.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<UserApplication> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IShopService _shopService;
        private readonly ICategoryProductService _categoryProductService;
        public HomeController(ILogger<HomeController> logger, UserManager<UserApplication> userManager, RoleManager<IdentityRole> roleManager, IShopService shopService, ICategoryProductService categoryProductService)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _shopService = shopService;
            _categoryProductService = categoryProductService;
        }

        public async Task<IActionResult> Index()
        {
            var user =await _userManager.GetUserAsync(User);

            if (user != null && user.IsFirstLogin)
            {
                return Redirect($"/LoginFirstTime");
            }

            if (user != null)
            {
                var role = await _userManager.GetRolesAsync(user);
                if (role.Contains("Admin"))
                {
                    return Redirect("/Admin");
                }
            }
            var categories = await _categoryProductService.GetAllCategoryAsync();
            ViewBag.listCates = categories;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
