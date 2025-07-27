using MKxStore247.Models;

namespace MKxStore247.Services.Interface
{
    public interface ICartService
    {
        Task<bool> AddToCartAsync(string userId, int productId, int quantity = 1);
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
        Task<bool> UpdateCartItemQuantityAsync(string userId, int productId, int quantity);
        Task<bool> RemoveFromCartAsync(string userId, int productId);
        Task<bool> ClearCartAsync(string userId);
    }
}
