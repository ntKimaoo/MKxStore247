using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MKxStore247.Models;
using MKxStore247.Services.Interface;

namespace MKxStore247.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserApplicationService _userService;

        public UsersController(IUserApplicationService userService)
        {
            _userService = userService;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index(string? searchTerm = "")
        {
            //ViewData["Layout"] = GetActiveLayout();

            var users = string.IsNullOrEmpty(searchTerm)
                ? await _userService.GetAllUsersAsync()
                : await _userService.SearchUsersAsync(searchTerm);

            ViewBag.SearchTerm = searchTerm;
            return View(users);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json(new { success = false, message = "ID không hợp lệ" });

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy người dùng" });

            var userRoles = await _userService.GetUserRolesAsync(id);

            var result = new
            {
                success = true,
                data = new
                {
                    user.Id,
                    user.FullName,
                    user.UserName,
                    user.Email,
                    user.PhoneNumber,
                    user.IsActive,
                    user.CreatedAt,
                    user.AvatarUrl,
                    Roles = userRoles
                }
            };

            return Json(result);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            //ViewData["Layout"] = GetActiveLayout();
            return View(new UserApplication());
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserApplication user, string password)
        {
            //ViewData["Layout"] = GetActiveLayout();

            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUserAsync(user, password);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Tạo người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            //ViewData["Layout"] = GetActiveLayout();
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserApplication user)
        {
            if (id != user.Id)
                return NotFound();

            //ViewData["Layout"] = GetActiveLayout();

            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUserAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Cập nhật người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result.Succeeded)
            {
                TempData["Success"] = "Xóa người dùng thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi khi xóa người dùng!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Users/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var result = await _userService.ToggleUserStatusAsync(id);
            if (result.Succeeded)
            {
                TempData["Success"] = "Đã thay đổi trạng thái người dùng!";
            }
            else
            {
                TempData["Error"] = "Có lỗi khi thay đổi trạng thái!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Users/ManageRoles/5
        public async Task<IActionResult> ManageRoles(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            //ViewData["Layout"] = GetActiveLayout();
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            ViewBag.UserRoles = await _userService.GetUserRolesAsync(id);
            ViewBag.AllRoles = new[] { "Admin", "Manager", "User" }; // Customize as needed

            return View(user);
        }

        // POST: Admin/Users/AssignRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var result = await _userService.AddToRoleAsync(userId, role);
            if (result.Succeeded)
            {
                TempData["Success"] = "Đã gán quyền thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi khi gán quyền!";
            }

            return RedirectToAction(nameof(ManageRoles), new { id = userId });
        }

        // POST: Admin/Users/RemoveRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            var result = await _userService.RemoveFromRoleAsync(userId, role);
            if (result.Succeeded)
            {
                TempData["Success"] = "Đã xóa quyền thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi khi xóa quyền!";
            }

            return RedirectToAction(nameof(ManageRoles), new { id = userId });
        }

       
    }
}

