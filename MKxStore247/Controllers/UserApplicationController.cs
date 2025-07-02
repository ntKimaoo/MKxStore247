using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MKxStore247.Models;
using MKxStore247.Services.Interface;
using System.Security.Claims;

namespace MKxStore247.Controllers
{
    public class UserApplicationController : Controller
    {
        private readonly SignInManager<UserApplication> _signInManager;
        private readonly UserManager<UserApplication> _userManager;
        private readonly IUserApplicationService _userApplicationService;
        private readonly ILogger<UserApplication> _logger;

        public UserApplicationController(SignInManager<UserApplication> signInManager, UserManager<UserApplication> userManager, IUserApplicationService userApplicationService, ILogger<UserApplication> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userApplicationService = userApplicationService;
            _logger = logger;
        }
        [Route("/LoginFirstTime")]
        public IActionResult FirstLogin()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> RegisterComplete([FromForm] UserApplication userInfo)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


                var updatedUser = await _userApplicationService.CompleteFirstLoginAsync(userId, userInfo);

                if (updatedUser == null)
                {
                    return BadRequest("Cập nhật thông tin thất bại");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing first login");
                return StatusCode(500, "Lỗi hệ thống");
            }
        }
    }
}
