using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MKxStore247.Services;
using MKxStore247.Models;
using MKxStore247.Data;
using MKxStore247.Services.Interface;

namespace MKxStore247.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly UserManager<UserApplication> _userManager;
        private readonly MKxStore247Context _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderService orderService,
            ICartService cartService,
            UserManager<UserApplication> userManager,
            MKxStore247Context context,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                // Lấy cart
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.Items.Any())
                {
                    TempData["Error"] = "Giỏ hàng của bạn đang trống!";
                    return RedirectToAction("ViewCart", "Cart");
                }

                // Kiểm tra stock
                var isStockValid = await _orderService.ValidateStockAsync(userId);
                if (!isStockValid)
                {
                    TempData["Error"] = "Một số sản phẩm trong giỏ hàng không đủ số lượng. Vui lòng kiểm tra lại!";
                    return RedirectToAction("ViewCart", "Cart");
                }

                // Lấy thông tin user
                var user = await _userManager.GetUserAsync(User);

                // Lấy addresses của user
                var addresses = await _context.Address
                    .Where(a => a.UserId == userId && !a.IsDeleted)
                    .OrderByDescending(a => a.IsDefault)
                    .ToListAsync();

                // Lấy payment methods
                var paymentMethods = await _context.PaymentMethod
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                // Lấy available coupons
                //var availableCoupons = await _context.Coupons
                //    .Where(c => c.IsActive &&
                //           (!c.ExpiryDate.HasValue || c.ExpiryDate > DateTime.UtcNow) &&
                //           (!c.MinOrderAmount.HasValue || c.MinOrderAmount <= cart.Items.Sum(i => i.Product.Price * i.Quantity)))
                //    .ToListAsync();

                ViewBag.Addresses = addresses;
                ViewBag.PaymentMethods = paymentMethods;
                //ViewBag.AvailableCoupons = availableCoupons;
                ViewBag.User = user;

                return View(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading checkout page");
                TempData["Error"] = "Có lỗi xảy ra khi tải trang thanh toán!";
                return RedirectToAction("ViewCart", "Cart");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Error"] = "Vui lòng đăng nhập để tiếp tục!";
                    return RedirectToAction("Index", "Home");
                }

                // Lấy cart
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.Items.Any())
                {
                    TempData["Error"] = "Giỏ hàng của bạn đang trống!";
                    return RedirectToAction("ViewCart", "Cart");
                }

                // Kiểm tra stock
                var isStockValid = await _orderService.ValidateStockAsync(userId);
                if (!isStockValid)
                {
                    TempData["Error"] = "Một số sản phẩm trong giỏ hàng không đủ số lượng. Vui lòng kiểm tra lại!";
                    return RedirectToAction("ViewCart", "Cart");
                }

                // Lấy thông tin user
                var user = await _userManager.GetUserAsync(User);

                // Lấy addresses của user
                var addresses = await _context.Address
                    .Where(a => a.UserId == userId && !a.IsDeleted)
                    .OrderByDescending(a => a.IsDefault)
                    .ToListAsync();

                // Lấy payment methods
                var paymentMethods = await _context.PaymentMethod
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                // Lấy available coupons (uncomment khi có bảng Coupons)
                /*
                var availableCoupons = await _context.Coupons
                    .Where(c => c.IsActive &&
                           (!c.ExpiryDate.HasValue || c.ExpiryDate > DateTime.UtcNow) &&
                           (!c.MinOrderAmount.HasValue || c.MinOrderAmount <= cart.Items.Sum(i => i.Product.Price * i.Quantity)))
                    .ToListAsync();
                */

                ViewBag.Addresses = addresses;
                ViewBag.PaymentMethods = paymentMethods;
                // ViewBag.AvailableCoupons = availableCoupons;
                ViewBag.User = user;

                return View("Index", cart); // Sử dụng view Index cho checkout
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading checkout page");
                TempData["Error"] = "Có lỗi xảy ra khi tải trang thanh toán!";
                return RedirectToAction("ViewCart", "Cart");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int? addressId, int paymentMethodId, int? couponId, string note)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                // Validate inputs
                if (!addressId.HasValue)
                {
                    TempData["Error"] = "Vui lòng chọn địa chỉ giao hàng!";
                    return RedirectToAction("Index");
                }

                if (paymentMethodId <= 0)
                {
                    TempData["Error"] = "Vui lòng chọn phương thức thanh toán!";
                    return RedirectToAction("Index");
                }

                // Kiểm tra cart
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.Items.Any())
                {
                    TempData["Error"] = "Giỏ hàng của bạn đang trống!";
                    return RedirectToAction("ViewCart", "Cart");
                }

                // Tạo order
                var order = await _orderService.CreateOrderAsync(userId, addressId, paymentMethodId, couponId, note);

                TempData["Success"] = "Đặt hàng thành công!";
                return RedirectToAction("Confirmation", new { id = order.OrderId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order");
                TempData["Error"] = "Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại!";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var order = await _orderService.GetOrderByIdAsync(id, userId);

                if (order == null)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng!";
                    return RedirectToAction("Index", "Home");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading order confirmation for order {id}");
                TempData["Error"] = "Có lỗi xảy ra!";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var result = await _orderService.CancelOrderAsync(orderId, userId);

                if (result)
                {
                    TempData["Success"] = "Đơn hàng đã được hủy thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể hủy đơn hàng này!";
                }

                return RedirectToAction("Index", "Order", new { id = orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling order {orderId}");
                TempData["Error"] = "Có lỗi xảy ra khi hủy đơn hàng!";
                return RedirectToAction("", "Order", new { id = orderId });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> ApplyCoupon(int couponId)
        //{
        //    try
        //    {
        //        var userId = _userManager.GetUserId(User);
        //        var cart = await _cartService.GetCartByUserIdAsync(userId);

        //        if (cart == null)
        //        {
        //            return Json(new { success = false, message = "Giỏ hàng trống!" });
        //        }

        //        var tmpAmount = cart.Items.Sum(i => i.Product.Price * i.Quantity);
        //        var discountAmount = await _orderService.ApplyDiscountAsync(couponId, tmpAmount, userId);

        //        if (discountAmount > 0)
        //        {
        //            var finalAmount = tmpAmount - discountAmount;
        //            return Json(new
        //            {
        //                success = true,
        //                discountAmount = discountAmount,
        //                finalAmount = finalAmount,
        //                message = $"Áp dụng mã giảm giá thành công! Giảm {discountAmount:N0}₫"
        //            });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc không áp dụng được!" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error applying coupon");
        //        return Json(new { success = false, message = "Có lỗi xảy ra!" });
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetShippingFee(int addressId)
        {
            try
            {
                var shippingFee = await _orderService.CalculateShippingFeeAsync(addressId);
                return Json(new { success = true, shippingFee = shippingFee });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating shipping fee");
                return Json(new { success = false, message = "Có lỗi xảy ra!" });
            }
        }
    }
}