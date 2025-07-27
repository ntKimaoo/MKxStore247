using MKxStore247.Models;
using MKxStore247.Data;
using Microsoft.EntityFrameworkCore;
using MKxStore247.Services.Interface;

namespace MKxStore247.Services
{
    public class CartService : ICartService
    {
        private readonly MKxStore247Context _context;
        private readonly ILogger<CartService> _logger;

        public CartService(MKxStore247Context context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddToCartAsync(string userId, int productId, int quantity = 1)
        {
            try
            {
                // Kiểm tra product có tồn tại không
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {productId} not found");
                    return false;
                }

                // Tìm hoặc tạo cart cho user
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(c => c.Product)
                    .ThenInclude(p => p.Images)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        CreatedBy = userId,
                        Items = new List<CartItem>()
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // Kiểm tra item đã có trong cart chưa
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

                if (existingItem != null)
                {
                    // Cập nhật quantity
                    existingItem.Quantity += quantity;
                    _context.CartItems.Update(existingItem);
                }
                else
                {
                    // Thêm item mới
                    var cartItem = new CartItem
                    {
                        CartId = cart.CartId,
                        ProductId = productId,
                        Quantity = quantity
                    };
                    _context.CartItems.Add(cartItem);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Added product {productId} to cart for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding product {productId} to cart for user {userId}");
                return false;
            }
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Shop)
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images) // Thêm dòng này để include hình ảnh
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart?.Items?.Sum(i => i.Quantity) ?? 0;
        }

        public async Task<bool> UpdateCartItemQuantityAsync(string userId, int productId, int quantity)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null) return false;

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null) return false;

                if (quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                    _context.CartItems.Update(item);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating cart item quantity for user {userId}");
                return false;
            }
        }

        public async Task<bool> RemoveFromCartAsync(string userId, int productId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null) return false;

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null) return false;

                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing item from cart for user {userId}");
                return false;
            }
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null) return true;

                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing cart for user {userId}");
                return false;
            }
        }
    }
}