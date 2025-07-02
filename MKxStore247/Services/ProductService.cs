using MKxStore247.Models;
using MKxStore247.Services.Interface;

namespace MKxStore247.Services
{
    public class ProductService : IProductService
    {
        public Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Product?> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task AddProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
