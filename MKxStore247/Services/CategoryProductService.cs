using Microsoft.EntityFrameworkCore;
using MKxStore247.Data;
using MKxStore247.Models;
using MKxStore247.Services.Interface;

namespace MKxStore247.Services
{
    public class CategoryProductService : ICategoryProductService
    {
        private readonly MKxStore247Context _context;

        public CategoryProductService(MKxStore247Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryProduct>> GetAllCategoryAsync()
        {
            var listCategory = await _context.CategoryProduct
                .Where(c => c.IsActive)
                .ToListAsync();
            return listCategory;
        }

        public async Task<CategoryProduct?> GetProductByIdAsync(int id)
        {
            return await _context.CategoryProduct
                .FirstOrDefaultAsync(c => c.CategoryId== id && c.IsActive);
        }

        public async Task AddCategoryAsync(CategoryProduct category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            _context.CategoryProduct.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(CategoryProduct category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            _context.CategoryProduct.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.CategoryProduct
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category != null)
            {
                // Soft delete - set IsActive to false
                category.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        // Alternative hard delete method if needed
        public async Task HardDeleteCategoryAsync(int id)
        {
            var category = await _context.CategoryProduct
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category != null)
            {
                _context.CategoryProduct.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}