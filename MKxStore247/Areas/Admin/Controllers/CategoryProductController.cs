using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MKxStore247.Data;
using MKxStore247.Models;
using MKxStore247.Services.Interface;

namespace MKxStore247.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryProductController : Controller
    {
        private readonly MKxStore247Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryProductController(MKxStore247Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string searchTerm = "", bool? isActive = null)
        {
            ViewData["Title"] = "Quản lý danh mục";
            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentIsActive"] = isActive;

            var categoriesQuery = _context.CategoryProduct.AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                categoriesQuery = categoriesQuery.Where(c =>
                    c.CategoryName.Contains(searchTerm) ||
                    c.Description.Contains(searchTerm));
            }

            // Lọc theo trạng thái
            if (isActive.HasValue)
            {
                categoriesQuery = categoriesQuery.Where(c => c.IsActive == isActive.Value);
            }

            categoriesQuery = categoriesQuery.OrderByDescending(c => c.CreatedAt);

            var totalItems = await categoriesQuery.CountAsync();
            var categories = await categoriesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["TotalItems"] = totalItems;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(categories);
        }

        // GET: Admin/CategoryProduct/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryProduct = await _context.CategoryProduct
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.CategoryId == id);

            if (categoryProduct == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsModal", categoryProduct);
        }

        // GET: Admin/CategoryProduct/Create
        public IActionResult Create()
        {
            return PartialView("_CreateEditModal", new CategoryProduct());
        }

        // POST: Admin/CategoryProduct/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryProduct categoryProduct, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý upload ảnh
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "categories");
                        Directory.CreateDirectory(uploadsPath);

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        categoryProduct.ImageUrl = "/images/categories/" + fileName;
                    }

                    _context.Add(categoryProduct);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Thêm danh mục thành công!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
                }
            }
            var allErrors = ModelState
                .SelectMany(x => x.Value.Errors.Select(error => new { Field = x.Key, Error = error.ErrorMessage }))
                .ToList();

            return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = allErrors });
        }

        // GET: Admin/CategoryProduct/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryProduct = await _context.CategoryProduct.FindAsync(id);
            if (categoryProduct == null)
            {
                return NotFound();
            }

            return PartialView("_CreateEditModal", categoryProduct);
        }

        // POST: Admin/CategoryProduct/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,Description,ImageUrl,IsActive,CreatedAt")] CategoryProduct categoryProduct, IFormFile? imageFile)
        {
            if (id != categoryProduct.CategoryId)
            {
                return Json(new { success = false, message = "ID không khớp" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý upload ảnh mới
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(categoryProduct.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, categoryProduct.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "categories");
                        Directory.CreateDirectory(uploadsPath);

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        categoryProduct.ImageUrl = "/images/categories/" + fileName;
                    }

                    _context.Update(categoryProduct);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Cập nhật danh mục thành công!" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryProductExists(categoryProduct.CategoryId))
                    {
                        return Json(new { success = false, message = "Danh mục không tồn tại" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật" });
                    }
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = errors });
        }

        // POST: Admin/CategoryProduct/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var categoryProduct = await _context.CategoryProduct.FindAsync(id);
                if (categoryProduct == null)
                {
                    return Json(new { success = false, message = "Danh mục không tồn tại" });
                }

                // Kiểm tra xem danh mục có sản phẩm không
                var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
                if (hasProducts)
                {
                    return Json(new { success = false, message = "Không thể xóa danh mục đang có sản phẩm" });
                }

                // Xóa ảnh nếu có
                if (!string.IsNullOrEmpty(categoryProduct.ImageUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, categoryProduct.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.CategoryProduct.Remove(categoryProduct);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa danh mục thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: Admin/CategoryProduct/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var categoryProduct = await _context.CategoryProduct.FindAsync(id);
                if (categoryProduct == null)
                {
                    return Json(new { success = false, message = "Danh mục không tồn tại" });
                }

                categoryProduct.IsActive = !categoryProduct.IsActive;
                _context.Update(categoryProduct);
                await _context.SaveChangesAsync();

                string status = categoryProduct.IsActive ? "kích hoạt" : "vô hiệu hóa";
                return Json(new { success = true, message = $"Đã {status} danh mục thành công!", isActive = categoryProduct.IsActive });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        private bool CategoryProductExists(int id)
        {
            return _context.CategoryProduct.Any(e => e.CategoryId == id);
        }
    }

}
