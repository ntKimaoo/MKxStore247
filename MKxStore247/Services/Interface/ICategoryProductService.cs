using MKxStore247.Models;

namespace MKxStore247.Services.Interface
{
    public interface ICategoryProductService
    {
        Task<IEnumerable<CategoryProduct>> GetAllCategoryAsync();
        Task<CategoryProduct?> GetProductByIdAsync(int id);
        Task AddCategoryAsync(CategoryProduct cateogry);
        Task UpdateCategoryAsync(CategoryProduct cateogry);
        Task DeleteCategoryAsync(int id);
    }
}
