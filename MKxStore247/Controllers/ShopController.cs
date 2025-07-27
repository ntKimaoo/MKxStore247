using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MKxStore247.Models;
using MKxStore247.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MKxStore247.Controllers
{
    [Authorize] // Add authorization to ensure only logged-in users can create shops
    public class ShopController : Controller
    {
        private readonly ICategoryProductService _categoryProduct;
        private readonly IUserApplicationService _userApplication;
        private readonly IShopService _shopService;
        private readonly IWebHostEnvironment _environment; // Add for file handling

        public ShopController(
            ICategoryProductService categoryProduct,
            IShopService shopService,
            IWebHostEnvironment environment,
            IUserApplicationService userApplication)
        {
            _categoryProduct = categoryProduct;
            _shopService = shopService;
            _environment = environment;
            _userApplication = userApplication;
        }

        public async Task<IActionResult> Create()
        {
            var listCategory = await _categoryProduct.GetAllCategoryAsync();
            ViewBag.listCategory = listCategory;
            return View();
        }
        public async Task<IActionResult> DashBoard()
        {
            try
            {
                var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var shop = await _shopService.GetShopsByOwnerAsync(ownerId);
                var listCategories = await _categoryProduct.GetAllCategoryAsync();
                var statistics = await _shopService.GetShopStatisticsAsync(shop.ShopId);
                ViewBag.listCategory = listCategories;
                ViewBag.Statistics = statistics;

                return View(shop);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error loading shop dashboard");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải dashboard";
                return View();
            }
        }
        [Route("/{ShopUrl}")]
        public async Task<IActionResult> ViewStore(string ShopUrl)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Shop shop, IFormFile? AvatarFile, IFormFile? CoverImageFile)
        {
            try
            {
                if (AvatarFile != null && AvatarFile.Length > 0)
                {
                    shop.Avatar = await SaveFileAsync(AvatarFile, "avatars");
                }
                if (CoverImageFile != null && CoverImageFile.Length > 0)
                {
                    shop.CoverImage = await SaveFileAsync(CoverImageFile, "covers");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userApplication.GetUserByIdAsync(userId);
                shop.OwnerId = userId;
                shop.CreatedBy = user.FullName;
                shop.CreatedAt = DateTime.UtcNow;
                shop.IsActive = false;
                shop.Status = 1;

                // Save to database using service
                var result = await _shopService.CreateShopAsync(shop);

                if (result.Success)
                {
                    return Json(new { success = true, redirectUrl = "/Shop/DashBoard" });   
                }
                else
                {
                    // If model state is invalid, reload categories for the view
                    var listCategory = await _categoryProduct.GetAllCategoryAsync();
                    ViewBag.listCategory = listCategory;
                    return Json(new
                    {
                        success = false,
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you should use a proper logging framework)
                // _logger.LogError(ex, "Error creating shop");

                return Json(new
                {
                    success = false,
                    errors = new[] { "An error occurred while creating the shop. Please try again." }
                });
            }
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            // Validate file size (e.g., max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("File size exceeds 5MB limit");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid file type. Only JPG, JPEG, PNG, and GIF files are allowed.");

            // Create unique filename
            var fileName = Guid.NewGuid().ToString() + extension;
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);

            // Ensure directory exists
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative path for storing in database
            return $"/uploads/{folder}/{fileName}";
        }

        //public async Task<IActionResult> Dashboard()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var userShops = await _shopService.GetShopsByOwnerAsync(userId);

        //    return View(userShops);
        //}

        //// Additional action methods you might need:

        //public async Task<IActionResult> Details(int id)
        //{
        //    var shop = await _shopService.GetShopByIdAsync(id);

        //    if (shop == null)
        //        return NotFound();

        //    return View(shop);
        //}

        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var shop = await _shopService.GetShopByIdAsync(id);

        //    if (shop == null || shop.OwnerId != userId)
        //        return NotFound();

        //    var listCategory = await _categoryProduct.GetAllCategoryAsync();
        //    ViewBag.listCategory = listCategory;

        //    return View(shop);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Shop shop, IFormFile AvatarFile, IFormFile CoverImageFile)
        //{
        //    try
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        var existingShop = await _shopService.GetShopByIdAsync(id);

        //        if (existingShop == null || existingShop.OwnerId != userId)
        //            return NotFound();

        //        if (ModelState.IsValid)
        //        {
        //            // Update properties
        //            existingShop.Name = shop.ShopName;
        //            existingShop.Description = shop.Description;
        //            existingShop.CategoryId = shop.MainCategoryId;
        //            existingShop.UpdatedAt = DateTime.UtcNow;
        //            existingShop.UpdatedBy = userId;

        //            // Handle file uploads
        //            if (AvatarFile != null && AvatarFile.Length > 0)
        //            {
        //                // Delete old avatar if exists
        //                if (!string.IsNullOrEmpty(existingShop.Avatar))
        //                {
        //                    DeleteFile(existingShop.Avatar);
        //                }
        //                existingShop.Avatar = await SaveFileAsync(AvatarFile, "avatars");
        //            }

        //            if (CoverImageFile != null && CoverImageFile.Length > 0)
        //            {
        //                // Delete old cover image if exists
        //                if (!string.IsNullOrEmpty(existingShop.CoverImage))
        //                {
        //                    DeleteFile(existingShop.CoverImage);
        //                }
        //                existingShop.CoverImage = await SaveFileAsync(CoverImageFile, "covers");
        //            }

        //            var result = await _shopService.UpdateShopAsync(existingShop);

        //            if (result.Success)
        //            {
        //                return Json(new { success = true, redirectUrl = "/Shop/Dashboard" });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, errors = new[] { result.Message } });
        //            }
        //        }

        //        return Json(new
        //        {
        //            success = false,
        //            errors = ModelState.Values
        //                .SelectMany(v => v.Errors)
        //                .Select(e => e.ErrorMessage)
        //                .ToList()
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            errors = new[] { "An error occurred while updating the shop. Please try again." }
        //        });
        //    }
        //}

        private void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var shop = await _shopService.GetShopByIdAsync(id);

                if (shop == null || shop.OwnerId != userId)
                    return NotFound();

                // Delete associated files
                if (!string.IsNullOrEmpty(shop.Avatar))
                    DeleteFile(shop.Avatar);
                if (!string.IsNullOrEmpty(shop.CoverImage))
                    DeleteFile(shop.CoverImage);

                var result = await _shopService.DeleteShopAsync(id);

                if (result.Success)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = "An error occurred while deleting the shop. Please try again."
                });
            }
        }
    }
}