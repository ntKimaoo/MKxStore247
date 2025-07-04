using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MKxStore247.Data;
using MKxStore247.Services.Interface;

namespace MKxStore247.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryProduct : Controller
    {
        private readonly IUserApplicationService _userService;
        private readonly MKxStore247Context _context;
        public CategoryProduct(IUserApplicationService userService, MKxStore247Context context)
        {
            _userService = userService;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
