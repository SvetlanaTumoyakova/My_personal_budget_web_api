using Microsoft.EntityFrameworkCore;
using My_personal_budget_web_api.Context;
using My_personal_budget_web_api.DTO.CategoryDto;
using My_personal_budget_web_api.Models;
using My_personal_budget_web_api.Providers.Interface;

namespace My_personal_budget_web_api.Providers
{
    public class CategoryProvider : ICategoryProvider
    {
        private readonly DataBaseContext _dataBaseContext;
        public CategoryProvider(DataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }

        public async Task<Category> CreateCategoryAsync(Guid userId, CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = createCategoryDto.Name,
                IsDefault = false,
                TransactionTypeId = createCategoryDto.TransactionTypeId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _dataBaseContext.Categories.Add(category);
            await _dataBaseContext.SaveChangesAsync();

            return category;
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid categoryId, Guid userId)
        {
            return await _dataBaseContext.Categories
                .Include(c => c.TransactionType)
                .FirstOrDefaultAsync(c =>
                    c.Id == categoryId &&
                    c.UserId == userId &&
                    !c.IsDeleted); 
        }

        public async Task<List<Category>> GetCategoriesAsync(Guid userId)
        {
            return await _dataBaseContext.Categories
                .Include(c => c.TransactionType)
                .Where(c =>
                    c.UserId == userId &&
                    !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> UpdateCategoryNameAsync(Guid categoryId, Guid userId, string newName)
        {
            var category = await _dataBaseContext.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

            if (category == null)
            {
                return null;
            }

            category.Name = newName;
            category.UpdatedAt = DateTime.UtcNow; 

            await _dataBaseContext.SaveChangesAsync(); 

            return category;
        }
        public async Task<bool> DeleteCategoryAsync(Guid categoryId, Guid userId)
        {
            var category = await _dataBaseContext.Categories
                .FirstOrDefaultAsync(c =>
                    c.Id == categoryId &&
                    c.UserId == userId &&
                    !c.IsDeleted);

            if (category == null)
            {
                return false;
            }

            category.IsDeleted = true;
            category.UpdatedAt = DateTime.UtcNow;

            await _dataBaseContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> IsCategoryNameUniqueAsync(Guid userId, string categoryName, Guid? excludeCategoryId = null)
        {
            return !await _dataBaseContext.Categories.AnyAsync(c =>
                c.UserId == userId &&
                c.Name == categoryName &&
                (!excludeCategoryId.HasValue || c.Id != excludeCategoryId));
        }
    }
}
