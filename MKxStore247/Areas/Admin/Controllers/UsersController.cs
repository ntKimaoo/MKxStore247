using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MKxStore247.Data;
using MKxStore247.Models;
using MKxStore247.Services.Interface;
using PagedList;

namespace MKxStore247.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserApplicationService _userService;
        private readonly MKxStore247Context _context;
        public UsersController(IUserApplicationService userService, MKxStore247Context context)
        {
            _userService = userService;
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index(string? searchTerm, string? statusFilter, string? dateFilter,
    string? sortBy = "created", string? sortOrder = "desc", int pageSize = 10, int page = 1,
    DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Users.AsQueryable();
            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.UserName.Contains(searchTerm) ||
                                         u.Email.Contains(searchTerm) ||
                                         u.PhoneNumber.Contains(searchTerm));

            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                bool isActive = statusFilter == "active";
                query = query.Where(u => u.IsActive == isActive);
            }

            // Apply date filters
            if (!string.IsNullOrEmpty(dateFilter))
            {
                DateTime filterDate = DateTime.Now;
                switch (dateFilter)
                {
                    case "today":
                        query = query.Where(u => u.CreatedAt.Date == filterDate.Date);
                        break;
                    case "week":
                        var startOfWeek = filterDate.AddDays(-(int)filterDate.DayOfWeek);
                        query = query.Where(u => u.CreatedAt >= startOfWeek);
                        break;
                    case "month":
                        query = query.Where(u => u.CreatedAt.Month == filterDate.Month &&
                                                u.CreatedAt.Year == filterDate.Year);
                        break;
                    case "year":
                        query = query.Where(u => u.CreatedAt.Year == filterDate.Year);
                        break;
                }
            }

            if (fromDate.HasValue)
                query = query.Where(u => u.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(u => u.CreatedAt <= toDate.Value);

            // Apply sorting
            switch (sortBy)
            {
                case "name":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.FullName) : query.OrderByDescending(u => u.FullName);
                    break;
                case "email":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email);
                    break;
                case "status":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.IsActive) : query.OrderByDescending(u => u.IsActive);
                    break;
                default:
                    query = sortOrder == "asc" ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt);
                    break;
            }

            // Pass data to ViewBag
            ViewBag.SearchTerm = searchTerm;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.DateFilter = dateFilter;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;
            ViewBag.PageSize = pageSize;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.TotalUsers = await _context.Users.CountAsync();

            var pagedUsers = query.ToPagedList(page, pageSize);
            return View(pagedUsers);
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

