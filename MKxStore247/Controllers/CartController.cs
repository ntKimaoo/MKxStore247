using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MKxStore247.Services;
using MKxStore247.Models;
using MKxStore247.Services.Interface;

namespace MKxStore247.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<UserApplication> _userManager;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ICartService cartService,
            UserManager<UserApplication> userManager,
            ILogger<CartController> logger)
        {
            _cartService = cartService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
                }

                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập!" });
                }

                var result = await _cartService.AddToCartAsync(userId, request.ProductId, request.Quantity);

                if (result)
                {
                    var cartCount = await _cartService.GetCartItemCountAsync(userId);
                    return Json(new
                    {
                        success = true,
                        message = "Đã thêm sản phẩm vào giỏ hàng!",
                        cartCount = cartCount
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể thêm sản phẩm vào giỏ hàng!" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddToCart action");
                return Json(new { success = false, message = "Có lỗi xảy ra!" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { count = 0 });
                }

                var count = await _cartService.GetCartItemCountAsync(userId);
                return Json(new { count = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart count");
                return Json(new { count = 0 });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Index", "Home");
                }

                var cart = await _cartService.GetCartByUserIdAsync(userId);
                return View(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cart");
                return View(new Cart());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var result = await _cartService.UpdateCartItemQuantityAsync(userId, request.ProductId, request.Quantity);

                if (result)
                {
                    var cartCount = await _cartService.GetCartItemCountAsync(userId);
                    return Json(new { success = true, cartCount = cartCount });
                }

                return Json(new { success = false, message = "Không thể cập nhật số lượng!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quantity");
                return Json(new { success = false, message = "Có lỗi xảy ra!" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem([FromBody] RemoveItemRequest request)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var result = await _cartService.RemoveFromCartAsync(userId, request.ProductId);

                if (result)
                {
                    var cartCount = await _cartService.GetCartItemCountAsync(userId);
                    return Json(new { success = true, cartCount = cartCount });
                }

                return Json(new { success = false, message = "Không thể xóa sản phẩm!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item");
                return Json(new { success = false, message = "Có lỗi xảy ra!" });
            }
        }
    }

    // Request Models
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateQuantityRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class RemoveItemRequest
    {
        public int ProductId { get; set; }
    }
}