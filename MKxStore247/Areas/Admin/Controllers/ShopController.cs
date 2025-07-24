using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MKxStore247.Data;
using MKxStore247.Models;
using MKxStore247.Services.Interface;

namespace MKxStore247.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShopsController : Controller
    {
        private readonly UserManager<UserApplication> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IShopService _shopService;

        public ShopsController(
            UserManager<UserApplication> userManager,
            IWebHostEnvironment webHostEnvironment,IShopService shopService)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _shopService = shopService;
        }

        // GET: Admin/Shops
        public async Task<IActionResult> Index()
        {
            var shops = await _shopService.GetAllShopsAsync();

            return View(shops);
        }

        //// GET: Admin/Shops/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var shop = await _context.Shop
        //        .Include(s => s.Owner)
        //        .Include(s => s.MainCategoryProduct)
        //        .Include(s => s.Products)
        //        .FirstOrDefaultAsync(m => m.ShopId == id && !m.IsDeleted);

        //    if (shop == null) return NotFound();

        //    return View(shop);
        //}

        //// GET: Admin/Shops/Create
        //public async Task<IActionResult> Create()
        //{
        //    await PopulateDropDownLists();
        //    return View();
        //}

        //// POST: Admin/Shops/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Shop shop, IFormFile? avatarFile, IFormFile? coverFile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Handle file uploads
        //            if (avatarFile != null)
        //                shop.Avatar = await SaveFileAsync(avatarFile, "avatars");

        //            if (coverFile != null)
        //                shop.CoverImage = await SaveFileAsync(coverFile, "covers");

        //            var currentUser = await _userManager.GetUserAsync(User);
        //            shop.CreatedBy = currentUser.Id;
        //            shop.CreatedAt = DateTime.UtcNow;

        //            _context.Add(shop);
        //            await _context.SaveChangesAsync();

        //            TempData["Success"] = "Tạo shop thành công!";
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
        //        }
        //    }

        //    await PopulateDropDownLists(shop);
        //    return View(shop);
        //}

        //// GET: Admin/Shops/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var shop = await _context.Shops.FindAsync(id);
        //    if (shop == null || shop.IsDeleted) return NotFound();

        //    await PopulateDropDownLists(shop);
        //    return View(shop);
        //}

        //// POST: Admin/Shops/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Shop shop, IFormFile? avatarFile, IFormFile? coverFile)
        //{
        //    if (id != shop.ShopId) return NotFound();

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var existingShop = await _context.Shops.AsNoTracking().FirstOrDefaultAsync(s => s.ShopId == id);
        //            if (existingShop == null) return NotFound();

        //            // Handle file uploads
        //            if (avatarFile != null)
        //            {
        //                if (!string.IsNullOrEmpty(existingShop.Avatar))
        //                    DeleteFile(existingShop.Avatar);
        //                shop.Avatar = await SaveFileAsync(avatarFile, "avatars");
        //            }
        //            else
        //            {
        //                shop.Avatar = existingShop.Avatar;
        //            }

        //            if (coverFile != null)
        //            {
        //                if (!string.IsNullOrEmpty(existingShop.CoverImage))
        //                    DeleteFile(existingShop.CoverImage);
        //                shop.CoverImage = await SaveFileAsync(coverFile, "covers");
        //            }
        //            else
        //            {
        //                shop.CoverImage = existingShop.CoverImage;
        //            }

        //            shop.UpdatedAt = DateTime.UtcNow;
        //            shop.CreatedBy = existingShop.CreatedBy;
        //            shop.CreatedAt = existingShop.CreatedAt;

        //            _context.Update(shop);
        //            await _context.SaveChangesAsync();

        //            TempData["Success"] = "Cập nhật shop thành công!";
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ShopExists(shop.ShopId))
        //                return NotFound();
        //            throw;
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
        //        }
        //    }

        //    await PopulateDropDownLists(shop);
        //    return View(shop);
        //}

        //// POST: Admin/Shops/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var shop = await _context.Shops.FindAsync(id);
        //    if (shop != null)
        //    {
        //        shop.IsDeleted = true;
        //        shop.UpdatedAt = DateTime.UtcNow;
        //        await _context.SaveChangesAsync();
        //        TempData["Success"] = "Xóa shop thành công!";
        //    }
        //    return RedirectToAction(nameof(Index));
        //}

        //// POST: Admin/Shops/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var shop = await _shopService.GetShopByIdAsync(id);
            if (shop != null)
            {
                shop.IsActive = !shop.IsActive;
                shop.UpdatedAt = DateTime.UtcNow;
                await _shopService.UpdateShopAsync(shop);

                string status = shop.IsActive ? "kích hoạt" : "vô hiệu hóa";
                TempData["Success"] = $"Đã {status} shop thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ShopExists(int id)
        {
            return await _shopService.ShopExistsAsync(id);
        }

        //private async Task PopulateDropDownLists(Shop shop = null)
        //{
        //    ViewData["OwnerId"] = new SelectList(
        //        await _context.Users.Where(u => !u.IsDeleted).ToListAsync(),
        //        "Id", "UserName", shop?.OwnerId);

        //    ViewData["MainCategoryId"] = new SelectList(
        //        await _context.CategoryProducts.Where(c => c.IsActive).ToListAsync(),
        //        "CategoryId", "CategoryName", shop?.MainCategoryId);
        //}

        private async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/{folder}/{uniqueFileName}";
        }

        private void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}