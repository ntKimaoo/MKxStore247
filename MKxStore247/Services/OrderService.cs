using Microsoft.EntityFrameworkCore;
using MKxStore247.Data;
using MKxStore247.Models;
using MKxStore247.Services.Interface;

namespace MKxStore247.Services
{
    public class OrderService : IOrderService
    {
        private readonly MKxStore247Context _context;
        private readonly ICartService _cartService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            MKxStore247Context context,
            ICartService cartService,
            ILogger<OrderService> logger)
        {
            _context = context;
            _cartService = cartService;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(string userId, int? addressId, int paymentMethodId, int? couponId, string note)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Lấy cart của user
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.Items.Any())
                {
                    throw new InvalidOperationException("Giỏ hàng trống");
                }

                // Kiểm tra stock
                foreach (var item in cart.Items)
                {
                    if (item.Product.StockQuantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Sản phẩm {item.Product.ProductName} không đủ số lượng trong kho");
                    }
                }

                // Tính toán giá
                decimal tmpAmount = cart.Items.Sum(i => i.Product.Price * i.Quantity);
                decimal shippingFee = await CalculateShippingFeeAsync(addressId);
                decimal discountAmount = 0;
                decimal totalAmount = tmpAmount + shippingFee - discountAmount;

                // Tạo order
                var order = new Order
                {
                    UserId = userId,
                    AddressId = addressId,
                    PaymentMethodId = paymentMethodId,
                    CouponId = couponId,
                    TmpAmount = tmpAmount,
                    ShippingFee = shippingFee,
                    DiscountAmount = discountAmount,
                    TotalAmount = totalAmount,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    Items = new List<OrderDetail>()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Tạo order details
                foreach (var cartItem in cart.Items)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price
                    };

                    _context.OrderDetails.Add(orderDetail);

                    // Cập nhật stock
                    var product = await _context.Products.FindAsync(cartItem.ProductId);
                    product.StockQuantity -= cartItem.Quantity;
                    product.SoldQuantity += cartItem.Quantity;
                    _context.Products.Update(product);
                }

                await _context.SaveChangesAsync();

                // Xóa cart
                await _cartService.ClearCartAsync(userId);

                await transaction.CommitAsync();

                _logger.LogInformation($"Order {order.OrderId} created successfully for user {userId}");
                return order;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error creating order for user {userId}");
                throw;
            }
        }

        public async Task<Order> GetOrderByIdAsync(int orderId, string userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Address)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Coupon)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId, int page = 1, int pageSize = 10)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Address)
                .Include(o => o.PaymentMethod)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null) return false;

                order.Status = status;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order status for order {orderId}");
                return false;
            }
        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

                if (order == null) return false;

                // Chỉ cho phép hủy order có status là Pending hoặc Confirmed
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                {
                    return false;
                }

                // Hoàn lại stock
                foreach (var item in order.Items)
                {
                    var product = item.Product;
                    product.StockQuantity += item.Quantity;
                    product.SoldQuantity -= item.Quantity;
                    _context.Products.Update(product);
                }

                // Cập nhật status
                order.Status = OrderStatus.Cancelled;
                _context.Orders.Update(order);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error cancelling order {orderId}");
                return false;
            }
        }

        public async Task<decimal> CalculateShippingFeeAsync(int? addressId)
        {
            // Logic tính phí ship - có thể dựa trên địa chỉ, khoảng cách, trọng lượng...
            // Ở đây tạm thời return 0 (free ship)
            return 0;
        }

        //public async Task<decimal> ApplyDiscountAsync(int? couponId, decimal amount, string userId)
        //{
        //    if (!couponId.HasValue) return 0;

        //    var coupon = await _context.Coupons.FindAsync(couponId.Value);
        //    if (coupon == null || !coupon.IsActive) return 0;

        //    // Kiểm tra điều kiện coupon
        //    if (coupon.MinOrderAmount.HasValue && amount < coupon.MinOrderAmount) return 0;
        //    if (coupon.ExpiryDate.HasValue && DateTime.UtcNow > coupon.ExpiryDate) return 0;
        //    if (coupon.UsageLimit.HasValue)
        //    {
        //        var usedCount = await _context.Orders.CountAsync(o => o.CouponId == couponId);
        //        if (usedCount >= coupon.UsageLimit) return 0;
        //    }

        //    // Tính discount
        //    decimal discount = 0;
        //    if (coupon.DiscountType == "Percentage")
        //    {
        //        discount = amount * (coupon.DiscountValue / 100);
        //        if (coupon.MaxDiscountAmount.HasValue && discount > coupon.MaxDiscountAmount)
        //        {
        //            discount = coupon.MaxDiscountAmount.Value;
        //        }
        //    }
        //    else if (coupon.DiscountType == "FixedAmount")
        //    {
        //        discount = coupon.DiscountValue;
        //    }

        //    return discount;
        //}

        public async Task<bool> ValidateStockAsync(string userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null) return false;

            foreach (var item in cart.Items)
            {
                if (item.Product.StockQuantity < item.Quantity)
                {
                    return false;
                }
            }

            return true;
        }
    }
}